using System;
using System.Collections.Generic;
using CodeBase.Units.Enemy;
using UnityEngine;

namespace CodeBase.Logic
{
	public class WayPointsSetter : MonoBehaviour
	{
		[SerializeField] private TriggerListener _setZone;
		[Space(10)]
		[SerializeField] private List<Transform> _wayPoints;

		private void OnEnable()
		{
			_setZone.Entered += OnSetZoneEnter;
		}

		private void OnDisable()
		{
			_setZone.Entered -= OnSetZoneEnter;
		}

		private void OnSetZoneEnter(Collider2D obj)
		{
			if (obj.TryGetComponent(out EnemyPatroller patroller))
				patroller.SetWayPoints(_wayPoints);
		}
	}
}