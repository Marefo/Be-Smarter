using System;
using CodeBase.Infrastructure;
using CodeBase.Services;
using Zenject;

namespace CodeBase.Logic
{
	public class GameReStarter : IInitializable, IDisposable
	{
		private readonly IInputService _inputService;
		private readonly SceneLoader _sceneLoader;

		public GameReStarter(IInputService inputService, SceneLoader sceneLoader)
		{
			_inputService = inputService;
			_sceneLoader = sceneLoader;
		}

		public void Initialize()
		{
			_inputService.RestartBtnPressed += OnRestartBtnPress;
		}

		public void Dispose()
		{
			_inputService.RestartBtnPressed -= OnRestartBtnPress;
		}

		private void OnRestartBtnPress() => _sceneLoader.ReloadCurrentScene();
	}
}