using System;
using CodeBase.Units.Enemy;
using UnityEngine;

namespace CodeBase.Units
{
	public abstract class UnitDeath : MonoBehaviour
	{
		public abstract event Action<UnitDeath> Died;
		public event Action<UnitDeath> Destroyed;
		
		protected virtual void OnDestroy() => Destroyed?.Invoke(this);

		public abstract void Die();
		public abstract void Immobilize();
	}
}