using Core.Initialization.Interfaces;
using Core.Services;
using Game.Characters.Player.Stats.Model;
using Game.Characters.Player.Stats.Presenter;
using Game.Characters.Player.Stats.View;
using UnityEngine;
using Zenject;

namespace Core.Initialization.StatsInit
{
    public class StatsInitializer: MonoBehaviour, IInitializeData
    {
        [SerializeField] private PlayerStatsView _playerStatsView;

        private DataService _dataService;
        
        [Inject]
        private void Constructor(DataService dataService)
        {
            _dataService = dataService;
        }
        
        private void Start()
        {
            Init();
        }

        public void Init()
        {
            PlayerStatsModel playerStatsModel = new PlayerStatsModel(_playerStatsView);
            PlayerStatsPresenter playerStatsPresenter = new PlayerStatsPresenter(playerStatsModel, _dataService);
            
            _playerStatsView.Init(playerStatsPresenter);
        }
    }
}