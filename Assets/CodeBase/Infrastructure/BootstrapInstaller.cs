using CodeBase.Services;
using Zenject;

namespace CodeBase.Infrastructure
{
	public class BootstrapInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<InputService>().AsSingle();
		}
	}
}