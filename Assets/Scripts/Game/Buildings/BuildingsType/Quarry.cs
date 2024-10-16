using Game.Buildings.Interfaces;
using Game.ProductionResources.Enum;
using Game.WorldGeneration.Hex;
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
        
        public override void Initialize(HexModel hexModel)
        {
            ResourceType = ResourceType.Stone;
            ResourceAmountProduction = 6;
        }

        protected override void SetupActions()
        {
            
        }
    }
}