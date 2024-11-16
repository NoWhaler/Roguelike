using System.Collections.Generic;
using Game.Buildings.Enum;
using ScriptableObjects;
using UnityEngine;
using Zenject;

namespace Core.Services
{
    public class BuildingsConfigurationsService
    {
        private readonly Dictionary<BuildingType, BuildingConfigSO> _buildingConfigs = new();

        [Inject]
        public BuildingsConfigurationsService(
            [Inject(Id = BuildingType.Lumber)] ResourceBuildingConfigSO lumberConfig,
            [Inject(Id = BuildingType.Quarry)] ResourceBuildingConfigSO quarryConfig,
            [Inject(Id = BuildingType.Farm)] ResourceBuildingConfigSO farmConfig,
            [Inject(Id = BuildingType.MainBuilding)] BuildingConfigSO mainBuildingConfig,
            [Inject(Id = BuildingType.Tower)] BuildingConfigSO towerConfig,
            [Inject(Id = BuildingType.House)] BuildingConfigSO houseConfig,
            [Inject(Id = BuildingType.WatchTower)] BuildingConfigSO watchTowerConfig,
            [Inject(Id = BuildingType.Wall)] BuildingConfigSO wallConfig)
        {
            _buildingConfigs[BuildingType.Lumber] = lumberConfig;
            _buildingConfigs[BuildingType.Quarry] = quarryConfig;
            _buildingConfigs[BuildingType.Farm] = farmConfig;
            _buildingConfigs[BuildingType.MainBuilding] = mainBuildingConfig;
            _buildingConfigs[BuildingType.Tower] = towerConfig;
            _buildingConfigs[BuildingType.House] = houseConfig;
            _buildingConfigs[BuildingType.WatchTower] = watchTowerConfig;
            _buildingConfigs[BuildingType.Wall] = wallConfig;
        }

        public T GetConfig<T>(BuildingType buildingType) where T : BuildingConfigSO
        {
            if (_buildingConfigs.TryGetValue(buildingType, out var config))
            {
                return config as T;
            }
            Debug.LogError($"No configuration found for building type: {buildingType}");
            return null;
        }

        public BuildingConfigSO GetConfig(BuildingType buildingType)
        {
            if (_buildingConfigs.TryGetValue(buildingType, out var config))
            {
                return config;
            }
            Debug.LogError($"No configuration found for building type: {buildingType}");
            return null;
        }    
    }
}