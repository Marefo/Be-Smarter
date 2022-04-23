using System;
using CodeBase.Logic;
using CodeBase.Units.Hero;
using UnityEngine;

namespace CodeBase.DeathRay
{
	[RequireComponent(typeof(Interactable), typeof(Animator))]
	public class DeathRaySwitchingButton : DeathRayButton
	{
		public override event Action StateChanged;

		private Interactable _interactable;
		private Animator _animator;
		
		private readonly int _pressHash = Animator.StringToHash("Press");

		private void Awake()
		{
			_interactable = GetComponent<Interactable>();
			_animator = GetComponent<Animator>();
		}

		private void OnEnable()
		{
			_interactable.Interacted += OnInteract;
		}

		private void OnDisable()
		{
			_interactable.Interacted -= OnInteract;
		}

		private void OnInteract(HeroMovement heroMovement)
		{
			_animator.SetTrigger(_pressHash);
			StateChanged?.Invoke();
		}
	}
}