using System;
using Game.Characters.Player.Common.Models;
using Game.Inputs.Common.Model;
using UnityEngine;
using Zenject;

namespace Game.Characters.Player.Common.Controllers
{
    public class PlayerMotionController: IInitializable, IDisposable, ITickable, IFixedTickable
    {
        private PlayerModel _playerModel;

        private InputModel _inputModel;

        private Animator _characterAnimator;
        
        private CharacterController _characterController;
        
        private Vector3 _velocity;

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
            HorizontalMovement();
        }

        public void FixedTick()
        {
            
        }

        private void HorizontalMovement()
        {
            Vector3 horizontalMovement = new Vector3(_inputModel.KeyboardHorizontalInputClick, 0, _inputModel.KeyboardVerticalInputClick);

            horizontalMovement = horizontalMovement.normalized;

            _characterController.Move(horizontalMovement * Time.deltaTime * _playerModel.MoveSpeed);

            _velocity = Vector3.zero;
        }
    }
}