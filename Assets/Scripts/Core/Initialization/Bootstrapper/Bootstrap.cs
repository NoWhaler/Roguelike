using System.Threading.Tasks;
using Core.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Core.Initialization.Bootstrapper
{
    public class Bootstrap: IInitializable
    {
        private DataService _dataService;

        [Inject]
        private void Constructor(DataService dataService)
        {
            _dataService = dataService;
        }
        
        public async void Initialize()
        {
            await _dataService.LoadPlayerData();
            
            await Loading();

            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        }

        private async UniTask Loading()
        {
            await Task.Delay(3000);
        }
    }
}