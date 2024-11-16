using System.Collections.Generic;
using Game.Units.Enum;
using ScriptableObjects;
using Zenject;

namespace Core.Services
{
    public class UnitsConfigurationsService
    {
        private readonly Dictionary<UnitType, UnitConfigSO> _unitConfigs = new();
        
        [Inject]
        public UnitsConfigurationsService(
            [Inject(Id = UnitType.Archer)] UnitConfigSO archerConfig,
            [Inject(Id = UnitType.Swordsman)] UnitConfigSO swordsmanConfig,
            [Inject(Id = UnitType.Crossbowman)] UnitConfigSO crossbowmanConfig,
            [Inject(Id = UnitType.Horseman)] UnitConfigSO horsemanConfig)
        {
            _unitConfigs[UnitType.Archer] = archerConfig;
            _unitConfigs[UnitType.Swordsman] = swordsmanConfig;
            _unitConfigs[UnitType.Crossbowman] = crossbowmanConfig;
            _unitConfigs[UnitType.Horseman] = horsemanConfig;
        }

        public UnitConfigSO GetConfig(UnitType unitType)
        {
            return _unitConfigs.TryGetValue(unitType, out var config) ? config : null;
        }
    }
}