using Game.Characters.Player.PlayerAttack.Controllers;
using Game.Characters.Player.PlayerAttack.Models;
using Zenject;

namespace Game.Characters.Player.PlayerAttack.Installers
{
    public class PlayerAttackInstaller: MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<PlayerAttackModel>().AsSingle();
            Container.BindInterfacesAndSelfTo<PlayerAttackController>().AsSingle();
        }
    }
}