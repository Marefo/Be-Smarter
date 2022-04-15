using System.Diagnostics;
using UnityEngine;

namespace Vertx.Debugging
{
	public static partial class DebugUtils
	{
		[Conditional("UNITY_EDITOR")]
		public static void DrawMeshNormals(Mesh mesh, Transform transform, float rayLength, Color color) =>
			DrawMeshNormals(mesh, transform.localToWorldMatrix, rayLength, color);

		[Conditional("UNITY_EDITOR")]
		public static void DrawMeshNormals(Mesh mesh, Matrix4x4 localToWorld, float rayLength, Color color)
		{
			
		}

		private static Vector3 Vector3Abs(Vector3 vector3) =>
			new Vector3(Mathf.Abs(vector3.x), Mathf.Abs(vector3.y), Mathf.Abs(vector3.z));
	}
}