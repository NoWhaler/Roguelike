using Game.Buildings.BuildingActions;
using Game.Buildings.Interfaces;
using Game.ProductionResources.Enum;
using Game.WorldGeneration.Hex;
using UnityEngine;

namespace Game.Buildings.BuildingsType
{
    public class Lumber: Building, IProduceResource
    {
        [field: SerializeField] public ResourceType ResourceType { get; set; }
        
        [field: SerializeField] public int ResourceAmountProduction { get; set; }
        public void ProduceResources()
        {
            _resourcesController.AddResource(ResourceType, ResourceAmountProduction);
        }

        public override void Initialize(HexModel hexModel)
        {
            ResourceType = ResourceType.Wood;
            ResourceAmountProduction = 4;
        }

        protected override void SetupActions()
        {
            _availableActions.Add(new ProductionUpgradeAction("Lumber Efficiency", 150, 2, this));
            _availableActions.Add(new ResearchAction("Efficient Logging", 250, 1, _researchesController, this));
            _availableActions.Add(new ResearchAction("Sustainable Forestry", 250, 1, _researchesController, this));
        }
        
    }
}