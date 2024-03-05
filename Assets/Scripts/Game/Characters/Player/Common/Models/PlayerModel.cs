using UnityEngine;

namespace Game.Characters.Player.Common.Models
{
    public class PlayerModel: MonoBehaviour
    {
        [field: SerializeField] public float MoveSpeed { get; set; }
    }
}
