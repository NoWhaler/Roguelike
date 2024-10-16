using Game.Researches.Controller;
using Zenject;

namespace Game.Researches.Installer
{
    public class ResearchesInstaller: MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<ResearchController>().AsSingle();
        }
    }
}