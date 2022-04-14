using System;
using System.Collections;
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

		public void ReloadCurrentScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);

		public void Load(string nextScene, Action onLoaded = null)
		{
			_coroutineRunner.StartCoroutine(LoadCoroutine(nextScene, onLoaded));
		}

		private IEnumerator LoadCoroutine(string nextScene, Action onLoaded = null)
		{
			if (SceneManager.GetActiveScene().name == nextScene)
			{
				onLoaded?.Invoke();
				yield break;
			}
      
			AsyncOperation waitNextScene = SceneManager.LoadSceneAsync(nextScene);

			while (!waitNextScene.isDone)
				yield return null;
      
			onLoaded?.Invoke();
		}
	}
}