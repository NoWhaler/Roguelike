using System;
using System.Collections.Generic;
using Game.Buildings.Interfaces;
using Game.UI.UIGameplayScene.BuildingsActionPanel;
using Game.UI.UIGameplayScene.UIResourcesPanel;
using Zenject;

namespace Core.TurnBasedSystem
{
    public class GameTurnController: IInitializable
    {
        public event Action OnTurnEnded;
        public event Action<int> OnTurnChanged;

        private int _currentTurn = 1;
        
        private List<IBuildingAction> _activeActions = new List<IBuildingAction>();
        
        private UIBuildingsActionPanel _buildingActionPanel;

        private ResourcesPanel _resourcesPanel;
        
        [Inject]
        private void Constructor(UIBuildingsActionPanel buildingsActionPanel, ResourcesPanel resourcesPanel)
        {
            _buildingActionPanel = buildingsActionPanel;
            _resourcesPanel = resourcesPanel;
        }
        
        public void Initialize()
        {
            OnTurnChanged?.Invoke(_currentTurn);
        }
        
        public void EndTurn()
        {
            _currentTurn++;
            OnTurnChanged?.Invoke(_currentTurn);

            for (int i = _activeActions.Count - 1; i >= 0; i--)
            {
                var action = _activeActions[i];
                action.Duration--;
                if (action.Duration <= 0)
                {
                    action.Complete();
                    _activeActions.RemoveAt(i);
                }
            }

            _buildingActionPanel.UpdateActionViews();
            _buildingActionPanel.Hide();
            OnTurnEnded?.Invoke();
            
            _resourcesPanel.UpdateResourcesAmount();
        }
        
        public void AddActiveAction(IBuildingAction action)
        {
            _activeActions.Add(action);
        }

        public int GetCurrentTurn()
        {
            return _currentTurn;
        }
    }
}