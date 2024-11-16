using System;
using System.Collections.Generic;
using Core.Builder;
using Core.Services;
using Core.TurnBasedSystem;
using Game.Buildings.BuildingsType;
using Game.Buildings.Interfaces;
using Game.Hex;
using UnityEngine;
using Zenject;

namespace Game.Buildings.Controller
{
    public class BuildingsController: IInitializable, IDisposable
    {
        private List<IProduceResource> _resourcesProductionBuildings = new List<IProduceResource>();

        private List<IHireUnit> _unitsHiringBuildings = new List<IHireUnit>();

        private GameTurnController _gameTurnController;

        private BuildingsConfigurationsService _buildingsConfigurationsService;

        private DiContainer _diContainer;

        public event Action<Building> OnBuildingPlaced;

        [Inject]
        private void Constructor(GameTurnController gameTurnController,
            BuildingsConfigurationsService buildingsConfigurationsService, DiContainer diContainer)
        {
            _gameTurnController = gameTurnController;
            _buildingsConfigurationsService = buildingsConfigurationsService;
            _diContainer = diContainer;
        }

        public void Initialize()
        {
            _gameTurnController.OnTurnEnded += AddResourcesFromBuildings;
        }

        public void Dispose()
        {
            _gameTurnController.OnTurnEnded -= AddResourcesFromBuildings;
        }

        private void AddResourcesFromBuildings()
        {
            foreach (var resourcesProductionBuilding in _resourcesProductionBuildings)
            {
                resourcesProductionBuilding.ProduceResources();
            }
        }

        public Building SpawnBuilding(Building buildingPrefab, HexModel targetHex)
        {
            var building = _diContainer.InstantiatePrefabForComponent<Building>(buildingPrefab,
                new Vector3(targetHex.HexPosition.x, targetHex.HexPosition.y + 2.5f, targetHex.HexPosition.z),
                Quaternion.identity, targetHex.transform);

            var config = _buildingsConfigurationsService.GetConfig(building.BuildingType);
            
            building = new BuildingBuilder(building, config)
                .WithHealth()
                .WithType()
                .WithFogOfWarRange()
                .AtPosition(targetHex)
                .Build();
            
            OnBuildingPlaced?.Invoke(building);

            return building;
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