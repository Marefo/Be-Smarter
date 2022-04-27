using System;
using System.Runtime.InteropServices;
using CodeBase.DeathRay;
using CodeBase.Logic;
using DG.Tweening;
using UnityEngine;
using Object = System.Object;

namespace CodeBase.UI
{
	public class InteractionHint : MonoBehaviour
	{
		[SerializeField] private GameObject _visualHint;
		[SerializeField] private Interactable _interactable;
		[Space(10)]
		[SerializeField] private float _showEffectForce;
		[SerializeField] private float _showEffectDuration;
		[SerializeField] private float _hideTargetScale;
		[SerializeField] private float _hideEffectDuration;

		private Tweener _show;
		private Tweener _hide;
		
		private void OnEnable()
		{
			_interactable.InteractionEnabled += Show;
			_interactable.InteractionDisabled += Hide;
			_interactable.InteractionStarted += Hide;
		}

		private void OnDisable()
		{
			_interactable.InteractionEnabled -= Show;
			_interactable.InteractionDisabled -= Hide;
			_interactable.InteractionStarted -= Hide;
		}

		private void Start() => Hide();

		private void Show()
		{
			_hide?.Kill();

			_visualHint.SetActive(true);
			_visualHint.transform.localScale = Vector3.one;
			_show = _visualHint.transform.DOPunchScale(Vector3.one * _showEffectForce, _showEffectDuration, 1);
		}

		private void Hide()
		{
			_show?.Kill();
			
			_hide = _visualHint.transform.DOScale(Vector3.one * _hideTargetScale, _hideEffectDuration)
				.OnComplete(OnHideComplete)
				.OnComplete(OnHideComplete);
		}

		private void OnHideComplete() => _visualHint.SetActive(false);
	}
}