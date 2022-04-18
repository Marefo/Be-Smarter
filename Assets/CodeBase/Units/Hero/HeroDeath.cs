using CodeBase.Logic;
using UnityEngine;

namespace CodeBase.Units.Hero
{
	public class HeroDeath : MonoBehaviour, IMortal
	{
		[SerializeField] private ParticleSystem _vfxPrefab;
		[SerializeField] private SpriteRenderer _spriteRenderer;
		
		public void Die()
		{
			if (TryGetComponent(out HeroMovement heroMove))
				heroMove.enabled = false;

			HideSprite();
			SpawnVfx();
			
			enabled = false;
		}

		private void SpawnVfx()
		{
			Instantiate(_vfxPrefab, transform.position, Quaternion.identity);
		}

		private void HideSprite()
		{
			_spriteRenderer.enabled = false;
		}
	}
}