using System.Collections.Generic;
using Core.TurnBasedSystem;
using Game.Buildings.Enum;
using Game.Buildings.Interfaces;
using Game.ProductionResources.Controller;
using Game.Researches.Controller;
using Game.Units.Enum;
using Game.WorldGeneration.Hex;
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
        
        [field: SerializeField] public BuildingType BuildingType { get; set; }
        
        [field: SerializeField] public float MaxHealth { get; set; }
        
        [field: SerializeField] public float CurrentHealth { get; set; }
        
        public HexModel CurrentHex { get; private set; }

        private Collider _buildingCollider;
        
        protected List<IBuildingAction> _availableActions = new List<IBuildingAction>();

        protected ResearchController _researchesController;

        protected ResourcesController _resourcesController;

        protected HexGridController _hexGridController;

        protected GameTurnController _gameTurnController;

        [Inject]
        private void Constructor(GameTurnController gameTurnController, ResourcesController resourcesController,
            ResearchController researchesController, HexGridController hexGridController)
        {
            _gameTurnController = gameTurnController;
            _researchesController = researchesController;
            _hexGridController = hexGridController;
            _resourcesController = resourcesController;
        }

        public virtual void Initialize(HexModel hexModel)
        {
            CurrentHealth = MaxHealth;
            CurrentHex = hexModel;
            SetupActions();
        }
        
        protected abstract void SetupActions();
        
        public List<IBuildingAction> GetAvailableActions()
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

        public bool DecreaseUnitCount(UnitType unitType)
        {
            if (_unitCounts.ContainsKey(unitType) && _unitCounts[unitType] > 0)
            {
                _unitCounts[unitType]--;
                return true;
            }
            return false;
        }

        public int GetUnitCount(UnitType unitType)
        {
            return _unitCounts.ContainsKey(unitType) ? _unitCounts[unitType] : 0;
        }
    }
}