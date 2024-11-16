using System.Collections.Generic;
using Game.Buildings.Enum;
using Game.Buildings.Interfaces;
using Game.Buildings.Struct;
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
        
        [field: SerializeField] public float MaxHealth { get; set; }
        
        [field: SerializeField] public float CurrentHealth { get; set; }
        
        [field: SerializeField] public int RevealFogOfWarRange { get; set; }
        
        public HexModel CurrentHex { get; set; }

        private Collider _buildingCollider;
        
        protected List<IBuildingAction> _availableActions = new List<IBuildingAction>();

        protected ResearchController _researchesController;

        protected ResourcesController _resourcesController;

        [Inject]
        private void Constructor(ResourcesController resourcesController,
            ResearchController researchesController)
        {
            _researchesController = researchesController;
            _resourcesController = resourcesController;
        }

        public virtual void Initialize()
        {
            CurrentHealth = MaxHealth;
            SetupActions();
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
    }
}