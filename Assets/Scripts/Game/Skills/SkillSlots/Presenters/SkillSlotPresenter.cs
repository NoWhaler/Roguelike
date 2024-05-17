using Game.Skills.SkillsBook.Models;
using Game.Skills.SkillSlots.Views;
using Game.Skills.SkillsSelection.Models;
using Zenject;

namespace Game.Skills.SkillSlots.Presenters
{
    public class SkillSlotPresenter
    {
        private SkillModel _skillModel;

        private SkillSelectorModel _skillSelectorModel;

        public SkillSlotPresenter(SkillSelectorModel skillSelectorModel)
        {
            _skillSelectorModel = skillSelectorModel;
        }

        public void OnSkillSlotButtonClick(SkillSlotView skillSlotView)
        {
            if (_skillSelectorModel.CurrentSkill != null)
            {
                var skill = _skillSelectorModel.CurrentSkill;
                
                _skillModel = skill;
                _skillModel.SkillName = skill.SkillName;
                _skillModel.SkillSprite = skill.SkillSprite;
            }
        }
    }
}