using System;
using Core.TurnBasedSystem;
using Cysharp.Threading.Tasks;
using Game.Units.Controller;
using UnityEngine;
using Zenject;

namespace Game.AI
{
    public class EnemyAIController: IInitializable, IDisposable
    {
        private GameTurnController _gameTurnController;
        private UnitsController _unitsController;
        
        private float _turnDelay = 1f;

        [Inject]
        private void Constructor(GameTurnController gameTurnController, UnitsController unitsController)
        {
            _gameTurnController = gameTurnController;
            _unitsController = unitsController;
        }
        
        public void Initialize()
        {
            _gameTurnController.OnEnemyTurnStarted += StartTurn;
        }

        public void Dispose()
        {
            _gameTurnController.OnEnemyTurnStarted -= StartTurn;
        }

        private async void StartTurn()
        {
            await UniTask.Delay((int)(_turnDelay * 1000));
            var enemyUnits = _unitsController.GetEnemyUnits();
            
            // TODO: Implement AI logic
            await ProcessEnemyActions();
            
        }

        private async UniTask ProcessEnemyActions()
        {
            var enemyUnits = _unitsController.GetEnemyUnits();
            
            foreach (var unit in enemyUnits)
            {
                // TODO: Implement unit AI logic here
                await UniTask.Delay(500);
            }
            
            await UniTask.DelayFrame(1);
            
            Debug.Log("Enemy turn ended");
            _gameTurnController.EndTurn();
        }
    }
}