using UnityEngine;
using Zenject;

namespace Game.UI.UIGameplayScene.BuildingsActionPanel.Installer
{
    public class BuildingsActionPanelInstaller: MonoInstaller
    {
        [SerializeField] private UIBuildingsActionPanel _uiBuildingsActionPanel;

        [SerializeField] private GameActionsPanel _gameActionsPanel;
        
        public override void InstallBindings()
        {
            Container.BindInstance(_uiBuildingsActionPanel).AsSingle().NonLazy();
            Container.BindInstance(_gameActionsPanel).AsSingle().NonLazy();
        }
    }
}