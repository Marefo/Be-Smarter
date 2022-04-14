using System;
using System.Linq;
using CodeBase.Hero;
using CodeBase.Logic;
using UnityEngine;

namespace CodeBase.DeathRay
{
	[RequireComponent(typeof(Collider2D))]
	public class DeathRayHoldingButton : DeathRayButton
	{
		public override event Action StateChanged;

		[SerializeField] private TriggerListener _activeArea;

		private void Start()
		{
			_activeArea.Entered += ChangeState;
			_activeArea.Canceled += ChangeState;
		}

		private void OnDestroy()
		{
			_activeArea.Entered -= ChangeState;
			_activeArea.Canceled -= ChangeState;
		}

		private void ChangeState(Collider2D obj)
		{
			if (obj.TryGetComponent(out HeroMovement heroMove))
				StateChanged?.Invoke();
		}
	}
}