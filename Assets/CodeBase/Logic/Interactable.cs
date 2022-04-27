using System;
using CodeBase.Services;
using CodeBase.Units.Hero;
using UnityEngine;
using Zenject;

namespace CodeBase.Logic
{
	public class Interactable : MonoBehaviour
	{
		public event Action InteractionEnabled;
		public event Action InteractionDisabled;
		public event Action InteractionStarted;
		public event Action InteractionFinished;
		public event Action<HeroInteractable> Interacted;
		

		public void EnableInteraction() => InteractionEnabled?.Invoke();

		public void DisableInteraction() => InteractionDisabled?.Invoke();

		public void Interact(HeroInteractable heroInteractable) => Interacted?.Invoke(heroInteractable);

		public void StartInteraction() => InteractionStarted?.Invoke();

		public void FinishInteraction() => InteractionFinished?.Invoke();
	}
}