using UnityEngine;

namespace CodeBase.StaticData
{
	[CreateAssetMenu(fileName = "PushSettings", menuName = "StaticData/PushSettings")]
	public class PushSettings : ScriptableObject
	{
		public float PushDistance;
		public float PushHeight;
		public float PushDuration;
	}
}