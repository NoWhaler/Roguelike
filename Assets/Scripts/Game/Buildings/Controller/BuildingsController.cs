using System;
using System.Collections.Generic;
using Core.Builder;
using Core.ObjectPooling.Pools;
using Core.Services;
using Core.TurnBasedSystem;
using Game.Buildings.BuildingsType;
using Game.Buildings.Enum;
using Game.Buildings.Interfaces;
using Game.Hex;
using Game.Units.Enum;
using UnityEngine;
using Zenject;

namespace Game.Buildings.Controller
{
    public class BuildingsController: IInitializable, IDisposable
    {
        private readonly Dictionary<BuildingType, BuildingsPool> _buildingsPools = new();
        
        private List<IProduceResource> _resourcesProductionBuildings = new List<IProduceResource>();

        private List<IHireUnit> _unitsHiringBuildings = new List<IHireUnit>();

        private GameTurnController _gameTurnController;

        private BuildingsConfigurationsService _buildingsConfigurationsService;

        private DiContainer _diContainer;

        private BuildingsPool _buildingsPoolPrefab;
        
        private Dictionary<BuildingType, Building> _buildingPrefabs;
        
        public event Action<Building> OnBuildingPlaced;

        [Inject]
        private void Constructor(GameTurnController gameTurnController,
            BuildingsConfigurationsService buildingsConfigurationsService, DiContainer diContainer,
            [Inject(Id = "BuildingsPool")] BuildingsPool buildingsPool,
            [Inject(Id = BuildingType.MainBuilding)] Building mainBuilding,
            [Inject(Id = BuildingType.WatchTower)] Building watchTower,
            [Inject(Id = BuildingType.Tower)] Building tower,
            [Inject(Id = BuildingType.Farm)] Building farm,
            [Inject(Id = BuildingType.Wall)] Building wall,
            [Inject(Id = BuildingType.House)] Building house,
            [Inject(Id = BuildingType.Lumber)] Building lumber,
            [Inject(Id = BuildingType.Quarry)] Building quarry)
        {
            _gameTurnController = gameTurnController;
            _buildingsConfigurationsService = buildingsConfigurationsService;
            _diContainer = diContainer;

            _buildingsPoolPrefab = buildingsPool;
            
            _buildingPrefabs = new Dictionary<BuildingType, Building>
            {
                { BuildingType.Farm, farm},
                { BuildingType.Lumber, lumber},
                { BuildingType.Quarry, quarry},
                { BuildingType.House, house},
                { BuildingType.MainBuilding, mainBuilding},
                { BuildingType.WatchTower, watchTower},
                { BuildingType.Tower, tower},
                { BuildingType.Wall, wall}
            };
        }

        public void Initialize()
        {
            _gameTurnController.OnTurnEnded += AddResourcesFromBuildings;
            InitializePool();
        }

        public void Dispose()
        {
            _gameTurnController.OnTurnEnded -= AddResourcesFromBuildings;
        }

        private void InitializePool()
        {
            foreach (BuildingType unitType in System.Enum.GetValues(typeof(BuildingType)))
            {
                var buildingsPool = _diContainer.InstantiatePrefabForComponent<BuildingsPool>(_buildingsPoolPrefab);
                buildingsPool.name = $"BuildingsPool_{unitType}";
                buildingsPool.ObjectPrefab = _buildingPrefabs[unitType];
                _buildingsPools[unitType] = buildingsPool;
                buildingsPool.InitPool();
            }
        }

        private void AddResourcesFromBuildings()
        {
            foreach (var resourcesProductionBuilding in _resourcesProductionBuildings)
            {
                resourcesProductionBuilding.ProduceResources();
            }
        }

        public Building SpawnBuilding(BuildingType buildingType, HexModel targetHex, TeamOwner teamOwner = TeamOwner.Player)
        {
            if (!_buildingsPools.TryGetValue(buildingType, out var pool))
            {
                Debug.LogError($"No pool found for player unit type: {buildingType}");
                return null;
            }

            var building = pool.Get();
            if (building == null)
            {
                Debug.LogWarning($"Pool for player unit type {buildingType} is empty");
                return null;
            }

            var config = _buildingsConfigurationsService.GetConfig(building.BuildingType);
            
            building = new BuildingBuilder(building, config)
                .WithHealth()
                .WithType()
                .WithTeam(teamOwner)
                .WithFogOfWarRange()
                .AtPosition(targetHex)
                .Build();
            
            OnBuildingPlaced?.Invoke(building);

            return building;
        }
        
        public void ReturnBuildingToPool(Building building)
        {
            if (!_buildingsPools.TryGetValue(building.BuildingType, out var pool))
            {
                Debug.LogError($"No pool found for building type: {building.BuildingType}");
                return;
            }

            switch (building)
            {
                case IProduceResource produceResource:
                    UnregisterProductionBuilding(produceResource);
                    break;
                case IHireUnit hireUnit:
                    UnregisterHiringBuilding(hireUnit);
                    break;
            }

            pool.ReturnToPool(building);
            building.gameObject.SetActive(false);
        }
        
        public void RegisterProductionBuilding(IProduceResource building)
        {
            _resourcesProductionBuildings.Add(building);
        }

        public void UnregisterProductionBuilding(IProduceResource building)
        {
            _resourcesProductionBuildings.Remove(building);
        }
        
        public void RegisterHiringBuilding(IHireUnit building)
        {
            _unitsHiringBuildings.Add(building);
        }

        public void UnregisterHiringBuilding(IHireUnit building)
        {
            _unitsHiringBuildings.Remove(building);
        }

        public List<IProduceResource> GetProducingBuildings()
        {
            return _resourcesProductionBuildings;
        }
        
        public List<IHireUnit> GetHiringBuildings()
        {
            return _unitsHiringBuildings;
        }
    }
}