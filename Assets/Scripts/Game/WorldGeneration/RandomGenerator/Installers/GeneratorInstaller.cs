using Game.WorldGeneration.RandomGenerator.Controllers;
using Game.WorldGeneration.RandomGenerator.Models;
using UnityEngine;
using Zenject;

namespace Game.WorldGeneration.RandomGenerator.Installers
{
    public class GeneratorInstaller: MonoInstaller
    {
        [SerializeField] private GeneratorModel _generatorModel;
        
        public override void InstallBindings()
        {
            Container.BindInstance(_generatorModel).AsSingle();
            Container.BindInterfacesAndSelfTo<GeneratorController>().AsSingle(); 
        }
    }
}