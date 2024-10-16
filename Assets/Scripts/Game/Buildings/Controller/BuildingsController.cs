using System;
using System.Collections.Generic;
using Core.TurnBasedSystem;
using Game.Buildings.Interfaces;
using Zenject;

namespace Game.Buildings.Controller
{
    public class BuildingsController: IInitializable, IDisposable
    {
        private List<IProduceResource> _resourcesProductionBuildings = new List<IProduceResource>();

        private GameTurnController _gameTurnController;
        
        [Inject]
        private void Constructor(GameTurnController gameTurnController)
        {
            _gameTurnController = gameTurnController;
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
        
        public void RegisterBuilding(IProduceResource building)
        {
            _resourcesProductionBuildings.Add(building);
        }

        public void UnregisterBuilding(IProduceResource building)
        {
            _resourcesProductionBuildings.Remove(building);
        }
    }
}