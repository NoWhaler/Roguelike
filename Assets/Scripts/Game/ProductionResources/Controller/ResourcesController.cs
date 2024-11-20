using System.Collections.Generic;
using Game.Hex;
using Game.ProductionResources.Enum;
using Game.UI.UIGameplayScene.UIResourcesPanel;
using UnityEngine;
using Zenject;

namespace Game.ProductionResources.Controller
{
    public class ResourcesController: IInitializable
    {
        private Dictionary<ResourceType, int> _resources = new Dictionary<ResourceType, int>()
        {   
            {
                ResourceType.Food, 100
            },
            
            {
                ResourceType.Stone, 100
            },
            
            {
                ResourceType.Wood, 100
            }
        };
        
        private Dictionary<ResourceType, ResourceDeposit> _resourcesDepositPrefabs;
        
        private ResourcesPanel _resourcesPanel;

        private HexGridController _hexGridController;
        
        [Inject]
        private void Constructor(ResourcesPanel resourcesPanel,
            DiContainer diContainer,
            [Inject(Id = ResourceType.Wood)] ResourceDeposit wood,
            [Inject(Id = ResourceType.Stone)] ResourceDeposit stone,
            [Inject(Id = ResourceType.Food)] ResourceDeposit food)
        {
            _resourcesPanel = resourcesPanel;

            _resourcesDepositPrefabs = new Dictionary<ResourceType, ResourceDeposit>()
            {
                { ResourceType.Food , food},
                { ResourceType.Stone , stone},
                { ResourceType.Wood , wood}
            };
        }
        
        public void Initialize()
        {
            foreach (ResourceType resourceType in System.Enum.GetValues(typeof(ResourceType)))
            {
                _resources[resourceType] = 100;
            }
            
            _resourcesPanel.UpdateResourcesAmount();
        }
        
        public void AddResource(ResourceType resourceType, int amount)
        {
            if (_resources.ContainsKey(resourceType))
            {
                _resources[resourceType] += amount;
                _resourcesPanel.UpdateResourcesAmount();
            }
        }
        
        public ResourceDeposit GetResourcePrefab(ResourceType resourceType)
        {
           return _resourcesDepositPrefabs.GetValueOrDefault(resourceType);
        }

        public int GetResourceAmount(ResourceType resourceType)
        {
            return _resources.ContainsKey(resourceType) ? _resources[resourceType] : 0;
        }
        
        public void DeductResource(ResourceType resourceType, int amount)
        {
            if (_resources.ContainsKey(resourceType))
            {
                _resources[resourceType] = Mathf.Max(0, _resources[resourceType] - amount);
                _resourcesPanel.UpdateResourcesAmount();
            }
        }
    }
}