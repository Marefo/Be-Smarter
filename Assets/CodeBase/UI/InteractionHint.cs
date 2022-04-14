using System;
using System.Runtime.InteropServices;
using CodeBase.DeathRay;
using CodeBase.Logic;
using UnityEngine;
using Object = System.Object;

namespace CodeBase.UI
{
	public class InteractionHint : MonoBehaviour
	{
		[SerializeField] private GameObject _visualHint;
		[SerializeField] private Interactable _interactable;
		
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