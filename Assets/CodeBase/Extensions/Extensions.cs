using UnityEngine;

namespace CodeBase.Extensions
{
	public static class Extensions
	{
		public static bool CompareLayers(this GameObject obj, LayerMask layer) => (obj.layer & (1 << layer)) > 0;

		public static TComponent GetComponentInObjectOrChildren<TComponent>(this GameObject gameObject)
		{
			TComponent component = gameObject.TryGetComponent(out TComponent targetComponent)
				? targetComponent
				: gameObject.GetComponentInChildren<TComponent>();

			return component;
		}
	}
}