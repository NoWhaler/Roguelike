using Game.Units.Enum;
using UnityEngine;

namespace Game.Units
{
    public abstract class Unit: MonoBehaviour
    {
        [field: SerializeField] public UnitType UnitType { get; set; }
        
        [field: SerializeField] public float MaxHealth { get; set; }
        
        [field: SerializeField] public float CurrentHealth { get; set; }

        public void Initialize()
        {
            CurrentHealth = MaxHealth;
        }
    }
}