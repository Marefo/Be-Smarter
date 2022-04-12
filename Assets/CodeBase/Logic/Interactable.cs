using System;
using UnityEngine;

namespace CodeBase.Logic
{
	public abstract class Interactable : MonoBehaviour
	{
		public virtual event Action InteractionEnabled;
		public virtual event Action InteractionDisabled;
	}
}