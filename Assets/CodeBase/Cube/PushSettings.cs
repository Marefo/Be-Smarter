using UnityEngine;

namespace CodeBase.Cube
{
	[CreateAssetMenu(fileName = "PushSettings", menuName = "Settings/Push")]
	public class PushSettings : ScriptableObject
	{
		public float PushDistance;
		public float PushHeight;
		public float PushDuration;
	}
}