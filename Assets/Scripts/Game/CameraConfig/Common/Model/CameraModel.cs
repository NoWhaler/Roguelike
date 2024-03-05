using System;
using UnityEngine;

namespace Game.CameraConfig.Common.Model
{
    public class CameraModel: MonoBehaviour
    {
        public event Func<Camera> OnGetMainCamera;
        public Camera GetMainCamera() => OnGetMainCamera?.Invoke();
    }
}