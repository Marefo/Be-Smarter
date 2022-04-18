using CodeBase.Logic;
using UnityEngine;

namespace CodeBase.Units.Enemy
{
	public class EnemyDeath : MonoBehaviour, IMortal
	{
		[SerializeField] private ParticleSystem _vfxPrefab;
		
		public void Die()
		{
			SpawnVfx();
			Destroy(gameObject);
		}
		
		private void SpawnVfx()
		{
			Instantiate(_vfxPrefab, transform.position, Quaternion.identity);
		}
	}
}