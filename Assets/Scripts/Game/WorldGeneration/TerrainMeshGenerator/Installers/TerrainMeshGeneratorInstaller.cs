using Game.WorldGeneration.RTT;
using Game.WorldGeneration.TerrainMeshGenerator.Models;
using UnityEngine;
using Zenject;

namespace Game.WorldGeneration.TerrainMeshGenerator.Installers
{
    public class TerrainMeshGeneratorInstaller: MonoInstaller
    {
        [SerializeField] private TerrainMeshGeneratorModel _terrainMeshGeneratorModel;
        
        public override void InstallBindings()
        {
            Container.BindInstance(_terrainMeshGeneratorModel).AsSingle();
            Container.BindInterfacesAndSelfTo<TerrainMeshGeneratorController>().AsSingle();
        }
    }
}