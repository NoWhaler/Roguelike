using Game.Buildings.Enum;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "BuildingConfig", menuName = "Configurations/Building Config", order = 0)]
    public class BuildingConfigSO : ScriptableObject
    {
        [field: SerializeField] public BuildingType BuildingType { get; private set; }
        [field: SerializeField] public float MaxHealth { get; private set; }
        [field: SerializeField] public int RevealFogOfWarRange { get; private set; }
        [field: SerializeField] public BuildingCostSO BuildingCost { get; private set; }
    }
}