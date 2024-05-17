using System.Threading;
using Core.ObjectPooling.Pools;
using Cysharp.Threading.Tasks;
using Game.Skills.SkillsSelection.Models;
using UnityEngine;
using Zenject;

namespace Core.ObjectPooling
{
    public class Arrow: MonoBehaviour
    {
        [SerializeField] private float _maxLifetime;
        
        private ArrowsPool _arrowsPool;
        
        private Rigidbody _rigidBody;

        private CancellationTokenSource _cancellationTokenSource;
        private SkillSelectorModel _skillSelectorModel;
        
        [Inject]
        private void Constructor(ArrowsPool arrowsPool, SkillSelectorModel skillSelectorModel)
        {
            _arrowsPool = arrowsPool;
            _skillSelectorModel = skillSelectorModel;
        }

        private void OnEnable()
        {
            _rigidBody = GetComponent<Rigidbody>();
        }

        private void MoveArrow(RaycastHit hit)
        {
            var position = hit.point;
            
            var direction = position - transform.position;

            direction.y = 1f;
            
            direction.Normalize();

            var forceMagnitude = 25f;

            Vector3 movementForce = direction * forceMagnitude;
            
            _rigidBody.AddForce(movementForce, ForceMode.Impulse);
        }

        private async UniTask MoveArrowTowardsPosition(RaycastHit hit)
        {
            var elapsedTime = 0f;

            _cancellationTokenSource = new CancellationTokenSource();
            
            MoveArrow(hit);
            while (elapsedTime <= _maxLifetime)
            {
                elapsedTime += Time.deltaTime;
                await UniTask.Yield(PlayerLoopTiming.Update, _cancellationTokenSource.Token);
            }
            _arrowsPool.ReturnToPool(this);
        }
        
        public async void ShootArrow(RaycastHit hit)
        {
            _rigidBody.velocity = Vector3.zero;
            _cancellationTokenSource?.Cancel();
            await MoveArrowTowardsPosition(hit);
        }

        private void OnTriggerEnter(Collider other)
        {
            _cancellationTokenSource?.Cancel();
            _arrowsPool.ReturnToPool(this);
        }
    }
}