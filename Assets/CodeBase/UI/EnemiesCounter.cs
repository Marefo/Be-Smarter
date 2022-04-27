using CodeBase.Infrastructure;
using CodeBase.Units;
using CodeBase.Units.Enemy;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Zenject;

namespace CodeBase.UI
{
	public class EnemiesCounter : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _textField;
		[Space(10)]
		[SerializeField] private float _punchScaleForce = 0.1f;
		[SerializeField] private float _punchScaleDuration = 0.1f;
		
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
			_textField.transform.DOPunchScale(Vector3.one * _punchScaleForce, _punchScaleDuration, 1);
		}

		private void UpdateText()
		{
			_textField.text = $"{_currentEnemiesCount}/{_maxEnemiesCount}";
		}
	}
}