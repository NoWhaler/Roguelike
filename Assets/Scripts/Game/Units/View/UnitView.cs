using Game.UI.UIGameplayScene.SelectionHandling;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

namespace Game.Units.View
{
    public class UnitView: MonoBehaviour
    {
        [field: SerializeField] private Image _iconImage;

        [field: SerializeField] private Button _imageButton;

        [field: SerializeField] private Unit _selectedUnit;

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
            _uiSelectionHandler.SelectUnit(_selectedUnit);
            
            Debug.Log($"Selected Unit - {_selectedUnit}");
        }
    }
}