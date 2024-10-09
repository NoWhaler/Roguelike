using Game.UI.UIMainMenuScene.GenerateWorldSettings.GeneratorSetting.Model;

namespace Game.UI.UIMainMenuScene.GenerateWorldSettings.GeneratorSetting.Presenter
{
    public class GeneratorSettingPresenter
    {
        private GeneratorSettingModel _generatorSettingModel;

        public GeneratorSettingPresenter(GeneratorSettingModel generatorSettingModel)
        {
            _generatorSettingModel = generatorSettingModel;
        }
        
        public void SwapRight()
        {
            _generatorSettingModel.ChangeCurrentFrequency();
        }
        
        public void SwapLeft()
        {
            _generatorSettingModel.ChangeCurrentFrequency();
        }
    }
}