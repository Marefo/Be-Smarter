using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CodeBase.Infrastructure
{
	public class SceneLoader
	{
		private readonly CoroutineRunner _coroutineRunner;

		public SceneLoader(CoroutineRunner coroutineRunner)
		{
			_coroutineRunner = coroutineRunner;
		}

		public int GetSceneCount() => SceneManager.sceneCountInBuildSettings;

		public int GetCurrentSceneIndex() => SceneManager.GetActiveScene().buildIndex;
		
		public void ReloadCurrentScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);


		public void Load(string sceneName, Action onLoaded = null) => 
			_coroutineRunner.StartCoroutine(LoadSceneCoroutine(sceneName, onLoaded));


		public void Load(int sceneIndex, Action onLoaded = null) => 
			_coroutineRunner.StartCoroutine(LoadSceneCoroutine(sceneIndex, onLoaded));

		private IEnumerator LoadSceneCoroutine(string sceneName, Action onLoaded = null)
		{
			AsyncOperation waitNextScene = SceneManager.LoadSceneAsync(sceneName);

			while (!waitNextScene.isDone)
				yield return null;
      
			onLoaded?.Invoke();
		}
		
		private IEnumerator LoadSceneCoroutine(int sceneIndex, Action onLoaded = null)
		{
			AsyncOperation waitNextScene = SceneManager.LoadSceneAsync(sceneIndex);

			while (!waitNextScene.isDone)
				yield return null;
      
			onLoaded?.Invoke();
		}
	}
}