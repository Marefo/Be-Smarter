using System;
using System.Collections;
using UnityEngine;

namespace CodeBase.Infrastructure
{
	public class CoroutineRunner : MonoBehaviour
	{
		public Coroutine CallWithDelay(Action action, float delay)
		{ 
			return StartCoroutine(CallWithDelayCoroutine(action, delay));
		}

		public IEnumerator CallWithDelayCoroutine(Action action, float delay)
		{
			yield return new WaitForSeconds(delay);
			action();
		}
	}
}