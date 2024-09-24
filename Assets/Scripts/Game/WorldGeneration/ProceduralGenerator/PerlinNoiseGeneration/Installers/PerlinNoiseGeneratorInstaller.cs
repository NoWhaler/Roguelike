using Game.WorldGeneration.ProceduralGenerator.PerlinNoiseGeneration.Controllers;
using Game.WorldGeneration.ProceduralGenerator.PerlinNoiseGeneration.Models;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Game.WorldGeneration.ProceduralGenerator.PerlinNoiseGeneration.Installers
{
    public class PerlinNoiseGeneratorInstaller: MonoInstaller
    {
        [FormerlySerializedAs("_generatorModel")] [SerializeField] private PerlinNoiseGeneratorModel perlinNoiseGeneratorModel;
        
        public override void InstallBindings()
        {
            Container.BindInstance(perlinNoiseGeneratorModel).AsSingle();
            Container.BindInterfacesAndSelfTo<PerlinNoiseGeneratorController>().AsSingle(); 
        }
    }
}