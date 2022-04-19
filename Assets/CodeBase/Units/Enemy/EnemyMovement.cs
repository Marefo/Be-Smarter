using System;
using CodeBase.Collisions;
using CodeBase.Infrastructure;
using CodeBase.Logic;
using CodeBase.Units.Hero;
using UnityEngine;

namespace CodeBase.Units.Enemy
{
	[RequireComponent(typeof(CollisionDetector), typeof(SpriteRenderer), typeof(UnitAnimator))]
	public abstract class EnemyMovement : UnitMovement, IHoldingBtnActivator
	{
		protected CoroutineRunner _coroutineRunner;
		
		public void Construct(CoroutineRunner coroutineRunner)
		{
			_coroutineRunner = coroutineRunner;
		}
	}
}