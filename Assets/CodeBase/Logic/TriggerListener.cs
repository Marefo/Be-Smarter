using System;
using UnityEngine;

namespace CodeBase.Logic
{
	[RequireComponent(typeof(Collider2D))]
	public class TriggerListener : MonoBehaviour
	{
		public event Action<Collider2D> Entered;
		public event Action<Collider2D> Canceled;

		private void OnTriggerEnter2D(Collider2D col) => Entered?.Invoke(col);
		
		private void OnTriggerExit2D(Collider2D col) => Canceled?.Invoke(col);
	}
}