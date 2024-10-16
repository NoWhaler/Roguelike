using Game.ProductionResources.Controller;
using Game.UI.UIGameplayScene.UIResourcesPanel;
using UnityEngine;
using Zenject;

namespace Game.ProductionResources.Installer
{
    public class ResourcesControllerInstaller: MonoInstaller
    {
        [SerializeField] private ResourcesPanel _resourcesPanel;
        
        public override void InstallBindings()
        {
            Container.BindInstance(_resourcesPanel).AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<ResourcesController>().AsSingle().NonLazy();
        }
    }
}