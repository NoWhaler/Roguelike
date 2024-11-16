using Zenject;

namespace Core.Services.Installers
{
    public class ServicesInstaller: MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<UnitsConfigurationsService>().AsTransient().NonLazy();
            Container.BindInterfacesAndSelfTo<BuildingsConfigurationsService>().AsTransient().NonLazy();
        }
    }
}