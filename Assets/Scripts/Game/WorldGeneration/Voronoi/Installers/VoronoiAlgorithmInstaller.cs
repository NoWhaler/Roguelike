using Zenject;

namespace Game.WorldGeneration.Voronoi.Installers
{
    public class VoronoiAlgorithmInstaller: MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<VoronoiBiomeDistributor>().AsSingle();
            Container.BindInterfacesAndSelfTo<VoronoiTextureGenerator>().AsSingle();
        }
    }
}