using Game.Characters.Player.PlayerMovement.Controllers;
using Game.Characters.Player.PlayerMovement.Models;
using UnityEngine;
using Zenject;

namespace Game.Characters.Player.PlayerMovement.Installers
{
    public class PlayerInstaller : MonoInstaller
    {
        [SerializeField] private CharacterController _characterController;

        [SerializeField] private PlayerModel _playerModel; 
    
        public override void InstallBindings()
        {
            Container.BindInstance(_playerModel).AsSingle();
            Container.BindInterfacesAndSelfTo<PlayerMotionController>().AsSingle();

            Container.BindInstance(_characterController).AsSingle();
        }
    }
}