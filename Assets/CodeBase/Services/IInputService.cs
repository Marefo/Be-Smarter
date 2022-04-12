using System;

namespace CodeBase.Services
{
	public interface IInputService
	{
		event Action JumpBtnPressed;
		event Action InteractBtnPressed;
		float AxisX { get; }
	}
}