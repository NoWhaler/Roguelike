using System;
using System.Collections.Generic;
using Game.ProductionResources.Enum;
using UnityEngine;

namespace Game.Buildings.Struct
{
    [Serializable]
    public class BuildingCost
    {
        [field: SerializeField] public ResourceType ResourceType { get; set; }
        [field: SerializeField] public int Amount { get; set; }
    }
}