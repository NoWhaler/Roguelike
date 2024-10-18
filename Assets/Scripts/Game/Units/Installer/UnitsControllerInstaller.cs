using Game.Units.Controller;
using Zenject;

namespace Game.Units.Installer
{
    public class UnitsControllerInstaller: MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<UnitsController>().AsSingle().NonLazy();
        }
    }
}