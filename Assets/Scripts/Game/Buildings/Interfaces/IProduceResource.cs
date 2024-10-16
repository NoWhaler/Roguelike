using Game.ProductionResources.Enum;

namespace Game.Buildings.Interfaces
{
    public interface IProduceResource
    {
        ResourceType ResourceType { get; set; }
        
        int ResourceAmountProduction { get; set; }

        void ProduceResources();
    }
}