using CodeBase.Services;
using CodeBase.UI;
using UnityEngine;
using Zenject;

namespace CodeBase.Infrastructure
{
	public class GameBootstrapper : MonoBehaviour
	{
		private LoadingCurtain _loadingCurtain;
		private IProgressService _progressService;
		private SceneLoader _sceneLoader;

		[Inject]
		private void Construct(LoadingCurtain loadingCurtain, IProgressService progressService, SceneLoader sceneLoader)
		{
			_loadingCurtain = loadingCurtain;
			_progressService = progressService;
			_sceneLoader = sceneLoader;
			
			StartBootstrap();
		}

		private void StartBootstrap()
		{
			_loadingCurtain.FadeIn();
			
			string sceneName = _progressService.SceneName;
			_sceneLoader.Load(sceneName, OnLoad);
		}

		private void OnLoad()
		{
			_loadingCurtain.FadeOut();
		}
	}
}
