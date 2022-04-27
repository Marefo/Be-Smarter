using System;
using System.Collections.Generic;
using CodeBase.Audio;
using CodeBase.Extensions;
using CodeBase.Logic;
using CodeBase.Services;
using CodeBase.StaticData;
using CodeBase.UI;
using CodeBase.Units;
using CodeBase.Units.Enemy;
using CodeBase.Units.Hero;
using UnityEngine;
using Zenject;

namespace CodeBase.Infrastructure
{
	public class LevelPreparer : MonoBehaviour
	{
		[SerializeField] private AudioClip _backgroundMusic;
		
		private CoroutineRunner _coroutineRunner;
		private StaticDataService _staticDataService;
		private LevelStaticData _levelData;
		private EnemyFactory _enemyFactory;
		private GameStatus _gameStatus;
		private HeroMovement _heroMovement;
		private LoadingCurtain _loadingCurtain;
		private readonly List<UnitDeath> _enemies = new List<UnitDeath>();
		private bool _unitsActivated = false;
		private BackgroundMusicPlayer _backgroundMusicPlayer;

		[Inject]
		private void Construct(GameStatus gameStatus, HeroMovement heroMovement, CoroutineRunner coroutineRunner, 
			EnemyFactory enemyFactory, StaticDataService staticDataService, LoadingCurtain loadingCurtain,
			BackgroundMusicPlayer backgroundMusicPlayer)
		{
			_gameStatus = gameStatus;
			_heroMovement = heroMovement;
			_coroutineRunner = coroutineRunner;
			_staticDataService = staticDataService;
			_enemyFactory = enemyFactory;
			_loadingCurtain = loadingCurtain;
			_backgroundMusicPlayer = backgroundMusicPlayer;
		}

		private void Awake()
		{
			LoadData();
			SpawnEnemies();
			InitGameStatus();
			StartUnitsActivation();
			_backgroundMusicPlayer.ChangeMusicTo(_backgroundMusic);
		}

		private void Start()
		{
			_loadingCurtain.BecameInvisible += OnLoadingCurtainBecomeInvisible;
		}

		private void OnDestroy()
		{
			_loadingCurtain.BecameInvisible -= OnLoadingCurtainBecomeInvisible;
		}

		private void LoadData() => _levelData = _staticDataService.LoadLevelData(gameObject.scene.name);

		private void SpawnEnemies()
		{
			foreach (EnemySpawnPointStaticData spawnPoint in _levelData.EnemySpawners)
			{
				GameObject enemy = _enemyFactory.CreateEnemy(spawnPoint.EnemyTypeId, spawnPoint.Position);
				UnitDeath enemyDeath = enemy.GetComponentInObjectOrChildren<UnitDeath>();
				
				_enemies.Add(enemyDeath);
			}
		}

		private void InitGameStatus()
		{
			HeroDeath hero = _heroMovement.GetComponent<HeroDeath>();
			_gameStatus.SetUnits(hero, _enemies);
		}
		
		private void StartUnitsActivation()
		{
			if (_loadingCurtain.Invisible)
				ActivateUnits();
		}
		
		private void OnLoadingCurtainBecomeInvisible()
		{
			if(_unitsActivated) return;
			ActivateUnits();
		}
		
		private void ActivateUnits()
		{
			_unitsActivated = true;
			_heroMovement.Activate();
			_enemies.ForEach(enemy => enemy.GetComponent<EnemyMovement>().Activate());
		}
	}
}