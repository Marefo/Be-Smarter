using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace CodeBase.UI
{
	public class LoadingCurtain : MonoBehaviour
	{
		[SerializeField] private float _fadeInDuration = 0.5f;
		[SerializeField] private float _fadeOutDuration = 0.5f;
		[Space(10)]
		[SerializeField] private CanvasGroup _curtain;
		[SerializeField] private TextMeshProUGUI _field;

		private string _defaultFieldValue;
		private Coroutine _loadingEffectCoroutine;
		private Tweener _fadeIn;
		private Tweener _fadeOut;

		private void Awake()
		{
			DontDestroyOnLoad(this);
			_defaultFieldValue = _field.text;
			Hide();
		}

		public void Show()
		{
			gameObject.SetActive(true);
			_curtain.alpha = 1;
		}
    
		public void Hide()
		{
			gameObject.SetActive(false);
			StopLoadingEffect();
		}

		public void FadeIn(Action onComplete = null)
		{
			_fadeOut?.Kill();
			gameObject.SetActive(true);
			_curtain.alpha = 0;
			
			PlayLoadingEffect();
			
			_fadeIn = _curtain.DOFade(1, _fadeInDuration)
				.OnComplete(() => onComplete?.Invoke())
				.OnKill(() => onComplete?.Invoke());
		}

		public void FadeOut(Action onComplete = null)
		{
			_fadeIn?.Kill();
			_curtain.alpha = 1;
			
			_fadeOut = _curtain.DOFade(0, _fadeOutDuration)
				.OnComplete(() => OnFadeOutComplete(onComplete))
				.OnKill(() => OnFadeOutComplete(onComplete));
		}

		private void OnFadeOutComplete(Action callback = null)
		{
			callback?.Invoke();
			StopLoadingEffect();
			gameObject.SetActive(false);
		}
		
		/*private IEnumerator DoFadeIn()
		{
			while (_curtain.alpha > 0)
			{
				_curtain.alpha -= 0.03f;
				yield return new WaitForSeconds(0.03f);
			}
      
			StopLoadingEffect();
			gameObject.SetActive(false);
		}*/

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
