using Zenject;

namespace Game.WorldGeneration.Hex.Installer
{
    public class HexConfigurationInstaller: MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<HexMouseDetector>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<HexInteraction>().AsSingle().NonLazy();
        }
    }
}