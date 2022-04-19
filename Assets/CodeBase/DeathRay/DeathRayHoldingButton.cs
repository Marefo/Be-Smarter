using System;
using System.Collections.Generic;
using System.Linq;
using CodeBase.Logic;
using CodeBase.Units;
using UnityEngine;

namespace CodeBase.DeathRay
{
	[RequireComponent(typeof(Collider2D), typeof(Animator))]
	public class DeathRayHoldingButton : DeathRayButton
	{
		public override event Action StateChanged;

		[SerializeField] private TriggerListener _activeArea;

		private Animator _animator;
		private readonly List<IHoldingBtnActivator> _activators = new List<IHoldingBtnActivator>();
		private bool _activated = false;
		
		private readonly int _isPressedHash = Animator.StringToHash("IsPressed");

		private void Awake()
		{
			_animator = GetComponent<Animator>();
		}

		private void OnEnable()
		{
			_activeArea.Entered += OnEnter;
			_activeArea.Canceled += OnCancel;
		}

		private void OnDisable()
		{
			_activeArea.Entered -= OnEnter;
			_activeArea.Canceled -= OnCancel;
		}

		private void OnEnter(Collider2D obj)
		{
			if (obj.TryGetComponent(out IHoldingBtnActivator activator) == false) return;

			if (_activated == false)
			{
				_activated = true;
				ChangeState();
				
				_animator.SetBool(_isPressedHash, true);
			}
				
			_activators.Add(activator);
		}

		private void OnCancel(Collider2D obj)
		{
			if (obj.TryGetComponent(out IHoldingBtnActivator activator) == false) return;
			
			_activators.Remove(activator);

			if (_activated == true && _activators.Count == 0)
			{
				_activated = false;
				ChangeState();
				
				_animator.SetBool(_isPressedHash, false);
			}
		}
		
		private void ChangeState() => StateChanged?.Invoke();
	}
}