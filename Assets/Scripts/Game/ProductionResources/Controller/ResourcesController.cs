using System.Collections.Generic;
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
        
        private ResourcesPanel _resourcesPanel;
        
        [Inject]
        private void Constructor(ResourcesPanel resourcesPanel)
        {
            _resourcesPanel = resourcesPanel;
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