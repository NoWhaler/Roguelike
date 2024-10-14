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
            HandleCameraMovement();
            HandleCameraZoom();
            HandleCameraRotation();
        }
        
        private void HandleCameraMovement()
        {
            Vector3 movement = Vector3.zero;
            
            if (Input.GetKey(KeyCode.W))
                movement.z += 1;
            if (Input.GetKey(KeyCode.S))
                movement.z -= 1;
            if (Input.GetKey(KeyCode.A))
                movement.x -= 1;
            if (Input.GetKey(KeyCode.D))
                movement.x += 1;
            

            // if (Input.mousePosition.x <= _cameraModel.EdgeThreshold)
            //     movement.x = -1;
            // else if (Input.mousePosition.x >= Screen.width - _cameraModel.EdgeThreshold)
            //     movement.x = 1;
            //
            // if (Input.mousePosition.y <= _cameraModel.EdgeThreshold)
            //     movement.z = -1;
            // else if (Input.mousePosition.y >= Screen.height - _cameraModel.EdgeThreshold)
            //     movement.z = 1;

            if (movement.magnitude > 0)
            {
                movement.Normalize(); 

                Vector3 forward = Vector3.ProjectOnPlane(_cameraModel.transform.forward, Vector3.up).normalized;
                Vector3 right = Vector3.ProjectOnPlane(_cameraModel.transform.right, Vector3.up).normalized;

                Vector3 moveDirection = (forward * movement.z + right * movement.x).normalized;

                _cameraModel.transform.position += moveDirection * _cameraModel.CameraMoveSpeed * Time.deltaTime;
            }
        }

        private void HandleCameraZoom()
        {
            var scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll == 0) return;
            
            var transform = _cameraModel.transform;
            var currentPosition = transform.position;
            var forwardDirection = transform.forward;
                
            var newPosition = currentPosition + forwardDirection * scroll * _cameraModel.ZoomSpeed;
                
            var groundPoint = new Vector3(newPosition.x, 0, newPosition.z);
                
            var newHeight = newPosition.y - groundPoint.y;
                
            if (newHeight >= _cameraModel.MinZoom && newHeight <= _cameraModel.MaxZoom)
            {
                _cameraModel.transform.position = newPosition;
            }
        }
        
        private void HandleCameraRotation()
        {
            if (Input.GetKey(KeyCode.Q))
                _cameraModel.transform.Rotate(Vector3.up, -_cameraModel.RotationSpeed * Time.deltaTime, Space.World);
            else if (Input.GetKey(KeyCode.E))
                _cameraModel.transform.Rotate(Vector3.up, _cameraModel.RotationSpeed * Time.deltaTime, Space.World);
        }

    }
}