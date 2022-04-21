using System;
using UnityEngine;

namespace CodeBase.Units
{
	public abstract class UnitMovement : MonoBehaviour
	{
		protected bool _activated = false;

		public void Activate() => _activated = true;

		public abstract void Disable();
	}
}