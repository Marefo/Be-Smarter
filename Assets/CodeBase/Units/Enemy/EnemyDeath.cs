using System;
using CodeBase.Audio;
using CodeBase.Logic;
using UnityEngine;

namespace CodeBase.Units.Enemy
{
	public class EnemyDeath : UnitDeath
	{
		public override event Action<UnitDeath> Died;
		
		[SerializeField] private GameObject _vfxPrefab;
		[SerializeField] private AudioClip _sfx;
		
		private SFXPlayer _sfxPlayer;

		public void Construct(SFXPlayer sfxPlayer)
		{
			_sfxPlayer = sfxPlayer;
		}
		
		public override void Die()
		{
			_sfxPlayer.Play(_sfx);
			SpawnVfx();
			Died?.Invoke(this);
			Destroy(gameObject);
		}

		public override void Immobilize() => Died?.Invoke(this);

		private void SpawnVfx() => Instantiate(_vfxPrefab, transform.position, Quaternion.identity);
	}
}