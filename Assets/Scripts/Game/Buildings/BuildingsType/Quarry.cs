using Game.Buildings.Interfaces;
using Game.Hex;
using Game.ProductionResources.Enum;
using UnityEngine;

namespace Game.Buildings.BuildingsType
{
    public class Quarry: Building, IProduceResource
    {
        [field: SerializeField] public ResourceType ResourceType { get; set; }
        
        [field: SerializeField] public int ResourceAmountProduction { get; set; }
        
        public void ProduceResources()
        {
            _resourcesController.AddResource(ResourceType, ResourceAmountProduction);
        }

        protected override void SetupActions()
        {
            
        }
    }
}