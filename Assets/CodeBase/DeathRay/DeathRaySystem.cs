using UnityEngine;

namespace CodeBase.DeathRay
{
	public class DeathRaySystem : MonoBehaviour
	{
		[SerializeField] private DeathRay _deathRay;
		[SerializeField] private DeathRayButton _deathRayButton;
		[Space(10)]
		[SerializeField] private bool _startActive = true;

		private void Start()
		{
			_deathRayButton.StateChanged += OnStateChange;
			Init();
		}

		private void OnDestroy()
		{
			_deathRayButton.StateChanged -= OnStateChange;
		}

		private void Init()
		{
			if (_startActive == true)
				_deathRay.Enable();
			else
				_deathRay.Disable();
		}
		
		private void OnStateChange()
		{
			if(_deathRay.Active == false)
				_deathRay.Enable();
			else
				_deathRay.Disable();
		}
	}
}