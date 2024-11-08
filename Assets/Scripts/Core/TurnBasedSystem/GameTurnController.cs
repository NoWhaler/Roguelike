using System;
using System.Collections.Generic;
using Game.Buildings.Interfaces;
using Game.Technology.Controller;
using Game.Technology.Model;
using Game.UI.UIGameplayScene.BuildingsActionPanel;
using Game.UI.UIGameplayScene.TechnologyPanel;
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

        private List<TechnologyModel> _activeTechnologies = new List<TechnologyModel>();

        private TechnologiesController _technologiesController;
        
        private UIBuildingsActionPanel _buildingActionPanel;

        private UITechnologyPanel _technologyPanel;

        private ResourcesPanel _resourcesPanel;
        
        [Inject]
        private void Constructor(UIBuildingsActionPanel buildingsActionPanel,
            ResourcesPanel resourcesPanel, UITechnologyPanel technologyPanel,
            TechnologiesController technologiesController)
        {
            _buildingActionPanel = buildingsActionPanel;
            _technologyPanel = technologyPanel;
            _technologiesController = technologiesController;
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

            for (int i = _activeTechnologies.Count - 1; i >= 0; i--)
            {
                var activeTechnology = _activeTechnologies[i];
                activeTechnology.TurnsLeft--;
                if (activeTechnology.TurnsLeft <= 0)
                {
                    _activeTechnologies.RemoveAt(i);
                    _technologiesController.CompleteTechnology(activeTechnology);
                }
            }

            _buildingActionPanel.UpdateActionViews();
            _technologyPanel.UpdateTechViews();
            _buildingActionPanel.Hide();
            OnTurnEnded?.Invoke();
            
            _resourcesPanel.UpdateResourcesAmount();
        }
        
        public void AddActiveBuildingAction(IBuildingAction action)
        {
            _activeActions.Add(action);
        }

        public void AddActiveTechnology(TechnologyModel technologyModel)
        {
            _activeTechnologies.Add(technologyModel);
        }

        public int GetCurrentTurn()
        {
            return _currentTurn;
        }
    }
}