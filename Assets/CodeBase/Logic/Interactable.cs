using System;
using UnityEngine;

namespace CodeBase.Logic
{
	public class Interactable : MonoBehaviour
	{
		public event Action InteractionEnabled;
		public event Action InteractionDisabled;

		public void Enable() => InteractionEnabled?.Invoke();
		
		public void Disable() => InteractionDisabled?.Invoke();
	}
}