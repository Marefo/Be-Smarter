using UnityEngine;
using Zenject;

namespace CodeBase.Infrastructure
{
	public class LevelPreparer : MonoBehaviour
	{
		public bool Prepared { get; private set; }

		private const float preparationDuration = 0.5f;
		private CoroutineRunner _coroutineRunner;

		[Inject]
		private void Construct(CoroutineRunner coroutineRunner)
		{
			_coroutineRunner = coroutineRunner;
		}

		private void Start()
		{
			_coroutineRunner.CallWithDelay(() => Prepared = true, preparationDuration);
		}
	}
}