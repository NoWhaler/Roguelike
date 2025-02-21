using Core.Lifetime.Initialization;
using Zenject;

namespace Core.Lifetime.Installer
{
    public class InitializationsInstaller: MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GameEntryPoint>().AsSingle();
            Container.BindInterfacesAndSelfTo<ResearchesInitialization>().AsSingle();
            Container.BindInterfacesAndSelfTo<TechnologiesInitialization>().AsSingle();
        }
    }
}