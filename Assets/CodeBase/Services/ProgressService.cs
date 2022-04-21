using CodeBase.StaticData;
using UnityEngine;

namespace CodeBase.Services
{
	public class ProgressService : IProgressService
	{
		private const int FirstLevelIndex = 1;

		public int SceneIndex { get => LoadSavedScene(); set => SaveScene(value); }
		
		private ProgressStaticData _progress => _staticDataService.LoadProgressData();
		private readonly StaticDataService _staticDataService;

		public ProgressService(StaticDataService staticDataService)
		{
			_staticDataService = staticDataService;
		}

		public int GetFirstLevelIndex() => FirstLevelIndex;

		private int LoadSavedScene()
		{
			int savedSceneIndex = _progress.SceneIndex;
			
			if (savedSceneIndex == 0)
			{
				savedSceneIndex = FirstLevelIndex;
				SaveScene(savedSceneIndex);
			}

			return savedSceneIndex;
		}

		private void SaveScene(int index) => _progress.SceneIndex = index;
	}
}