using System;
using Game.Characters.Player.PlayerMovement.Models;
using Game.Inputs.Common.Model;
using UnityEngine;
using Zenject;

namespace Game.Characters.Player.PlayerMovement.Controllers
{
    public class PlayerMotionController: IInitializable, IDisposable, ITickable, IFixedTickable
    {
        private PlayerModel _playerModel;

        private InputModel _inputModel;

        private Animator _characterAnimator;
        
        private Quaternion _targetRotation;
        
        private CharacterController _characterController;

        private Vector3 _movementDirection;
        
        [Inject]
        private void Constructor(PlayerModel playerModel, InputModel inputModel, CharacterController characterController)
        {
            _playerModel = playerModel;
            _inputModel = inputModel;
            _characterController = characterController;
        }
        
        public void Initialize()
        {
            
        }

        public void Dispose()
        {
            
        }

        public void Tick()
        {
            HandleMovement();
            HandleRotation();
        }

        public void FixedTick()
        {
            
        }

        private void HandleMovement()
        {
            _movementDirection = new Vector3(_inputModel.KeyboardHorizontalInputClick, 0, _inputModel.KeyboardVerticalInputClick);

            _movementDirection = _movementDirection.normalized;

            _characterController.Move(_movementDirection * Time.deltaTime * _playerModel.MoveSpeed);
        }

        private void HandleRotation()
        {
            if (Mathf.Abs(_movementDirection.magnitude) < 0.1f)
            {
                _characterController.transform.rotation = Quaternion.Slerp(_characterController.transform.rotation, 
                                                                       _characterController.transform.rotation, 
                                                                       1f);            
            }
            else
            { 
                _targetRotation = Quaternion.LookRotation(_movementDirection, Vector3.up);
                _characterController.transform.rotation = Quaternion.Slerp(_characterController.transform.rotation,
                                                                       _targetRotation, 
                                                                       _playerModel.RotationSmoothness * _playerModel.RotationSpeed * Time.deltaTime);
            }
        }
    }
}