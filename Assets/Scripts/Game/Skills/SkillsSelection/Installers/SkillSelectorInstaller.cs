using System.ComponentModel;
using Game.Skills.SkillsSelection.Controllers;
using Game.Skills.SkillsSelection.Models;
using Zenject;

namespace Game.Skills.SkillsSelection.Installers
{
    public class SkillSelectorInstaller: MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<SkillSelectorModel>().AsSingle();
            Container.BindInterfacesAndSelfTo<SkillSelectorController>().AsSingle();
        }
    }
}