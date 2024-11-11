using Game.EditorSettings.Controller;
using Zenject;

namespace Game.EditorSettings.Installer
{
    public class SettingsInstaller: MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<SettingsController>().AsSingle().NonLazy();
        }
    }
}