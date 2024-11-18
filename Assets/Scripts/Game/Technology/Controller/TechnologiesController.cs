using System;
using System.Collections.Generic;
using System.Linq;
using Game.Buildings.BuildingsType;
using Game.Buildings.Controller;
using Game.Buildings.Interfaces;
using Game.Technology.Model;
using Game.Units;
using Game.Units.Controller;
using Game.Units.Enum;
using UnityEngine;
using Zenject;

namespace Game.Technology.Controller
{
    public class TechnologiesController: IInitializable, IDisposable
    {
        private Dictionary<string, TechnologyModel> _allTechnologies = new Dictionary<string, TechnologyModel>();
        private readonly TechnologyBuffs _currentBuffs = new();

        private UnitsController _unitsController;

        private BuildingsController _buildingsController;

        [Inject]
        private void Constructor(UnitsController unitsController, BuildingsController buildingsController)
        {
            _unitsController = unitsController;
            _buildingsController = buildingsController;
        }
        
        public void Initialize()
        {
            _unitsController.OnUnitHired += ApplyStoredBuffsToUnit;
            _buildingsController.OnBuildingPlaced += ApplyStoredBuffsToBuilding;
        }
        
        public void Dispose()
        {
            _unitsController.OnUnitHired -= ApplyStoredBuffsToUnit;
            _buildingsController.OnBuildingPlaced -= ApplyStoredBuffsToBuilding;
        }

        public void InitializeResearch(Dictionary<string, TechnologyModel> technologies)
        {
            _allTechnologies = technologies;
        }
        
        public void StartTechnology(TechnologyModel technology)
        {
            Debug.Log("Tech started");
        }

        public void CompleteTechnology(TechnologyModel technology)
        {
            technology.IsResearched = true;
            
            foreach (var effect in technology.Effects)
            {
                ApplyTechnologyEffect(effect);
            }
            
            Debug.Log($"Technology {technology.Name} completed");
        }
        
        private void ApplyStoredBuffsToUnit(Unit unit)
        {
            if (unit.TeamOwner != TeamOwner.Player) return;
            
            unit.MinDamage += _currentBuffs.UnitDamageBonus;
            unit.MaxDamage += _currentBuffs.UnitDamageBonus;
            unit.CurrentMovementPoints += _currentBuffs.UnitMovementBonus;
            unit.MaxMovementPoints += _currentBuffs.UnitMovementBonus;
            unit.MaxHealth += _currentBuffs.UnitHealthBonus;
            unit.CurrentHealth += _currentBuffs.UnitHealthBonus;
        }
        
        private void ApplyStoredBuffsToBuilding(Building building)
        {
            switch (building)
            {
                case IProduceResource produceResourceBuilding:
                    produceResourceBuilding.ResourceAmountProduction += _currentBuffs.ResourceProductionBonus;
                    break;
                case IHireUnit hiringBuilding:
                {
                    foreach (var action in hiringBuilding.GetAvailableActions())
                    {
                        action.Duration = Math.Max(1, action.Duration - _currentBuffs.UnitProductionSpeedBonus);
                    }

                    break;
                }
            }
        }
        
        private void ApplyTechnologyEffect(TechnologyEffect effect)
        {
            switch (effect.EffectType)
            {
                case TechnologyEffectType.UnitDamage:
                    _currentBuffs.UnitDamageBonus += effect.Value;        
                    ApplyUnitDamageBonus(effect.Value);
                    break;
                    
                case TechnologyEffectType.UnitMovement:
                    _currentBuffs.UnitMovementBonus += (int)effect.Value;        
                    ApplyUnitMovementBonus((int)effect.Value);
                    break;
                    
                case TechnologyEffectType.UnitHealth:
                    _currentBuffs.UnitHealthBonus += effect.Value;     
                    ApplyUnitHealthBonus(effect.Value);
                    break;
                
                case TechnologyEffectType.ResourceProduction:
                    _currentBuffs.ResourceProductionBonus += (int)effect.Value;        
                    ApplyResourceProductionBonus((int)effect.Value);
                    break;
                
                case TechnologyEffectType.UnitProduction:
                    _currentBuffs.UnitDamageBonus += effect.Value; 
                    ApplyUnitProductionBonus(effect.Value);
                    break;
            }
        }
        
        private void ApplyUnitDamageBonus(float bonus) => 
                    _unitsController.GetPlayerUnits().ForEach(u => { u.MinDamage += bonus; u.MaxDamage += bonus; });
        
        private void ApplyUnitMovementBonus(int bonus) => 
            _unitsController.GetPlayerUnits().ForEach(u => { u.MaxMovementPoints += bonus; u.ResetMovementPoints(); });

        private void ApplyUnitHealthBonus(float bonus) => 
            _unitsController.GetPlayerUnits().ForEach(u => { u.MaxHealth += bonus; u.CurrentHealth += bonus; });

        private void ApplyResourceProductionBonus(int bonus) => 
            _buildingsController.GetProducingBuildings().ForEach(b => 
                { if (b is Farm f) f.ResourceAmountProduction += bonus; });

        private void ApplyUnitProductionBonus(float bonus) => 
            _buildingsController.GetHiringBuildings().ForEach(b => 
                b.GetAvailableActions()
                    .ToList()
                    .ForEach(a => a.Duration = Math.Max(1, a.Duration - bonus)));

        public Dictionary<string, TechnologyModel> GetAllTechnologies()
        {
            return _allTechnologies;
        }
    }
}