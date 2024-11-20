using Game.ProductionResources.Enum;
using UnityEngine;

namespace Game.ProductionResources
{
    public class ResourceDeposit: MonoBehaviour
    {
        [field: SerializeField] public ResourceType ResourceType { get; set; }
    }
}