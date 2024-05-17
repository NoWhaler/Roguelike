using Core.Services;
using Game.Characters.Player.Stats.Model;

namespace Game.Characters.Player.Stats.Presenter
{
    public class PlayerStatsPresenter
    {
        private PlayerStatsModel _playerStatsModel;

        private DataService _dataService;
        
        public PlayerStatsPresenter(PlayerStatsModel playerStatsModel, DataService dataService)
        {
            _playerStatsModel = playerStatsModel;
            _dataService = dataService;
        }

        public void UpdateStatsData()
        {
            _playerStatsModel.UpdateStatsView(_dataService);
        }
    }
}