using System;
using System.Collections.Generic;
using System.Linq;
using CodeBase.Extensions;
using CodeBase.Logic;
using CodeBase.StaticData;
using JetBrains.Annotations;
using UnityEngine;
using Vertx.Debugging;

namespace CodeBase.Collisions
{
	public class CollisionDetector : MonoBehaviour
	{
		public event Action Collided;
		public event Action BottomCollided;

		public BoxCollisions BoxCollisions { get; private set; } = new BoxCollisions();
		public BoxCollisions LastFrameBoxCollisions { get; private set; } = new BoxCollisions();
		public Bounds Bounds => _settings.UnitBounds;

		[SerializeField] private CollisionDetectorSettings _settings;

		private Bounds _unitsBoundsInWorld = new Bounds();
		private BoxRays _boxRays;

		private void OnDrawGizmos() {
			DrawBounds();
			DrawRays();
		}
		
		public bool CanMoveToPoint(Vector3 point) => GetCollisionsInPoint(point).Count == 0;

		private List<Collider2D> GetCollisionsInPoint(Vector3 point)
		{
			List<Collider2D> overlapped = new List<Collider2D>();
			Physics2D.OverlapBox(point, _settings.UnitBounds.size, 0, _settings.PhysicalObjFilter, overlapped);

			return overlapped.Where(obj => obj.gameObject != gameObject).ToList();
		}

		public Collider2D GetFirstRightSideCollision() => GetCollidedObjects(_boxRays.RightRays)[0];
		public Collider2D GetFirstLeftSideCollision() => GetCollidedObjects(_boxRays.LeftRays)[0];

		public Vector2 GetClimbPointToCollider(Collider2D colliderForClimb, float climbHeight)
		{
			bool canClimb = _unitsBoundsInWorld.min.y - _settings.VerticalRaysLength + climbHeight > colliderForClimb.bounds.max.y;

			if (canClimb == false) return Vector2.zero;
			
			float collidedMinX = colliderForClimb.bounds.min.x;
			float collidedMaxX = colliderForClimb.bounds.max.x;
			
			float closestPositionX =
				Mathf.Abs(transform.position.x - collidedMinX) <
				Mathf.Abs(transform.position.x - collidedMaxX)
					? collidedMinX
					: collidedMaxX;

			float climbedPositionY = transform.position.y + Mathf.Abs(colliderForClimb.bounds.max.y - _unitsBoundsInWorld.min.y) + _settings.VerticalRaysLength;
		
			return new Vector2(closestPositionX, climbedPositionY);
		}

		public void CalculateRaysPosition()
		{
			_unitsBoundsInWorld = _settings.UnitBounds;
			_unitsBoundsInWorld.center = transform.position;

			RayData topRay = new RayData(
				new Vector2(_unitsBoundsInWorld.min.x + _settings.RaysShift, _unitsBoundsInWorld.max.y - _settings.RaysInnerOffset),
				new Vector2(_unitsBoundsInWorld.max.x - _settings.RaysShift, _unitsBoundsInWorld.max.y - _settings.RaysInnerOffset),
				Vector2.up, _settings.VerticalRaysLength
			);
			RayData bottomRay = new RayData(
				new Vector2(_unitsBoundsInWorld.min.x + _settings.RaysShift, _unitsBoundsInWorld.min.y + _settings.RaysInnerOffset),
				new Vector2(_unitsBoundsInWorld.max.x - _settings.RaysShift, _unitsBoundsInWorld.min.y + _settings.RaysInnerOffset),
				Vector2.down, _settings.VerticalRaysLength
			);
			RayData leftRay = new RayData(
				new Vector2(_unitsBoundsInWorld.min.x + _settings.RaysInnerOffset, _unitsBoundsInWorld.min.y + _settings.RaysShift),
				new Vector2(_unitsBoundsInWorld.min.x + _settings.RaysInnerOffset, _unitsBoundsInWorld.max.y - _settings.RaysShift),
				Vector2.left, _settings.HorizontalRaysLength
			);
			RayData rightRay = new RayData(
				new Vector2(_unitsBoundsInWorld.max.x - _settings.RaysInnerOffset, _unitsBoundsInWorld.min.y + _settings.RaysShift),
				new Vector2(_unitsBoundsInWorld.max.x - _settings.RaysInnerOffset, _unitsBoundsInWorld.max.y - _settings.RaysShift),
				Vector2.right, _settings.HorizontalRaysLength
			);

			_boxRays = new BoxRays(topRay, bottomRay, leftRay, rightRay);
		}

		public void UpdateBoxCollisions()
		{
			LastFrameBoxCollisions.BottomCollision = BoxCollisions.BottomCollision;
			LastFrameBoxCollisions.TopCollision = BoxCollisions.TopCollision;
			LastFrameBoxCollisions.LeftCollision = BoxCollisions.LeftCollision;
			LastFrameBoxCollisions.RightCollision = BoxCollisions.RightCollision;
			
			BoxCollisions.BottomCollision = RaysDetect(_boxRays.BottomRays);
			BoxCollisions.TopCollision = RaysDetect(_boxRays.TopRays);
			BoxCollisions.LeftCollision = RaysDetect(_boxRays.LeftRays);
			BoxCollisions.RightCollision = RaysDetect(_boxRays.RightRays);
		}

		public void UpdateCollisionEvents()
		{
			if (
				LastFrameBoxCollisions.TopCollision == false && BoxCollisions.TopCollision == true ||
				LastFrameBoxCollisions.BottomCollision == false && BoxCollisions.BottomCollision == true ||
				LastFrameBoxCollisions.LeftCollision == false && BoxCollisions.LeftCollision == true ||
				LastFrameBoxCollisions.RightCollision == false && BoxCollisions.RightCollision == true
			)
			{
				Collided?.Invoke();
			}
				
			if (LastFrameBoxCollisions.BottomCollision == false && BoxCollisions.BottomCollision == true)
				BottomCollided?.Invoke();
		}

		public void TryGetComponentFromLeftCollisions<T>(out List<T> results)
		{
			List<GameObject> collisions = GetCollidedObjects(_boxRays.LeftRays).Where(obj => obj != null).Select(obj => obj.gameObject).ToList();

			GetComponentsFromList(collisions, out List<T> leftResults);

			results = leftResults.ToList();
		}

		public void TryGetComponentFromRightCollisions<T>(out List<T> results)
		{			
			List<GameObject> collisions = GetCollidedObjects(_boxRays.RightRays).Where(obj => obj != null).Select(obj => obj.gameObject).ToList();

			GetComponentsFromList(collisions, out List<T> rightResults);

			results = rightResults.ToList();
		}

		public void TryGetComponentFromBottomCollisions<T>(out List<T> results)
		{			
			List<GameObject> collisions = GetCollidedObjects(_boxRays.BottomRays).Where(obj => obj != null).Select(obj => obj.gameObject).ToList();

			GetComponentsFromList(collisions, out List<T> rightResults);

			results = rightResults.ToList();
		}

		private void GetComponentsFromList<T>(List<GameObject> gameObjects, out List<T> list)
		{
			list = new List<T>();
			
			foreach (GameObject gameObj in gameObjects)
			{
				if (gameObj.TryGetComponent(out T component))
				{
					list?.Add(component);
				}
			}
		}

		private List<Collider2D> GetCollidedObjects(RayData ray)
		{
			var rays = EvaluateRaysPositions(ray);
			List<Collider2D> collided = new List<Collider2D>();

			foreach (Vector2 rayPoint in rays)
			{
				RaycastHit2D hit = Physics2D.Raycast(rayPoint, ray.Direction, ray.Length, _settings.PhysicalObjLayer);
				collided.Add(hit.collider);
			}

			return collided;
		}

		private bool RaysDetect(RayData ray)
		{
			return EvaluateRaysPositions(ray)
				.Any(point => Physics2D.Raycast(point, ray.Direction, ray.Length, _settings.PhysicalObjLayer));
		}

		private IEnumerable<Vector2> EvaluateRaysPositions(RayData ray) {
			for (int i = 0; i < _settings.SideRaysCount; i++) {
				float sidePercent = (float) i / (_settings.SideRaysCount - 1);
				yield return Vector2.Lerp(ray.Start, ray.End, sidePercent);
			}
		}

		private void DrawRays()
		{
			if (Application.isPlaying == false)
				CalculateRaysPosition();
				
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
			Gizmos.DrawWireCube(transform.position + _settings.UnitBounds.center, _settings.UnitBounds.size);
		}
	}
}