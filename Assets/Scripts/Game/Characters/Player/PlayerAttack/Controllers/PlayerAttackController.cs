using System;
using Core.ObjectPooling.Pools;
using Game.CameraConfig.Common.Model;
using Game.Characters.Player.PlayerAttack.Models;
using Game.Characters.Player.PlayerMovement.Models;
using Game.Inputs.Common.Model;
using UnityEngine;
using Zenject;

namespace Game.Characters.Player.PlayerAttack.Controllers
{
    public class PlayerAttackController: ITickable
    {
        private ArrowsPool _arrowsPool;
        private InputModel _inputModel;
        private PlayerModel _playerModel;
        private CameraModel _cameraModel;
        private PlayerAttackModel _playerAttackModel;
        
        [Inject]
        private void Constructor(ArrowsPool arrowsPool, InputModel inputModel, PlayerModel playerModel, CameraModel cameraModel, PlayerAttackModel playerAttackModel)
        {
            _arrowsPool = arrowsPool;
            _inputModel = inputModel;
            _playerModel = playerModel;
            _cameraModel = cameraModel;
            _playerAttackModel = playerAttackModel;
        }
        
        public void Tick()
        {
            if (_inputModel.LeftMouseButtonInputClick)
            {
                _playerModel.CancelRotationTowardsTarget();
                Shoot();
            }
        }

        private void Shoot()
        {
            SpawnArrow();
        }

        private void SpawnArrow()
        {
            var ray = _cameraModel.GetMainCamera().ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit))
            {
                if (IsShootAvailable(hit))
                {
                    var arrow = _arrowsPool.Get();

                    arrow.transform.position = _playerModel.ArrowSpawnPosition.position;
                    arrow.gameObject.SetActive(true);

                    arrow.ShootArrow(hit);
                }

                else
                {
                    var hitPoint = hit.point;
                    hitPoint.y = 0f;
                    
                    _playerAttackModel.AttackDirection = hitPoint - _playerModel.transform.position;       
                    _playerAttackModel.AttackDirection = _playerAttackModel.AttackDirection.normalized;
                    _playerAttackModel.AttackDirection = new Vector3(_playerAttackModel.AttackDirection.x, 0f, _playerAttackModel.AttackDirection.z);
                    _playerModel.RotateTowardsAttackTarget(_playerAttackModel.AttackDirection, hit);
                }
            }
        }

        private bool IsShootAvailable(RaycastHit hit)
        {
            var hitPoint = hit.point;
            hitPoint.y = 0f;
            
            var direction = hitPoint - _playerModel.transform.position;
            direction.y = 0f;
            direction = direction.normalized;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                return Quaternion.Angle(_playerModel.transform.rotation, targetRotation) <= 5f;
            }
            
            return false;
        }
    }
}

