using Core.Initialization.MainMenuInitialization.Models;
using UnityEngine;
using Zenject;

namespace Core.Initialization.MainMenuInitialization.Installers
{
    public class MainMenuInitInstaller: MonoInstaller
    {
        [SerializeField] private MainMenuInitModel _mainMenuInitModel;
        
        public override void InstallBindings()
        {
            Container.BindInstance(_mainMenuInitModel).AsSingle();
        }
    }
}