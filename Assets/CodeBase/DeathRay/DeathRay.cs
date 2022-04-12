using CodeBase.Hero;
using UnityEngine;

namespace CodeBase.DeathRay
{
	[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
	public class DeathRay : MonoBehaviour
	{
		public bool Active { get; private set; } = true;
		
		private Collider2D _collider;
		private SpriteRenderer _spriteRenderer;

		private void Start()
		{
			_collider = GetComponent<Collider2D>();
			_spriteRenderer = GetComponent<SpriteRenderer>();
		}

		private void OnTriggerEnter2D(Collider2D col)
		{
			if(col.TryGetComponent(out HeroDeath heroDeath))
				heroDeath.Die();
		}
		
		public void Enable()
		{
			Active = true;
			
			_collider.enabled = true;
			_spriteRenderer.enabled = true;
		}
		
		public void Disable()
		{
			Active = false;
			
			_collider.enabled = false;
			_spriteRenderer.enabled = false;
		}
	}
}