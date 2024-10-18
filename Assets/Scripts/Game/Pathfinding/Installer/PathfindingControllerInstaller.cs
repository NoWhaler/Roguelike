using Zenject;

namespace Game.Pathfinding.Installer
{
    public class PathfindingControllerInstaller: MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<PathfindingController>().AsSingle().NonLazy();
        }
    }
}