using Game.UI.UIMainMenuScene.MainMenu.Presenter;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.UIMainMenuScene.MainMenu.View
{
    public class MainMenuView: MonoBehaviour
    {
        private MainMenuPresenter _mainMenuPresenter;
        
        [SerializeField] private Button _loadGamePlayButton;

        [SerializeField] private Button _loadGeneratedMapButton;

        private const string GENERATED_MAP_SCENE = "MapGeneratedScene";
        private const string GAMEPLAY_SCENE = "GamePlayScene";

        private void Start()
        {
            _loadGamePlayButton.onClick.AddListener(() => _mainMenuPresenter.LoadScene(GAMEPLAY_SCENE));
            
            _loadGeneratedMapButton.onClick.AddListener(() => _mainMenuPresenter.LoadScene(GENERATED_MAP_SCENE));
        }

        public void Init(MainMenuPresenter mainMenuPresenter)
        {
            _mainMenuPresenter = mainMenuPresenter;
        }
    }
}