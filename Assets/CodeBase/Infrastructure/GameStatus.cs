using System;
using System.Collections.Generic;
using CodeBase.Audio;
using CodeBase.Infrastructure.AssetManagement;
using CodeBase.UI;
using CodeBase.Units;
using CodeBase.Units.Enemy;
using CodeBase.Units.Hero;
using UnityEngine;

namespace CodeBase.Infrastructure
{
	public class GameStatus
	{
		public event Action Won;
		public event Action Lost;
		public event Action<int> EnemiesInitialized;
		public event Action<UnitDeath> EnemyDied;

		private readonly LoadingCurtain _loadingCurtain;
		private readonly AssetProvider _assetProvider;
		private readonly SFXPlayer _sfxPlayer;

		private HeroDeath _hero;
		private List<UnitDeath> _enemies;
		private AudioClip _winSfx;

		public GameStatus(AssetProvider assetProvider, SFXPlayer sfxPlayer)
		{
			_assetProvider = assetProvider;
			_sfxPlayer = sfxPlayer;
			
			Init();
		}

		public void SetUnits(HeroDeath hero, List<UnitDeath> enemies)
		{
			_hero = hero;
			_enemies = enemies;

			EnemiesInitialized?.Invoke(enemies.Count);
			SubscribeUnitEvents();
		}

		private void Init() => _winSfx = _assetProvider.LoadClip(AssetPath.WinAudioClip);

		private void SubscribeUnitEvents()
		{
			_hero.Died += OnHeroDie;
			_hero.Destroyed += UnSubscribeHeroEvents;
			_enemies.ForEach(SubscribeEnemyEvents);
		}

		private void SubscribeEnemyEvents(UnitDeath enemy)
		{
			enemy.Died += OnEnemyDie;
			enemy.Destroyed += UnSubscribeEnemyEvents;
		}

		private void OnHeroDie(UnitDeath unitDeath)
		{
			Debug.Log("Lose!");
			Lost?.Invoke();
		}

		private void UnSubscribeHeroEvents(UnitDeath hero)
		{
			hero.Died -= OnHeroDie;
			hero.Destroyed -= UnSubscribeHeroEvents;
		}

		private void OnEnemyDie(UnitDeath enemy)
		{
			EnemyDied?.Invoke(enemy);
			UnSubscribeEnemyEvents(enemy);

			if (enemy != null)
				_enemies.Remove(enemy);
			else
				_enemies.RemoveAll(x => x == null);
			
			if (_enemies.Count == 0)
				OnWin();
		}

		private void OnWin()
		{
			Debug.Log("Won!");
			_sfxPlayer.Play(_winSfx);
			Won?.Invoke();
		}

		private void UnSubscribeEnemyEvents(UnitDeath enemy)
		{
			enemy.Died -= OnEnemyDie;
			enemy.Destroyed -= UnSubscribeEnemyEvents;
		}
	}
}