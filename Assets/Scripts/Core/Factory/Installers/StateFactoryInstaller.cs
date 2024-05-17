using Zenject;

namespace Core.Factory.Installers
{
    public class StateFactoryInstaller: MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<StateFactory>().AsSingle().NonLazy();
        }
    }
}