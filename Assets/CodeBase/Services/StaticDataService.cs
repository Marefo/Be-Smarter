using System.Collections.Generic;
using System.Linq;
using CodeBase.EnemySpawner;
using CodeBase.Infrastructure;
using CodeBase.StaticData;
using CodeBase.Units.Enemy;
using UnityEngine;
using Zenject;

namespace CodeBase.Services
{
	public class StaticDataService : IInitializable
	{
		private const string EnemiesPath = "StaticData/Enemies";
		private const string LevelsPath = "StaticData/Levels";
		private const string GameSettingsPath = "StaticData/GameSettings";

		private Dictionary<EnemyTypeId, EnemyStaticData> _enemies;
		private Dictionary<string, LevelStaticData> _levels;
		private GameSettings _gameSettings;

		public void Initialize() => LoadResources();

		private void LoadResources()
		{
			_enemies = Resources.LoadAll<EnemyStaticData>(EnemiesPath).ToDictionary(x => x.EnemyTypeId, x => x);
			_levels = Resources.LoadAll<LevelStaticData>(LevelsPath).ToDictionary(x => x.Name, x => x);
			_gameSettings = Resources.Load<GameSettings>(GameSettingsPath);
		}


		public EnemyStaticData LoadEnemyData(EnemyTypeId enemyTypeId) => 
			_enemies.TryGetValue(enemyTypeId, out EnemyStaticData enemyData) ? enemyData : null;

		public LevelStaticData LoadLevelData(string name) =>
			_levels.TryGetValue(name, out LevelStaticData levelData) ? levelData : null;

		public GameSettings LoadGameSettings() => _gameSettings;
	}
}