using System.Linq;
using CodeBase.EnemySpawner;
using CodeBase.StaticData;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CodeBase.Editor
{
	[CustomEditor(typeof(LevelStaticData))]
	public class LevelStaticDataEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			LevelStaticData levelData = (LevelStaticData) target;

			if (GUILayout.Button("Collect"))
			{
				levelData.Name = SceneManager.GetActiveScene().name;

				levelData.EnemySpawners = FindObjectsOfType<EnemySpawnPoint>()
					.Select(x => new EnemySpawnPointStaticData(x.EnemyTypeId, x.transform.position)).ToList();
			}
			
			EditorUtility.SetDirty(target);
		}
	}
}