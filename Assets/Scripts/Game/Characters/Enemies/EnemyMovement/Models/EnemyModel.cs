using System;
using Core.ObjectPooling;
using Game.Characters.Player.Stats.Models;
using UnityEngine;
using Zenject;

namespace Game.Characters.Enemies.EnemyMovement.Models
{
    public class EnemyModel: MonoBehaviour
    {
        public event Action<float> OnTakeDamage;

        private PlayerStatsModel _playerStatsModel;
        
        [Inject]
        private void Constructor(PlayerStatsModel playerStatsModel)
        {
            _playerStatsModel = playerStatsModel;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            var arrow = other.GetComponent<Arrow>();
            
            if (arrow != null)
            {
                OnTakeDamage?.Invoke(_playerStatsModel.Damage);
            }
        }
    }
}