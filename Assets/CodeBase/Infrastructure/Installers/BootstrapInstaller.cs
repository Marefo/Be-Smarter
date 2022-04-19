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
			Container.Bind<IProgressService>().To<ProgressService>().AsSingle();
			Container.Bind<CoroutineRunner>().FromInstance(_coroutineRunner).AsSingle();
			Container.Bind<SceneLoader>().AsSingle();
			Container.Bind<EnemyFactory>().AsSingle();
			
			Container.BindInterfacesAndSelfTo<InputService>().AsSingle();
			Container.BindInterfacesAndSelfTo<StaticDataService>().AsSingle();
		}
	}
}