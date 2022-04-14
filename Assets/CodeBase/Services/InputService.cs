using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Zenject;

namespace CodeBase.Services
{
	public class InputService : IInputService, IInitializable, IDisposable
	{
		public event Action JumpBtnPressed;
		public event Action InteractBtnPressed;
		public event Action RestartBtnPressed;
		public float AxisX => _input.Player.Move.ReadValue<Vector2>().x;

		private readonly InputActions _input;

		public InputService() => _input = new InputActions();

		public void Initialize()
		{
			_input.Enable();
			SubscribeEvents();

			Debug.Log("Input service initialized\n----");
		}

		public void Dispose()
		{
			CleanUp();
			_input.Disable();
		}

		private void SubscribeEvents()
		{
			_input.Player.Jump.performed += OnJumpActionPerformed;
			_input.Player.Interact.performed += OnInteractActionPerformed;
			_input.Gameplay.Restart.performed += OnRestartActionPerformed;
		}

		private void CleanUp()
		{
			_input.Player.Jump.performed -= OnJumpActionPerformed;
			_input.Player.Interact.performed -= OnInteractActionPerformed;
			_input.Gameplay.Restart.performed -= OnRestartActionPerformed;
		}

		private void OnRestartActionPerformed(InputAction.CallbackContext obj) => RestartBtnPressed?.Invoke();
		private void OnJumpActionPerformed(InputAction.CallbackContext ctx) => JumpBtnPressed?.Invoke();
		private void OnInteractActionPerformed(InputAction.CallbackContext obj) => InteractBtnPressed?.Invoke();
	}
}