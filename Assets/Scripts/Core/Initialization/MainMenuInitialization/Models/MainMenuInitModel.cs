using System;
using Game.UI.UIMainMenuScene.MainMenu.Presenter;
using Game.UI.UIMainMenuScene.MainMenu.View;
using UnityEngine;

namespace Core.Initialization.MainMenuInitialization.Models
{
    public class MainMenuInitModel: MonoBehaviour
    {
        [SerializeField] private MainMenuView _mainMenuView;
        
        private void Awake()
        {
            InitView();
        }

        private void InitView()
        {
            MainMenuPresenter mainMenuPresenter = new MainMenuPresenter();
            _mainMenuView.Init(mainMenuPresenter);
        }
    }
}