using System;
using Game.EditorSettings.Controller;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.UI.UIGameplayScene.Settings
{
    public class UISettingsPanel: MonoBehaviour
    {
        [SerializeField] private Button _closeButton;

        [SerializeField] private Toggle _toggleFog;

        [SerializeField] private Toggle _gridToggle;

        private SettingsController _settingsController;

        [Inject]
        private void Constructor(SettingsController settingsController)
        {
            _settingsController = settingsController;
        }

        private void Start()
        {
            _gridToggle.isOn = Shader.GetGlobalFloat(_settingsController.GetGridParameter()) > 0;
        }

        private void OnEnable()
        {
            _closeButton.onClick.AddListener(HidePanel);
            _toggleFog.onValueChanged.AddListener(OnFogToggleChanged);
            _gridToggle.onValueChanged.AddListener(OnToggleGridTexture);
            
            
        }

        private void OnDisable()
        {
            _closeButton.onClick.RemoveListener(HidePanel);
            _toggleFog.onValueChanged.AddListener(OnFogToggleChanged);
            _gridToggle.onValueChanged.AddListener(OnToggleGridTexture);
        }

        private void HidePanel()
        {
            gameObject.SetActive(false);
        }
        
        public void ShowPanel()
        {
            gameObject.SetActive(true);
        }

        private void OnFogToggleChanged(bool value)
        {
            _settingsController.EnableDisableFog(value);
        }

        private void OnToggleGridTexture(bool value)
        {
            _settingsController.EnableDisableGrid(value);
        }
    }
}