using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Core.ObjectPooling
{
    public class ObjectPool<T> : MonoBehaviour where T: MonoBehaviour
    {
        [SerializeField] protected T _objectPrefab;
        [SerializeField] protected int _poolSize;

        [Inject] private DiContainer _diContainer;
        
        protected List<T> _freeObjects;
        private List<T> _usedObjects;

        private void Awake()
        {
            _freeObjects = new List<T>();
            _usedObjects = new List<T>();
            for (var i = 0; i < _poolSize; i++)
            {
                var pooledObject = _diContainer.InstantiatePrefabForComponent<T>(_objectPrefab, transform);
                pooledObject.gameObject.SetActive(false);
                _freeObjects.Add(pooledObject);
            }
        }

        public T Get()
        {
            var amountFreeObjects = _freeObjects.Count;
            if (amountFreeObjects == 0)
                return null;

            var pooledObject = _freeObjects[amountFreeObjects - 1];
            _freeObjects.RemoveAt(amountFreeObjects - 1);
            _usedObjects.Add(pooledObject);
            return pooledObject;
        }

        public void ReturnToPool(T pooledObject)
        {
            _usedObjects.Remove(pooledObject);
            _freeObjects.Add(pooledObject);
            
            var pooledObjectTransform = pooledObject.transform;
            pooledObjectTransform.parent = transform;
            pooledObjectTransform.localPosition = Vector3.zero;
            pooledObject.gameObject.SetActive(false);
        }
    }
}