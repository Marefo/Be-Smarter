using System.Collections.Generic;
using CodeBase.Audio;
using CodeBase.Collisions;
using CodeBase.DeathRay;
using CodeBase.Logic;
using CodeBase.Services;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace CodeBase.Units.Hero
{
	[RequireComponent(typeof(CollisionDetector), typeof(BoxCollider2D))]
	[RequireComponent(typeof(UnitAnimator))]
	public class HeroMovement : UnitMovement, IHoldingBtnActivator
	{
		public float MoveDirection => transform.localScale.x;
		public bool IsWalking => Mathf.Abs(_currentHorizontalSpeed) > 0;
		public float SpeedPercent => Mathf.InverseLerp(0, _settings.MaxMoveSpeed, Mathf.Abs(_currentHorizontalSpeed));
		
		[SerializeField] private HeroMoveSettings _settings;
		[Space(10)]
		[SerializeField] private ParticleSystem _walkVfx;
		[Space(10)]
		[SerializeField] private AudioClip _jumpSfx;

		private IInputService _inputService;
		private Vector2 _colliderBottomCenter;
		private CollisionDetector _collisionDetector;
		private UnitAnimator _animator;
		private float _currentHorizontalSpeed;
		private float _currentVerticalSpeed;
		private float _lastMoveDirection;
		private float _apexPoint;
		private Vector2 _velocity;
		private Vector3 _lastPosition;
		private float _fallSpeed;
		private float _lastJumpBtnPressTime;
		private bool _inputDisabled = false;
		private Vector3 _walkVfxDefaultPosition;
		private Quaternion _walkVfxDefaultRotation;
		private SFXPlayer _sfxPlayer;

		[Inject]
		private void Construct(IInputService inputService, SFXPlayer sfxPlayer)
		{
			_inputService = inputService;
			_sfxPlayer = sfxPlayer;
		}

		private void Awake()
		{
			_collisionDetector = GetComponent<CollisionDetector>();
			_animator = GetComponent<UnitAnimator>();
		}

		private void OnEnable()
		{
			_inputService.JumpBtnPressed += OnJumpBtnPress;
			_inputService.RestartBtnPressed += Disable;
			_collisionDetector.BottomCollided += OnBottomCollide;
		}

		private void OnDisable()
		{
			_inputService.JumpBtnPressed -= OnJumpBtnPress;
			_inputService.RestartBtnPressed -= Disable;
			_collisionDetector.BottomCollided -= OnBottomCollide;
		}

		private void Start()
		{
			_walkVfxDefaultPosition = _walkVfx.transform.localPosition;
			_walkVfxDefaultRotation = _walkVfx.transform.localRotation;
		}

		private void Update()
		{
			CalculateVelocity();
			UpdateCollisionDetector();

			if(_activated == false) return;

			FlipSprite();
			Walk();
			Tilt();
			CalculateJumpApex();
			CalculateGravity();
			FlipWalkVfx();
			ControlWalkVfxVisibility();

			Move();
		}

		public override void Disable()
		{
			_inputDisabled = true;
			_currentHorizontalSpeed = 0;
			_currentVerticalSpeed = 0;
			_animator.PlayIdle();
		}

		private void UpdateCollisionDetector()
		{
			_collisionDetector.CalculateRaysPosition();
			_collisionDetector.UpdateBoxCollisions();
			_collisionDetector.UpdateCollisionEvents();
		}

		private void OnBottomCollide()
		{
			_animator.PlayLanding();
			
			_collisionDetector.TryGetComponentFromBottomCollisions(out List<DeathRayHoldingButton> holdingButtons);
			if (holdingButtons.Count == 0)
				SetWalkVfxParent();

			if (HasBufferedJump())
				Jump();
		}

		private void OnJumpBtnPress()
		{
			_lastJumpBtnPressTime = Time.time;
			Jump();
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
		
		private void ControlWalkVfxVisibility()
		{
			ParticleSystem.EmissionModule walkVfxEmission = _walkVfx.emission;
			walkVfxEmission.enabled = _currentHorizontalSpeed != 0;
		}

		private void CalculateVelocity()
		{
			if (_lastPosition != Vector3.zero)
				_velocity = (transform.position - _lastPosition) / Time.deltaTime;

			_lastPosition = transform.position;
		}

		private void FlipSprite()
		{
			if (_inputService.AxisX > 0)
				transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
			else if (_inputService.AxisX < 0)
				transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
		}

		private void FlipWalkVfx()
		{
			ParticleSystem.VelocityOverLifetimeModule walkVfxVelocityOverLifetime = _walkVfx.velocityOverLifetime;

			if(_inputService.AxisX > 0)
				walkVfxVelocityOverLifetime.x = -1 * Mathf.Abs(walkVfxVelocityOverLifetime.x.constant);
			else if(_inputService.AxisX < 0)
				walkVfxVelocityOverLifetime.x = Mathf.Abs(walkVfxVelocityOverLifetime.x.constant);
		}

		private void Tilt()
		{
			Vector3 targetRotationVector = new Vector3(0, 0, 
				Mathf.Lerp(-_settings.MaxTilt, _settings.MaxTilt, Mathf.InverseLerp(-1, 1, _inputService.AxisX)));
			
			transform.rotation = Quaternion.RotateTowards(
				transform.rotation, Quaternion.Euler(targetRotationVector), _settings.TiltSpeed * Time.deltaTime);

			float targetWalkVfxRotationZ = Mathf.Lerp(-_settings.MaxTilt - _walkVfxDefaultRotation.z,
				_settings.MaxTilt + _walkVfxDefaultRotation.z, Mathf.InverseLerp(-1, 1, _inputService.AxisX));
			Vector3 walkVfxTargetRotationVector = new Vector3(0, 0, targetWalkVfxRotationZ);
			_walkVfx.transform.rotation = Quaternion.RotateTowards(
				_walkVfx.transform.rotation, Quaternion.Euler(walkVfxTargetRotationVector), _settings.TiltSpeed * Time.deltaTime);
		}

		private void Walk()
		{
			if (_inputService.AxisX != 0)
			{
				ResetSpeedForOppositeDirection();
				
				float apexSpeed = Mathf.Sign(_inputService.AxisX) * _settings.ApexAdditionalSpeed * _apexPoint;

				_currentHorizontalSpeed += _inputService.AxisX * _settings.Acceleration * Time.deltaTime;
				_currentHorizontalSpeed = Mathf.Clamp(_currentHorizontalSpeed, -_settings.MaxMoveSpeed, _settings.MaxMoveSpeed);
				_currentHorizontalSpeed += apexSpeed * Time.deltaTime;
			}
			else
				_currentHorizontalSpeed = Mathf.MoveTowards(_currentHorizontalSpeed, 0, _settings.DeAcceleration * Time.deltaTime);

			if (CanMove() == false)
			{
				if (TryClimbAssist() == false && _inputService.AxisX != 0)
					_currentHorizontalSpeed = 0;
			}

			float animationSpeed = Mathf.Clamp(Mathf.Abs(_currentHorizontalSpeed), 0, 1.5f);
			_animator.PlayWalk(animationSpeed);
		}

		private bool CanMove()
		{
			return _inputService.AxisX > 0 && _collisionDetector.BoxCollisions.RightCollision == false ||
			       _inputService.AxisX < 0 && _collisionDetector.BoxCollisions.LeftCollision == false;
		}
		
		private void ResetSpeedForOppositeDirection()
		{
			if (_lastMoveDirection != 0)
			{
				if (Mathf.Sign(_inputService.AxisX) != _lastMoveDirection)
					_currentHorizontalSpeed = 0;
			}

			_lastMoveDirection = Mathf.Sign(_inputService.AxisX);
		}

		private void MoveToClosestAvailablePoint(Vector3 move, Collider2D collided)
		{
			Vector3 currentPosition = transform.position;
			Vector3 furthestPoint = currentPosition + move;

			for (int i = 1; i < _settings.ClosestPointFindAttempts; i++)
			{
				float t = (float) i / _settings.ClosestPointFindAttempts;
				Vector2 currentTryPosition = Vector2.Lerp(currentPosition, furthestPoint, t);

				if (_collisionDetector.CanMoveToPoint(currentTryPosition) == false)
				{
					if (i == 1)
					{
						Vector2 closestPoint = collided.ClosestPoint(transform.position);
						Vector3 dir = transform.position - new Vector3(closestPoint.x, closestPoint.y, 0);
						transform.position += dir.normalized * move.magnitude;
					}
					
					continue;
				}

				transform.position = currentTryPosition;
			}
		}

		private bool TryClimbAssist()
		{
			Collider2D collided = null;

			if (_inputService.AxisX > 0)
				collided = _collisionDetector.GetFirstRightSideCollision();
			else if (_inputService.AxisX < 0)
				collided = _collisionDetector.GetFirstLeftSideCollision();

			if(collided == null) return false;

			Vector2 climbPoint = _collisionDetector.GetClimbPointToCollider(collided, _settings.ClimbAssistHeight);
			bool canClimbAssist = climbPoint != Vector2.zero;

			if (canClimbAssist && _inputDisabled == false)
			{
				_inputDisabled = true;
				UnParentWalkVfx();
				transform.DOMove(climbPoint, _settings.ClimbDuration).OnComplete(() => _inputDisabled = false);
			}
				
			return canClimbAssist;
		}
		
		private void CalculateGravity()
		{
			if (_collisionDetector.BoxCollisions.TopCollision && _currentVerticalSpeed > 0)
				_currentVerticalSpeed = 0;
			
			if (_collisionDetector.BoxCollisions.BottomCollision)
			{
				if(_currentVerticalSpeed < 0)
					_currentVerticalSpeed = 0;
			}
			else
				_currentVerticalSpeed = Mathf.Clamp(_currentVerticalSpeed - _fallSpeed * Time.deltaTime, -_settings.MaxFallSpeed, float.MaxValue);
		}

		private void CalculateJumpApex()
		{
			if (_collisionDetector.BoxCollisions.BottomCollision == false)
			{
				_apexPoint = Mathf.InverseLerp(_settings.ApexThreshold, 0, Mathf.Abs(_velocity.y));
				_fallSpeed = Mathf.Lerp(_settings.MinFallSpeed, _settings.MaxFallSpeed, _apexPoint);
			}
			else
				_apexPoint = 0;
		}

		private bool HasBufferedJump() => _lastJumpBtnPressTime + _settings.BufferedJumpTime > Time.time;

		private void Jump()
		{
			if (_collisionDetector.BoxCollisions.BottomCollision == false) return;

			UnParentWalkVfx();
			_currentVerticalSpeed = _settings.JumpHeight;
			_animator.PlayJump();
			_sfxPlayer.Play(_jumpSfx);
		}

		private void SetWalkVfxParent()
		{
			_walkVfx.transform.SetParent(transform);
			_walkVfx.transform.localPosition = _walkVfxDefaultPosition;
			_walkVfx.transform.localScale = Vector3.one;
			_walkVfx.transform.localRotation = _walkVfxDefaultRotation;
		}
		
		private void UnParentWalkVfx() => _walkVfx.transform.SetParent(null);
	}
}