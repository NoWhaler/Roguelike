using System;
using Game.Characters.Enemies.EnemyMovement.Models;
using Game.Characters.Enemies.EnemyStats.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Game.Characters.Enemies.EnemyStats.Views
{
    public class EnemyStatsView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _healthText;

        [SerializeField] private Image _fillImage;

        private EnemyStatModel _enemyStatModel;

        private EnemyModel _enemyModel;
        
        private void UpdateHealthBar()
        {
            float fraction = _enemyStatModel.CurrentHealthPoints / _enemyStatModel.MaxHealthPoints;

            _fillImage.fillAmount = fraction;

            _healthText.text = $"{(int)_enemyStatModel.CurrentHealthPoints}/{(int)_enemyStatModel.MaxHealthPoints}";
        }

        public void OnEnable()
        {
            Debug.Log("Stats initialized");
            _enemyModel = GetComponentInParent<EnemyModel>();
            _enemyStatModel = new EnemyStatModel(Random.Range(10, 40));
            _enemyStatModel.CurrentHealthPoints = _enemyStatModel.MaxHealthPoints;
            _enemyModel.OnTakeDamage += UpdateCurrentHealth;
            
            UpdateHealthBar();
        }

        private void OnDisable()
        {
            _enemyModel.OnTakeDamage -= UpdateCurrentHealth;
        }
        
        private void UpdateCurrentHealth(float damage)
        {
            _enemyStatModel.CurrentHealthPoints -= damage;
            
            UpdateHealthBar();
        }
    }
}