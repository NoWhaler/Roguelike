using Core.Lifetime.Initialization;
using Zenject;

namespace Core.Lifetime.Installer
{
    public class InitializationsInstaller: MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<ResearchesInitialization>().AsSingle();
        }
    }
}