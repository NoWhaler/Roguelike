using Game.Spawners.Controllers;
using Game.Spawners.Models;
using UnityEngine;
using Zenject;

namespace Game.Spawners.Installers
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