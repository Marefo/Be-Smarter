using UnityEngine;

namespace CodeBase.Units.Enemy
{
	[CreateAssetMenu(fileName = "ChaserSettings", menuName = "Settings/Chaser")]
	public class ChaserSettings : ScriptableObject
	{
		public float ConfusionTime;
		[Space(10)]
		public float JumpPercentLength;
		public float JumpHeight;
		public float JumpDuration;
	}
}