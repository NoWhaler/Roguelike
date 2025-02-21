using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Game.Buildings.BuildingsType;
using Game.Buildings.Controller;
using Game.Buildings.Interfaces;
using Game.Pathfinding;
using Game.ProductionResources.Controller;
using Game.Technology.Controller;
using Game.UI.UIGameplayScene.BuildingsActionPanel;
using Game.UI.UIGameplayScene.SelectedEntityInformation;
using Game.UI.UIGameplayScene.SelectionHandling;
using Game.Units;
using Game.Units.Controller;
using Game.Units.Enum;
using UnityEngine;
using Zenject;

namespace Game.Hex
{
    public class HexInteraction: IInitializable, ILateDisposable
    {
        private HashSet<HexModel> _highlightedHexes = new HashSet<HexModel>();
        
        private HashSet<HexModel> _attackableHexes = new HashSet<HexModel>();
        
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

        private ResourcesController _resourcesController;

        private TechnologiesController _technologiesController;
        
        private DiContainer _diContainer;
        
        private bool _isUnitMoving;

        [Inject]
        private void Constructor(HexMouseDetector hexMouseDetector, UISelectionHandler uiSelectionHandler,
            DiContainer diContainer, UIBuildingsActionPanel uiBuildingsActionPanel,
            BuildingsController buildingsController, HexGridController hexGridController,
            UnitsController unitsController, UISelectedEntityView uiSelectedEntityView,
            PathfindingController pathfindingController, ResourcesController resourcesController,
            TechnologiesController technologiesController)
        {
            _hexMouseDetector = hexMouseDetector;
            _uiSelectionHandler = uiSelectionHandler;
            _buildingsActionPanel = uiBuildingsActionPanel;
            _buildingsController = buildingsController;
            _hexGridController = hexGridController;
            _unitsController = unitsController;
            _pathfindingController = pathfindingController;
            _uiSelectedEntityView = uiSelectedEntityView;
            _resourcesController = resourcesController;
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
            
            var reachableHexes = _unitsController.GetAvailableHexes(unit);
            _attackableHexes = _unitsController.GetAttackableHexes(unit);
            HighlightHexes(reachableHexes);
        }

        private void HandleHexHovered(HexModel hexModel)
        {
            if (!hexModel.IsVisible) return;

            if (_currentSelectedUnit != null && _currentSelectedUnit.CurrentHex != null && !_isUnitMoving &&
                _currentSelectedUnit.TeamOwner != TeamOwner.Enemy) 
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

        private async void HandleHexClicked(HexModel hexModel)
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
                    if (_currentSelectedUnit != null && 
                        hexModel.CurrentUnit.TeamOwner != _currentSelectedUnit.TeamOwner &&
                        _attackableHexes.Contains(hexModel))
                    {
                        await HandleCombat(_currentSelectedUnit, hexModel.CurrentUnit);
                        return;
                    }
                    
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
            var newUnit = _unitsController.SpawnPlayerUnit(_currentUISelectedUnit.UnitType, hexModel);
            
            if (newUnit == null) return;
            
            _currentSelectedBuilding.DecreaseUnitCount(newUnit.UnitType);
            _buildingsActionPanel.SetUnitCount(ref _currentSelectedBuilding);
                
            _currentSelectedUnit = null;
            _currentSelectedBuilding = null;
        }
 
        private void PlaceBuilding(HexModel hexModel)
        {
            if (_currentUISelectedBuilding is IProduceResource productionBuilding)
            {
                if (!IsValidResourceDepositForBuilding(hexModel, productionBuilding))
                {
                    Debug.Log($"Cannot place {_currentUISelectedBuilding.BuildingType} here - requires matching resource deposit");
                    return;
                }
            }
            
            var buildingCost = _currentUISelectedBuilding.GetBuildingCost();

            var newBuilding = _buildingsController.SpawnBuilding(_currentUISelectedBuilding.BuildingType, hexModel);
            
            foreach (var resourceCost in buildingCost.ResourceCosts)
            {
                _resourcesController.DeductResource(resourceCost.ResourceType, resourceCost.Amount);
            }

            newBuilding.CurrentHex = hexModel;
             
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
            List<HexModel> nearestAltarCells = _hexGridController.GetHexesInRadius(hexModel, newBuilding.ProtectionRadius);
            RevealFogOfWar(nearestCells);
            SetAltarDefence(nearestAltarCells);
            
            _currentSelectedBuilding = null;
        }
        
        private bool IsValidResourceDepositForBuilding(HexModel hexModel, IProduceResource productionBuilding)
        {
            if (hexModel.ResourceDeposit == null)
            {
                return false;
            }

            return hexModel.ResourceDeposit.ResourceType == productionBuilding.ResourceType;
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

        private void SetAltarDefence(List<HexModel> cells)
        {
            foreach (var cell in cells)
            {
                cell.SetAltarDefence(true);
            }
        }
        
        private async UniTask HandleCombat(Unit attackingUnit, Unit targetUnit)
        {
            int attackRange = attackingUnit.AttackRange;
            int currentDistance =
                _pathfindingController.CalculatePathDistance(attackingUnit.CurrentHex, targetUnit.CurrentHex);
            
            if (currentDistance > attackRange)
            {
                var pathToTarget = new List<HexModel>(_currentPath);
                if (pathToTarget is { Count: > 0 })
                {
                    int stepsNeeded = currentDistance - attackRange;
                    var movementPath = pathToTarget.Take(stepsNeeded + 1).ToList();
                    
                    _isUnitMoving = true;
                    ClearPathHighlight();
                    await _unitsController.MoveUnitAlongPath(attackingUnit, movementPath);
                    _isUnitMoving = false;
                }
            }
            
            _unitsController.ProcessCombat(attackingUnit, targetUnit);
            _uiSelectedEntityView.UpdateSelectedEntityInfo();
            ClearAllSelections();
        }
         
        private async UniTask MoveSelectedUnit(HexModel targetHex)
        {
            if (_highlightedHexes.Contains(targetHex))
            {
                var path = new List<HexModel>(_currentPath);
                
                if (path.Count > 0)
                {
                    _isUnitMoving = true;
                    ClearPathHighlight();
                    await _unitsController.MoveUnitAlongPath(_currentSelectedUnit, path);
                    _isUnitMoving = false;
                    
                    ClearAllSelections();
                    _uiSelectedEntityView.UpdateSelectedEntityInfo();
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
    }
}