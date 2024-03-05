using System;
using UnityEngine;

namespace Game.Characters.Enemies.EnemyStats.Models
{
    public class EnemyStatModel 
    {
        public float CurrentHealthPoints { get; set; }
        
        public float MaxHealthPoints { get; set; }

        public EnemyStatModel(int maxHealth)
        {
            MaxHealthPoints = maxHealth;
        }
    }
}