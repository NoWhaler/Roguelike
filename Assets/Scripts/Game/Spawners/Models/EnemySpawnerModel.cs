using UnityEngine;

namespace Game.Spawners.Models
{
    public class EnemySpawnerModel : MonoBehaviour
    {
        [field: SerializeField] public GameObject GameObjectPrefab { get; set; }
        
        [field: SerializeField] public Collider GroundCollider { get; set; }
        
        [field: SerializeField] public int EnemiesAmount { get; set; }
        
        [field: SerializeField] public int[] ObstaclesLayers { get; set; }
    }
}