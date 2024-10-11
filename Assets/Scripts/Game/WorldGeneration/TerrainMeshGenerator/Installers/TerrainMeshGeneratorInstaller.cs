using Game.WorldGeneration.RTT;
using Game.WorldGeneration.TerrainMeshGenerator.Controllers;
using Game.WorldGeneration.TerrainMeshGenerator.Models;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Game.WorldGeneration.TerrainMeshGenerator.Installers
{
    public class TerrainMeshGeneratorInstaller: MonoInstaller
    {
        [SerializeField] private TerrainMeshGeneratorModel _terrainMeshGeneratorModel;
        
        [SerializeField] private HexagonalTerrainMeshGeneratorModel _hexagonalTerrainMeshGeneratorModel;
        
        public override void InstallBindings()
        {
            Container.BindInstance(_terrainMeshGeneratorModel).AsSingle();
            Container.BindInstance(_hexagonalTerrainMeshGeneratorModel).AsSingle();
            Container.BindInterfacesAndSelfTo<TerrainMeshGeneratorController>().AsSingle();
            Container.BindInterfacesAndSelfTo<HexagonalTerrainMeshGeneratorController>().AsSingle();
        }
    }
}