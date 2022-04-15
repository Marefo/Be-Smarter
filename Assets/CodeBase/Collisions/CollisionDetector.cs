using System;
using System.Collections.Generic;
using System.Linq;
using CodeBase.Extensions;
using UnityEngine;

namespace CodeBase.Collisions
{
	public class CollisionDetector : MonoBehaviour
	{
		public event Action BottomCollided;
		
		public BoxCollisions BoxCollisions { get; private set; }
		public Bounds Bounds => _unitBounds;
		public float VerticalRaysLength => _verticalRaysLength;
		
		[SerializeField] private ContactFilter2D _physicalObjFilter;
		[SerializeField] private LayerMask _physicalObjLayer;
		[Space(10)]
		[SerializeField] private Bounds _unitBounds;
		[SerializeField] private int _sideRaysCount = 3;
		[SerializeField] private float _verticalRaysLength = 0.1f;
		[SerializeField] private float _horizontalRaysLength = 0.1f;
		[SerializeField, Range(0.01f, 0.1f)] private float _raysShift = 0.1f;

		private Bounds _unitsBoundsInWorld;
		private BoxRays _boxRays;

		private void Start()
		{
			InitBoxCollisions();
		}

		private void LateUpdate()
		{
			UpdateRaysPosition();
			UpdateBoxCollisions();
		}

		private void OnDrawGizmos() {
			DrawBounds();
			DrawRays();
		}
		
		public bool CanMoveToPoint(Vector3 point) => GetCollisionsInPoint(point).Count == 0;

		public List<Collider2D> GetCollisionsInPoint(Vector3 point)
		{
			List<Collider2D> overlapped = new List<Collider2D>();
			Physics2D.OverlapBox(point, _unitBounds.size, 0, _physicalObjFilter, overlapped);

			return overlapped.Where(obj => obj.gameObject != gameObject).ToList();
		}

		public Collider2D GetFirstRightSideCollision() => GetCollidedObjects(_boxRays.RightRays)[0];
		public Collider2D GetFirstLeftSideCollision() => GetCollidedObjects(_boxRays.LeftRays)[0];

		private void InitBoxCollisions() => BoxCollisions = new BoxCollisions();

		private void UpdateRaysPosition()
		{
			_unitsBoundsInWorld = _unitBounds;
			_unitsBoundsInWorld.center = transform.position;

			RayData topRay = new RayData(
				new Vector2(_unitsBoundsInWorld.min.x + _raysShift, _unitsBoundsInWorld.max.y),
				new Vector2(_unitsBoundsInWorld.max.x - _raysShift, _unitsBoundsInWorld.max.y),
				Vector2.up, _verticalRaysLength
			);
			RayData bottomRay = new RayData(
				new Vector2(_unitsBoundsInWorld.min.x + _raysShift, _unitsBoundsInWorld.min.y),
				new Vector2(_unitsBoundsInWorld.max.x - _raysShift, _unitsBoundsInWorld.min.y),
				Vector2.down, _verticalRaysLength
			);
			RayData leftRay = new RayData(
				new Vector2(_unitsBoundsInWorld.min.x, _unitsBoundsInWorld.min.y + _raysShift),
				new Vector2(_unitsBoundsInWorld.min.x, _unitsBoundsInWorld.max.y - _raysShift),
				Vector2.left, _horizontalRaysLength
			);
			RayData rightRay = new RayData(
				new Vector2(_unitsBoundsInWorld.max.x, _unitsBoundsInWorld.min.y + _raysShift),
				new Vector2(_unitsBoundsInWorld.max.x, _unitsBoundsInWorld.max.y - _raysShift),
				Vector2.right, _horizontalRaysLength
			);

			_boxRays = new BoxRays(topRay, bottomRay, leftRay, rightRay);
		}

		private void UpdateBoxCollisions()
		{
			bool oldBottomCollision = BoxCollisions.BottomCollision;
			
			BoxCollisions.BottomCollision = RaysDetect(_boxRays.BottomRays);
			BoxCollisions.TopCollision = RaysDetect(_boxRays.TopRays);
			BoxCollisions.LeftCollision = RaysDetect(_boxRays.LeftRays);
			BoxCollisions.RightCollision = RaysDetect(_boxRays.RightRays);

			if (oldBottomCollision == false && BoxCollisions.BottomCollision == true)
				BottomCollided?.Invoke();
		}
		
		private List<Collider2D> GetCollidedObjects(RayData ray)
		{
			var rays = EvaluateRaysPositions(ray);
			List<Collider2D> collided = new List<Collider2D>();

			foreach (Vector2 rayPoint in rays)
			{
				RaycastHit2D hit = Physics2D.Raycast(rayPoint, ray.Direction, ray.Length, _physicalObjLayer);
				collided.Add(hit.collider);
			}

			return collided;
		}

		private bool RaysDetect(RayData ray)
		{
			return EvaluateRaysPositions(ray)
				.Any(point => Physics2D.Raycast(point, ray.Direction, ray.Length, _physicalObjLayer));
		}

		private IEnumerable<Vector2> EvaluateRaysPositions(RayData ray) {
			for (int i = 0; i < _sideRaysCount; i++) {
				float sidePercent = (float) i / (_sideRaysCount - 1);
				yield return Vector2.Lerp(ray.Start, ray.End, sidePercent);
			}
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
					Gizmos.DrawRay(point, ray.Direction * ray.Length);
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