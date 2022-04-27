using System;
using CodeBase.Audio;
using CodeBase.Logic;
using CodeBase.Units.Hero;
using UnityEngine;
using Zenject;

namespace CodeBase.DeathRay
{
	[RequireComponent(typeof(Interactable), typeof(Animator))]
	public class DeathRaySwitchingButton : DeathRayButton
	{
		public override event Action StateChanged;

		[SerializeField] private AudioClip _interactSfx;
		
		private Interactable _interactable;
		private Animator _animator;
		private SFXPlayer _sfxPlayer;
		
		private readonly int _pressHash = Animator.StringToHash("Press");

		[Inject]
		private void Construct(SFXPlayer sfxPlayer)
		{
			_sfxPlayer = sfxPlayer;
		}
		
		private void Awake()
		{
			_interactable = GetComponent<Interactable>();
			_animator = GetComponent<Animator>();
		}

		private void OnEnable()
		{
			_interactable.Interacted += OnInteract;
		}

		private void OnDisable()
		{
			_interactable.Interacted -= OnInteract;
		}

		private void OnInteract(HeroInteractable heroInteractable)
		{
			_animator.SetTrigger(_pressHash);
			_sfxPlayer.Play(_interactSfx);
			StateChanged?.Invoke();
		}
	}
}