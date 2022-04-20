using System;
using CodeBase.Logic;
using UnityEngine;

namespace CodeBase.Units.Enemy
{
	public class EnemyDeath : UnitDeath
	{
		public override event Action<UnitDeath> Died;
		
		[SerializeField] private GameObject _vfxPrefab;

		public override void Die()
		{
			SpawnVfx();
			Died?.Invoke(this);
			Destroy(gameObject);
		}

		public override void Immobilize() => Died?.Invoke(this);

		private void SpawnVfx() => Instantiate(_vfxPrefab, transform.position, Quaternion.identity);
	}
}