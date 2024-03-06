using System;
using Core.ObjectPooling.Pools;
using Game.Inputs.Common.Model;
using UnityEngine;
using Zenject;

namespace Core.ObjectPooling
{
    public class Arrow: MonoBehaviour
    {
        
        [SerializeField] private float _currentLifeTime;

        [SerializeField] private float _maxLifetime = 5;

        private ArrowsPool _arrowsPool;

        private InputModel _inputModel;

        [Inject]
        private void Constructor(ArrowsPool arrowsPool, InputModel inputModel)
        {
            _arrowsPool = arrowsPool;
            _inputModel = inputModel;
        }
        
        private void Update()
        {
            _currentLifeTime += Time.deltaTime;

            if (_currentLifeTime >= _maxLifetime)
            {
                _currentLifeTime = 0f;
                _arrowsPool.ReturnToPool(this);
            }
        }


        private void MoveTowardsPosition()
        {
            transform.position = _inputModel.MousePosition;
        }
    }
}