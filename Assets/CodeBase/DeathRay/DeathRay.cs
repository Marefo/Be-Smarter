using System;
using CodeBase.Logic;
using CodeBase.Units;
using DG.Tweening;
using UnityEngine;

namespace CodeBase.DeathRay
{
	[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
	public class DeathRay : MonoBehaviour
	{
		[SerializeField] private bool _startActive = true;
		[Space(10)]
		[SerializeField] private AudioSource _audioSource;
		
		private Collider2D _collider;
		private SpriteRenderer _spriteRenderer;
		private bool _active;

		private void Awake()
		{
			_collider = GetComponent<Collider2D>();
			_spriteRenderer = GetComponent<SpriteRenderer>();
		}

		private void Start() => Init();

		private void OnTriggerEnter2D(Collider2D col)
		{
			if(col.TryGetComponent(out UnitDeath unitDeath))
				unitDeath.Die();
		}

		public void ChangeState()
		{
			if(_active)
				Disable();
			else
				Enable();
		}

		private void Init()
		{
			if(_startActive)
				Enable();
			else
				Disable();
		}
		
		private void Enable()
		{
			_active = true;
			_collider.enabled = true;
			_audioSource.Play();
			_spriteRenderer.DOFade(1, 0.18f);
		}

		private void Disable()
		{
			_active = false;
			_collider.enabled = false;
			_audioSource.Stop();
			_spriteRenderer.DOFade(0, 0.18f);
		}
	}
}