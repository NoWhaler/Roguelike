using System;
using System.Threading;
using UnityEngine;

namespace Game.Characters.Player.PlayerMovement.Models
{
    public class PlayerModel: MonoBehaviour
    {
        [field: SerializeField] public float RotationSmoothness { get; set; }
        
        [field: SerializeField] public float RotationSpeed { get; set; }
        
        [field: SerializeField] public float MoveSpeed { get; set; }
        
        [field: SerializeField] public Transform ArrowSpawnPosition { get; set; }
       
        [field: SerializeField] public Transform PlayerLookAtPoint { get; set; }
        
        public Vector3 MovementDirection { get; set; }

        public Vector3 PlayerFacingDirection { get; set; }
        
        public Quaternion TargetRotation { get; set; }

        public event Action<Vector3, RaycastHit> OnRotateTowardsTarget;

        public bool IsTurningForAttack { get; set; } = false;

        public CancellationTokenSource CancellationTokenSource { get; set; }
        
        public void RotateTowardsAttackTarget(Vector3 attackDirection, RaycastHit hit)
        {
            CancelRotationTowardsTarget();
            CancellationTokenSource = new CancellationTokenSource();
            OnRotateTowardsTarget?.Invoke(attackDirection, hit);
        }

        public void CancelRotationTowardsTarget()
        {
            CancellationTokenSource?.Cancel();
        }
    }
}
