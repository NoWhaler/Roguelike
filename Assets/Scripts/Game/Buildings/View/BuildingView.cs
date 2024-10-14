using Game.Buildings.BuildingsType;
using Game.UI.UIGameplayScene.SelectionHandling;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Buildings.View
{
    public class BuildingView : MonoBehaviour
    {
        [field: SerializeField] private Image _iconImage;

        [field: SerializeField] private Button _imageButton;

        [field: SerializeField] private Building _selectedBuilding;

        private UISelectionHandler _uiSelectionHandler;

        [Inject]
        private void Constructor(UISelectionHandler uiSelectionHandler)
        {
            _uiSelectionHandler = uiSelectionHandler;
        }

        private void OnEnable()
        {
            _imageButton.onClick.AddListener(OnButtonClick);
        }

        private void OnDisable()
        {
            _imageButton.onClick.RemoveListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            _uiSelectionHandler.SelectBuilding(_selectedBuilding);
            
            Debug.Log($"Selected building - {_selectedBuilding}");
        }
    }
}