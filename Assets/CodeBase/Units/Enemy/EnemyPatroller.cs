using System;
using System.Collections.Generic;
using UnityEngine;

namespace CodeBase.Units.Enemy
{
	public class EnemyPatroller : EnemyMovement
	{
		[SerializeField] private PatrollerSettings _patrollerSettings;
		[Space(10)] 
		[SerializeField] private ParticleSystem _brakeVfx;

		private int _previousPointIndex => _targetPointIndex != 0 ? _targetPointIndex - 1 : _wayPoints.Count - 1;
		private List<Transform> _wayPoints;
		private int _targetPointIndex = 0;
		private bool _patrolling = false;
		private float _currentVerticalSpeed = 0;
		private UnitAnimator _animator;
		private ParticleSystem.EmissionModule _brakeVfxEmission;

		private void Awake()
		{
			Init();
			_animator = GetComponent<UnitAnimator>();
		}

		private void Start() => InitBrakeVfx();

		private void Update()
		{
			UpdateCollisionDetector();
			
			if(_activated == false || _wayPoints == null) return;

			Patrol();
			Tilt();
			FlipWalkVfx();
			CalculateGravity();
			Move();
		}

		public void SetWayPoints(List<Transform> wayPoints)
		{
			if(_wayPoints != null) return;
			
			_wayPoints = wayPoints;
			StartPatrolling();
		}

		public override void Disable()
		{
			_inputDisabled = true;
			_currentHorizontalSpeed = 0;
			_animator.PlayIdle();
		}

		protected override int GetMoveDirection() => 
			_wayPoints[_previousPointIndex].position.x > _wayPoints[_targetPointIndex].position.x ? -1 : 1;

		private void UpdateCollisionDetector()
		{
			_collisionDetector.CalculateRaysPosition();
			_collisionDetector.UpdateBoxCollisions();
			_collisionDetector.UpdateCollisionEvents();
		}
		
		private void InitBrakeVfx()
		{
			_brakeVfxEmission = _brakeVfx.emission;
			_brakeVfxEmission.enabled = false;
		}

		private void PlayBrakeVfx() => _brakeVfxEmission.enabled = true;

		private void StopBrakeVfx() => _brakeVfxEmission.enabled = false;

		private void FlipWalkVfx()
		{
			ParticleSystem.VelocityOverLifetimeModule brakeVfxVelocityOverLifetime = _brakeVfx.velocityOverLifetime;

			if(_moveDirection > 0)
				brakeVfxVelocityOverLifetime.x = -1 * Mathf.Abs(brakeVfxVelocityOverLifetime.x.constant);
			else if(_moveDirection < 0)
				brakeVfxVelocityOverLifetime.x = Mathf.Abs(brakeVfxVelocityOverLifetime.x.constant);
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
						transform.position += dir.normalized * move.magnitude;
					}
					
					continue;
				}

				transform.position = currentTryPosition;
			}
		}
		
		private void StartPatrolling()
		{
			transform.position = new Vector3(_wayPoints[0].position.x, transform.position.y, transform.position.z);
			_targetPointIndex = 1;
			_patrolling = true;
		}

		private void Patrol()
		{
			if (ReachedTargetPoint() && _patrolling)
				OnReachTargetPoint();

			if (CanMove() == false)
			{
				TryClimbAssist();
				_currentHorizontalSpeed = 0;
				_animator.PlayWalk(0);
				return;
			}
			
			if (_patrolling)
			{
				StopBrakeVfx();
				FlipSprite();
				_currentHorizontalSpeed += _moveDirection * _enemyMovementSettings.Acceleration * Time.deltaTime;
				_currentHorizontalSpeed = Mathf.Clamp(_currentHorizontalSpeed, -_enemyMovementSettings.MaxMoveSpeed, _enemyMovementSettings.MaxMoveSpeed);
			}
			else
			{
				PlayBrakeVfx();
				_currentHorizontalSpeed = Mathf.MoveTowards(_currentHorizontalSpeed, 0, _enemyMovementSettings.DeAcceleration * Time.deltaTime);
			}

			float animationSpeed = Mathf.Clamp(Mathf.Abs(_currentHorizontalSpeed), 0, 1.5f);
			_animator.PlayWalk(animationSpeed);
		}

		private void FlipSprite()
		{
			if (_moveDirection > 0)
				transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
			else if (_moveDirection < 0)
				transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
		}
		
		private bool ReachedTargetPoint()
		{
			return _moveDirection > 0 && transform.position.x >= _wayPoints[_targetPointIndex].position.x ||
			       _moveDirection < 0 && transform.position.x <= _wayPoints[_targetPointIndex].position.x;
		}

		private void OnReachTargetPoint()
		{
			_patrolling = false;
			_coroutineRunner.CallWithDelay(SetNewTargetPoint, _patrollerSettings.ReachPointDelay);
		}

		private void SetNewTargetPoint()
		{
			if (_targetPointIndex == _wayPoints.Count - 1)
				_targetPointIndex = 0;
			else
				_targetPointIndex += 1;

			_patrolling = true;
		}

		private void CalculateGravity()
		{
			if (_collisionDetector.BoxCollisions.BottomCollision)
			{
				if(_currentVerticalSpeed < 0)
					_currentVerticalSpeed = 0;
			}
			else
			{
				_currentVerticalSpeed = Mathf.Clamp(_currentVerticalSpeed - _enemyMovementSettings.FallSpeed * Time.deltaTime,
					-_enemyMovementSettings.MaxFallSpeed, float.MaxValue);
			}
		}
	}
}