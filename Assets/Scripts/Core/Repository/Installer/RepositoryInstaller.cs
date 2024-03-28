using Core.Repository.Interfaces;
using UnityEngine;
using Zenject;

namespace Core.Repository.Installer
{
    public class RepositoryInstaller: MonoInstaller
    {
        [SerializeField] private JsonSkillsLoader _jsonSkillsLoader;
        
        public override void InstallBindings()
        {
            Container.Bind<IRepository>().To<JsonSkillsLoader>().FromComponentInNewPrefab(_jsonSkillsLoader).AsSingle().NonLazy();
        }
    }
}