using System;

namespace CodeBase.Services
{
	public interface IInputService
	{
		event Action BootstrapBtnPressed;
		event Action JumpBtnPressed;
		event Action InteractBtnPressed;
		event Action RestartBtnPressed;
		float AxisX { get; }
	}
}