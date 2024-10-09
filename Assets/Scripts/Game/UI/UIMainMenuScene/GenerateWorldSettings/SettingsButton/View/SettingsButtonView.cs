using Game.UI.UIMainMenuScene.GenerateWorldSettings.SettingsButton.Presenter;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI.UIMainMenuScene.GenerateWorldSettings.SettingsButton.View
{
    public class SettingsButtonView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private SettingsButtonPresenter _settingsButtonPresenter;
        
        [SerializeField] private Button _settingsTabButton;

        private void OnEnable()
        {
            
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log($"Enter {gameObject.name} button");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Debug.Log($"Exit {gameObject.name} button");
        }

        public void Init(SettingsButtonPresenter settingsButtonPresenter)
        {
            _settingsButtonPresenter = settingsButtonPresenter;
        }
    }
}