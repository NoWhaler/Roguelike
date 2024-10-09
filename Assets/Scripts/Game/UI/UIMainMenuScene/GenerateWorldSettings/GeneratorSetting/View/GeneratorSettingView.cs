using Game.UI.UIMainMenuScene.GenerateWorldSettings.GeneratorSetting.Enum;
using Game.UI.UIMainMenuScene.GenerateWorldSettings.GeneratorSetting.Presenter;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.UIMainMenuScene.GenerateWorldSettings.GeneratorSetting.View
{
    public class GeneratorSettingView : MonoBehaviour
    {
        private GeneratorSettingPresenter _generatorSettingPresenter;
        
        [SerializeField] private Button _leftArrowButton;

        [SerializeField] private Button _rightArrowButton;

        [SerializeField] private GeneratorSettingType _generatorSettingType;
        
        [SerializeField] private Image _settingImage;

        [SerializeField] private TMP_Text _settingNameText;
        
        [SerializeField] private TMP_Text _settingFrequencyText;

        private void OnEnable()
        {
            _leftArrowButton.onClick.AddListener(() => _generatorSettingPresenter.SwapLeft());
            
            _rightArrowButton.onClick.AddListener(() => _generatorSettingPresenter.SwapRight());
        }

        public void SetSettingImage(Sprite settingSprite)
        {
            _settingImage.sprite = settingSprite;
        }

        public void SetSettingName(string settingName)
        {
            _settingNameText.text = settingName;
        }
        
        public void SetSettingFrequency(string settingName)
        {
            _settingFrequencyText.text = settingName;
        }
        
        public void Init(GeneratorSettingPresenter generatorSettingPresenter)
        {
            _generatorSettingPresenter = generatorSettingPresenter;
        }
    }
}