using UnityEngine;

namespace CodeBase.StaticData
{
	[CreateAssetMenu(fileName = "HeroMoveSettings", menuName = "StaticData/HeroMoveSettings")]
	public class HeroMoveSettings : ScriptableObject
	{
		public float MaxMoveSpeed;
		public float Acceleration;
		public float DeAcceleration;
		[Space(10)]
		public float MaxTilt;
		public float TiltSpeed;
		[Space(10)]
		public float ClimbDuration;
		public float ClimbAssistHeight;
		[Space(10)]
		public float MinFallSpeed;
		public float MaxFallSpeed;
		[Space(10)]
		public float JumpHeight;
		public float ApexThreshold;
		public float ApexAdditionalSpeed;
		public float BufferedJumpTime;
	}
}