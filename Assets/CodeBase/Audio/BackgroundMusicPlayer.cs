using DG.Tweening;
using UnityEngine;

namespace CodeBase.Audio
{
	[RequireComponent(typeof(AudioSource))]
	public class BackgroundMusicPlayer : MonoBehaviour
	{
		[SerializeField] private float _fadeOutDuration = 1;
		[SerializeField] private float _fadeInDuration = 1;
		
		private AudioSource _audioSource;
		private float _defaultVolume;

		private void Awake()
		{
			DontDestroyOnLoad(this);
			_audioSource = GetComponent<AudioSource>();
		}

		private void Start()
		{
			_defaultVolume = _audioSource.volume;
		}

		public void ChangeMusicTo(AudioClip clip)
		{
			_audioSource.DOFade(0, _fadeOutDuration).OnComplete(() => OnFadeOutComplete(clip));
		}

		private void OnFadeOutComplete(AudioClip clip)
		{
			_audioSource.clip = clip;
			_audioSource.Play();
			_audioSource.DOFade(_defaultVolume, _fadeInDuration);
		}
	}
}