using System;
using System.Collections.Generic;
using Core.TurnBasedSystem;
using Game.ProductionResources.Enum;
using Zenject;

namespace Game.ProductionResources.Controller
{
    public class ResourcesController: IInitializable
    {
        private Dictionary<ResourceType, int> _resources = new Dictionary<ResourceType, int>()
        {   
            {
                ResourceType.Food, 0
            },
            
            {
                ResourceType.Stone, 0
            },
            
            {
                ResourceType.Wood, 0
            }
        };
        
        public void Initialize()
        {
            foreach (ResourceType resourceType in System.Enum.GetValues(typeof(ResourceType)))
            {
                _resources[resourceType] = 0;
            }
        }
        
        public void AddResource(ResourceType resourceType, int amount)
        {
            if (_resources.ContainsKey(resourceType))
            {
                _resources[resourceType] += amount;
            }
        }

        public int GetResourceAmount(ResourceType resourceType)
        {
            return _resources.ContainsKey(resourceType) ? _resources[resourceType] : 0;
        }
    }
}