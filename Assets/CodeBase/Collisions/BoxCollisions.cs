namespace CodeBase.Collisions
{
	public class BoxCollisions
	{
		public bool TopCollision;
		public bool BottomCollision;
		public bool LeftCollision;
		public bool RightCollision;

		public BoxCollisions()
		{
			TopCollision = false;
			BottomCollision = false;
			LeftCollision = false;
			RightCollision = false;
		}
	}
}