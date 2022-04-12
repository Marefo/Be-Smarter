using System;
using CodeBase.Logic;
using UnityEngine;

namespace CodeBase.UI
{
	public class InteractionHint : MonoBehaviour
	{
		[SerializeField] private Interactable _interactable;
		[SerializeField] private GameObject _visualHint;
		
		private void Start()
		{
			Hide();
			
			_interactable.InteractionEnabled += Show;
			_interactable.InteractionDisabled += Hide;
		}

		private void OnDestroy()
		{
			_interactable.InteractionEnabled -= Show;
			_interactable.InteractionDisabled -= Hide;
		}

		private void Show()
		{
			_visualHint.SetActive(true);
		}

		private void Hide()
		{
			_visualHint.SetActive(false);
		}
	}
}