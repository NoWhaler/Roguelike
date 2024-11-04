using UnityEngine;
using Zenject;

namespace Game.UI.UIGameplayScene.SelectedEntityInformation.Installer
{
    public class SelectedEntityViewInstaller: MonoInstaller
    {
        [SerializeField] private UISelectedEntityView _uiSelectedEntityView;
        
        public override void InstallBindings()
        {
            Container.BindInstance(_uiSelectedEntityView).AsSingle().NonLazy();
        }
    }
}