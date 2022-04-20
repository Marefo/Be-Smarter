using System;
using CodeBase.Logic;
using CodeBase.Units;
using CodeBase.Units.Hero;
using DG.Tweening;
using UnityEngine;

namespace CodeBase.Cube
{
	[RequireComponent(typeof(Rigidbody2D), typeof(Interactable))]
	public class Cube : MonoBehaviour, IHoldingBtnActivator
	{
		[SerializeField] private float _offsetX = 0.1f;
		[SerializeField] private float _pickUpDuration = 0.1f;
		[Space(10)]
		[SerializeField] private PushSettings _stayPushSettings;
		[SerializeField] private PushSettings _walkPushSettings;
		
		private Interactable _interactable;
		private Rigidbody2D _rigidbody;
		private HeroMovement _heroMovement;
		private HeroDeath _heroDeath;
		private bool _pickedUp = false;
		private int _defaultLayer;
		private Sequence _push;

		private void Awake()
		{
			_interactable = GetComponent<Interactable>();
			_rigidbody = GetComponent<Rigidbody2D>();
		}
		
		private void OnEnable()
		{
			_interactable.Interacted += OnInteract;
		}

		private void OnDisable()
		{
			_interactable.Interacted -= OnInteract;	
		}

		private void OnDestroy()
		{
			if (_heroDeath)
				_heroDeath.Died -= OnHeroDie;
		}

		private void Start()
		{
			_defaultLayer = gameObject.layer;
		}

		private void OnCollisionEnter2D(Collision2D col)
		{
			StopPush();
		}

		private void OnInteract(HeroMovement heroMovement)
		{
			_heroMovement = heroMovement;

			if (_heroDeath == null)
			{
				_heroDeath = _heroMovement.GetComponent<HeroDeath>();
				_heroDeath.Died += OnHeroDie;
			}
			
			if(_pickedUp)
				Drop();
			else
				PickUp();
		}

		private void OnHeroDie(UnitDeath obj) => Drop();

		private void PickUp()
		{
			_pickedUp = true;

			gameObject.layer = 0;
			_rigidbody.constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
			transform.SetParent(_heroMovement.transform);

			Vector3 pickedPosition = new Vector3(_offsetX, 0, 0);
			transform.DOLocalMove(pickedPosition, _pickUpDuration);
			
			_interactable.StartInteraction();
		}

		private void Drop()
		{
			_pickedUp = false;

			_interactable.FinishInteraction();
			
			gameObject.layer = _defaultLayer;
			_rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
			transform.SetParent(null);
			transform.localScale = Vector3.one;

			Push();
		}

		private void Push()
		{
			float pushDistance = _heroMovement.IsWalking ? _walkPushSettings.PushDistance * _heroMovement.SpeedPercent : _stayPushSettings.PushDistance;
			float pushHeight = _heroMovement.IsWalking ? _walkPushSettings.PushHeight * _heroMovement.SpeedPercent : _stayPushSettings.PushHeight;
			float pushDuration = _heroMovement.IsWalking ? _walkPushSettings.PushDuration * _heroMovement.SpeedPercent : _stayPushSettings.PushDuration;
			
			Vector3 currentPosition = transform.position;
			Vector3 targetPosition = currentPosition + Vector3.right * _heroMovement.MoveDirection * pushDistance;
			_push = transform.DOJump(targetPosition, pushHeight, 1, pushDuration);
		}

		private void StopPush() => _push?.Kill();
	}
}