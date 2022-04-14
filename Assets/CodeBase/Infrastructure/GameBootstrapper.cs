using CodeBase.Services;
using CodeBase.UI;
using UnityEngine;
using Zenject;

namespace CodeBase.Infrastructure
{
	public class GameBootstrapper : MonoBehaviour
	{
		[SerializeField] private GameObject _loadingCurtainPrefab;

		private LoadingCurtain _loadingCurtain;
		private IProgressService _progressService;
		private SceneLoader _sceneLoader;

		[Inject]
		private void Construct(IProgressService progressService, SceneLoader sceneLoader)
		{
			_progressService = progressService;
			_sceneLoader = sceneLoader;
			
			StartBootstrap();
		}

		private void StartBootstrap()
		{
			_loadingCurtain = Instantiate(_loadingCurtainPrefab).GetComponent<LoadingCurtain>();
			_loadingCurtain.Show();
			
			string sceneName = _progressService.SceneName;
			_sceneLoader.Load(sceneName, OnLoad);
		}

		private void OnLoad()
		{
			_loadingCurtain.Hide();
		}
	}
}
