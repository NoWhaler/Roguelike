using Game.Buildings.BuildingsType;
using Game.Hex;
using Game.UI.UIGameplayScene.SelectionHandling;
using Game.Units;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.UI.UIGameplayScene.SelectedEntityInformation
{
    public class UISelectedEntityView: MonoBehaviour
    {
        [SerializeField] private Image portraitImage;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text hitPointsText;
        [SerializeField] private TMP_Text movementPointsText;
        [SerializeField] private Image hitPointsBar;
        [SerializeField] private Image movementPointsBar;
        // [SerializeField] private GameObject unitInfoPanel;
        // [SerializeField] private GameObject buildingInfoPanel;

        private UISelectionHandler _uiSelectionHandler;
        private Unit _currentUnit;
        private Building _currentBuilding;

        [Inject]
        private void Construct(UISelectionHandler uiSelectionHandler, HexInteraction hexInteraction)
        {
            _uiSelectionHandler = uiSelectionHandler;
        }

        private void OnEnable()
        {
            _uiSelectionHandler.OnSelectedBuilding += HandleBuildingSelected;
            _uiSelectionHandler.OnSelectedUnit += HandleUnitSelected;
            _uiSelectionHandler.OnSelectionCleared += HandleSelectionCleared;
        }

        private void OnDisable()
        {
            _uiSelectionHandler.OnSelectedBuilding -= HandleBuildingSelected;
            _uiSelectionHandler.OnSelectedUnit -= HandleUnitSelected;
            _uiSelectionHandler.OnSelectionCleared -= HandleSelectionCleared;
        }

        private void HandleBuildingSelected(Building building)
        {
            _currentBuilding = building;
            _currentUnit = null;
            DisplayBuildingInfo(building);
        }

        private void HandleUnitSelected(Unit unit)
        {
            _currentUnit = unit;
            _currentBuilding = null;
            DisplayUnitInfo(unit);
        }

        private void HandleSelectionCleared()
        {
            _currentUnit = null;
            _currentBuilding = null;
            HideAllPanels();
        }

        private void DisplayUnitInfo(Unit unit)
        {
            UpdateUnitStats(unit);
        }

        private void DisplayBuildingInfo(Building building)
        {
            UpdateBuildingStats(building);
        }

        private void UpdateUnitStats(Unit unit)
        {
            hitPointsText.text = $"HP: {unit.CurrentHealth}/{unit.MaxHealth}";
            movementPointsText.text = $"MP: {unit.CurrentMovementPoints}/{unit.MaxMovementPoints}";

            hitPointsBar.fillAmount = (float)unit.CurrentHealth / unit.MaxHealth;
            movementPointsBar.fillAmount = (float)unit.CurrentMovementPoints / unit.MaxMovementPoints;
            
            movementPointsText.gameObject.SetActive(true);
            movementPointsBar.gameObject.SetActive(true);
        }

        private void UpdateBuildingStats(Building building)
        {
            hitPointsText.text = $"HP: {building.CurrentHealth}/{building.MaxHealth}";
            hitPointsBar.fillAmount = (float)building.CurrentHealth / building.MaxHealth;

            movementPointsText.gameObject.SetActive(false);
            movementPointsBar.gameObject.SetActive(false);
        }

        private void HideAllPanels()
        {
            // unitInfoPanel.SetActive(false);
            // buildingInfoPanel.SetActive(false);
        }

        public void UpdateSelectedEntityInfo()
        {
            if (_currentUnit != null)
            {
                UpdateUnitStats(_currentUnit);
            }
            else if (_currentBuilding != null)
            {
                UpdateBuildingStats(_currentBuilding);
            }
        }
    }
}