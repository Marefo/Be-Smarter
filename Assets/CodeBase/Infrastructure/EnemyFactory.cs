using CodeBase.EnemySpawner;
using CodeBase.Extensions;
using CodeBase.Services;
using CodeBase.StaticData;
using CodeBase.Units.Enemy;
using CodeBase.Units.Hero;
using UnityEngine;

namespace CodeBase.Infrastructure
{
	public class EnemyFactory
	{
		private readonly StaticDataService _staticDataService;
		private readonly CoroutineRunner _coroutineRunner;
		private readonly LevelPreparer _levelPreparer;

		public EnemyFactory(StaticDataService staticDataService, CoroutineRunner coroutineRunner)
		{
			_staticDataService = staticDataService;
			_coroutineRunner = coroutineRunner;
		}

		public GameObject CreateEnemy(EnemyTypeId enemyTypeId, Vector3 position)
		{
			EnemyStaticData enemyData = _staticDataService.LoadEnemyData(enemyTypeId);
			GameObject enemy = Object.Instantiate(enemyData.Prefab, position, Quaternion.identity);
			EnemyMovement movement = enemy.GetComponentInObjectOrChildren<EnemyMovement>();
			
			movement.Construct(_coroutineRunner);

			return enemy;
		}
	}
}