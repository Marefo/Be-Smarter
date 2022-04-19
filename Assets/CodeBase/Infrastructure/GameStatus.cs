using System;
using System.Collections.Generic;
using System.Linq;
using CodeBase.Units;
using CodeBase.Units.Enemy;
using CodeBase.Units.Hero;
using UnityEngine;

namespace CodeBase.Infrastructure
{
	public class GameStatus : IDisposable
	{
		public event Action Won;
		public event Action Lost;
		public event Action<int> EnemiesInitialized;
		public event Action<UnitDeath> EnemyDied;

		private HeroDeath _hero;
		private List<UnitDeath> _enemies;
		
		public void SetUnits(HeroDeath hero, List<UnitDeath> enemies)
		{
			_hero = hero;
			_enemies = enemies;

			EnemiesInitialized?.Invoke(enemies.Count);
			SubscribeEvents();
		}

		public void Dispose()
		{
			CleanUp();
		}

		private void SubscribeEvents()
		{
			_hero.Died += OnHeroDie;
			_enemies.ForEach(enemy => enemy.Died += OnEnemyDie);
		}

		private void CleanUp()
		{
			_hero.Died += OnHeroDie;
		}

		private void OnHeroDie(UnitDeath unitDeath)
		{
			Debug.Log("Lose!");
			Lost?.Invoke();
		}

		private void OnEnemyDie(UnitDeath enemy)
		{
			EnemyDied?.Invoke(enemy);
			enemy.Died -= OnEnemyDie;

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
	}
}