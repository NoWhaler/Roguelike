using Core.TurnBasedSystem;
using Game.Buildings.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Buildings.View
{
    public class BuildingActionView : MonoBehaviour
    {
        [SerializeField] private Image _actionIcon;

        [SerializeField] private Button _actionButton;

        [SerializeField] private TMP_Text _descriptionText;
        
       [SerializeField] private TMP_Text _durationText; 

        [SerializeField] private TMP_Text _actionName;
        
        private IBuildingAction _buildingAction;

        private GameTurnController _gameTurnController;

        [Inject]
        private void Constructor(GameTurnController gameTurnController)
        {
            _gameTurnController = gameTurnController;
        }
        
        private void OnDisable()
        {
            _actionButton.onClick.RemoveListener(ExecuteAction);
        }
        
        public void SetAction(IBuildingAction action)
        {
            _buildingAction = action;
            _actionName.text = action.Name;
            _descriptionText.text = action.Description;
            _actionButton.onClick.AddListener(ExecuteAction);
        }
        
        public void UpdateView()
        {
            _actionName.text = _buildingAction.Name;
            _descriptionText.text = _buildingAction.Description;
            _durationText.text = _buildingAction.IsActive ? $"{_buildingAction.Duration}" : "";
            _actionButton.interactable = _buildingAction.CanExecute() && !_buildingAction.IsActive;
        }

        private void ExecuteAction()
        {
            if (_buildingAction.CanExecute() && !_buildingAction.IsActive)
            {
                _buildingAction.Execute();
                _gameTurnController.AddActiveAction(_buildingAction);
                UpdateView();
            }
        }
    }
}