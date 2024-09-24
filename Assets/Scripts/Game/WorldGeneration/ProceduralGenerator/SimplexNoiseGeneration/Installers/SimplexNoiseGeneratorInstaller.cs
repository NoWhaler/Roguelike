using Game.WorldGeneration.ProceduralGenerator.SimplexNoiseGeneration.Controllers;
using Game.WorldGeneration.ProceduralGenerator.SimplexNoiseGeneration.Models;
using UnityEngine;
using Zenject;

namespace Game.WorldGeneration.ProceduralGenerator.SimplexNoiseGeneration.Installers
{
    public class SimplexNoiseGeneratorInstaller: MonoInstaller
    {
        [SerializeField] private SimplexNoiseGeneratorModel perlinNoiseGeneratorModel;
        
        public override void InstallBindings()
        {
            Container.BindInstance(perlinNoiseGeneratorModel).AsSingle();
            Container.BindInterfacesAndSelfTo<SimplexNoiseGeneratorController>().AsSingle(); 
        }
    }
}