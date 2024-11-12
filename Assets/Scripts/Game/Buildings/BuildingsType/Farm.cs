using Game.Buildings.Interfaces;
using Game.Hex;
using Game.ProductionResources.Enum;
using UnityEngine;

namespace Game.Buildings.BuildingsType
{
    public class Farm: Building, IProduceResource
    {
        [field: SerializeField] public ResourceType ResourceType { get; set; }
        
        [field: SerializeField] public int ResourceAmountProduction { get; set; }
        
        public void ProduceResources()
        {
            _resourcesController.AddResource(ResourceType, ResourceAmountProduction);
        }
        
        public override void Initialize(HexModel hexModel)
        {
            base.Initialize(hexModel);
            ResourceType = ResourceType.Food;
            ResourceAmountProduction = 10;
        }
        
        protected override void SetupActions()
        {
            
        }
    }
}