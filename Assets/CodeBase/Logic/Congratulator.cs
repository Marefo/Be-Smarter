using System;
using System.Collections.Generic;
using CodeBase.Infrastructure;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace CodeBase.Logic
{
	public class Congratulator : MonoBehaviour
	{
		[SerializeField] private List<ParticleSystem> _confetti;
		[SerializeField] private Vector2 _showDelay;
		
		private CoroutineRunner _coroutineRunner;

		[Inject]
		private void Construct(CoroutineRunner coroutineRunner)
		{
			_coroutineRunner = coroutineRunner;
		}
		
		private void Start() => Init();

		private void Init()
		{
			foreach (ParticleSystem confetti in _confetti)
			{
				float delay = Random.Range(_showDelay.x, _showDelay.y);
				_coroutineRunner.CallWithDelay(() => ShowConfetti(confetti), delay);
			}
		}

		private void ShowConfetti(ParticleSystem confetti)
		{
			if(confetti == null) return;
			
			confetti.Play();
			
			if(confetti.TryGetComponent(out AudioSource audioSource))
				audioSource.Play();
		}
	}
}