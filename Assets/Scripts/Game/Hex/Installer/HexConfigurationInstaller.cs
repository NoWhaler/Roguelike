using Zenject;

namespace Game.Hex.Installer
{
    public class HexConfigurationInstaller: MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<HexMouseDetector>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<HexInteraction>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<HexGridController>().AsSingle().NonLazy();
        }
    }
}