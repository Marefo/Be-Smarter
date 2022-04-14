using System;
using CodeBase.Collisions;
using CodeBase.Services;
using UnityEngine;
using Zenject;

namespace CodeBase.Hero
{
	[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(SpriteRenderer))]
	[RequireComponent(typeof(CollisionDetector))]
	public class HeroMovement : MonoBehaviour
	{
		[SerializeField] private float _acceleration = 90;
		[SerializeField] private float _deAcceleration = 60;
		[SerializeField] private float _maxMoveSpeed;
		
		private float _moveSpeed = 2;
		private float _jumpForce = 3;
		
		private IInputService _inputService;
		private Rigidbody2D _rigidbody;
		private SpriteRenderer _sprite;
		private Vector2 _colliderBottomCenter;
		private CollisionDetector _collisionDetector;
		private float _currentHorizontalSpeed;

		[Inject]
		private void Construct(IInputService inputService)
		{
			_inputService = inputService;
			_rigidbody = GetComponent<Rigidbody2D>();
			_sprite = GetComponent<SpriteRenderer>();
			_collisionDetector = GetComponent<CollisionDetector>();
		}
		
		private void Start()
		{
			_inputService.JumpBtnPressed += Jump;
		}

		private void OnDestroy()
		{
			_inputService.JumpBtnPressed -= Jump;
		}

		private void Update()
		{
			FlipSprite();
			Move();
		}

		private void FlipSprite()
		{
			if (_inputService.AxisX > 0)
				_sprite.flipX = false;
			else if (_inputService.AxisX < 0)
				_sprite.flipX = true;
		}

		private void Move()
		{
			if (
				_inputService.AxisX > 0 && _collisionDetector.BoxCollisions.RightCollision ||
				_inputService.AxisX < 0 && _collisionDetector.BoxCollisions.LeftCollision
				)
				return;

			if (_inputService.AxisX != 0) {
				_currentHorizontalSpeed += _inputService.AxisX * _acceleration * Time.deltaTime;
				_currentHorizontalSpeed = Mathf.Clamp(_currentHorizontalSpeed, -_maxMoveSpeed, _maxMoveSpeed);
			}
			else
				_currentHorizontalSpeed = Mathf.MoveTowards(_currentHorizontalSpeed, 0, _deAcceleration * Time.deltaTime);

			transform.position += Vector3.right * _currentHorizontalSpeed;
		}

		private void Jump()
		{
			if(_collisionDetector.BoxCollisions.BottomCollision == false) return;
			
			_rigidbody.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
		}
	}
}