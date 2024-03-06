using Game.CameraConfig.Common.Controller;
using Game.CameraConfig.Common.Model;
using UnityEngine;
using Zenject;

namespace Game.CameraConfig.Common.Installers
{
    public class CameraInstaller : MonoInstaller
    {
        [SerializeField] private CameraModel _cameraModel;
        
        public override void InstallBindings()
        {
            Container.BindInstance(_cameraModel).AsSingle();
            Container.BindInterfacesAndSelfTo<CameraController>().AsSingle();
        }
    }
}