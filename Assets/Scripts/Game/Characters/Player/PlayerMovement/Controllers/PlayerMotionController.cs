using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Characters.Player.PlayerAttack.Controllers;
using Game.Characters.Player.PlayerAttack.Models;
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
        
        private CharacterController _characterController;

        [Inject]
        private void Constructor(PlayerModel playerModel, InputModel inputModel, CharacterController characterController)
        {
            _playerModel = playerModel;
            _inputModel = inputModel;
            _characterController = characterController;
        }
        
        public void Initialize()
        {
            _playerModel.OnRotateTowardsTarget += HandleRotationTowardsTarget;
        }

        public void Dispose()
        {
            
        }

        public void Tick()
        {
            HandleFacingDirection();
            HandleRotation();
            HandleMovement();
        }

        public void FixedTick()
        {
            
        }

        private void HandleMovement()
        {
            _playerModel.MovementDirection = new Vector3(_inputModel.KeyboardHorizontalInputClick, 0, _inputModel.KeyboardVerticalInputClick);
            
            _playerModel.MovementDirection = _playerModel.MovementDirection.normalized;

            _characterController.Move(_playerModel.MovementDirection * Time.deltaTime * _playerModel.MoveSpeed);
        }

        private void HandleFacingDirection()
        {
            _playerModel.PlayerFacingDirection =
                _playerModel.PlayerLookAtPoint.position - _playerModel.transform.position;
            _playerModel.PlayerFacingDirection = new Vector3(_playerModel.PlayerFacingDirection.x, 0f,
                _playerModel.PlayerFacingDirection.z);
            _playerModel.PlayerFacingDirection = _playerModel.PlayerFacingDirection.normalized;
            
            Debug.DrawRay(_playerModel.transform.position, _playerModel.PlayerFacingDirection, Color.red, 1f);
        }

        private void HandleRotation()
        {
            if (Mathf.Abs(_playerModel.MovementDirection.magnitude) < 0.1f)
            {
                _characterController.transform.rotation = Quaternion.Slerp(_characterController.transform.rotation, 
                                                                       _characterController.transform.rotation,1f);            
            }
            
            else
            { 
                _playerModel.TargetRotation = Quaternion.LookRotation(_playerModel.MovementDirection, Vector3.up);
                _characterController.transform.rotation = Quaternion.Slerp(_characterController.transform.rotation,
                                                                        _playerModel.TargetRotation, 
                                                                       _playerModel.RotationSmoothness * _playerModel.RotationSpeed * Time.deltaTime);
            }
        }

        private async void HandleRotationTowardsTarget(Vector3 attackDirection, RaycastHit hit)
        {
            await RotateTowardsTarget(attackDirection, hit);
        }

        private async UniTask RotateTowardsTarget(Vector3 attackDirection, RaycastHit hit)
        {
            Debug.DrawRay(_playerModel.transform.position, attackDirection, Color.yellow, 10f);
            
            while (IsRotating(hit))
            {
                _playerModel.TargetRotation = Quaternion.LookRotation(attackDirection, Vector3.up);
                _characterController.transform.rotation = Quaternion.RotateTowards(
                    _characterController.transform.rotation, _playerModel.TargetRotation,
                    _playerModel.RotationSpeed * 100f * Time.deltaTime);
                
                await UniTask.Yield(PlayerLoopTiming.Update, _playerModel.CancellationTokenSource.Token);
            }
            
            _playerModel.CancellationTokenSource?.Cancel();
        }

        private bool IsRotating(RaycastHit hit)
        {
            var direction = hit.point - _playerModel.transform.position;
            direction.y = 0;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                var angle = Quaternion.Angle(_characterController.transform.rotation, targetRotation);

                if (angle >= 5f)
                {
                    return true;
                }
            }
            
            return false;
        }
    }
}