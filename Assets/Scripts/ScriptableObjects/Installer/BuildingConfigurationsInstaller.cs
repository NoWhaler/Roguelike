using Game.Buildings.Enum;
using UnityEngine;
using Zenject;

namespace ScriptableObjects.Installer
{
    public class BuildingConfigurationsInstaller: MonoInstaller
    {
        [Header("Resource Buildings")]
        [SerializeField] private ResourceBuildingConfigSO _lumberConfig;
        [SerializeField] private ResourceBuildingConfigSO _quarryConfig;
        [SerializeField] private ResourceBuildingConfigSO _farmConfig;

        [Header("Other Buildings")]
        [SerializeField] private BuildingConfigSO _mainBuildingConfig;
        [SerializeField] private BuildingConfigSO _towerConfig;
        [SerializeField] private BuildingConfigSO _watchTowerConfig;
        [SerializeField] private BuildingConfigSO _houseConfig;
        [SerializeField] private BuildingConfigSO _wallConfig;

        public override void InstallBindings()
        {
            Container.Bind<ResourceBuildingConfigSO>()
                .WithId(BuildingType.Lumber)
                .FromInstance(_lumberConfig);
                
            Container.Bind<ResourceBuildingConfigSO>()
                .WithId(BuildingType.Quarry)
                .FromInstance(_quarryConfig);
                
            Container.Bind<ResourceBuildingConfigSO>()
                .WithId(BuildingType.Farm)
                .FromInstance(_farmConfig);

            Container.Bind<BuildingConfigSO>()
                .WithId(BuildingType.MainBuilding)
                .FromInstance(_mainBuildingConfig);

            Container.Bind<BuildingConfigSO>()
                .WithId(BuildingType.Tower)
                .FromInstance(_towerConfig);
                
            Container.Bind<BuildingConfigSO>()
                .WithId(BuildingType.WatchTower)
                .FromInstance(_watchTowerConfig);

            Container.Bind<BuildingConfigSO>()
                .WithId(BuildingType.House)
                .FromInstance(_houseConfig);
                
            Container.Bind<BuildingConfigSO>()
                .WithId(BuildingType.Wall)
                .FromInstance(_wallConfig);

        }    
    }
}