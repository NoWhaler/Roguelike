using Game.Inputs.Common.Controllers;
using Game.Inputs.Common.Model;
using Zenject;

namespace Game.Inputs.Common.Installers
{
    public class InputInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<InputModel>().AsSingle();
            Container.BindInterfacesAndSelfTo<InputController>().AsSingle();
        }
    }
}