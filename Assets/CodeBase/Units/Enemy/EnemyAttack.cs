using CodeBase.Logic;
using CodeBase.Units.Hero;
using UnityEngine;

namespace CodeBase.Units.Enemy
{
	public class EnemyAttack : MonoBehaviour
	{
		[SerializeField] private TriggerListener _killZone;

		private void OnEnable()
		{
			_killZone.Entered += OnKillZoneEnter;
		}

		private void OnDisable()
		{
			_killZone.Entered -= OnKillZoneEnter;
		}

		private void OnKillZoneEnter(Collider2D obj)
		{
			if(obj.TryGetComponent(out HeroDeath heroDeath))
				heroDeath.Die();
		}
	}
}