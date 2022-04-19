﻿using System;
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

		private void OnEnable()
		{
			_interactable.InteractionEnabled += Show;
			_interactable.InteractionDisabled += Hide;
			_interactable.InteractionStarted += Hide;
			_interactable.InteractionFinished += Show;
		}

		private void OnDisable()
		{
			_interactable.InteractionEnabled -= Show;
			_interactable.InteractionDisabled -= Hide;
			_interactable.InteractionStarted -= Hide;
			_interactable.InteractionFinished -= Show;
		}

		private void Start() => Hide();

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