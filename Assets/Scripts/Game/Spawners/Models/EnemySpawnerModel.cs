using Game.Units;
using UnityEngine;

namespace Game.Spawners.Models
{
    public class EnemySpawnerModel : MonoBehaviour
    {
        [field: SerializeField] public Unit[] EnemyUnitPrefabs { get; private set; }
        
        [field: SerializeField] public int MinUnitsPerWave { get; private set; }
        
        [field: SerializeField] public int MaxUnitsPerWave { get; private set; }
        
        [field: SerializeField] public int MinTurnsBetweenWaves { get; private set; }
        
        [field: SerializeField] public int MaxTurnsBetweenWaves { get; private set; }
    }
}