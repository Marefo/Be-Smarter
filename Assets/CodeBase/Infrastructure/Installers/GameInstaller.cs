using CodeBase.Logic;
using CodeBase.Units.Hero;
using UnityEngine;
using Zenject;

namespace CodeBase.Infrastructure.Installers
{
	public class GameInstaller : MonoInstaller
	{
		[SerializeField] private LevelPreparer _levelPreparer;
		[SerializeField] private GameObject _heroPrefab;
		[SerializeField] private Transform _heroSpawnPoint;

		public override void InstallBindings()
		{
			Container.Bind<GameStatus>().AsSingle();
			Container.BindInterfacesTo<GameReStarter>().AsSingle().NonLazy();
			BindHero();
			Container.BindInstance(_levelPreparer).AsSingle();
		}

		private void BindHero()
		{
			HeroMovement heroMovement = Container.InstantiatePrefabForComponent<HeroMovement>(_heroPrefab, _heroSpawnPoint.position, Quaternion.identity, null);
			Container.Bind<HeroMovement>().FromInstance(heroMovement).AsSingle();
		}
	}
}