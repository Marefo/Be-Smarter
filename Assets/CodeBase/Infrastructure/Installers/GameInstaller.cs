using CodeBase.Hero;
using CodeBase.Logic;
using UnityEngine;
using Zenject;

namespace CodeBase.Infrastructure.Installers
{
	public class GameInstaller : MonoInstaller
	{
		[SerializeField] private GameObject _heroPrefab;
		[SerializeField] private Transform _heroSpawnPoint;

		public override void InstallBindings()
		{
			Container.BindInterfacesTo<GameReStarter>().AsSingle().NonLazy();
			BindHero();
		}

		private void BindHero()
		{
			HeroMovement heroMovement = Container.InstantiatePrefabForComponent<HeroMovement>(_heroPrefab, _heroSpawnPoint.position, Quaternion.identity, null);
			Container.Bind<HeroMovement>().FromInstance(heroMovement).AsSingle();
		}
	}
}