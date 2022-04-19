using System.Collections.Generic;
using CodeBase.Services;
using CodeBase.StaticData;
using CodeBase.Units;
using CodeBase.Units.Enemy;
using CodeBase.Units.Hero;
using UnityEngine;
using Zenject;

namespace CodeBase.Infrastructure
{
	public class LevelPreparer : MonoBehaviour
	{
		private CoroutineRunner _coroutineRunner;
		private StaticDataService _staticDataService;
		private LevelStaticData _levelData;
		private EnemyFactory _enemyFactory;
		private GameStatus _gameStatus;
		private HeroMovement _heroMovement;
		private readonly List<UnitDeath> _enemies = new List<UnitDeath>();

		[Inject]
		private void Construct(GameStatus gameStatus, HeroMovement heroMovement, CoroutineRunner coroutineRunner, EnemyFactory enemyFactory, StaticDataService staticDataService)
		{
			_gameStatus = gameStatus;
			_heroMovement = heroMovement;
			_coroutineRunner = coroutineRunner;
			_staticDataService = staticDataService;
			_enemyFactory = enemyFactory;
		}

		private void Start()
		{
			LoadData();
			SpawnEnemies();
			InitGameStatus();
		}

		private void LoadData()
		{
			_levelData = _staticDataService.LoadLevelData(gameObject.scene.name);
		}

		private void SpawnEnemies()
		{
			foreach (EnemySpawnPointStaticData spawnPoint in _levelData.EnemySpawners)
			{
				GameObject enemy = _enemyFactory.CreateEnemy(spawnPoint.EnemyTypeId, spawnPoint.Position);
				_enemies.Add(enemy.GetComponent<UnitDeath>());
			}
		}

		private void InitGameStatus()
		{
			HeroDeath hero = _heroMovement.GetComponent<HeroDeath>();
			_gameStatus.SetUnits(hero, _enemies);
		}
	}
}