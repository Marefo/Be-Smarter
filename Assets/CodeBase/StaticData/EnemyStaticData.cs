using CodeBase.EnemySpawner;
using CodeBase.Units.Enemy;
using UnityEngine;

namespace CodeBase.StaticData
{
	[CreateAssetMenu(fileName = "EnemyStaticData", menuName = "StaticData/Enemy")]
	public class EnemyStaticData : ScriptableObject
	{
		public EnemyTypeId EnemyTypeId;
		public GameObject Prefab;
	}
}