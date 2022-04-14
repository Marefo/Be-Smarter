using UnityEngine;

namespace CodeBase.Services
{
	public class ProgressService : IProgressService
	{
		private const string SceneKey = "Scene";
		private const string FirstLevelScene = "Level-1";
		
		public string SceneName { get => LoadSavedScene(); set => SaveScene(value); }

		private string LoadSavedScene()
		{
			string savedScene = PlayerPrefs.GetString(SceneKey);

			return savedScene != "" ? savedScene : FirstLevelScene;
		}

		private void SaveScene(string name) => PlayerPrefs.SetString(SceneKey, name);
	}
}