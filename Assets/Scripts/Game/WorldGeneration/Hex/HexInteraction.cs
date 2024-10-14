using Game.Buildings;
using Game.Buildings.BuildingsType;
using Game.UI.UIGameplayScene.BuildingsActionPanel;
using Game.UI.UIGameplayScene.SelectionHandling;
using Game.Units;
using UnityEngine;
using Zenject;

namespace Game.WorldGeneration.Hex
{
    public class HexInteraction: IInitializable, ILateDisposable
    {
         private HexMouseDetector _hexMouseDetector;

         private UISelectionHandler _uiSelectionHandler;

         private Building _currentSelectedBuilding;

         private Unit _currentSelectedUnit;

         private UIBuildingsActionPanel _buildingsActionPanel;

         private DiContainer _diContainer;

         [Inject]
         private void Constructor(HexMouseDetector hexMouseDetector, UISelectionHandler uiSelectionHandler,
             DiContainer diContainer, UIBuildingsActionPanel uiBuildingsActionPanel)
         {
             _hexMouseDetector = hexMouseDetector;
             _uiSelectionHandler = uiSelectionHandler;
             _buildingsActionPanel = uiBuildingsActionPanel;
             _diContainer = diContainer;
         }
             
         public void Initialize()
         {
             _hexMouseDetector.OnHexagonHovered.AddListener(HandleHexHovered);
             _hexMouseDetector.OnHexagonUnhovered.AddListener(HandleHexUnhovered);
             _hexMouseDetector.OnHexagonClicked.AddListener(HandleHexClicked);
             _uiSelectionHandler.OnSelectedBuilding += SelectBuilding;
             _uiSelectionHandler.OnSelectedUnit += SelectUnit;
         }
     
         public void LateDispose()
         {
             _uiSelectionHandler.OnSelectedBuilding -= SelectBuilding;
             _uiSelectionHandler.OnSelectedUnit -= SelectUnit;
         }

         private void SelectBuilding(Building building)
         {
             _currentSelectedBuilding = building;
             _currentSelectedUnit = null;
         }
         
         private void SelectUnit(Unit unit)
         {
             _currentSelectedUnit = unit;
             _currentSelectedBuilding = null;
         }
         
         private void HandleHexHovered(HexModel hexModel)
         {
             
         }
         
         private void HandleHexUnhovered(HexModel hexModel)
         {
             
         }
     
         private void HandleHexClicked(HexModel hexModel)
         {
             Debug.Log($"Hit hex on {hexModel.Q}, {hexModel.R}, {hexModel.S}");
             
             if (!hexModel.IsHexEmpty() && hexModel.CurrentBuilding != null)
             {
                 Building clickedBuilding = hexModel.CurrentBuilding;
                 if (clickedBuilding != null)
                 {
                     SelectBuilding(clickedBuilding);
                     UpdateBuildingActionPanel();
                     return;
                 }
             }
             
             if (!hexModel.IsHexEmpty()) return;
             
             if (_currentSelectedBuilding != null)
             {
                 var newBuilding = _diContainer.InstantiatePrefabForComponent<Building>(_currentSelectedBuilding,
                     new Vector3(hexModel.HexPosition.x, hexModel.HexPosition.y + 2.5f, hexModel.HexPosition.z),
                     Quaternion.identity, hexModel.transform);

                 hexModel.SetBuilding(ref newBuilding);
                 _currentSelectedBuilding = null;
             }

             if (_currentSelectedUnit != null)
             {
                 var newUnit = _diContainer.InstantiatePrefabForComponent<Unit>(_currentSelectedUnit,
                     new Vector3(hexModel.HexPosition.x, hexModel.HexPosition.y + 5f, hexModel.HexPosition.z),
                     Quaternion.identity, hexModel.transform);

                 hexModel.SetUnit(ref newUnit);
                 _currentSelectedUnit = null;
             }
         }
         
         private void UpdateBuildingActionPanel()
         {
             _buildingsActionPanel.ShowActions(_currentSelectedBuilding.GetAvailableActions());
             // _buildingsActionPanel.Hide();
         }
    }
}