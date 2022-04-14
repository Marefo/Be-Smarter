using System;
using CodeBase.Collisions;
using CodeBase.Hero;
using CodeBase.Logic;
using UnityEngine;
using Zenject;

namespace CodeBase.Enemy
{
	public class EnemyChaser : EnemyMovement
	{
		[SerializeField] private TriggerListener _agroZone;
		[Space(10)]
		[SerializeField] private float _minDistanceToHero = 1;
		[SerializeField] private float _maxMoveSpeed = 1;
		[SerializeField] private float _acceleration = 1;
		[SerializeField] private float _deAcceleration;
		
		private Transform _hero;
		private SpriteRenderer _sprite;
		private CollisionDetector _collisionsDetector;
		private float _currentMoveSpeed = 0;
		private bool _canChase = false;

		[Inject]
		private void Construct(HeroMovement heroMovement)
		{
			_hero = heroMovement.transform;
			_sprite = GetComponent<SpriteRenderer>();
			_collisionsDetector = GetComponent<CollisionDetector>();
		}

		private void Start()
		{
			_agroZone.Entered += OnAgroZoneEnter;
			_agroZone.Canceled += OnAgroZoneCancel;
		}

		private void Update() => Chase();

		private void OnAgroZoneEnter(Collider2D obj)
		{
			if (obj.TryGetComponent(out HeroMovement heroMovement))
				_canChase = true;
		}

		private void OnAgroZoneCancel(Collider2D obj)
		{
			if (obj.TryGetComponent(out HeroMovement heroMovement))
				_canChase = false;
		}

		private void Chase()
		{
			int moveDirection = GetMoveDirection();

			if (
				moveDirection == 1 && _collisionsDetector.BoxCollisions.RightCollision ||
				moveDirection == -1 && _collisionsDetector.BoxCollisions.LeftCollision
			)
				return;

			if (_canChase == true) {
				Flip();
				Debug.Log(_currentMoveSpeed);
				_currentMoveSpeed += moveDirection * _acceleration * Time.deltaTime;
				_currentMoveSpeed = Mathf.Clamp(_currentMoveSpeed, -_maxMoveSpeed, _maxMoveSpeed);
			}
			else
				_currentMoveSpeed = Mathf.MoveTowards(_currentMoveSpeed, 0, _deAcceleration * Time.deltaTime);

			transform.position += Vector3.right * _currentMoveSpeed;
		}

		private void Flip() => _sprite.flipX = GetMoveDirection() == -1;

		private bool CanMoveToHero()
		{
			float heroX = _hero.position.x;
			float positionX = transform.position.x;

			int direction = GetMoveDirection();
			float distanceToHero = Mathf.Abs(direction == 1 ? heroX - positionX : positionX - heroX);

			return distanceToHero > _minDistanceToHero;
		}

		private int GetMoveDirection() => _hero.position.x > transform.position.x ? 1 : -1;
	}
}