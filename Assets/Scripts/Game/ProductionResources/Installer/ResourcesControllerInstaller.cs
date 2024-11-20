using Game.ProductionResources.Controller;
using Game.ProductionResources.Enum;
using Game.UI.UIGameplayScene.UIResourcesPanel;
using UnityEngine;
using Zenject;

namespace Game.ProductionResources.Installer
{
    public class ResourcesControllerInstaller: MonoInstaller
    {
        [SerializeField] private ResourcesPanel _resourcesPanel;

        [SerializeField] private ResourceDeposit _woodResourceDeposit;

        [SerializeField] private ResourceDeposit _stoneResourceDeposit;

        [SerializeField] private ResourceDeposit _foodResourceDeposit;
        
        public override void InstallBindings()
        {
            Container.BindInstance(_resourcesPanel).AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<ResourcesController>().AsSingle().NonLazy();

            Container.Bind<ResourceDeposit>().WithId(ResourceType.Wood).FromInstance(_woodResourceDeposit);
            Container.Bind<ResourceDeposit>().WithId(ResourceType.Stone).FromInstance(_stoneResourceDeposit);
            Container.Bind<ResourceDeposit>().WithId(ResourceType.Food).FromInstance(_foodResourceDeposit);
        }
    }
}