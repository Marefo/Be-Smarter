using System;
using CodeBase.Infrastructure;
using CodeBase.Services;
using CodeBase.UI;
using Zenject;

namespace CodeBase.Logic
{
	public class GameReStarter : IInitializable, IDisposable
	{
		private readonly GameStatus _gameStatus;
		private readonly StaticDataService _staticDataService;
		private readonly IInputService _inputService;
		private readonly SceneLoader _sceneLoader;
		private readonly CoroutineRunner _coroutineRunner;
		private readonly LoadingCurtain _loadingCurtain;

		private bool _restarting = false;
		
		public GameReStarter(GameStatus gameStatus, StaticDataService staticDataService, IInputService inputService, 
			SceneLoader sceneLoader, CoroutineRunner coroutineRunner, LoadingCurtain loadingCurtain)
		{
			_gameStatus = gameStatus;
			_staticDataService = staticDataService;
			_inputService = inputService;
			_sceneLoader = sceneLoader;
			_coroutineRunner = coroutineRunner;
			_loadingCurtain = loadingCurtain;
		}

		public void Initialize()
		{
			_inputService.RestartBtnPressed += OnRestartBtnPress;
			_gameStatus.Lost += OnLose;
		}

		public void Dispose()
		{
			_inputService.RestartBtnPressed -= OnRestartBtnPress;
			_gameStatus.Lost -= OnLose;
		}

		private void OnRestartBtnPress()
		{
			if(_restarting || _loadingCurtain.Active == true) return;
			
			_restarting = true;
			_loadingCurtain.FadeIn(OnFadeIn);
		}

		private void OnLose()
		{
			if(_restarting) return;
			
			_restarting = true;
			float reloadDelay = _staticDataService.LoadGameSettings().LoseReloadDelay;
			_coroutineRunner.CallWithDelay(() => _loadingCurtain.FadeIn(OnFadeIn), reloadDelay);
		}

		private void OnFadeIn()
		{
			_sceneLoader.ReloadCurrentScene();
			_loadingCurtain.FadeOutWithDelay();
		}
	}
}