using Zenject;

namespace Core.Services.Installers
{
    public class DataServiceInstaller: MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<DataService>().AsSingle();
        }
    }
}