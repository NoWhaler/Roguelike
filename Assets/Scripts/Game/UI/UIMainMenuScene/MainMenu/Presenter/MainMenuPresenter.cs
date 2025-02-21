using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Game.UI.UIMainMenuScene.MainMenu.Presenter
{
    public class MainMenuPresenter
    {
        public async void LoadScene(string sceneName)
        {
            await SceneManager.LoadSceneAsync(sceneName);
        }
    }
}