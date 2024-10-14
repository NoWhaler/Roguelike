using UnityEngine;
using Zenject;

namespace Game.UI.UIGameplayScene.BuildingsActionPanel.Installer
{
    public class BuildingsActionPanelInstaller: MonoInstaller
    {
        [SerializeField] private UIBuildingsActionPanel _uiBuildingsActionPanel;
        
        public override void InstallBindings()
        {
            Container.BindInstance(_uiBuildingsActionPanel).AsSingle().NonLazy();
        }
    }
}