using Game.Buildings;
using Game.Buildings.BuildingsType;
using Game.Buildings.Controller;
using Game.Buildings.Interfaces;
using Game.Researches.Controller;
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

         private BuildingsController _buildingsController;

         private ResearchController _researchController;

         private DiContainer _diContainer;

         [Inject]
         private void Constructor(HexMouseDetector hexMouseDetector, UISelectionHandler uiSelectionHandler,
             DiContainer diContainer, UIBuildingsActionPanel uiBuildingsActionPanel, ResearchController researchController,
             BuildingsController buildingsController)
         {
             _hexMouseDetector = hexMouseDetector;
             _uiSelectionHandler = uiSelectionHandler;
             _buildingsActionPanel = uiBuildingsActionPanel;
             _researchController = researchController;
             _buildingsController = buildingsController;
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
         }
         
         private void HandleHexHovered(HexModel hexModel)
         {
             
         }
         
         private void HandleHexUnhovered(HexModel hexModel)
         {
             
         }
     
         private void HandleHexClicked(HexModel hexModel)
         {
             // Debug.Log($"Hit hex on {hexModel.Q}, {hexModel.R}, {hexModel.S}");
             
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
             

             if (_currentSelectedUnit != null && _currentSelectedBuilding != null)
             {
                 DeployUnit(hexModel);
             }
             
             else if (_currentSelectedBuilding != null)
             {
                 PlaceBuilding(hexModel);
             }
         }
         
         private void DeployUnit(HexModel hexModel)
         {
             var newUnit = _diContainer.InstantiatePrefabForComponent<Unit>(_currentSelectedUnit,
                 new Vector3(hexModel.HexPosition.x, hexModel.HexPosition.y + 5f, hexModel.HexPosition.z),
                 Quaternion.identity, hexModel.transform);

             hexModel.SetUnit(ref newUnit);
             _currentSelectedBuilding.DecreaseUnitCount(newUnit.UnitType);
             _buildingsActionPanel.SetUnitCount(ref _currentSelectedBuilding);
             _currentSelectedUnit = null;
             _currentSelectedBuilding = null;
         }
 
         private void PlaceBuilding(HexModel hexModel)
         {
             var newBuilding = _diContainer.InstantiatePrefabForComponent<Building>(_currentSelectedBuilding,
                 new Vector3(hexModel.HexPosition.x, hexModel.HexPosition.y + 2.5f, hexModel.HexPosition.z),
                 Quaternion.identity, hexModel.transform);
 
             hexModel.SetBuilding(ref newBuilding);
             _buildingsController.RegisterBuilding(newBuilding as IProduceResource);
             _currentSelectedBuilding = null;
         }
         
         private void UpdateBuildingActionPanel()
         {
             _buildingsActionPanel.ShowActions(_currentSelectedBuilding.GetAvailableActions());
             _buildingsActionPanel.SetUnitCount(ref _currentSelectedBuilding);
         }
    }
}