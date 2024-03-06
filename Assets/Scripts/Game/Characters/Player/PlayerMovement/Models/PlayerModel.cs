using UnityEngine;

namespace Game.Characters.Player.PlayerMovement.Models
{
    public class PlayerModel: MonoBehaviour
    {
        [field: SerializeField] public float RotationSmoothness { get; set; }
        
        [field: SerializeField] public float RotationSpeed { get; set; }
        
        [field: SerializeField] public float MoveSpeed { get; set; }
        
        [field: SerializeField] public Transform ArrowSpawnPosition { get; set; }
    }
}
