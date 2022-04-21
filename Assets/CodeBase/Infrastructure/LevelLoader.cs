using CodeBase.Services;
using CodeBase.UI;

namespace CodeBase.Infrastructure
{
	public class LevelLoader
	{
		private readonly IProgressService _progressService;
		private readonly SceneLoader _sceneLoader;
		private readonly LoadingCurtain _loadingCurtain;

		public LevelLoader(IProgressService progressService, SceneLoader sceneLoader, LoadingCurtain loadingCurtain)
		{
			_progressService = progressService;
			_sceneLoader = sceneLoader;
			_loadingCurtain = loadingCurtain;
		}

		public void LoadSavedLevel()
		{
			int savedSceneIndex = _progressService.SceneIndex;
			_loadingCurtain.FadeIn(() => LoadSceneWithFadeOut(savedSceneIndex));
		}

		public void LoadNextLevel()
		{
			int nextLevelIndex = _progressService.GetFirstLevelIndex();
			int currentSceneIndex = _sceneLoader.GetCurrentSceneIndex();
			int maxSceneIndex = _sceneLoader.GetSceneCount() - 1;
			
			if (currentSceneIndex != maxSceneIndex)
				nextLevelIndex = _sceneLoader.GetCurrentSceneIndex() + 1;

			_progressService.SceneIndex = nextLevelIndex;
			_loadingCurtain.FadeIn(() => LoadSceneWithFadeOut(nextLevelIndex));
		}

		private void LoadSceneWithFadeOut(int sceneIndex) => 
			_sceneLoader.Load(sceneIndex, () => _loadingCurtain.FadeOutWithDelay());
	}
}