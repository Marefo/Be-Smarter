using System;
using CodeBase.Camera;
using CodeBase.Logic;
using UnityEngine;

namespace CodeBase.Units.Hero
{
	public class HeroDeath : UnitDeath
	{
		public override event Action<UnitDeath> Died;

		[SerializeField] private CameraShake _cameraShake;
		[SerializeField] private CameraShakeSettings _cameraShakeSettings;
		[Space(10)]
		[SerializeField] private GameObject _vfxPrefab;
		[SerializeField] private SpriteRenderer _spriteRenderer;

		public override void Die()
		{
			if (TryGetComponent(out HeroMovement heroMove))
				heroMove.enabled = false;
			
			ShakeCamera();
			HideSprite();
			SpawnVfx();

			Died?.Invoke(this);
			enabled = false;
		}

		private void ShakeCamera()
		{
			_cameraShake.Shake(_cameraShakeSettings);
		}

		public override void Immobilize() => Died?.Invoke(this);

		private void SpawnVfx()
		{
			Instantiate(_vfxPrefab, transform.position, Quaternion.identity);
		}

		private void HideSprite() => _spriteRenderer.enabled = false;
	}
}