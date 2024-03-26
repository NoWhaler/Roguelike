using System;
using UnityEngine;

namespace Game.Inputs.Common.Model
{
    public class InputModel
    {
        public event Func<float> OnGetHorizontalMouseAxis;

        public event Func<float> OnGetVerticalMouseAxis;
        
        public event Func<Vector3> OnGetMousePosition;

        public event Func<bool> OnAttackButtonClicked;

        public event Func<bool> OnDashButtonClicked;
        
        public event Func<float> OnGetHorizontalKeyboardAxis;

        public event Func<float> OnGetVerticalKeyboardAxis;

        public event Func<bool> OnLeftMouseButtonClicked;

        public event Func<bool> OnRightMouseButtonClicked;

        public bool DashButtonInputClick => OnDashButtonClicked?.Invoke() ?? false;

        public bool AttackButtonInputClick => OnAttackButtonClicked?.Invoke() ?? false;

        public float KeyboardHorizontalInputClick => OnGetHorizontalKeyboardAxis?.Invoke() ?? 0f;
        
        public float KeyboardVerticalInputClick => OnGetVerticalKeyboardAxis?.Invoke() ?? 0f;
        
        public float MouseHorizontalInputClick => OnGetHorizontalMouseAxis?.Invoke() ?? 0f;
        
        public float MouseVerticalInputClick => OnGetVerticalMouseAxis?.Invoke() ?? 0f;

        public bool LeftMouseButtonInputClick => OnLeftMouseButtonClicked?.Invoke() ?? false;
        
        public bool RightMouseButtonInputClick => OnRightMouseButtonClicked?.Invoke() ?? false;
        
        public Vector3 MousePosition => OnGetMousePosition?.Invoke() ?? Vector3.zero;
    }
}