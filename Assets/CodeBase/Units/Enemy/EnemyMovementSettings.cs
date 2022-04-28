using UnityEngine;

namespace CodeBase.Units.Enemy
{
	[CreateAssetMenu(fileName = "EnemyMovementSettings", menuName = "Settings/EnemyMovement")]
	public class EnemyMovementSettings : ScriptableObject
	{
		[Space(10)]
		public float Acceleration;
		public float DeAcceleration;
		public float MaxMoveSpeed;
		[Space(10)]
		public float FallSpeed;
		public float MaxFallSpeed;
		[Space(10)]
		public float MaxTilt;
		public float TiltSpeed;
		[Space(10)]
		public float ClimbAssistHeight;
		public float ClimbDuration;
		public int ClosestPointFindAttempts;
	}
}