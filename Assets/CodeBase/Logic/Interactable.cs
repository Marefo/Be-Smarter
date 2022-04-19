using System;
using CodeBase.Services;
using CodeBase.Units.Hero;
using UnityEngine;
using Zenject;

namespace CodeBase.Logic
{
	public class Interactable : MonoBehaviour
	{
		public event Action InteractionEnabled;
		public event Action InteractionDisabled;
		public event Action InteractionStarted;
		public event Action InteractionFinished;
		public event Action<HeroMovement> Interacted;
		
		[SerializeField] private TriggerListener _interactionZone;

		private IInputService _inputService;
		private HeroMovement _heroMovement;
		private bool _inInteractionZone = false;
		private bool _canInteract = false;

		[Inject]
		private void Construct(IInputService inputService) => _inputService = inputService;

		private void OnEnable()
		{
			_interactionZone.Entered += OnInteractionZoneEnter;
			_interactionZone.Canceled += OnInteractionZoneCancel;
			_inputService.InteractBtnPressed += OnInteractBtnPress;
		}

		private void OnDisable()
		{
			_interactionZone.Entered -= OnInteractionZoneEnter;
			_interactionZone.Canceled -= OnInteractionZoneCancel;
			_inputService.InteractBtnPressed -= OnInteractBtnPress;
		}

		private void Update()
		{
			ControlRightDirection();
		}

		public void StartInteraction() => InteractionStarted?.Invoke();

		public void FinishInteraction() => InteractionFinished?.Invoke();

		private void ControlRightDirection()
		{
			if (_inInteractionZone == false) return;

			float rightDirection = _heroMovement.transform.position.x - transform.position.x > 0 ? -1 : 1;

			if (_heroMovement.MoveDirection == rightDirection && _canInteract == false)
				EnableInteraction(_heroMovement);
			else if (_heroMovement.MoveDirection != rightDirection && _canInteract == true)
				DisableInteraction();
		}

		private void OnInteractionZoneEnter(Collider2D col)
		{
			if (col.TryGetComponent(out HeroMovement heroMovement) == false) return;
			
			_inInteractionZone = true;
			EnableInteraction(heroMovement);
		}

		private void OnInteractionZoneCancel(Collider2D other)
		{
			if (other.TryGetComponent(out HeroMovement heroMovement) == false) return;
			
			_inInteractionZone = false;
			DisableInteraction();
		}

		private void EnableInteraction(HeroMovement heroMovement)
		{
			_canInteract = true;
			_heroMovement = heroMovement;
			InteractionEnabled?.Invoke();
		}

		private void DisableInteraction()
		{
			_canInteract = false;
			InteractionDisabled?.Invoke();
		}

		private void OnInteractBtnPress()
		{
			if(_canInteract == false) return;
			
			Interacted?.Invoke(_heroMovement);
		}
	}
}