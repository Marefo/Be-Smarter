using UnityEngine;

namespace CodeBase.DeathRay
{
	public class DeathRaySystem : MonoBehaviour
	{
		[SerializeField] private DeathRay _deathRay;
		[SerializeField] private DeathRayButton _deathRayButton;

		private void Start()
		{
			_deathRayButton.Pressed += OnButtonPress;
		}

		private void OnDestroy()
		{
			_deathRayButton.Pressed -= OnButtonPress;
		}

		private void OnButtonPress()
		{
			if(_deathRay.Active == false)
				_deathRay.Enable();
			else
				_deathRay.Disable();
		}
	}
}