using System.ComponentModel;
using Zenject;

namespace Core.Initialization.Bootstrapper.Installers
{
    public class BootstrapInstaller: MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<Bootstrap>().AsSingle().NonLazy();
        }
    }
}