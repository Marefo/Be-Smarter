using System;
using System.Collections.Generic;
using UnityEngine;

namespace CodeBase.DeathRay
{
	public class DeathRaySystem : MonoBehaviour
	{
		[SerializeField] private List<DeathRay> _deathRays;
		[Space(10)]
		[SerializeField] private List<DeathRayButton> _deathRayButton;

		private void OnEnable()
		{
			_deathRayButton.ForEach(btn => btn.StateChanged += OnStateChange);
		}

		private void OnDisable()
		{
			_deathRayButton.ForEach(btn => btn.StateChanged -= OnStateChange);
		}

		private void OnStateChange() => _deathRays.ForEach(ray => ray.ChangeState());
	}
}