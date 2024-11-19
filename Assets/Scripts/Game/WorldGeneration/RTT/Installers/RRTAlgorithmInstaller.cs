using Game.WorldGeneration.RTT.Controllers;
using Game.WorldGeneration.RTT.Models;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Game.WorldGeneration.RTT.Installers
{
    public class RRTAlgorithmInstaller: MonoInstaller
    {
        [FormerlySerializedAs("_rrtAlgorithModel")] [SerializeField] private RRTAlgorithmModel rrtAlgorithmModel;
        
        public override void InstallBindings()
        {
            Container.BindInstance(rrtAlgorithmModel).AsSingle();
            Container.BindInterfacesAndSelfTo<RRTAlgorithmController>().AsSingle();
        }
    }
}