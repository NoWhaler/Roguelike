using System;
using UnityEngine;

namespace Core.Data
{
    [Serializable]
    public class PlayerData
    {
        [field: SerializeField] public float PlayerCurrentHealth {get; set; }
        
        [field: SerializeField] public float PlayerMaxHealth { get; set; }
        
        [field: SerializeField] public float PlayerDamage { get; set; }
    }
}