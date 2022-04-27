using System;
using CodeBase.Audio;
using CodeBase.Camera;
using CodeBase.Logic;
using UnityEngine;
using Zenject;

namespace CodeBase.Units.Hero
{
	public class HeroDeath : UnitDeath
	{
		public override event Action<UnitDeath> Died;

		[SerializeField] private CameraShake _cameraShake;
		[SerializeField] private CameraShakeSettings _cameraShakeSettings;
		[Space(10)]
		[SerializeField] private SpriteRenderer _spriteRenderer;
		[SerializeField] private GameObject _vfxPrefab;
		[SerializeField] private AudioClip _sfx;
		
		private SFXPlayer _sfxPlayer;
		private bool _dead = false;

		[Inject]
		private void Construct(SFXPlayer sfxPlayer)
		{
			_sfxPlayer = sfxPlayer;
		}
		
		public override void Die()
		{
			if(_dead) return;
			_dead = true;
			
			if (TryGetComponent(out HeroMovement heroMove))
				heroMove.enabled = false;

			_sfxPlayer.Play(_sfx);
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