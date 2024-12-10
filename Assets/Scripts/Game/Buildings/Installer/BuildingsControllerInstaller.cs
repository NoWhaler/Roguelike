using Core.ObjectPooling.Pools;
using Game.Buildings.BuildingsType;
using Game.Buildings.Controller;
using Game.Buildings.Enum;
using UnityEngine;
using Zenject;

namespace Game.Buildings.Installer
{
    public class BuildingsControllerInstaller: MonoInstaller
    {
        [SerializeField] private Building _lumberPrefab;
        [SerializeField] private Building _quarryPrefab;
        [SerializeField] private Building _farmPrefab;

        [SerializeField] private Building _mainBuildingPrefab;
        [SerializeField] private Building _towerPrafab;
        [SerializeField] private Building _watchTowerPrefab;
        [SerializeField] private Building _wallPrefab;
        [SerializeField] private Building _housePrefab;
        [SerializeField] private Building _holyAltarPrefab;
        
        [SerializeField] private BuildingsPool _buildingsPool;
        
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<BuildingsController>().AsSingle();

            Container.Bind<Building>()
                .WithId(BuildingType.MainBuilding)
                .FromInstance(_mainBuildingPrefab);
            Container.Bind<Building>()
                .WithId(BuildingType.WatchTower)
                .FromInstance(_watchTowerPrefab);
            Container.Bind<Building>()
                .WithId(BuildingType.Farm)
                .FromInstance(_farmPrefab);
            Container.Bind<Building>()
                .WithId(BuildingType.House)
                .FromInstance(_housePrefab);
            Container.Bind<Building>()
                .WithId(BuildingType.Tower)
                .FromInstance(_towerPrafab);
            Container.Bind<Building>()
                .WithId(BuildingType.Lumber)
                .FromInstance(_lumberPrefab);
            Container.Bind<Building>()
                .WithId(BuildingType.Quarry)
                .FromInstance(_quarryPrefab);
            Container.Bind<Building>()
                .WithId(BuildingType.Wall)
                .FromInstance(_wallPrefab);
            Container.Bind<Building>()
                .WithId(BuildingType.HolyAltar)
                .FromInstance(_holyAltarPrefab);
            
            
            Container.Bind<BuildingsPool>()
                .WithId("BuildingsPool")
                .FromInstance(_buildingsPool);
        }
    }
}