namespace CodeBase.Collisions
{
	public class BoxRays
	{
		public RayData TopRays;
		public RayData BottomRays;
		public RayData LeftRays;
		public RayData RightRays;

		public BoxRays(RayData topRays, RayData bottomRays, RayData leftRays, RayData rightRays)
		{
			TopRays = topRays;
			BottomRays = bottomRays;
			LeftRays = leftRays;
			RightRays = rightRays;
		}
	}
}