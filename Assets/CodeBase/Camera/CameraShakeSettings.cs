using UnityEngine;

namespace CodeBase.Camera
{
	[CreateAssetMenu(fileName = "CameraShakeSettings", menuName = "Settings/CameraShake")]
	public class CameraShakeSettings : ScriptableObject
	{
		public float Intensity;
		public float Time;
	}
}