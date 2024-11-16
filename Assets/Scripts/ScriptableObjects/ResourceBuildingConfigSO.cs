using Game.ProductionResources.Enum;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "ResourceBuildingConfig", menuName = "Configurations/Resource Building Config")]
    public class ResourceBuildingConfigSO: BuildingConfigSO
    {
        [field: SerializeField] public ResourceType ResourceType { get; private set; }
        [field: SerializeField] public int ResourceAmountProduction { get; private set; }
    }
}