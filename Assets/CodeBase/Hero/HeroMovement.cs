using System;
using System.Collections.Generic;
using System.Linq;
using CodeBase.Collisions;
using CodeBase.Infrastructure;
using CodeBase.Services;
using UnityEngine;
using Vertx.Debugging;
using Zenject;

namespace CodeBase.Hero
{
	[RequireComponent(typeof(CollisionDetector), typeof(BoxCollider2D), typeof(SpriteRenderer))]
	public class HeroMovement : MonoBehaviour
	{
		[SerializeField] private float _maxMoveSpeed = 3;
		[SerializeField] private float _acceleration = 90;
		[SerializeField] private float _deAcceleration = 60;
		[Space(10)]
		[SerializeField] private float _climbAssistHeight = 0.01f;
		[Space(10)]
		[SerializeField] private float _minFallSpeed = 2;
		[SerializeField] private float _maxFallSpeed = 4;
		[Space(10)]
		[SerializeField] private float _jumpHeight = 3;
		[SerializeField] private float _apexThreshold = 2;
		[SerializeField] private float _apexAdditionalSpeed = 1.5f;
		[SerializeField] private float _bufferedJumpTime = 0.05f;
		
		private readonly int _closestPointFindAttempts = 5;
		
		private IInputService _inputService;
		private SpriteRenderer _sprite;
		private Vector2 _colliderBottomCenter;
		private CollisionDetector _collisionDetector;
		private CoroutineRunner _coroutineRunner;
		private float _currentHorizontalSpeed;
		private float _currentVerticalSpeed;
		private float _lastMoveDirection;
		private float _apexPoint;
		private Vector2 _velocity;
		private Vector3 _lastPosition;
		private float _fallSpeed;
		private float _lastJumpBtnPressTime;
		private bool _active = false;

		[Inject]
		private void Construct(IInputService inputService, CoroutineRunner coroutineRunner)
		{
			_inputService = inputService;
			_coroutineRunner = coroutineRunner;
			_sprite = GetComponent<SpriteRenderer>();
			_collisionDetector = GetComponent<CollisionDetector>();
		}
		
		private void Start()
		{
			_inputService.JumpBtnPressed += OnJumpBtnPress;
			_collisionDetector.BottomCollided += OnBottomCollide;

			Activate();
		}

		private void OnDestroy()
		{
			_inputService.JumpBtnPressed -= OnJumpBtnPress;
			_collisionDetector.BottomCollided -= OnBottomCollide;
		}

		private void Update()
		{
			if(_active == false) return;
			
			CalculateVelocity();
			FlipSprite();
			Walk();
			CalculateJumpApex();
			CalculateGravity();
			CalculateJump();
		}

		private void OnBottomCollide()
		{
			if (HasBufferedJump())
				Jump();
		}

		private void OnJumpBtnPress()
		{
			_lastJumpBtnPressTime = Time.time;
			Jump();
		}

		private void Activate() => _coroutineRunner.CallWithDelay(() => _active = true, 0.5f);
		
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

		private void Walk()
		{
			if (CanMove() == false)
			{
				if(TryClimbAssist() == false)
					MoveToClosestAvailablePoint();
				
				return;
			}

			if (_inputService.AxisX != 0)
			{
				ResetSpeedForOppositeDirection();

				float apexSpeed = Mathf.Sign(_inputService.AxisX) * _apexAdditionalSpeed * _apexPoint;
				
				_currentHorizontalSpeed += _inputService.AxisX * _acceleration * Time.deltaTime;
				_currentHorizontalSpeed = Mathf.Clamp(_currentHorizontalSpeed, -_maxMoveSpeed, _maxMoveSpeed);
				_currentHorizontalSpeed += apexSpeed * Time.deltaTime;
			}
			else
				_currentHorizontalSpeed = Mathf.MoveTowards(_currentHorizontalSpeed, 0, _deAcceleration * Time.deltaTime);

			transform.position += Vector3.right * _currentHorizontalSpeed * Time.deltaTime;
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
			return _inputService.AxisX > 0 && _collisionDetector.BoxCollisions.RightCollision == false ||
			       _inputService.AxisX < 0 && _collisionDetector.BoxCollisions.LeftCollision == false ||
			       _lastMoveDirection > 0 && _collisionDetector.BoxCollisions.RightCollision == false ||
			       _lastMoveDirection < 0 && _collisionDetector.BoxCollisions.LeftCollision == false;
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
					
			bool canClimbAssist = _collisionDetector.Bounds.min.y + _climbAssistHeight >= collided.bounds.max.y;
			if (canClimbAssist)
			{
				float collidedMinX = collided.bounds.min.x;
				float collidedMaxX = collided.bounds.max.x;
				
				float closestPositionX =
					Mathf.Abs(transform.position.x - collidedMinX) <
					Mathf.Abs(transform.position.x - collidedMaxX)
						? collidedMinX
						: collidedMaxX;

				float climbedPositionY = transform.position.y + collided.bounds.max.y + _collisionDetector.VerticalRaysLength - _collisionDetector.Bounds.min.y;
				
				transform.position = new Vector2(closestPositionX, climbedPositionY);
			}

			return canClimbAssist;
		}

		private void CalculateGravity() {
			if (_collisionDetector.BoxCollisions.BottomCollision == false)
				_currentVerticalSpeed = Mathf.Clamp(_currentVerticalSpeed - _fallSpeed * Time.deltaTime, -_maxFallSpeed, float.MaxValue);
			else if(_currentVerticalSpeed < 0)
				_currentVerticalSpeed = 0;
		}

		private void CalculateJumpApex() {
			if (_collisionDetector.BoxCollisions.BottomCollision == false)
			{
				_apexPoint = Mathf.InverseLerp(_apexThreshold, 0, Mathf.Abs(_velocity.y));
				_fallSpeed = Mathf.Lerp(_minFallSpeed, _maxFallSpeed, _apexPoint);
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

		private bool HasBufferedJump() => _lastJumpBtnPressTime + _bufferedJumpTime > Time.time;

		private void Jump()
		{
			if (_collisionDetector.BoxCollisions.BottomCollision == false) return;

			_currentVerticalSpeed = _jumpHeight;
		}
	}
}