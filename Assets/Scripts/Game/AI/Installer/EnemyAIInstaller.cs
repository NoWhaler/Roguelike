using Zenject;

namespace Game.AI.Installer
{
    public class EnemyAIInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<EnemyAIController>().AsSingle();
        }
    }
}