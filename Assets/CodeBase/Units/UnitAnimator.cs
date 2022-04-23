using UnityEngine;

namespace CodeBase.Units
{
	public class UnitAnimator : MonoBehaviour
	{
		[SerializeField] private Animator _animator;
		[SerializeField] private float _walkSpeedMultiplier = 1;
		
		private readonly int _isWalkingHash = Animator.StringToHash("IsWalking");
		private readonly int _walkingSpeedHash = Animator.StringToHash("WalkingSpeed");
		private readonly int _jumpHash = Animator.StringToHash("Jump");
		private readonly int _landingHash = Animator.StringToHash("Land");

		public void PlayWalk(float speed)
		{
			_animator.SetBool(_isWalkingHash, speed != 0);
			_animator.SetFloat(_walkingSpeedHash, speed * _walkSpeedMultiplier);
		}

		public void PlayJump()
		{
			_animator.ResetTrigger(_landingHash);
			_animator.SetTrigger(_jumpHash);
		}

		public void PlayLanding() => _animator.SetTrigger(_landingHash);

		public void PlayIdle()
		{
			_animator.ResetTrigger(_jumpHash);
			_animator.ResetTrigger(_landingHash);
			_animator.SetBool(_isWalkingHash, false);
			
			_animator.Play("idle");
		}
	}
}