using Game.Buildings.Struct;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "BuildingCost", menuName = "Game/Building Cost")]
    public class BuildingCostSO: ScriptableObject
    {
        [SerializeField] private BuildingCost[] _resourceCosts;
        
        public BuildingCost[] ResourceCosts => _resourceCosts;
    }
}