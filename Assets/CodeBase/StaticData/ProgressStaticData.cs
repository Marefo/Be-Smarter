using UnityEngine;

namespace CodeBase.StaticData
{
	[CreateAssetMenu(fileName = "ProgressStaticData", menuName = "StaticData/Progress")]
	public class ProgressStaticData : ScriptableObject
	{
		public int SceneIndex;
	}
}