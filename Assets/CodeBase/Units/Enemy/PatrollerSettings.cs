using UnityEngine;

namespace CodeBase.Units.Enemy
{
	[CreateAssetMenu(fileName = "PatrollerSettings", menuName = "Settings/Patroller")]
	public class PatrollerSettings : ScriptableObject
	{
		public float ReachPointDelay;
	}
}