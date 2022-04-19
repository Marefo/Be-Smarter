using System;
using CodeBase.EnemySpawner;
using CodeBase.Units.Enemy;
using UnityEngine;

namespace CodeBase.StaticData
{
	[Serializable]
	public class EnemySpawnPointStaticData
	{
		public EnemyTypeId EnemyTypeId;
		public Vector3 Position;

		public EnemySpawnPointStaticData(EnemyTypeId enemyTypeId, Vector3 position)
		{
			EnemyTypeId = enemyTypeId;
			Position = position;
		}
	}
}