using UnityEngine;
using Zenject;

namespace Game.Characters.Player.CharacterAnimation.Installers
{
    public class CharacterStateMachineInstaller: MonoInstaller
    {
        [SerializeField] private CharacterStateMachine _characterStateMachine;
        
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<CharacterStateMachine>().FromInstance(_characterStateMachine).AsSingle().NonLazy();
        }
    }
}