using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace CodeBase.UI
{
	public class LoadingCurtain : MonoBehaviour
	{
		[SerializeField] private CanvasGroup _curtain;
		[SerializeField] private TextMeshProUGUI _field;

		private string _defaultFieldValue;
		private Coroutine _loadingEffectCoroutine;

		private void Awake()
		{
			DontDestroyOnLoad(this);
			FastHide();
		}

		private void FastHide() => gameObject.SetActive(false);

		public void Show()
		{
			gameObject.SetActive(true);
			_curtain.alpha = 1;
			
			PlayLoadingEffect();
		}
    
		public void Hide() => StartCoroutine(DoFadeIn());

		private IEnumerator DoFadeIn()
		{
			while (_curtain.alpha > 0)
			{
				_curtain.alpha -= 0.03f;
				yield return new WaitForSeconds(0.03f);
			}
      
			StopLoadingEffect();
			gameObject.SetActive(false);
		}

		private void PlayLoadingEffect()
		{
			_defaultFieldValue = _field.text;
			_loadingEffectCoroutine = StartCoroutine(PlayLoadingEffectCoroutine());
		}

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
			StopCoroutine(_loadingEffectCoroutine);
			_field.text = _defaultFieldValue;
		}
	}
}
