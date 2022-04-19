using System;
using CodeBase.Logic;
using UnityEngine;

namespace CodeBase.Units.Hero
{
	public class HeroDeath : UnitDeath
	{
		public override event Action<UnitDeath> Died;
		
		[SerializeField] private ParticleSystem _vfxPrefab;
		[SerializeField] private SpriteRenderer _spriteRenderer;
		
		public override void Die()
		{
			if (TryGetComponent(out HeroMovement heroMove))
				heroMove.enabled = false;

			HideSprite();
			SpawnVfx();

			Died?.Invoke(this);
			enabled = false;
		}

		public override void Immobilize() => Died?.Invoke(this);

		private void SpawnVfx()
		{
			Instantiate(_vfxPrefab, transform.position, Quaternion.identity);
		}

		private void HideSprite() => _spriteRenderer.enabled = false;
	}
}