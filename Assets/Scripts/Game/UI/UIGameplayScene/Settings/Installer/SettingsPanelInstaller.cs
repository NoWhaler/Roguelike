using UnityEngine;
using Zenject;

namespace Game.UI.UIGameplayScene.Settings.Installer
{
    public class SettingsPanelInstaller: MonoInstaller
    {
        [SerializeField] private UISettingsPanel _settingsPanel;
        
        public override void InstallBindings()
        {
            Container.BindInstance(_settingsPanel).AsSingle();
        }
    }
}