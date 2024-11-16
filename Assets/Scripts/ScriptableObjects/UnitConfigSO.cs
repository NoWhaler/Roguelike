using Game.Units.Enum;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "UnitConfig", menuName = "Configurations/Unit Config", order = 0)]
    public class UnitConfigSO : ScriptableObject
    {
        [field: SerializeField] public UnitType UnitType { get; private set; }
        [field: SerializeField] public float MaxHealth { get; private set; }
        [field: SerializeField] public int MaxMovementPoints { get; private set; }
        [field: SerializeField] public float MinDamage { get; private set; }
        [field: SerializeField] public float MaxDamage { get; private set; }
        [field: SerializeField] public int AttackRange { get; private set; }
    }
}