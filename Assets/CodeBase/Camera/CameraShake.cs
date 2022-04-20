using System.Collections;
using Cinemachine;
using UnityEngine;

namespace CodeBase.Camera
{
	public class CameraShake : MonoBehaviour
	{
		private CinemachineVirtualCamera _virtualCamera;
		private CinemachineBasicMultiChannelPerlin _basicMultiChannelPerlin;
		private float _shakeTimer;
		private float _shakeTimerTotal;
		private float _startingIntensity;
		private Coroutine _shakeCoroutine;

		private void Awake()
		{
			_virtualCamera = GetComponent<CinemachineVirtualCamera>();
			_basicMultiChannelPerlin = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
		}

		public void Shake(CameraShakeSettings settings)
		{
			if(_shakeCoroutine != null)
				StopCoroutine(_shakeCoroutine);
			
			_basicMultiChannelPerlin.m_AmplitudeGain = settings.Intensity;

			_startingIntensity = settings.Intensity;
			_shakeTimerTotal = settings.Time;
			_shakeTimer = settings.Time;

			_shakeCoroutine = StartCoroutine(ShakeCoroutine());
		}

		private IEnumerator ShakeCoroutine()
		{
			while (true)
			{
				_shakeTimer = Mathf.Clamp(_shakeTimer - Time.deltaTime, 0, float.MaxValue);
				_basicMultiChannelPerlin.m_AmplitudeGain = Mathf.Lerp(_startingIntensity, 0f, 1 - _shakeTimer / _shakeTimerTotal);

				if (_shakeTimer == 0)
					yield break;
				else
					yield return null;
			}
		}
	}
}