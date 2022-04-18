using CodeBase.Collisions;
using CodeBase.Infrastructure;
using CodeBase.Services;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace CodeBase.Units.Hero
{
	[RequireComponent(typeof(CollisionDetector), typeof(BoxCollider2D), typeof(SpriteRenderer))]
	[RequireComponent(typeof(UnitAnimator))]
	public class HeroMovement : MonoBehaviour
	{
		[SerializeField] private HeroMoveSettings _settings;
		
		private readonly int _closestPointFindAttempts = 5;
		
		private IInputService _inputService;
		private SpriteRenderer _sprite;
		private Vector2 _colliderBottomCenter;
		private CollisionDetector _collisionDetector;
		private LevelPreparer _levelPreparer;
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
		private bool _isJumping = false;

		[Inject]
		private void Construct(IInputService inputService, LevelPreparer levelPreparer)
		{
			_inputService = inputService;
			_levelPreparer = levelPreparer;
			_sprite = GetComponent<SpriteRenderer>();
			_collisionDetector = GetComponent<CollisionDetector>();
			_animator = GetComponent<UnitAnimator>();
		}
		
		private void Start()
		{
			_inputService.JumpBtnPressed += OnJumpBtnPress;
			_collisionDetector.BottomCollided += OnBottomCollide;
		}

		private void OnDestroy()
		{
			_inputService.JumpBtnPressed -= OnJumpBtnPress;
			_collisionDetector.BottomCollided -= OnBottomCollide;
		}

		private void Update()
		{
			_collisionDetector.CalculateRaysPosition();
			_collisionDetector.UpdateBoxCollisions();
			_collisionDetector.UpdateCollisionEvents();
			
			if(_levelPreparer.Prepared == false) return;
			
			CalculateVelocity();
			FlipSprite();
			Walk();
			Tilt();
			CalculateJumpApex();
			CalculateGravity();
			CalculateJump();
		}

		private void OnBottomCollide()
		{
			_animator.PlayLanding();
			
			if (HasBufferedJump())
				Jump();
		}

		private void OnJumpBtnPress()
		{
			_lastJumpBtnPressTime = Time.time;
			Jump();
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
				_sprite.flipX = false;
			else if (_inputService.AxisX < 0)
				_sprite.flipX = true;
		}

		private void Tilt()
		{
			Vector3 targetRotVector = new Vector3(0, 0, 
				Mathf.Lerp(-_settings.MaxTilt, _settings.MaxTilt, Mathf.InverseLerp(-1, 1, _inputService.AxisX)));
			transform.rotation = Quaternion.RotateTowards(
				transform.rotation, Quaternion.Euler(targetRotVector), _settings.TiltSpeed * Time.deltaTime);
		}

		private void Walk()
		{
			if (CanMove() == false)
			{
				_animator.PlayWalk(0);
				
				if (TryClimbAssist() == false)
					MoveToClosestAvailablePoint();
				
				return;
			}

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

			transform.position += Vector3.right * _currentHorizontalSpeed * Time.deltaTime;

			float animationSpeed = Mathf.Clamp(Mathf.Abs(_currentHorizontalSpeed), 0, 1.5f);
			_animator.PlayWalk(animationSpeed);
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

		private bool CanMove()
		{
			return _inputDisabled == false && 
			       (_inputService.AxisX > 0 && _collisionDetector.BoxCollisions.RightCollision == false ||
			       _inputService.AxisX < 0 && _collisionDetector.BoxCollisions.LeftCollision == false ||
			       _inputService.AxisX == 0 && _lastMoveDirection > 0 && _collisionDetector.BoxCollisions.RightCollision == false ||
			       _inputService.AxisX == 0 && _lastMoveDirection < 0 && _collisionDetector.BoxCollisions.LeftCollision == false);
	}

		private void MoveToClosestAvailablePoint()
		{
			Vector3 currentPosition = transform.position;
			Vector3 move = Vector3.right * _currentHorizontalSpeed * Time.deltaTime;
			Vector3 furthestPoint = currentPosition + move;

			for (int i = 1; i < _closestPointFindAttempts; i++)
			{
				float t = (float) i / _closestPointFindAttempts;
				Vector2 currentTryPosition = Vector2.Lerp(currentPosition, furthestPoint, t);

				if (_collisionDetector.CanMoveToPoint(currentTryPosition))
				{
					transform.position = currentTryPosition;
					return;
				}
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
				transform.DOMove(climbPoint, _settings.ClimbDuration).OnComplete(() => _inputDisabled = false);
			}
				
			return canClimbAssist;
		}

		private void CalculateGravity() {
			if (_collisionDetector.BoxCollisions.BottomCollision == false)
				_currentVerticalSpeed = Mathf.Clamp(_currentVerticalSpeed - _fallSpeed * Time.deltaTime, -_settings.MaxFallSpeed, float.MaxValue);
			else if(_currentVerticalSpeed < 0)
				_currentVerticalSpeed = 0;
		}

		private void CalculateJumpApex() {
			if (_collisionDetector.BoxCollisions.BottomCollision == false)
			{
				_apexPoint = Mathf.InverseLerp(_settings.ApexThreshold, 0, Mathf.Abs(_velocity.y));
				_fallSpeed = Mathf.Lerp(_settings.MinFallSpeed, _settings.MaxFallSpeed, _apexPoint);
			}
			else
				_apexPoint = 0;
		}

		private void CalculateJump()
		{
			if (_collisionDetector.BoxCollisions.TopCollision == true && _currentVerticalSpeed > 0)
				_currentVerticalSpeed = 0;
			
			transform.position += Vector3.up * _currentVerticalSpeed * Time.deltaTime;
		}

		private bool HasBufferedJump() => _lastJumpBtnPressTime + _settings.BufferedJumpTime > Time.time;

		private void Jump()
		{
			if (_collisionDetector.BoxCollisions.BottomCollision == false) return;

			_currentVerticalSpeed = _settings.JumpHeight;
			_animator.PlayJump();
		}
	}
}