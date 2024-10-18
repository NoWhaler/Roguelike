using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Game.Buildings.BuildingsType;
using Game.Buildings.Controller;
using Game.Buildings.Interfaces;
using Game.Pathfinding;
using Game.UI.UIGameplayScene.BuildingsActionPanel;
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

        private Building _buildingToPlace;

        private Unit _currentSelectedUnit;

        private UIBuildingsActionPanel _buildingsActionPanel;
        
        private BuildingsController _buildingsController;
        
        private UnitsController _unitsController;

        private HexGridController _hexGridController;

        private PathfindingController _pathfindingController;
        
        private DiContainer _diContainer;
        
        private bool _isUnitMoving;

        [Inject]
        private void Constructor(HexMouseDetector hexMouseDetector, UISelectionHandler uiSelectionHandler,
            DiContainer diContainer, UIBuildingsActionPanel uiBuildingsActionPanel,
            BuildingsController buildingsController, HexGridController hexGridController,
            UnitsController unitsController,
            PathfindingController pathfindingController)
        {
            _hexMouseDetector = hexMouseDetector;
            _uiSelectionHandler = uiSelectionHandler;
            _buildingsActionPanel = uiBuildingsActionPanel;
            _buildingsController = buildingsController;
            _hexGridController = hexGridController;
            _unitsController = unitsController;
            _pathfindingController = pathfindingController;
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
            ClearHighlights();
            ClearPathHighlight();
            
            _currentSelectedBuilding = building;
            _currentSelectedUnit = null;
        }

        private void SelectUnit(Unit unit)
        {
            ClearHighlights();
            
            _currentSelectedUnit = unit;
            
            var reachableHexes = _unitsController.GetReachableHexes(unit);
            HighlightHexes(reachableHexes);
        }

        private void HandleHexHovered(HexModel hexModel)
        {
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

        private void  HandleHexClicked(HexModel hexModel)
        {
            if (!hexModel.IsHexEmpty() && hexModel.CurrentBuilding != null)
            {
                ClearHighlights();
                Building clickedBuilding = hexModel.CurrentBuilding;
                if (clickedBuilding != null)
                {
                    SelectBuilding(clickedBuilding);
                    UpdateBuildingActionPanel();
                    return;
                }
            }
             
            if (!hexModel.IsHexEmpty() && hexModel.CurrentUnit != null)
            {
                ClearHighlights();
                SelectUnit(hexModel.CurrentUnit);
                return;
            }

            if (_currentSelectedUnit != null && _currentSelectedBuilding == null)
            {
                MoveSelectedUnit(hexModel).Forget();
                ClearHighlights();
                ClearPathHighlight();
                return;
            }

            if (_currentSelectedUnit != null && _currentSelectedBuilding != null)
            {
                if (IsNeighboringHex(_currentSelectedBuilding.CurrentHex, hexModel))
                {
                    ClearHighlights();
                    DeployUnit(hexModel);
                }
            }

            if (_currentSelectedBuilding != null)
            {
                if (_currentSelectedBuilding.CurrentHex == null)
                {
                    ClearHighlights();
                    PlaceBuilding(hexModel);
                }
                else
                {
                    _currentSelectedBuilding = null;
                    ClearHighlights();
                }
            }
            else
            {
                ClearHighlights();
            }
        }
        
         
        private void DeployUnit(HexModel hexModel)
        {
            var newUnit = _diContainer.InstantiatePrefabForComponent<Unit>(_currentSelectedUnit,
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
            var newBuilding = _diContainer.InstantiatePrefabForComponent<Building>(_currentSelectedBuilding,
                new Vector3(hexModel.HexPosition.x, hexModel.HexPosition.y + 2.5f, hexModel.HexPosition.z),
                Quaternion.identity, hexModel.transform);
 
            hexModel.SetBuilding(ref newBuilding);
             
            if (newBuilding is IProduceResource produceResourceBuilding)
            {
                _buildingsController.RegisterProductionBuilding(produceResourceBuilding);
            }
            _currentSelectedBuilding = null;
        }
         
        private void UpdateBuildingActionPanel()
        {
            _buildingsActionPanel.ShowActions(_currentSelectedBuilding.GetAvailableActions());
            _buildingsActionPanel.SetUnitCount(ref _currentSelectedBuilding);
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
                if (unit.CurrentMovementPoints == 0)
                    break;
            }
            ClearHighlights();
            ClearPathHighlight();
            _currentSelectedUnit = null;
        }

        private async UniTask MoveUnitToHex(Unit unit, HexModel targetHex)
        {
            Vector3 startPosition = unit.transform.position;
            Vector3 endPosition = new Vector3(targetHex.HexPosition.x, targetHex.HexPosition.y + 5f, targetHex.HexPosition.z);
            float moveDuration = 0.5f; // Adjust this value to change movement speed

            float elapsedTime = 0f;
            while (elapsedTime < moveDuration)
            {
                unit.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / moveDuration);
                elapsedTime += Time.deltaTime;
                await UniTask.Yield();
            }

            unit.Move(targetHex, 1);
        }
    }
}