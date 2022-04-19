using System.Collections.Generic;
using UnityEngine;

namespace CodeBase.StaticData
{
	[CreateAssetMenu(fileName = "LevelStaticData", menuName = "StaticData/Level")]
	public class LevelStaticData : ScriptableObject
	{
		public string Name;
		public List<EnemySpawnPointStaticData> EnemySpawners;
	}
}