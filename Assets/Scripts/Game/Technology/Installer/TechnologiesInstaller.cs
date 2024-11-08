using Game.Technology.Controller;
using Zenject;

namespace Game.Technology.Installer
{
    public class TechnologiesInstaller: MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<TechnologiesController>().AsSingle().NonLazy();
        }
    }
}