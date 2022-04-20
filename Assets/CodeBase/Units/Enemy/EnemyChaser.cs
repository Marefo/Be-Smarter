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
		[SerializeField] private GameObject _landVfx;

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

		protected override int GetMoveDirection() => _hero.position.x > transform.position.x ? 1 : -1;

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
			SpawnLandVfx();
		}

		private void SpawnLandVfx()
		{
			Instantiate(_landVfx, transform.position - Vector3.up * _sprite.bounds.size.y / 2, Quaternion.identity);
		}

		private void Jump()
		{
			FlipSprite();
			
			_isJumping = true;
			_isJumpKilled = false;
			
			Vector3 currentPosition = transform.position;
			currentPosition.y = _hero.position.y;
			Vector3 jumpTargetPosition = Vector3.Lerp(currentPosition, _hero.position, _chaserSettings.JumpPercentLength / 100);
			
			_doJump = transform.DOJump(jumpTargetPosition, _chaserSettings.JumpHeight, 1, _chaserSettings.JumpDuration)
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
				_currentVerticalSpeed -= _enemyMovementSettings.FallSpeed * Time.deltaTime;
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
				_currentHorizontalSpeed += _moveDirection * _enemyMovementSettings.Acceleration * Time.deltaTime;
				_currentHorizontalSpeed = Mathf.Clamp(_currentHorizontalSpeed, -_enemyMovementSettings.MaxMoveSpeed, _enemyMovementSettings.MaxMoveSpeed);
			}
			else
				_currentHorizontalSpeed = Mathf.MoveTowards(_currentHorizontalSpeed, 0, _enemyMovementSettings.DeAcceleration * Time.deltaTime);

			transform.position += Vector3.right * _currentHorizontalSpeed * Time.deltaTime;
			
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