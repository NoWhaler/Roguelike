using System;
using UnityEngine;

namespace Game.CameraConfig.Common.Model
{
    public class CameraModel: MonoBehaviour
    {
        [field: SerializeField] public float CameraMoveSpeed { get; set; }
        
        [field: SerializeField] public float EdgeThreshold { get; set; }
        
        [field: SerializeField] public float ZoomSpeed { get; set; }
        
        [field: SerializeField] public float MinZoom { get; set; }
        
        [field: SerializeField] public float MaxZoom { get; set; }
        
        [field: SerializeField] public float RotationSpeed { get; set; }
        
        public event Func<Camera> OnGetMainCamera;
        public Camera GetMainCamera() => OnGetMainCamera?.Invoke();
    }
}