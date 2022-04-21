using UnityEngine;
using Zenject;

namespace CodeBase.Infrastructure
{
	public class GameBootstrapper : MonoBehaviour
	{
		private LevelLoader _levelLoader;

		[Inject]
		private void Construct(LevelLoader levelLoader)
		{
			_levelLoader = levelLoader;
			StartBootstrap();
		}

		private void StartBootstrap() => _levelLoader.LoadSavedLevel();
	}
}
