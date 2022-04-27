using System;
using CodeBase.Logic;
using UnityEngine;

namespace CodeBase.Claw
{
	[RequireComponent(typeof(Rigidbody2D), typeof(Interactable), typeof(Cube.Cube))]
	[RequireComponent(typeof(SpriteRenderer))]
	public class CubeCatchTarget : MonoBehaviour, ICatchTarget
	{
		public float SizeY => _sprite.bounds.size.y;
		public Transform Transform => transform;

		private SpriteRenderer _sprite;
		private Interactable _interactable;
		private Rigidbody2D _rigidbody;
		private Cube.Cube _cube;

		private void Awake()
		{
			_rigidbody = GetComponent<Rigidbody2D>();
			_sprite = GetComponent<SpriteRenderer>();
			_cube = GetComponent<Cube.Cube>();
			_interactable = GetComponent<Interactable>();
		}

		public void OnCatch()
		{
			_interactable.enabled = false;
			_cube.StopPush();
			_cube.enabled = false;
			_rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
		}
	}
}