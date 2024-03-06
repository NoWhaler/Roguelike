using System;
using Game.CameraConfig.Common.Model;
using Game.Inputs.Common.Model;
using UnityEngine;
using Zenject;

namespace Game.Inputs.Common.Controllers
{
    public class InputController: IInitializable, IDisposable
    {
        private Vector3 HandleOnGetMousePosition() => _cameraModel.GetMainCamera().ScreenToWorldPoint(Input.mousePosition);
        private float HandleHorizontalInputMouseAxis() => Input.GetAxis("Mouse X");
        private float HandleVerticalInputMouseAxis() => Input.GetAxis("Mouse Y");

        private bool HandleRightMouseButtonClick() => Input.GetButtonDown("Fire2");
        
        private bool HandleLeftMouseButtonClick() => Input.GetButtonDown("Fire1");

        private bool HandleDashButtonClick() => Input.GetKey(KeyCode.Space);

        private float HandleVerticalInputKeyboardAxis() => Input.GetAxisRaw("Vertical");

        private float HandleHorizontalInputKeyboardAxis() => Input.GetAxisRaw("Horizontal");

        private InputModel _inputModel;

        private CameraModel _cameraModel;

        private bool HandleAttackButtonClick() => Input.GetKeyDown(KeyCode.T);
        
        [Inject]
        private void Constructor(CameraModel cameraModel, InputModel inputModel)
        {
            _cameraModel = cameraModel;
            _inputModel = inputModel;
        } 
        
        private void InitializeKeyboardInput()
        {
            _inputModel.OnGetHorizontalKeyboardAxis += HandleHorizontalInputKeyboardAxis;
            _inputModel.OnGetVerticalKeyboardAxis += HandleVerticalInputKeyboardAxis;
            _inputModel.OnDashButtonClicked += HandleDashButtonClick;
            _inputModel.OnAttackButtonClicked += HandleAttackButtonClick;
        }

        private void DisposeKeyboardInput()
        {
            _inputModel.OnGetHorizontalKeyboardAxis -= HandleHorizontalInputKeyboardAxis;
            _inputModel.OnGetVerticalKeyboardAxis -= HandleVerticalInputKeyboardAxis;
            _inputModel.OnDashButtonClicked -= HandleDashButtonClick;
            _inputModel.OnAttackButtonClicked -= HandleAttackButtonClick;
        }
        
        private void InitializeMouseInput()
        {
            _inputModel.OnGetHorizontalMouseAxis += HandleHorizontalInputMouseAxis;
            _inputModel.OnGetVerticalMouseAxis += HandleVerticalInputMouseAxis;
            _inputModel.OnLeftMouseButtonClicked += HandleLeftMouseButtonClick;
            _inputModel.OnRightMouseButtonClicked += HandleRightMouseButtonClick;
            _inputModel.OnGetMousePosition += HandleOnGetMousePosition;
        }

        private void DisposeMouseInput()
        {
            _inputModel.OnGetHorizontalMouseAxis -= HandleHorizontalInputMouseAxis;
            _inputModel.OnGetVerticalMouseAxis -= HandleVerticalInputMouseAxis;
            _inputModel.OnLeftMouseButtonClicked -= HandleLeftMouseButtonClick;
            _inputModel.OnRightMouseButtonClicked -= HandleRightMouseButtonClick;
            _inputModel.OnGetMousePosition -= HandleOnGetMousePosition;
        }
        
        public void Initialize()
        {
            InitializeKeyboardInput();
            InitializeMouseInput();
        }

        public void Dispose()
        {
            DisposeKeyboardInput();
            DisposeMouseInput();
        }
    }
}