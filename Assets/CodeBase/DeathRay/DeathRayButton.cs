using System;
using UnityEngine;

namespace CodeBase.DeathRay
{
	public abstract class DeathRayButton : MonoBehaviour
	{
		public abstract event Action StateChanged;
	}
}