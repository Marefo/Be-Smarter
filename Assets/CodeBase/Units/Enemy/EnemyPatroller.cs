using System;
using System.Collections.Generic;
using UnityEngine;

namespace CodeBase.Units.Enemy
{
	public class EnemyPatroller : EnemyMovement
	{
		[SerializeField] private PatrollerSettings _patrollerSettings;
		[Space(10)]
		[SerializeField] private List<Transform> _wayPoints;
		[Space(10)] 
		[SerializeField] private ParticleSystem _brakeVfx;

		private int _previousPointIndex => _targetPointIndex != 0 ? _targetPointIndex - 1 : _wayPoints.Count - 1;
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

		private void Start()
		{
			InitBrakeVfx();
			StartPatrolling();
		}

		private void Update()
		{
			_collisionDetector.CalculateRaysPosition();
			_collisionDetector.UpdateBoxCollisions();
			_collisionDetector.UpdateCollisionEvents();
			
			Move();
			Tilt();
			FlipWalkVfx();
			CalculateGravity();
		}

		public override void Disable()
		{
			_inputDisabled = true;
			_currentHorizontalSpeed = 0;
			_animator.PlayIdle();
		}

		protected override int GetMoveDirection() => 
			_wayPoints[_previousPointIndex].position.x > _wayPoints[_targetPointIndex].position.x ? -1 : 1;

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

		private void StartPatrolling()
		{
			transform.position = new Vector3(_wayPoints[0].position.x, transform.position.y, transform.position.z);
			_targetPointIndex = 1;
			_patrolling = true;
		}

		private void Move()
		{
			if (ReachedTargetPoint() && _patrolling)
				OnReachTargetPoint();
			
			if (CanMove() == false)
			{
				TryClimbAssist();
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
			
			transform.position += Vector3.right * _currentHorizontalSpeed * Time.deltaTime;
			
			float animationSpeed = Mathf.Clamp(Mathf.Abs(_currentHorizontalSpeed), 0, 1.5f);
			_animator.PlayWalk(animationSpeed);
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

		private void CalculateGravity() {
			if (_collisionDetector.BoxCollisions.BottomCollision == false)
				_currentVerticalSpeed -= _enemyMovementSettings.FallSpeed * Time.deltaTime;
			else if(_currentVerticalSpeed < 0)
				_currentVerticalSpeed = 0;
			
			transform.position += Vector3.up * _currentVerticalSpeed * Time.deltaTime;
		}
	}
}