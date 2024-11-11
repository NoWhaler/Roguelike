using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Game.Buildings.BuildingsType;
using Game.Buildings.Controller;
using Game.Buildings.Interfaces;
using Game.Pathfinding;
using Game.UI.UIGameplayScene.BuildingsActionPanel;
using Game.UI.UIGameplayScene.SelectedEntityInformation;
using Game.UI.UIGameplayScene.SelectionHandling;
using Game.Units;
using Game.Units.Controller;
using UnityEngine;
using Zenject;

namespace Game.WorldGeneration.Hex
{
    public class HexInteraction: IInitializable, ILateDisposable
    {
        private HashSet<HexModel> _highlightedHexes = new HashSet<HexModel>();
        
        private List<HexModel> _currentPath = new List<HexModel>();
        
        private HexMouseDetector _hexMouseDetector;

        private UISelectionHandler _uiSelectionHandler;

        private Building _currentSelectedBuilding;
        
        private Building _currentUISelectedBuilding;
        
        private Unit _currentSelectedUnit;

        private Unit _currentUISelectedUnit;

        private UIBuildingsActionPanel _buildingsActionPanel;
        
        private BuildingsController _buildingsController;
        
        private UnitsController _unitsController;

        private HexGridController _hexGridController;

        private PathfindingController _pathfindingController;
        
        private UISelectedEntityView _uiSelectedEntityView;

        private DiContainer _diContainer;
        
        private bool _isUnitMoving;

        [Inject]
        private void Constructor(HexMouseDetector hexMouseDetector, UISelectionHandler uiSelectionHandler,
            DiContainer diContainer, UIBuildingsActionPanel uiBuildingsActionPanel,
            BuildingsController buildingsController, HexGridController hexGridController,
            UnitsController unitsController, UISelectedEntityView uiSelectedEntityView,
            PathfindingController pathfindingController)
        {
            _hexMouseDetector = hexMouseDetector;
            _uiSelectionHandler = uiSelectionHandler;
            _buildingsActionPanel = uiBuildingsActionPanel;
            _buildingsController = buildingsController;
            _hexGridController = hexGridController;
            _unitsController = unitsController;
            _pathfindingController = pathfindingController;
            _uiSelectedEntityView = uiSelectedEntityView;
            _diContainer = diContainer;
        }
            
        public void Initialize()
        {
            _hexMouseDetector.OnHexagonHovered.AddListener(HandleHexHovered);
            _hexMouseDetector.OnHexagonUnhovered.AddListener(HandleHexUnhovered);
            _hexMouseDetector.OnHexagonClicked.AddListener(HandleHexClicked);
            
            _uiSelectionHandler.OnUISelectedBuilding += SelectUIBuilding;
            _uiSelectionHandler.OnUISelectedUnit += SelectUIUnit;
        }

        public void LateDispose()
        {
            _uiSelectionHandler.OnUISelectedBuilding -= SelectUIBuilding;
            _uiSelectionHandler.OnUISelectedUnit -= SelectUIUnit;
        }

        private void SelectUIBuilding(Building building)
        {
            ClearHighlights();
            ClearPathHighlight();
            
            _currentUISelectedBuilding = building;
            _currentUISelectedUnit = null;
            _currentSelectedUnit = null;
            _currentSelectedBuilding = null;
        }

        private void SelectUIUnit(Unit unit)
        {
            ClearHighlights();
            ClearPathHighlight();
            
            _currentUISelectedUnit = unit;
            _currentUISelectedBuilding = null;
            _currentSelectedUnit = null;
        }
        
        private void SelectBuilding(Building building)
        {
            ClearHighlights();
            ClearPathHighlight();
            
            _currentSelectedBuilding = building;
            _currentUISelectedBuilding = null;
            _currentUISelectedUnit = null;
            _currentSelectedUnit = null;
        }

        private void SelectUnit(Unit unit)
        {
            ClearHighlights();
            ClearPathHighlight();
            
            _currentSelectedUnit = unit;
            _currentUISelectedBuilding = null;
            _currentUISelectedUnit = null;
            _currentSelectedBuilding = null;
            
            var reachableHexes = _unitsController.GetReachableHexes(unit);
            HighlightHexes(reachableHexes);
        }

        private void HandleHexHovered(HexModel hexModel)
        {
            if (!hexModel.IsVisible) return;
            
            if (_currentSelectedUnit != null && _currentSelectedUnit.CurrentHex != null && !_isUnitMoving)
            {
                ClearPathHighlight();
                _currentPath = _pathfindingController.FindPath(_currentSelectedUnit.CurrentHex, hexModel);
                HighlightPath(_currentPath);
            }
        }

        private void HandleHexUnhovered(HexModel hexModel)
        {
            ClearPathHighlight();
        }

        private void HandleHexClicked(HexModel hexModel)
        {
            if (!hexModel.IsVisible) return;
            
            if (!hexModel.IsHexEmpty())
            {
                if (hexModel.CurrentBuilding != null)
                {
                    if (_currentSelectedUnit != null)
                    {
                        if (_highlightedHexes.Contains(hexModel))
                        {
                            MoveSelectedUnit(hexModel).Forget();
                            ClearHighlights();
                            ClearPathHighlight();
                            return;
                        }
                    }
                    
                    SelectBuilding(hexModel.CurrentBuilding);
                    _uiSelectionHandler.SelectBuilding(hexModel.CurrentBuilding);
                    UpdateBuildingActionPanel();
                }
                else if (hexModel.CurrentUnit != null)
                {
                    SelectUnit(hexModel.CurrentUnit);
                    _buildingsActionPanel.Hide();
                    _uiSelectionHandler.SelectUnit(hexModel.CurrentUnit);
                }
            }
            else
            {
                if (_currentUISelectedBuilding != null)
                {
                    PlaceBuilding(hexModel);
                }

                else if (_currentUISelectedUnit != null || _currentSelectedUnit != null)
                {
                    if (_currentSelectedUnit == null && _currentSelectedBuilding != null)
                    {
                        if (IsNeighboringHex(_currentSelectedBuilding.CurrentHex, hexModel))
                        {
                            DeployUnit(hexModel);
                        }
                    }
                    else
                    {
                        MoveSelectedUnit(hexModel).Forget();
                        ClearHighlights();
                        ClearPathHighlight();
                    }
                }

                else
                {
                    ClearAllSelections();
                }
            }
        }

        private void ClearAllSelections()
        {
            _currentSelectedBuilding = null;
            _currentUISelectedBuilding = null;
            _currentSelectedUnit = null;
            _currentUISelectedUnit = null;
            
            ClearHighlights();
            ClearPathHighlight();
            
            _uiSelectionHandler.ClearSelection();
        }
        
        private void DeployUnit(HexModel hexModel)
        {
            var newUnit = _diContainer.InstantiatePrefabForComponent<Unit>(_currentUISelectedUnit,
                new Vector3(hexModel.HexPosition.x, hexModel.HexPosition.y + 5f, hexModel.HexPosition.z),
                Quaternion.identity, hexModel.transform);

            hexModel.SetUnit(ref newUnit);
             
            _unitsController.RegisterUnit(ref newUnit);
            newUnit.CurrentHex = hexModel;
            
            _currentSelectedBuilding.DecreaseUnitCount(newUnit.UnitType);
            _buildingsActionPanel.SetUnitCount(ref _currentSelectedBuilding);
             
            _currentSelectedUnit = null;
            _currentSelectedBuilding = null;
        }
 
        private void PlaceBuilding(HexModel hexModel)
        {
            var newBuilding = _diContainer.InstantiatePrefabForComponent<Building>(_currentUISelectedBuilding,
                new Vector3(hexModel.HexPosition.x, hexModel.HexPosition.y + 2.5f, hexModel.HexPosition.z),
                Quaternion.identity, hexModel.transform);
 
            hexModel.SetBuilding(ref newBuilding);
             
            switch (newBuilding)
            {
                case IProduceResource produceResourceBuilding:
                    _buildingsController.RegisterProductionBuilding(produceResourceBuilding);
                    break;
                case IHireUnit unitHiringBuilding:
                    _buildingsController.RegisterHiringBuilding(unitHiringBuilding);
                    break;
            }

            List<HexModel> nearestCells = _hexGridController.GetHexesInRadius(hexModel, newBuilding.RevealFogOfWarRange);
            RevealFogOfWar(nearestCells);
            
            _currentSelectedBuilding = null;
        }
         
        private void UpdateBuildingActionPanel()
        {
            _buildingsActionPanel.ShowActions(_currentSelectedBuilding.GetAvailableActions());
            _buildingsActionPanel.SetUnitCount(ref _currentSelectedBuilding);
        }
        
        private void RevealFogOfWar(List<HexModel> cells)
        {
            foreach (var cell in cells)
            {
                cell.SetFog(false);
            }
        }
         
        private async UniTaskVoid MoveSelectedUnit(HexModel targetHex)
        {
            if (_highlightedHexes.Contains(targetHex))
            {
                List<HexModel> path = _pathfindingController.FindPath(_currentSelectedUnit.CurrentHex, targetHex);
                if (path != null && path.Count > 0)
                {
                    _isUnitMoving = true;
                    ClearPathHighlight();
                    await MoveUnitAlongPath(_currentSelectedUnit, path);
                    _isUnitMoving = false;
                }
                else
                {
                    Debug.Log("No valid path found.");
                }
            }
            else
            {
                Debug.Log("Selected hex is not within the unit's movement range.");
                _currentSelectedUnit = null;
            }
        }
         
        private void HighlightHexes(HashSet<HexModel> hexes)
        {
            foreach (var hex in hexes)
            {
                hex.SetUnitRangeHighlight(true);
                _highlightedHexes.Add(hex);
            }
        }
        
        private void ClearHighlights()
        {
            foreach (var hex in _highlightedHexes)
            {
                hex.SetUnitRangeHighlight(false);
            }
            _highlightedHexes.Clear();
        }

        private void ClearPathHighlight()
        {
            if (_currentPath != null)
            {
                foreach (var hex in _currentPath)
                {
                    hex.SetUnitPathHighlight(false);
                }

                _currentPath.Clear();
            }
        }

        private void HighlightPath(List<HexModel> path)
        {
            if (_currentPath != null)
            {
                foreach (var hex in path)
                {
                    hex.SetUnitPathHighlight(true);
                }
            }
        }

        private bool IsNeighboringHex(HexModel sourceHex, HexModel targetHex)
        {
            List<HexModel> neighbors = _hexGridController.GetNeighbors(sourceHex);
            return neighbors.Contains(targetHex);
        }
        
        private async UniTask MoveUnitAlongPath(Unit unit, List<HexModel> path)
        {
            foreach (var hex in path.Skip(1))
            {
                await MoveUnitToHex(unit, hex);
                _uiSelectedEntityView.UpdateSelectedEntityInfo();
                if (unit.CurrentMovementPoints == 0)
                    break;
            }
            ClearHighlights();
            ClearPathHighlight();
            _currentSelectedUnit = null;
            _uiSelectionHandler.ClearSelection();
        }

        private async UniTask MoveUnitToHex(Unit unit, HexModel targetHex)
        {
            Vector3 startPosition = unit.transform.position;
            Vector3 endPosition = new Vector3(targetHex.HexPosition.x, targetHex.HexPosition.y + 5f, targetHex.HexPosition.z);
            float moveDuration = 0.3f;

            float elapsedTime = 0f;
            while (elapsedTime < moveDuration)
            {
                unit.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / moveDuration);
                elapsedTime += Time.deltaTime;
                await UniTask.Yield();
            }

            unit.Move(targetHex, 1);

            var targetHexCurrentBuilding = targetHex.CurrentBuilding;
            
            if (targetHexCurrentBuilding != null)
            {
                targetHex.CurrentUnit = null;
                
                targetHexCurrentBuilding.IncreaseUnitCount(unit.UnitType);
                _buildingsActionPanel.SetUnitCount(ref targetHexCurrentBuilding);
                unit.DisableUnit();
                
                ClearAllSelections();
            }
        }
    }
}