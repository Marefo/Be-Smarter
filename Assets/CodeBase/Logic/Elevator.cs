using System;
using CodeBase.Infrastructure;
using CodeBase.Units.Hero;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace CodeBase.Logic
{
	public class Elevator : MonoBehaviour
	{
		[SerializeField] private TriggerListener _activeZone;
		[Space(10)] 
		[SerializeField] private float _moveDelay;
		[SerializeField] private float _moveHeight;
		[SerializeField] private float _moveDuration;
		[Space(10)]
		[SerializeField] private float _loadNextLevelDelay;
		
		private LevelLoader _levelLoader;
		private CoroutineRunner _coroutineRunner;
		private Coroutine _loadNextLevelCoroutine;
		private bool _isHeroInside = false;

		[Inject]
		private void Construct(LevelLoader levelLoader, CoroutineRunner coroutineRunner)
		{
			_levelLoader = levelLoader;
			_coroutineRunner = coroutineRunner;
		}
		
		private void OnEnable()
		{
			_activeZone.Entered += OnActiveZoneEnter;
			_activeZone.Canceled += OnActiveZoneCancel;
		}

		private void OnDisable()
		{
			_activeZone.Entered -= OnActiveZoneEnter;
			_activeZone.Canceled -= OnActiveZoneCancel;
		}

		private void OnActiveZoneCancel(Collider2D obj)
		{
			if (_isHeroInside == false || obj.TryGetComponent(out HeroMovement heroMovement) == false) return;
			
			if(_loadNextLevelCoroutine != null)
				StopCoroutine(_loadNextLevelCoroutine);

			_isHeroInside = false;
			heroMovement.transform.SetParent(null);
		}

		private void OnActiveZoneEnter(Collider2D obj)
		{
			if (_isHeroInside == true || obj.TryGetComponent(out HeroMovement heroMovement) == false) return;
			
			_isHeroInside = true;
			heroMovement.transform.SetParent(transform);
			MoveWithDelay();
			LoadNextLevelWithDelay();
		}

		private void LoadNextLevelWithDelay()
		{
			_loadNextLevelCoroutine = 
				StartCoroutine(_coroutineRunner.CallWithDelayCoroutine(_levelLoader.LoadNextLevel, _loadNextLevelDelay));
		}

		private void MoveWithDelay() => _coroutineRunner.CallWithDelay(Move, _moveDelay);

		private void Move()
		{
			Vector3 currentPosition = transform.position;
			transform.DOMove(currentPosition + Vector3.up * _moveHeight, _moveDuration)
				.OnComplete(OnMoveComplete);
		}

		private void OnMoveComplete()
		{
			if(_isHeroInside) return;

			Vector3 currenPosition = transform.position;
			transform.DOMove(currenPosition - Vector3.up * _moveHeight, _moveDuration);
		}
	}
}