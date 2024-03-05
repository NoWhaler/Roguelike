using Game.Characters.Player.Common.Controllers;
using Game.Characters.Player.Common.Models;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Game.Characters.Player.Common.Installers
{
    public class PlayerInstaller : MonoInstaller
    {
        [SerializeField] private CharacterController _characterController;

        [FormerlySerializedAs("_characterModel")] [SerializeField] private PlayerModel playerModel; 
    
        public override void InstallBindings()
        {
            Container.BindInstance(playerModel).AsSingle();
            Container.BindInterfacesAndSelfTo<PlayerMotionController>().AsSingle();

            Container.BindInstance(_characterController).AsSingle();
        }
    }
}