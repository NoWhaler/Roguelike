using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Buildings
{
    public class BuildingActionView : MonoBehaviour
    {
        [SerializeField] private Image _actionIcon;

        [SerializeField] private Button _actionButton;

        [SerializeField] private TMP_Text _descriptionText;

        [SerializeField] private TMP_Text _actionName;
        
        private IBuildingAction _buildingAction;
        
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

        private void ExecuteAction()
        {
            if (_buildingAction.CanExecute())
            {
                _buildingAction.Execute();
            }
            else
            {
                Debug.Log($"Cannot execute action: {_buildingAction.Name}");
            }
        }
    }
}