using System;
using System.Collections;
using System.Collections.Generic;
using CodeBase.Collisions;
using CodeBase.Infrastructure;
using CodeBase.Logic;
using CodeBase.Units.Hero;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace CodeBase.Units.Enemy
{
	public class EnemyChaser : EnemyMovement
	{
		[SerializeField] private TriggerListener _agroZone;
		[SerializeField] private float _confusionTime = 0.5f;
		[Space(10)]
		[SerializeField] private float _maxMoveSpeed = 1;
		[SerializeField] private float _acceleration = 1;
		[SerializeField] private float _deAcceleration;
		[Space(10)]
		[SerializeField] private float _climbDuration = 4;
		[SerializeField] private float _climbAssistHeight = 0.01f;
		[Space(10)]
		[SerializeField] private float _fallSpeed = 4;
		[Space(10)]
		[SerializeField, Range(1, 100)] private float _jumpPercentLength = 60f;
		[SerializeField] private float _jumpHeight = 0.5f;
		[SerializeField] private float _jumpDuration = 1;
		[Space(10)]
		[SerializeField] private float _maxTilt = 1;
		[SerializeField] private float _tiltSpeed = 1;
		
		private int _moveDirection => GetMoveDirection();
		
		private SpriteRenderer _sprite;
		private CollisionDetector _collisionDetector;
		private Transform _hero;
		private float _currentHorizontalSpeed = 0;
		private bool _canChase = false;
		private float _currentVerticalSpeed;
		private float _lastMoveDirection;
		private bool _inputDisabled = false;
		private Coroutine _confusionCoroutine;
		private bool _isJumping = false;
		private bool _isJumpKilled = false;
		private Sequence _doJump;
		private UnitAnimator _animator;

		private void Awake()
		{
			_sprite = GetComponent<SpriteRenderer>();
			_collisionDetector = GetComponent<CollisionDetector>();
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
			_collisionDetector.CalculateRaysPosition();
			_collisionDetector.UpdateBoxCollisions();
			_collisionDetector.UpdateCollisionEvents();
			
			if(_hero != null)
				Chase();
			
			Tilt();
			CalculateGravity();
		}

		public override void Disable()
		{
			StopJump();
			_inputDisabled = true;
			_currentVerticalSpeed = 0;
			_currentHorizontalSpeed = 0;
			_animator.PlayIdle();
		}

		private void OnAgroZoneEnter(Collider2D obj)
		{
			if (obj.TryGetComponent(out HeroMovement heroMovement) == false) return;

			_hero = heroMovement.transform;
			
			if (BlockedByCube() == false)
				Jump();
			else
				_canChase = true;
		}

		private void OnAgroZoneCancel(Collider2D obj)
		{
			if (obj.TryGetComponent(out HeroMovement heroMovement))
				_canChase = false;
		}

		private void OnCollision()
		{
			StopJump();
		}

		private void OnBottomCollision()
		{
			if(_isJumpKilled == false) return;
			_animator.PlayLanding();
		}

		private void Jump()
		{
			FlipSprite();
			
			_isJumping = true;
			_isJumpKilled = false;
			
			Vector3 currentPosition = transform.position;
			currentPosition.y = _hero.position.y;
			Vector3 jumpTargetPosition = Vector3.Lerp(currentPosition, _hero.position, _jumpPercentLength / 100);
			
			_doJump = transform.DOJump(jumpTargetPosition, _jumpHeight, 1, _jumpDuration)
				.OnComplete(OnJumpComplete)
				.OnKill(() => _isJumpKilled = true);
			
			_animator.PlayJump();
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

		private void CalculateGravity() {
			if (_collisionDetector.BoxCollisions.BottomCollision == false && _isJumping == false)
				_currentVerticalSpeed -= _fallSpeed * Time.deltaTime;
			else if(_currentVerticalSpeed < 0)
				_currentVerticalSpeed = 0;
			
			transform.position += Vector3.up * _currentVerticalSpeed * Time.deltaTime;
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
				_animator.PlayWalk(0);
				return;
			}
			
			if (_canChase)
			{
				FlipSprite();
				_currentHorizontalSpeed += _moveDirection * _acceleration * Time.deltaTime;
				_currentHorizontalSpeed = Mathf.Clamp(_currentHorizontalSpeed, -_maxMoveSpeed, _maxMoveSpeed);
			}
			else
				_currentHorizontalSpeed = Mathf.MoveTowards(_currentHorizontalSpeed, 0, _deAcceleration * Time.deltaTime);

			transform.position += Vector3.right * _currentHorizontalSpeed * Time.deltaTime;
			
			float animationSpeed = Mathf.Clamp(Mathf.Abs(_currentHorizontalSpeed), 0, 1.5f);
			_animator.PlayWalk(animationSpeed);
		}

		private void Tilt()
		{
			Vector3 targetRotVector = new Vector3(0, 0, 
				Mathf.Lerp(-_maxTilt, _maxTilt, Mathf.InverseLerp(-1, 1, Mathf.Clamp(_currentHorizontalSpeed, -1, 1))));
			transform.rotation = Quaternion.RotateTowards(
				transform.rotation, Quaternion.Euler(targetRotVector), _tiltSpeed * Time.deltaTime);
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
			_confusionCoroutine = StartCoroutine(_coroutineRunner.CallWithDelayCoroutine(() => _inputDisabled = false, _confusionTime));
			
			_animator.PlayIdle();
		}

		private void TryClimbAssist()
		{
			Collider2D collided = null;

			if (_moveDirection > 0)
				collided = _collisionDetector.GetFirstRightSideCollision();
			else if (_moveDirection < 0)
				collided = _collisionDetector.GetFirstLeftSideCollision();

			if(collided == null) return;

			Vector2 climbPoint = _collisionDetector.GetClimbPointToCollider(collided, _climbAssistHeight);
			bool canClimbAssist = climbPoint != Vector2.zero;

			if (canClimbAssist && _inputDisabled == false)
			{
				_inputDisabled = true;
				transform.DOMove(climbPoint, _climbDuration).OnComplete(() => _inputDisabled = false);
			}
		}

		private void FlipSprite() => _sprite.flipX = GetMoveDirection() == -1;

		private bool BlockedByCube()
		{
			_collisionDetector.TryGetComponentFromLeftCollisions(out List<Cube> leftSideCubes);
			_collisionDetector.TryGetComponentFromRightCollisions(out List<Cube> rightSideCubes);

			return _moveDirection > 0 && rightSideCubes.Count > 0 || _moveDirection < 0 && leftSideCubes.Count > 0;
		}

		private bool CanMove()
		{
			return _inputDisabled == false &&
			       (_moveDirection > 0 && _collisionDetector.BoxCollisions.RightCollision == false ||
			       _moveDirection < 0 && _collisionDetector.BoxCollisions.LeftCollision == false);
		}

		private int GetMoveDirection() => _hero.position.x > transform.position.x ? 1 : -1;
	}
}