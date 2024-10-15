using Zenject;

namespace Core.TurnBasedSystem.Installer
{
    public class GameTurnHandlerInstaller: MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GameTurnController>().AsSingle();
        }
    }
}