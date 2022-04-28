using System;
using System.Collections.Generic;
using System.Linq;
using CodeBase.Audio;
using CodeBase.Logic;
using CodeBase.Units.Hero;
using DG.Tweening;
using UnityEngine;

namespace CodeBase.Units.Enemy
{
	public class EnemyChaser : EnemyMovement
	{
		[SerializeField] private ChaserSettings _chaserSettings;
		[Space(10)]
		[SerializeField] private TriggerListener _agroZone;
		[Space(10)]
		[SerializeField] private GameObject _landVfx;
		[SerializeField] private AudioClip _agroSfx;

		private Transform _hero;
		private bool _canChase = false;
		private float _currentVerticalSpeed;
		private float _lastMoveDirection;
		private Coroutine _confusionCoroutine;
		private bool _isJumping = false;
		private bool _isJumpKilled = false;
		private Sequence _doJump;
		private UnitAnimator _animator;

		private void Awake()
		{
			Init();
			_animator = GetComponent<UnitAnimator>();
		}

		private void OnEnable()
		{
			_agroZone.Entered += OnAgroZoneEnter;
			_agroZone.Canceled += OnAgroZoneCancel;
			_collisionDetector.Collided += OnCollision;
			_collisionDetector.BottomCollided += OnBottomCollision;
		}

		private void OnDisable()
		{
			_agroZone.Entered -= OnAgroZoneEnter;
			_agroZone.Canceled -= OnAgroZoneCancel;
			_collisionDetector.Collided -= OnCollision;
			_collisionDetector.BottomCollided -= OnBottomCollision;
		}

		private void Update()
		{
			UpdateCollisionDetector();
			
			if(_activated == false) return;
			
			if(_hero != null)
				Chase();
			
			Tilt();
			CalculateGravity();
			Move();
		}

		public override void Disable()
		{
			StopJump();
			_inputDisabled = true;
			_currentVerticalSpeed = 0;
			_currentHorizontalSpeed = 0;
			_animator.PlayIdle();
		}

		protected override int GetMoveDirection() => _hero.position.x > transform.position.x ? 1 : -1;

		private void UpdateCollisionDetector()
		{
			_collisionDetector.CalculateRaysPosition();
			_collisionDetector.UpdateBoxCollisions();
			_collisionDetector.UpdateCollisionEvents();
		}

		private void OnAgroZoneEnter(Collider2D obj)
		{
			if (obj.TryGetComponent(out HeroMovement heroMovement) == false) return;

			_hero = heroMovement.transform;
			
			if (BlockedByCube() == false && HeroAtSameHeight())
				Jump();
			else
				_canChase = true;
		}

		private void OnAgroZoneCancel(Collider2D obj)
		{
			if (obj.TryGetComponent(out HeroMovement heroMovement))
				_canChase = false;
		}

		private void OnCollision() => StopJump();

		private void OnBottomCollision()
		{
			if(_isJumpKilled == false) return;
			_animator.PlayLanding();
			SpawnLandVfx();
		}

		private bool HeroAtSameHeight() => transform.position.y <= _hero.position.y + Mathf.Abs(_collisionDetector.Bounds.extents.y);

		private void SpawnLandVfx() => 
			Instantiate(_landVfx, transform.position - Vector3.up * _sprite.bounds.size.y / 2, Quaternion.identity);
		
		private void Jump()
		{
			_sfxPlayer.Play(_agroSfx);
			FlipSprite();
			
			_isJumping = true;
			_isJumpKilled = false;
			
			Vector3 currentPosition = transform.position;
			currentPosition.y = _hero.position.y;
			Vector3 jumpTargetPosition = Vector3.Lerp(currentPosition, _hero.position, _chaserSettings.JumpPercentLength / 100);
			
			_doJump = transform.DOJump(jumpTargetPosition, _chaserSettings.JumpHeight, 1, _chaserSettings.JumpDuration)
				.OnComplete(OnJumpComplete)
				.OnUpdate(OnJumpUpdate)
				.OnKill(() => _isJumpKilled = true);
			
			_animator.PlayJump();
		}

		private void OnJumpUpdate()
		{
			if (_collisionDetector.BoxCollisions.TopCollision || _collisionDetector.BoxCollisions.LeftCollision ||
			    _collisionDetector.BoxCollisions.RightCollision)
			{
				StopJump();
			}
		}

		private void FlipSprite()
		{
			if (_moveDirection > 0)
				_sprite.flipX = false;
			else if (_moveDirection < 0)
				_sprite.flipX = true;
		}
		
		private void OnJumpComplete()
		{
			_isJumping = false;
			_canChase = true;
			
			_animator.PlayLanding();
		}

		private void StopJump()
		{
			if(_doJump == null) return;
			
			_doJump.Kill();
			_isJumping = false;
			_canChase = true;
		}

		private void Move()
		{
			Vector3 currentPosition = transform.position;
			Vector3 speed = new Vector3(_currentHorizontalSpeed, _currentVerticalSpeed);
			Vector3 move = speed * Time.deltaTime;
			Vector3 furthestPoint = currentPosition + move;
			Collider2D furthestPointCollision = _collisionDetector.GetCollisionInPoint(furthestPoint);
			bool canMove = furthestPointCollision == null;

			if (canMove)
				transform.position += move;
			else
				MoveToClosestAvailablePoint(move, furthestPointCollision);
		}
		
		private void MoveToClosestAvailablePoint(Vector3 move, Collider2D collided)
		{
			Vector3 currentPosition = transform.position;
			Vector3 furthestPoint = currentPosition + move;

			for (int i = 1; i < _enemyMovementSettings.ClosestPointFindAttempts; i++)
			{
				float t = (float) i / _enemyMovementSettings.ClosestPointFindAttempts;
				Vector2 currentTryPosition = Vector2.Lerp(currentPosition, furthestPoint, t);

				if (_collisionDetector.CanMoveToPoint(currentTryPosition) == false)
				{
					if (i == 1)
					{
						Vector2 closestPoint = collided.ClosestPoint(transform.position);
						Vector3 dir = transform.position - new Vector3(closestPoint.x, closestPoint.y, 0);
						
						if(_currentVerticalSpeed < 0)
							_currentVerticalSpeed = 0;
						
						transform.position += dir.normalized * move.magnitude;
					}
					
					continue;
				}

				transform.position = currentTryPosition;
			}
		}

		private void CalculateGravity()
		{
			if (_collisionDetector.BoxCollisions.BottomCollision)
			{
				if(_currentVerticalSpeed < 0)
					_currentVerticalSpeed = 0;
			}
			else if (_isJumping == false)
			{
				_currentVerticalSpeed = Mathf.Clamp(_currentVerticalSpeed - _enemyMovementSettings.FallSpeed * Time.deltaTime,
					-_enemyMovementSettings.MaxFallSpeed, float.MaxValue);
			}
		}

		private void Chase()
		{
			if (MovingToOppositeDirection())
			{
				Confuse();
				_currentHorizontalSpeed = 0;
			}
			
			if (CanMove() == false)
			{
				TryClimbAssist();
				_currentHorizontalSpeed = 0;
				_animator.PlayWalk(0);
				return;
			}
			
			if (_canChase)
			{
				FlipSprite();
				_currentHorizontalSpeed += _moveDirection * _enemyMovementSettings.Acceleration * Time.deltaTime;
				_currentHorizontalSpeed = Mathf.Clamp(_currentHorizontalSpeed, -_enemyMovementSettings.MaxMoveSpeed, _enemyMovementSettings.MaxMoveSpeed);
			}
			else
				_currentHorizontalSpeed = Mathf.MoveTowards(_currentHorizontalSpeed, 0, _enemyMovementSettings.DeAcceleration * Time.deltaTime);
			
			float animationSpeed = Mathf.Clamp(Mathf.Abs(_currentHorizontalSpeed), 0, 1.5f);
			_animator.PlayWalk(animationSpeed);
		}

		private bool MovingToOppositeDirection()
		{
			if (_lastMoveDirection == 0)
			{
				_lastMoveDirection = Mathf.Sign(_moveDirection);
				return false;
			}
				
			bool oppositeDirection = Mathf.Sign(_moveDirection) != _lastMoveDirection;
			_lastMoveDirection = Mathf.Sign(_moveDirection);

			return oppositeDirection;
		}

		private void Confuse()
		{
			if (_confusionCoroutine != null)
				StopCoroutine(_confusionCoroutine);
			
			_inputDisabled = true;
			_confusionCoroutine = StartCoroutine(_coroutineRunner.CallWithDelayCoroutine(() => _inputDisabled = false, _chaserSettings.ConfusionTime));
			
			_animator.PlayIdle();
		}
	}
}