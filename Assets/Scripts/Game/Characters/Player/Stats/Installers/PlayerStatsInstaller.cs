using Game.Characters.Player.Stats.Models;
using Game.Characters.Player.Stats.Views;
using UnityEngine;
using Zenject;

namespace Game.Characters.Player.Stats.Installers
{
    public class PlayerStatsInstaller: MonoInstaller<PlayerStatsInstaller>
    {
        [SerializeField] private PlayerStatsView _playerStatsView;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<PlayerStatsModel>().AsSingle().NonLazy();
            Container.BindInstance(_playerStatsView).AsSingle();
        }
    }
}