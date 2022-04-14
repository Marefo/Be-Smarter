using CodeBase.Services;
using UnityEngine;
using Zenject;

namespace CodeBase.Infrastructure.Installers
{
	public class BootstrapInstaller : MonoInstaller
	{
		[SerializeField] private CoroutineRunner _coroutineRunner;

		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<InputService>().AsSingle();
			Container.Bind<IProgressService>().To<ProgressService>().AsSingle();
			Container.Bind<CoroutineRunner>().FromInstance(_coroutineRunner).AsSingle();
			Container.Bind<SceneLoader>().AsSingle();
		}
	}
}