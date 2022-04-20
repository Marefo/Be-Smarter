using System.Collections.Generic;
using CodeBase.Collisions;
using CodeBase.Infrastructure;
using CodeBase.Logic;
using CodeBase.StaticData;
using DG.Tweening;
using UnityEngine;

namespace CodeBase.Units.Enemy
{
	[RequireComponent(typeof(CollisionDetector), typeof(SpriteRenderer), typeof(UnitAnimator))]
	public abstract class EnemyMovement : UnitMovement, IHoldingBtnActivator
	{
		[SerializeField] protected EnemyMovementSettings _enemyMovementSettings;

		protected int _moveDirection => GetMoveDirection();
		
		protected CoroutineRunner _coroutineRunner;
		protected SpriteRenderer _sprite;
		protected CollisionDetector _collisionDetector;
		protected float _currentHorizontalSpeed = 0;
		protected bool _inputDisabled = false;

		public void Construct(CoroutineRunner coroutineRunner)
		{
			_coroutineRunner = coroutineRunner;
		}

		protected void Init()
		{
			_sprite = GetComponent<SpriteRenderer>();
			_collisionDetector = GetComponent<CollisionDetector>();
		}

		protected abstract int GetMoveDirection();
		
		protected void FlipSprite()
		{
			if (_moveDirection > 0)
				transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
			else if (_moveDirection < 0)
				transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
		}

		protected void Tilt()
		{
			Vector3 targetRotVector = new Vector3(0, 0, 
				Mathf.Lerp(-_enemyMovementSettings.MaxTilt, _enemyMovementSettings.MaxTilt, Mathf.InverseLerp(-1, 1, Mathf.Clamp(_currentHorizontalSpeed, -1, 1))));
			transform.rotation = Quaternion.RotateTowards(
				transform.rotation, Quaternion.Euler(targetRotVector), _enemyMovementSettings.TiltSpeed * Time.deltaTime);
		}

		protected void TryClimbAssist()
		{
			Collider2D collided = null;

			if (_moveDirection > 0)
				collided = _collisionDetector.GetFirstRightSideCollision();
			else if (_moveDirection < 0)
				collided = _collisionDetector.GetFirstLeftSideCollision();

			if(collided == null) return;

			Vector2 climbPoint = _collisionDetector.GetClimbPointToCollider(collided, _enemyMovementSettings.ClimbAssistHeight);
			bool canClimbAssist = climbPoint != Vector2.zero;

			if (canClimbAssist && _inputDisabled == false)
			{
				_inputDisabled = true;
				transform.DOMove(climbPoint, _enemyMovementSettings.ClimbDuration).OnComplete(() => _inputDisabled = false);
			}
		}

		protected bool BlockedByCube()
		{
			_collisionDetector.TryGetComponentFromLeftCollisions(out List<Cube.Cube> leftSideCubes);
			_collisionDetector.TryGetComponentFromRightCollisions(out List<Cube.Cube> rightSideCubes);

			return _moveDirection > 0 && rightSideCubes.Count > 0 || _moveDirection < 0 && leftSideCubes.Count > 0;
		}

		protected bool CanMove()
		{
			return _inputDisabled == false &&
			       (_moveDirection > 0 && _collisionDetector.BoxCollisions.RightCollision == false ||
			        _moveDirection < 0 && _collisionDetector.BoxCollisions.LeftCollision == false);
		}
	}
}