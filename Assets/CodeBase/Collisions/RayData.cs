using UnityEngine;

namespace CodeBase.Collisions
{
	public class RayData
	{
		public readonly Vector2 Start;
		public readonly Vector2 End;
		public readonly Vector2 Direction;

		public RayData(Vector2 start, Vector2 end, Vector2 direction)
		{
			Start = start;
			End = end;
			Direction = direction;
		}
	}
}