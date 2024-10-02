using Game.WorldGeneration.RTT.Controllers;
using Game.WorldGeneration.RTT.Models;
using UnityEngine;
using Zenject;

namespace Game.WorldGeneration.RTT.Installers
{
    public class RRTAlgorithmInstaller: MonoInstaller
    {
        [SerializeField] private RRTAlgorithModel _rrtAlgorithModel;
        
        public override void InstallBindings()
        {
            Container.BindInstance(_rrtAlgorithModel).AsSingle();
            Container.BindInterfacesAndSelfTo<RRTAlgorithmController>().AsSingle();
        }
    }
}