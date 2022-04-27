using System;
using CodeBase.Audio;
using CodeBase.Infrastructure;
using CodeBase.Logic;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace CodeBase.Claw
{
	public class Claw : MonoBehaviour
	{
		[SerializeField] private float _targetMoveToClawDuration = 0.1f;
		[SerializeField] private float _clawCatchDuration = 0.2f;
		[Space(10)]
		[SerializeField] private float _minDistanceForMoveUp = 0.2f;
		[SerializeField] private float _moveUpDelay = 0.5f;
		[SerializeField] private float _moveUpDuration = 0.2f;
		[SerializeField] private Vector3 _moveUpShake;
		[SerializeField] private float _moveUpShakeDuration = 0.3f;
		[Space(10)]
		[SerializeField] private TriggerListener _catchZone;
		[Space(10)]
		[SerializeField] private SpriteRenderer _mountSprite;
		[SerializeField] private SpriteRenderer _chainSprite;
		[SerializeField] private SpriteRenderer _clawSprite;
		[Space(10)]
		[SerializeField] private AudioClip _splatSfx;
		
		private CoroutineRunner _coroutineRunner;
		private SFXPlayer _sfxPlayer;
		private Vector3 _clawDefaultPosition;
		private Vector3 _chainDefaultPosition;
		private Vector2 _chainDefaultSize;
		private ICatchTarget _caughtTarget;
		private float _catchPositionY;

		[Inject]
		private void Construct(CoroutineRunner coroutineRunner, SFXPlayer sfxPlayer)
		{
			_coroutineRunner = coroutineRunner;
			_sfxPlayer = sfxPlayer;
		}

		private void OnEnable()
		{
			_catchZone.Entered += OnCatchZoneEnter;
		}

		private void OnDisable()
		{
			_catchZone.Entered -= OnCatchZoneEnter;
		}

		private void Start()
		{
			_clawDefaultPosition = _clawSprite.transform.position;
			_chainDefaultSize = _chainSprite.size;
		}

		private void OnCatchZoneEnter(Collider2D obj)
		{
			if (_caughtTarget != null) return;
			if (obj.TryGetComponent(out ICatchTarget catchTarget) == false) return;

			_caughtTarget = catchTarget;
			Catch();
		}

		private void Catch()
		{
			_catchPositionY = _caughtTarget.Transform.position.y;
			_caughtTarget.OnCatch();
			
			SetUpClaw(OnCatchComplete);
			SetUpTarget();
		}

		private void OnCatchComplete()
		{
			_sfxPlayer.Play(_splatSfx);
			_caughtTarget.Transform.SetParent(_clawSprite.transform);
			Shake(TryMoveUp);
		}
		
		private void TryMoveUp()
		{
			if(_catchPositionY < _clawDefaultPosition.y - _minDistanceForMoveUp)
				_coroutineRunner.CallWithDelay(() => MoveUp(() => Shake()), _moveUpDelay);
		}
		
		private void MoveUp(Action onComplete)
		{
			_chainSprite.transform.DOMove(_chainDefaultPosition, _moveUpDuration);
			_clawSprite.transform.DOMove(_clawDefaultPosition, _moveUpDuration).OnComplete(() => onComplete());
		}

		private void Shake(Action onComplete = null)
		{
			_clawSprite.transform.DOPunchPosition(_moveUpShake, _moveUpShakeDuration);
			_chainSprite.transform.DOPunchPosition(_moveUpShake, _moveUpShakeDuration).OnComplete(() => onComplete?.Invoke());
		}

		private void SetUpClaw(Action onComplete)
		{
			Vector3 clawPosition = _clawSprite.transform.position;
			float clawPositionY = _caughtTarget.Transform.position.y + _caughtTarget.SizeY / 2;
			clawPosition.y = clawPositionY;

			_clawSprite.transform.DOMove(clawPosition, _clawCatchDuration)
				.OnComplete(() => onComplete());
			
			Vector3 chainPosition = _chainSprite.transform.position;
			float chainPositionY = Mathf.Lerp(_mountSprite.transform.position.y, clawPositionY + _clawSprite.bounds.size.y / 2, 0.5f);
			chainPosition.y = chainPositionY;

			Vector2 chainSize = _chainSprite.size;
			chainSize.y = _mountSprite.transform.position.y - clawPositionY;
			_chainSprite.size = chainSize;
			_chainSprite.transform.position += Vector3.up * (chainSize.y - _chainDefaultSize.y) / 2;
			_chainDefaultPosition = _chainSprite.transform.position;
			
			_chainSprite.transform.DOMove(chainPosition, _clawCatchDuration);
		}

		private void SetUpTarget()
		{
			Vector3 catchTargetPosition = _caughtTarget.Transform.position;
			catchTargetPosition.x = _clawSprite.transform.position.x;
			
			_caughtTarget.Transform.DOMove(catchTargetPosition, _targetMoveToClawDuration);
		}
	}
}