using Game.Buildings.BuildingsType;
using Game.UI.UIGameplayScene.SelectionHandling;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Units.View
{
    public class UnitView: MonoBehaviour
    {
        [field: SerializeField] private Image _iconImage;

        [field: SerializeField] private Button _imageButton;

        [field: SerializeField] private Unit _selectedUnit;

        [field: SerializeField] private TMP_Text _unitCountText;

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
        
        public void UpdateUnitCount(ref Building building)
        {
            int count = building.GetUnitCount(_selectedUnit.UnitType);
            _unitCountText.text = count.ToString();
            _imageButton.interactable = count > 0;
        }
    }
}