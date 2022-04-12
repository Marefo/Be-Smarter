using UnityEngine;

namespace CodeBase.Infrastructure
{
	public class GameBootstrapper : MonoBehaviour
	{
		private GameStateMachine _stateMachine;

		private void Awake()
		{
			_stateMachine = new GameStateMachine();
		}
	}
}
