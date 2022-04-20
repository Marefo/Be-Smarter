using CodeBase.Logic;
using CodeBase.Units;
using DG.Tweening;
using UnityEngine;

namespace CodeBase.DeathRay
{
	[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
	public class DeathRay : MonoBehaviour
	{
		public bool Active { get; private set; }

		private Collider2D _collider;
		private SpriteRenderer _spriteRenderer;

		private void Awake()
		{
			_collider = GetComponent<Collider2D>();
			_spriteRenderer = GetComponent<SpriteRenderer>();
		}

		private void OnTriggerEnter2D(Collider2D col)
		{
			if(col.TryGetComponent(out UnitDeath unitDeath))
				unitDeath.Die();
		}

		public void Enable()
		{
			Active = true;
			
			_collider.enabled = true;
			_spriteRenderer.DOFade(1, 0.15f);
		}

		public void Disable()
		{
			Active = false;
			
			_collider.enabled = false;
			_spriteRenderer.DOFade(0, 0.15f);
		}
	}
}