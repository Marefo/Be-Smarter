using UnityEngine;

namespace CodeBase.Extensions
{
	public static class Extensions
	{
		public static bool CompareLayers(this GameObject obj, LayerMask layer) => (obj.layer & (1 << layer)) > 0;
	}
}