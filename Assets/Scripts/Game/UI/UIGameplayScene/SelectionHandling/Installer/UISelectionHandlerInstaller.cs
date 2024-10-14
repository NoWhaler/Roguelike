using Zenject;

namespace Game.UI.UIGameplayScene.SelectionHandling.Installer
{
    public class UISelectionHandlerInstaller: MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<UISelectionHandler>().AsSingle().NonLazy();
        }
    }
}