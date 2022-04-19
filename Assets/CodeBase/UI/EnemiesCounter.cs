using CodeBase.Infrastructure;
using CodeBase.Units;
using CodeBase.Units.Enemy;
using TMPro;
using UnityEngine;
using Zenject;

namespace CodeBase.UI
{
	public class EnemiesCounter : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _textField;
		
		private int _maxEnemiesCount;
		private int _currentEnemiesCount;

		[Inject]
		private void Construct(GameStatus gameStatus)
		{
			gameStatus.EnemiesInitialized += OnEnemiesInitializing;
			gameStatus.EnemyDied += OnEnemyDeath;
		}
		
		private void OnEnemiesInitializing(int enemiesCount)
		{
			_maxEnemiesCount = enemiesCount;
			_currentEnemiesCount = 0;
			UpdateText();
		}

		private void OnEnemyDeath(UnitDeath enemy)
		{
			_currentEnemiesCount += 1;
			UpdateText();
		}

		private void UpdateText()
		{
			_textField.text = $"{_currentEnemiesCount}/{_maxEnemiesCount}";
		}
	}
}