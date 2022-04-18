using System;
using CodeBase.Logic;
using CodeBase.Services;
using CodeBase.Units.Hero;
using UnityEngine;
using Zenject;

namespace CodeBase.DeathRay
{
	[RequireComponent(typeof(Interactable))]
	public class DeathRaySwitchingButton : DeathRayButton
	{
		public override event Action StateChanged;

		[SerializeField] private TriggerListener _interactionZoneTrigger;
		
		private IInputService _inputService;
		private Interactable _interactable;
		private bool _canInteract = false;

		[Inject]
		private void Construct(IInputService inputService)
		{
			_inputService = inputService;
			_interactable = GetComponent<Interactable>();
		} 
		
		private void Start()
		{
			_interactionZoneTrigger.Entered += OnInteractionZoneEnter;
			_interactionZoneTrigger.Canceled += OnInteractionZoneCancel;
			_inputService.InteractBtnPressed += OnInteractBtnPress;
		}

		private void OnDestroy()
		{
			_interactionZoneTrigger.Entered -= OnInteractionZoneEnter;
			_interactionZoneTrigger.Canceled -= OnInteractionZoneCancel;
			_inputService.InteractBtnPressed -= OnInteractBtnPress;
		}

		private void OnInteractionZoneEnter(Collider2D col)
		{
			if (col.TryGetComponent(out HeroMovement heroMove))
				EnableInteraction();
		}

		private void OnInteractionZoneCancel(Collider2D other)
		{
			if (other.TryGetComponent(out HeroMovement heroMove))
				DisableInteraction();
		}

		private void EnableInteraction()
		{
			_canInteract = true;
			_interactable.Enable();
		}

		private void DisableInteraction()
		{
			_canInteract = false;
			_interactable.Disable();
		}

		private void OnInteractBtnPress()
		{
			if(_canInteract == false) return;		
			
			StateChanged?.Invoke();
		}
	}
}