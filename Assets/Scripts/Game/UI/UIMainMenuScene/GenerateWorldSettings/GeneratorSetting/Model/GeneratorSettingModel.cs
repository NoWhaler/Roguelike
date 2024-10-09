using Game.UI.UIMainMenuScene.GenerateWorldSettings.GeneratorSetting.View;

namespace Game.UI.UIMainMenuScene.GenerateWorldSettings.GeneratorSetting.Model
{
    public class GeneratorSettingModel
    {
        private GeneratorSettingView _generatorSettingView;
        
        private string _settingName;

        private string _currentFrequencySetting;

        public GeneratorSettingModel(GeneratorSettingView generatorSettingView)
        {
            _generatorSettingView = generatorSettingView;
        }

        public void ChangeCurrentFrequency()
        {
            
        }
    }
}