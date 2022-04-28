using System;
using CodeBase.Services;
using UnityEngine;
using Zenject;

namespace CodeBase.Infrastructure
{
	public class GameBootstrapper : MonoBehaviour
	{
		private LevelLoader _levelLoader;
		private IInputService _inputService;

		[Inject]
		private void Construct(LevelLoader levelLoader, IInputService inputService)
		{
			_levelLoader = levelLoader;
			_inputService = inputService;
		}

		private void Start()
		{
			_inputService.BootstrapBtnPressed += StartBootstrap;
		}

		private void OnDestroy() => _inputService.BootstrapBtnPressed -= StartBootstrap;

		private void StartBootstrap() => _levelLoader.LoadSavedLevel();
	}
}
