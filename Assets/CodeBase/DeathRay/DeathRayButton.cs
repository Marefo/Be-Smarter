using System;
using CodeBase.Hero;
using CodeBase.Logic;
using CodeBase.Services;
using UnityEngine;
using Zenject;

namespace CodeBase.DeathRay
{
	public class DeathRayButton : Interactable
	{
		public event Action Pressed;
		public override event Action InteractionEnabled;
		public override event Action InteractionDisabled;

		[SerializeField] private TriggerListener _interactionZoneTrigger;
		
		private IInputService _inputService;
		private bool _canInteract = false;

		[Inject]
		private void Construct(IInputService inputService)
		{
			_inputService = inputService;
		} 
		
		private void Start()
		{
			_interactionZoneTrigger.Entered += OnInteractionZoneEnter;
			_interactionZoneTrigger.Exited += OnInteractionZoneExit;
			_inputService.InteractBtnPressed += OnInteractBtnPress;
		}

		private void OnDestroy()
		{
			_interactionZoneTrigger.Entered -= OnInteractionZoneEnter;
			_interactionZoneTrigger.Exited -= OnInteractionZoneExit;
			_inputService.InteractBtnPressed -= OnInteractBtnPress;
		}

		private void OnInteractionZoneEnter(Collider2D col)
		{
			if (col.TryGetComponent(out HeroMove heroMove))
				EnableInteraction();
		}

		private void OnInteractionZoneExit(Collider2D other)
		{
			if (other.TryGetComponent(out HeroMove heroMove))
				DisableInteraction();
		}

		private void EnableInteraction()
		{
			_canInteract = true;
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
			
			Pressed?.Invoke();
		}
	}
}