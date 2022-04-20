using UnityEngine;

namespace CodeBase.Infrastructure
{
	[CreateAssetMenu(fileName = "GameSettings", menuName = "Settings/Game")]
	public class GameSettings : ScriptableObject
	{
		public float LoseReloadDelay;
	}
}