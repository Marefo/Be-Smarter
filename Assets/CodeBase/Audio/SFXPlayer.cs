using System;
using UnityEngine;

namespace CodeBase.Audio
{
	[RequireComponent(typeof(SFXPlayer))]
	public class SFXPlayer : MonoBehaviour
	{
		private AudioSource _audioSource;

		private void Awake()
		{
			DontDestroyOnLoad(this);
			_audioSource = GetComponent<AudioSource>();
		}

		public void Play(AudioClip clip) => _audioSource.PlayOneShot(clip);
	}
}