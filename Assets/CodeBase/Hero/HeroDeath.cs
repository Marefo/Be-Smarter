using UnityEngine;

namespace CodeBase.Hero
{
	public class HeroDeath : MonoBehaviour
	{
		[SerializeField] private ParticleSystem _vfxPrefab;
		[SerializeField] private SpriteRenderer _spriteRenderer;
		
		public void Die()
		{
			if (TryGetComponent(out HeroMovement heroMove))
				heroMove.enabled = false;

			HideSprite();
			SpawnVfx();
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