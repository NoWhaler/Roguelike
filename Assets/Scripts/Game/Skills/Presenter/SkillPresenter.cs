using Game.Skills.Models;
using TMPro;
using Image = UnityEngine.UI.Image;

namespace Game.Skills.Presenter
{
    public class SkillPresenter
    {
        private SkillModel _skillModel;

        public SkillPresenter(SkillModel skillModel)
        {
            _skillModel = skillModel;
        }
        
        public void SetSkill(TMP_Text skillDescription, TMP_Text skillName, Image skillImage)
        {
            skillDescription.text = _skillModel.Description;
            skillImage.sprite = _skillModel.SkillIcon;
            skillName.text = _skillModel.Name;
        }
    }
}