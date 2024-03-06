using Core.ObjectPooling.Pools;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Core.ObjectPooling.Installers
{
    public class ObjectPoolInstaller: MonoInstaller
    {
        [SerializeField] private ArrowsPool _arrowsPool;
        
        public override void InstallBindings()
        {
            Container.BindInstance(_arrowsPool).AsSingle().NonLazy();
        }
    }
}