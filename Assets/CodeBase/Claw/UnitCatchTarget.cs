using CodeBase.Collisions;
using CodeBase.Logic;
using CodeBase.Units;
using UnityEngine;

namespace CodeBase.Claw
{
	[RequireComponent(typeof(CollisionDetector), typeof(UnitMovement), typeof(UnitDeath))]
	public class UnitCatchTarget : MonoBehaviour, ICatchTarget
	{
		public float SizeY => _collisionDetector.Bounds.size.y;
		public Transform Transform => transform;

		private CollisionDetector _collisionDetector;
		private UnitMovement _unitMovement;
		private UnitDeath _mortal;

		private void Start()
		{
			_collisionDetector = GetComponent<CollisionDetector>();
			_unitMovement = GetComponent<UnitMovement>();
			_mortal = GetComponent<UnitDeath>();
		}

		public void OnCatch()
		{
			_unitMovement.Disable();
			_unitMovement.enabled = false;
			_mortal.Immobilize();
		}
	}
}