using System;
using CodeBase.Logic;
using CodeBase.Services;
using UnityEngine;
using Zenject;

namespace CodeBase.Units.Hero
{
	[RequireComponent(typeof(HeroMovement))]
	public class HeroInteractable : MonoBehaviour
	{
		[SerializeField] private TriggerListener _interactionZone;
		
		private IInputService _inputService;
		private HeroMovement _heroMovement;
		private Interactable _interactable;
		private Interactable _nextInteractable;
		private bool _inInteractionZone = false;
		private bool _canInteract = false;

		[Inject]
		private void Construct(IInputService inputService)
		{
			_inputService = inputService;
		}

		private void Awake()
		{
			_heroMovement = GetComponent<HeroMovement>();
		}

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

		private void Update() => ControlRightDirection();

		private void OnInteractionZoneEnter(Collider2D obj)
		{
			if (obj.TryGetComponent(out Interactable interactable) == false) return;
			
			if (_interactable != null)
			{
				if (_nextInteractable == null && interactable != _interactable)
					_nextInteractable = interactable;
				
				return;
			}

			_interactable = interactable;
			EnableInteraction();
			_inInteractionZone = true;
		}

		private void OnInteractionZoneCancel(Collider2D obj)
		{
			if (obj.TryGetComponent(out Interactable interactable) == false) return;

			if (interactable == _nextInteractable)
			{
				_nextInteractable = null;
				return;
			}
			
			if (interactable != _interactable) return;

			DisableInteraction();
			
			if (_nextInteractable != null)
			{
				_interactable = _nextInteractable;
				_nextInteractable = null;
				EnableInteraction();
				_inInteractionZone = true;
			}
			else
			{
				_interactable = null;
				_inInteractionZone = false;
			}
		}

		private void OnInteractBtnPress()
		{
			if(_canInteract == false) return;
			
			_interactable.Interact(this);
		}
		
		private void ControlRightDirection()
		{
			if (_inInteractionZone == false) return;

			float rightDirection = transform.position.x - _interactable.transform.position.x > 0 ? -1 : 1;
			
			if (_heroMovement.MoveDirection == rightDirection && _canInteract == false)
				EnableInteraction();
			else if (_heroMovement.MoveDirection != rightDirection && _canInteract == true)
				DisableInteraction();
		}

		private void EnableInteraction()
		{
			_canInteract = true;
			_interactable.EnableInteraction();
		}
		
		private void DisableInteraction()
		{
			_canInteract = false;
			_interactable.DisableInteraction();
		}
	}
}