using Core.Services;
using Game.Characters.Player.Stats.View;

namespace Game.Characters.Player.Stats.Model
{
    public class PlayerStatsModel
    {
        private PlayerStatsView _playerStatsView;
        
        public float CurrentHealth { get; set; }
        
        public float MaxHealth { get; set; }
        
        public float Damage { get; set; }

        public PlayerStatsModel(PlayerStatsView playerStatsView)
        {
            _playerStatsView = playerStatsView;
        }

        public void UpdateStatsView(DataService dataService)
        {
            CurrentHealth = dataService.PlayerDataConfig.PlayerData.PlayerCurrentHealth;
            MaxHealth = dataService.PlayerDataConfig.PlayerData.PlayerMaxHealth;
            Damage = dataService.PlayerDataConfig.PlayerData.PlayerDamage;
            
            _playerStatsView.SetStats(CurrentHealth, MaxHealth, Damage);
        }
    }
}