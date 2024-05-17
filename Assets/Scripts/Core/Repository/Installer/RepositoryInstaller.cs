using Core.Repository.Interfaces;
using UnityEngine;
using Zenject;

namespace Core.Repository.Installer
{
    public class RepositoryInstaller: MonoInstaller
    {
        [SerializeField] private JsonSkillsLoader _jsonSkillsLoader;
        
        [SerializeField] private SQLiteSkillsLoader _sqLiteSkillsLoader;
        
        public override void InstallBindings()
        {
            Container.Bind<IRepository>().To<JsonSkillsLoader>().FromComponentInNewPrefab(_jsonSkillsLoader).AsSingle().NonLazy();
        }
    }
}