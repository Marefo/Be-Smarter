using System;
using System.Collections.Generic;

namespace CodeBase.Infrastructure
{
	public class GameStateMachine
	{
		private Dictionary<Type, IState> _states;

		public GameStateMachine()
		{
			_states = new Dictionary<Type, IState>()
			{
				[typeof(BootstrapState)] = new BootstrapState(),
			};
		}
		
		public void Enter<TState>() where TState : class, IState
		{
			
		}
		
	}

	public class BootstrapState : IState
	{
		public void Enter()
		{
			
		}

		public void Exit()
		{
			
		}
	}
}