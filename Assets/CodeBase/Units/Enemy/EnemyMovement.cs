using System;
using CodeBase.Collisions;
using CodeBase.Logic;
using UnityEngine;

namespace CodeBase.Units.Enemy
{
	[RequireComponent(typeof(CollisionDetector), typeof(SpriteRenderer), typeof(UnitAnimator))]
	public abstract class EnemyMovement : MonoBehaviour, IHoldingBtnActivator
	{
		
	}
}