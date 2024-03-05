using Game.Spawners.EnemySpawnerControllers;
using Game.Spawners.EnemySpawnerModels;
using UnityEngine;
using Zenject;

namespace Game.Spawners.EnemySpawnerInstallers
{
    public class EnemySpawnerInstaller: MonoInstaller
    {
        [SerializeField] private EnemySpawnerModel _enemySpawnerModel;
        
        public override void InstallBindings()
        {
            Container.BindInstance(_enemySpawnerModel).AsSingle();
            Container.BindInterfacesAndSelfTo<EnemySpawnerController>().AsSingle(); 
        }
    }
}