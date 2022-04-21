using System;
using System.Collections;
using CodeBase.Infrastructure;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Zenject;

namespace CodeBase.UI
{
	public class LoadingCurtain : MonoBehaviour
	{
		public event Action BecameInvisible;
		
		public bool Invisible { get; private set; }
		public bool Active => _curtain.gameObject.activeSelf;

		[SerializeField] private float _fadeInDuration = 0.5f;
		[SerializeField] private float _fadeOutDelay = 3;
		[SerializeField] private float _fadeOutDuration = 0.5f;
		[Space(10)]
		[SerializeField] private CanvasGroup _curtain;
		[SerializeField] private TextMeshProUGUI _field;

		private string _defaultFieldValue;
		private CoroutineRunner _coroutineRunner;
		private Coroutine _loadingEffectCoroutine;
		private Tweener _fadeIn;
		private Tweener _fadeOut;

		[Inject]
		private void Construct(CoroutineRunner coroutineRunner)
		{
			_coroutineRunner = coroutineRunner;
		}
		
		private void Awake()
		{
			DontDestroyOnLoad(this);
			_defaultFieldValue = _field.text;
			Hide();
		}

		public void Show()
		{
			Invisible = false;
			_curtain.gameObject.SetActive(true);
			_curtain.alpha = 1;
		}
    
		public void Hide()
		{
			Invisible = true;
			BecameInvisible?.Invoke();
			
			StopLoadingEffect();
			_curtain.gameObject.SetActive(false);
		}

		public void FadeIn(Action onComplete = null)
		{
			_fadeOut?.Kill();
			_curtain.gameObject.SetActive(true);
			_curtain.alpha = 0;
			
			PlayLoadingEffect();
			
			_fadeIn = _curtain.DOFade(1, _fadeInDuration)
				.OnComplete(() => OnFadeInComplete(onComplete))
				.OnKill(() => OnFadeInComplete(onComplete));
		}

		public void FadeOutWithDelay(Action onComplete = null) => 
			_coroutineRunner.CallWithDelay(() => FadeOut(onComplete), _fadeOutDelay);

		public void FadeOut(Action onComplete = null)
		{
			_fadeIn?.Kill();
			_curtain.alpha = 1;
			
			_fadeOut = _curtain.DOFade(0, _fadeOutDuration)
				.OnUpdate(OnFadeOutUpdate)
				.OnComplete(() => OnFadeOutComplete(onComplete))
				.OnKill(() => OnFadeOutComplete(onComplete));
		}

		private void OnFadeInComplete(Action onComplete = null)
		{
			Invisible = false;
			onComplete?.Invoke();
		}

		private void OnFadeOutUpdate()
		{
			if (Invisible == false && _curtain.alpha <= 0.8f)
			{
				Invisible = true;
				BecameInvisible?.Invoke();
			}
		}

		private void OnFadeOutComplete(Action callback = null)
		{
			callback?.Invoke();
			StopLoadingEffect();
			_curtain.gameObject.SetActive(false);
		}

		private void PlayLoadingEffect() => _loadingEffectCoroutine = StartCoroutine(PlayLoadingEffectCoroutine());

		private IEnumerator PlayLoadingEffectCoroutine()
		{
			string dots = "";
			
			while (true)
			{
				dots = dots.Length < 3 ? dots + "." : "";
				_field.text = _defaultFieldValue + dots;

				yield return new WaitForSeconds(0.25f);
			}
		}

		private void StopLoadingEffect()
		{
			if(_loadingEffectCoroutine != null)
				StopCoroutine(_loadingEffectCoroutine);
			
			_field.text = _defaultFieldValue;
		}
	}
}
