using Core.ObjectPooling.Pools;
using Game.Units.Controller;
using Game.Units.Enum;
using UnityEngine;
using Zenject;

namespace Game.Units.Installer
{
    public class UnitsControllerInstaller: MonoInstaller
    {
        [SerializeField] private Unit _archerPrefab;
        [SerializeField] private Unit _crossbowmanPrefab;
        [SerializeField] private Unit _swordsmanPrefab;
        [SerializeField] private Unit _cavalryPrefab;
        
        [SerializeField] private UnitsPool _playerPoolPrefab;
        [SerializeField] private UnitsPool _enemyPoolPrefab;
        
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<UnitsController>().AsSingle().NonLazy();

            Container.Bind<Unit>()
                .WithId(UnitType.Archer)
                .FromInstance(_archerPrefab);
            Container.Bind<Unit>()
                .WithId(UnitType.Crossbowman)
                .FromInstance(_crossbowmanPrefab);
            Container.Bind<Unit>()
                .WithId(UnitType.Swordsman)
                .FromInstance(_swordsmanPrefab);
            Container.Bind<Unit>()
                .WithId(UnitType.Horseman)
                .FromInstance(_cavalryPrefab);

            Container.Bind<UnitsPool>()
                .WithId("PlayerPool")
                .FromInstance(_playerPoolPrefab);
            Container.Bind<UnitsPool>()
                .WithId("EnemyPool")
                .FromInstance(_enemyPoolPrefab);
        }
    }
}