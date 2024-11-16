using Game.Buildings.BuildingsType;
using Game.Buildings.Enum;
using Game.Buildings.Interfaces;
using Game.Hex;
using ScriptableObjects;
using UnityEngine;

namespace Core.Builder
{
    public class BuildingBuilder
    {
        private readonly Building _building;
        private BuildingConfigSO _config;
        
        public BuildingBuilder(Building building, BuildingConfigSO config)
        {
            _building = building;
            _config = config;
        }

        public BuildingBuilder WithType()
        {
            _building.BuildingType = _config.BuildingType;
            return this;
        }

        public BuildingBuilder WithHealth()
        {
            _building.MaxHealth = _config.MaxHealth;
            _building.CurrentHealth = _config.MaxHealth;
            return this;
        }

        public BuildingBuilder WithFogOfWarRange()
        {
            _building.RevealFogOfWarRange = _config.RevealFogOfWarRange;
            return this;
        }

        public BuildingBuilder AtPosition(HexModel hex)
        {
            _building.CurrentHex = hex;
            hex.CurrentBuilding = _building;
            _building.gameObject.SetActive(true);
            _building.transform.position = new Vector3(hex.HexPosition.x, hex.HexPosition.y + 2.5f, hex.HexPosition.z);
            return this;
        }

        public Building Build()
        {
            if (_building is IProduceResource resourceBuilding && _config is ResourceBuildingConfigSO resourceConfig)
            {
                resourceBuilding.ResourceType = resourceConfig.ResourceType;
                resourceBuilding.ResourceAmountProduction = resourceConfig.ResourceAmountProduction;
            }
            
            _building.Initialize();
            return _building;
        }
    }
}