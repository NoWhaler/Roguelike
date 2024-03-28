using Core.Initialization.SkillsInitialization.Model;
using UnityEngine;
using Zenject;

namespace Core.Initialization.SkillsInitialization
{
    public class SkillsInitializerInstaller: MonoInstaller
    {
        [SerializeField] private SkillsInitializerModel _skillsInitializerModel;
        
        public override void InstallBindings()
        {
            Container.BindInstance(_skillsInitializerModel).AsSingle();
            // _skillsInitializerModel.Init();
        }
    }
}