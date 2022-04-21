using CodeBase.Services;
using CodeBase.UI;
using UnityEngine;
using Zenject;

namespace CodeBase.Infrastructure.Installers
{
	public class BootstrapInstaller : MonoInstaller
	{
		[SerializeField] private GameObject _loadingCurtainPrefab;
		[SerializeField] private CoroutineRunner _coroutineRunner;

		public override void InstallBindings()
		{
			Container.Bind<IProgressService>().To<ProgressService>().AsSingle();
			Container.Bind<CoroutineRunner>().FromInstance(_coroutineRunner).AsSingle();
			Container.Bind<LoadingCurtain>().FromComponentInNewPrefab(_loadingCurtainPrefab).AsSingle();
			Container.Bind<SceneLoader>().AsSingle();
			Container.Bind<LevelLoader>().AsSingle();
			Container.Bind<EnemyFactory>().AsSingle();
			Container.Bind<StaticDataService>().AsSingle().NonLazy();
			Container.BindInterfacesAndSelfTo<InputService>().AsSingle();
		}
	}
}