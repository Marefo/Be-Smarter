using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CodeBase.Collisions
{
	public class CollisionDetector : MonoBehaviour
	{
		public BoxCollisions BoxCollisions { get; private set; }
		
		[SerializeField] private Bounds _unitBounds;
		[SerializeField] private LayerMask _physicalObjLayer;
		[SerializeField] private int _sideRaysCount = 3;
		[SerializeField] private float _raysLength = 0.1f;
		[SerializeField, Range(0.01f, 0.1f)] private float _raysShift = 0.1f;

		private Bounds _unitsBoundsInWorld;
		private BoxRays _boxRays;

		private void Start()
		{
			InitBoxCollisions();
		}

		private void Update()
		{
			UpdateRaysPosition();
			UpdateBoxCollisions();
		}

		private void InitBoxCollisions() => BoxCollisions = new BoxCollisions();

		private void UpdateRaysPosition()
		{
			_unitsBoundsInWorld = _unitBounds;
			_unitsBoundsInWorld.center = transform.position;

			RayData topRay = new RayData(
				new Vector2(_unitsBoundsInWorld.min.x + _raysShift, _unitsBoundsInWorld.max.y),
				new Vector2(_unitsBoundsInWorld.max.x - _raysShift, _unitsBoundsInWorld.max.y),
				Vector2.up
			);
			RayData bottomRay = new RayData(
				new Vector2(_unitsBoundsInWorld.min.x + _raysShift, _unitsBoundsInWorld.min.y),
				new Vector2(_unitsBoundsInWorld.max.x - _raysShift, _unitsBoundsInWorld.min.y),
				Vector2.down
			);
			RayData leftRay = new RayData(
				new Vector2(_unitsBoundsInWorld.min.x, _unitsBoundsInWorld.min.y + _raysShift),
				new Vector2(_unitsBoundsInWorld.min.x, _unitsBoundsInWorld.max.y - _raysShift),
				Vector2.left
			);
			RayData rightRay = new RayData(
				new Vector2(_unitsBoundsInWorld.max.x, _unitsBoundsInWorld.min.y + _raysShift),
				new Vector2(_unitsBoundsInWorld.max.x, _unitsBoundsInWorld.max.y - _raysShift),
				Vector2.right
			);

			_boxRays = new BoxRays(topRay, bottomRay, leftRay, rightRay);
		}

		private void UpdateBoxCollisions()
		{
			BoxCollisions.BottomCollision = RaysDetect(_boxRays.BottomRays);
			BoxCollisions.TopCollision = RaysDetect(_boxRays.TopRays);
			BoxCollisions.LeftCollision = RaysDetect(_boxRays.LeftRays);
			BoxCollisions.RightCollision = RaysDetect(_boxRays.RightRays);
		}

		private bool RaysDetect(RayData ray)
		{
			return EvaluateRaysPositions(ray)
				.Any(point => Physics2D.Raycast(point, ray.Direction, _raysLength, _physicalObjLayer));
		}

		private IEnumerable<Vector2> EvaluateRaysPositions(RayData ray) {
			for (int i = 0; i < _sideRaysCount; i++) {
				float sidePercent = (float) i / (_sideRaysCount - 1);
				yield return Vector2.Lerp(ray.Start, ray.End, sidePercent);
			}
		}

		private void OnDrawGizmos() {
			DrawBounds();
			DrawRays();
		}

		private void DrawRays()
		{
			if (Application.isPlaying == false)
				UpdateRaysPosition();
				
			Gizmos.color = Color.red;
			List<RayData> rays = new List<RayData>()
				{_boxRays.BottomRays, _boxRays.TopRays, _boxRays.LeftRays, _boxRays.RightRays};
			
			foreach (RayData ray in rays)
			{
				foreach (Vector2 point in EvaluateRaysPositions(ray))
				{
					Gizmos.DrawRay(point, ray.Direction * _raysLength);
				}
			}
		}

		private void DrawBounds()
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireCube(transform.position + _unitBounds.center, _unitBounds.size);
		}
	}
}