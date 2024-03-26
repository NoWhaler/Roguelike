using Game.Skills.Models;

namespace Game.Skills.Presenter
{
    public class SkillPresenter
    {
        private SkillModel _skillModel;

        public SkillPresenter(SkillModel skillModel)
        {
            _skillModel = skillModel;
        }
        
        public void SetSkill(SkillModel skillModel)
        {
            _skillDescription.text = skillModel.Description;
            _skillImage.sprite = skillModel.SkillIcon;
            _skillName.text = skillModel.Name;
        }
    }
}