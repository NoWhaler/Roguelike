using UnityEngine;
using Zenject;

namespace Game.UI.Installers
{
    public class UIStateMachineInstaller: MonoInstaller
    {
        [SerializeField] private UIStateMachine _uiStateMachine;
        
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<UIStateMachine>().FromInstance(_uiStateMachine).AsSingle().NonLazy();
        }
    }
}