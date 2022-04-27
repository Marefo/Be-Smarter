using CodeBase.Audio;
using CodeBase.Infrastructure.AssetManagement;
using CodeBase.Logic;
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
		[SerializeField] private GameObject _backgroundMusicPlayer;
		[SerializeField] private GameObject _sfxPlayer;

		public override void InstallBindings()
		{
			Container.Bind<AssetProvider>().AsSingle();
			Container.Bind<IProgressService>().To<ProgressService>().AsSingle();
			Container.Bind<CoroutineRunner>().FromInstance(_coroutineRunner).AsSingle();
			Container.Bind<LoadingCurtain>().FromComponentInNewPrefab(_loadingCurtainPrefab).AsSingle();
			Container.Bind<BackgroundMusicPlayer>().FromComponentInNewPrefab(_backgroundMusicPlayer).AsSingle().NonLazy();
			Container.Bind<SFXPlayer>().FromComponentInNewPrefab(_sfxPlayer).AsSingle().NonLazy();
			Container.Bind<SceneLoader>().AsSingle();
			Container.Bind<LevelLoader>().AsSingle();
			Container.Bind<EnemyFactory>().AsSingle();
			Container.Bind<StaticDataService>().AsSingle().NonLazy();
			Container.BindInterfacesAndSelfTo<InputService>().AsSingle();
		}
	}
}