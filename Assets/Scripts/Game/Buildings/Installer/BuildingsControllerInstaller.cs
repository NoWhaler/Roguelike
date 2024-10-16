using Game.Buildings.Controller;
using Zenject;

namespace Game.Buildings.Installer
{
    public class BuildingsControllerInstaller: MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<BuildingsController>().AsSingle();
        }
    }
}