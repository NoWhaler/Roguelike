using System.Collections.Generic;
using Game.Buildings.Controller;
using Game.Buildings.Enum;
using Game.Buildings.Interfaces;
using Game.Hex;
using Game.ProductionResources.Controller;
using Game.Researches.Controller;
using Game.Units.Enum;
using ScriptableObjects;
using UnityEngine;
using Zenject;

namespace Game.Buildings.BuildingsType
{
    public abstract class Building: MonoBehaviour
    {
        private Dictionary<UnitType, int> _unitCounts = new Dictionary<UnitType, int>()
        {
            {
                UnitType.Archer, 0
            },
            
            {
                UnitType.Swordsman, 0
            },
            
            {
                UnitType.Crossbowman, 0
            },
            
            {
                UnitType.Horseman, 0
            },
        };
        
        [SerializeField] protected BuildingCostSO _buildingCost;
        
        [field: SerializeField] public BuildingType BuildingType { get; set; }
        
        [field: SerializeField] public TeamOwner BuildingOwner { get; set; }
        
        [field: SerializeField] public float MaxHealth { get; set; }
        
        [field: SerializeField] public float CurrentHealth { get; set; }
        
        public int RevealFogOfWarRange { get; set; }
        
        public int ProtectionRadius { get; set; }
        
        public HexModel CurrentHex { get; set; }
        
        [SerializeField] protected MeshRenderer _meshRenderer;

        private Collider _buildingCollider;
        
        protected List<IBuildingAction> _availableActions = new List<IBuildingAction>();

        protected ResearchController _researchesController;

        protected ResourcesController _resourcesController;

        private BuildingsController _buildingsController;

        [Inject]
        private void Constructor(ResourcesController resourcesController,
            ResearchController researchesController, BuildingsController buildingsController)
        {
            _researchesController = researchesController;
            _resourcesController = resourcesController;
            _buildingsController = buildingsController;
        }

        public virtual void Initialize()
        {
            CurrentHealth = MaxHealth;
            SetupActions();
        }
        
        
        public void TakeDamage(float damage)
        {
            CurrentHealth -= damage;
            if (CurrentHealth <= 0)
            {
                DisableBuilding();
            }
        }
        
        protected abstract void SetupActions();
        
        public virtual List<IBuildingAction> GetAvailableActions()
        {
            return _availableActions;
        }
        
        public void AddAction(IBuildingAction action)
        {
            _availableActions.Add(action);
        }

        public void RemoveAction(IBuildingAction action)
        {
            _availableActions.Remove(action);
        }
        
        public void IncreaseUnitCount(UnitType unitType)
        {
            if (_unitCounts.ContainsKey(unitType))
            {
                _unitCounts[unitType]++;
            }
        }

        public void DecreaseUnitCount(UnitType unitType)
        {
            if (_unitCounts.ContainsKey(unitType) && _unitCounts[unitType] > 0)
            {
                _unitCounts[unitType]--;
            }
        }

        public int GetUnitCount(UnitType unitType)
        {
            return _unitCounts.ContainsKey(unitType) ? _unitCounts[unitType] : 0;
        }
        
        public BuildingCostSO GetBuildingCost() => _buildingCost;

        private void DisableBuilding()
        {
            _buildingsController.ReturnBuildingToPool(this);
        }
        
        public void UpdateMeshVisibility(bool isVisible)
        {
            _meshRenderer.enabled = isVisible;
        }
    }
}