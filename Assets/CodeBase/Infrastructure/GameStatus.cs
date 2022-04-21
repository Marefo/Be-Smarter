using System;
using System.Collections.Generic;
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
		
		private HeroDeath _hero;
		private List<UnitDeath> _enemies;

		public GameStatus(LoadingCurtain loadingCurtain)
		{
			_loadingCurtain = loadingCurtain;
		}

		public void SetUnits(HeroDeath hero, List<UnitDeath> enemies)
		{
			_hero = hero;
			_enemies = enemies;

			EnemiesInitialized?.Invoke(enemies.Count);
			SubscribeUnitEvents();
		}

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
			{
				Debug.Log("Won!");
				Won?.Invoke();
			}
		}

		private void UnSubscribeEnemyEvents(UnitDeath enemy)
		{
			enemy.Died -= OnEnemyDie;
			enemy.Destroyed -= UnSubscribeEnemyEvents;
		}
	}
}