using UnityEngine;
using Zenject;

namespace Game.UI.UIGameplayScene.TechnologyPanel.Installer
{
    public class TechnologyPanelInstaller: MonoInstaller
    {
        [SerializeField] private UITechnologyPanel _technologyPanel;
        
        public override void InstallBindings()
        {
            Container.BindInstance(_technologyPanel).AsSingle();
        }
    }
}