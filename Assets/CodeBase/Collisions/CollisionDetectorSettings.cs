using UnityEngine;

namespace CodeBase.Collisions
{
	[CreateAssetMenu(fileName = "CollisionDetectorSettings", menuName = "Settings/CollisionDetector")]
	public class CollisionDetectorSettings : ScriptableObject
	{
		public ContactFilter2D PhysicalObjFilter;
		public LayerMask PhysicalObjLayer;
		[Space(10)]
		public Bounds UnitBounds;
		public int SideRaysCount = 3;
		public float VerticalRaysLength = 0.1f;
		public float HorizontalRaysLength = 0.1f;
		public float RaysInnerOffset = 0.01f;
		public float RaysShift = 0.1f;
	}
}