using CodeBase.StaticData;
using UnityEngine;

namespace CodeBase.Services
{
	public class ProgressService : IProgressService
	{
		private const string SavedSceneKey = "Scene";
		private const int FirstLevelIndex = 1;

		public int SceneIndex { get => LoadSavedScene(); set => SaveScene(value); }

		public int GetFirstLevelIndex() => FirstLevelIndex;

		private int LoadSavedScene()
		{
			int savedSceneIndex = PlayerPrefs.GetInt(SavedSceneKey, 0);
			
			if (savedSceneIndex == 0)
			{
				savedSceneIndex = FirstLevelIndex;
				SaveScene(savedSceneIndex);
			}

			return savedSceneIndex;
		}

		private void SaveScene(int index) => PlayerPrefs.SetInt(SavedSceneKey, index);
	}
}