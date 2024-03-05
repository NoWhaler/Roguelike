using Game.Characters.Player.Stats.Models;
using TMPro;
using UnityEngine;
using Zenject;

namespace Game.Characters.Player.Stats.Views
{
    public class PlayerStatsView: MonoBehaviour
    {
        [SerializeField] private TMP_Text _healthStatText;

        [SerializeField] private TMP_Text _damageStatText;
        
        private PlayerStatsModel _playerStatsModel;
        
        [Inject]
        private void Constructor(PlayerStatsModel playerStatsModel)
        {
            _playerStatsModel = playerStatsModel;
        }

        private void Start()
        {
            SetStats();
        }

        private void SetStats()
        {
            _healthStatText.text = $"Health: {_playerStatsModel.Health}";
            _damageStatText.text = $"Damage: {_playerStatsModel.Damage}";
        }
    }
}