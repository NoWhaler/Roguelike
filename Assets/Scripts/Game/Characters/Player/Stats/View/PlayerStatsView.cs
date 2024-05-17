using Core.Services;
using Game.Characters.Player.Stats.Model;
using Game.Characters.Player.Stats.Presenter;
using TMPro;
using UnityEngine;
using Zenject;

namespace Game.Characters.Player.Stats.View
{
    public class PlayerStatsView: MonoBehaviour
    {
        [SerializeField] private TMP_Text _healthStatText;

        [SerializeField] private TMP_Text _damageStatText;
        
        private PlayerStatsPresenter _playerStatsPresenter;
        
        private void Start()
        {
            _playerStatsPresenter.UpdateStatsData();
        }

        public void SetStats(float currentHealth, float maxHealth, float damage)
        {
            _healthStatText.text = $"Health: {currentHealth} / {maxHealth}";
            _damageStatText.text = $"Damage: {damage}";
        }

        public void Init(PlayerStatsPresenter playerStatsPresenter)
        {
            _playerStatsPresenter = playerStatsPresenter;
        }
    }
}