using System;
using Game.CameraConfig.Common.Model;
using UnityEngine;
using Zenject;

namespace Game.CameraConfig.Common.Controller
{
    public class CameraController: IInitializable, IDisposable, ILateTickable
    {
        private Camera _mainCamera;
        
        private CameraModel _cameraModel;
        
        [Inject]
        private void Constructor(CameraModel cameraModel)
        {
            _cameraModel = cameraModel;
        } 
        
        private Camera HandleOnGetMainCamera()
        {
            return _mainCamera ??= Camera.main;
        }

        public void Initialize()
        {
            _cameraModel.OnGetMainCamera += HandleOnGetMainCamera;
        }

        public void Dispose()
        {
            _cameraModel.OnGetMainCamera += HandleOnGetMainCamera;
        }

        public void LateTick()
        {
            
        }
    }
}