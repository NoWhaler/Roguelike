using Game.Skills.SkillsBook.Models;
using Game.Skills.SkillsSelection.Controllers;
using Game.Skills.SkillsSelection.Models;
using TMPro;
using UnityEngine;
using Zenject;
using Image = UnityEngine.UI.Image;

namespace Game.Skills.SkillsBook.Presenter
{
    public class SkillBookPresenter
    {
        private SkillModel _skillModel;

        private SkillSelectorModel _skillSelectorModel;

        public SkillBookPresenter(SkillModel skillModel, SkillSelectorModel skillSelectorModel)
        {
            _skillModel = skillModel;
            _skillSelectorModel = skillSelectorModel;
        }
        
        public void SetSkill(TMP_Text skillDescription, TMP_Text skillName, Image skillImage)
        {
            skillDescription.text = _skillModel.Description;
            skillImage.sprite = _skillModel.SkillIcon;
            skillName.text = _skillModel.SkillName;
        }

        public void OnSkillButtonClick()
        {
            SelectSkill();
        }

        private void SelectSkill()
        {
            _skillSelectorModel.CurrentSkill = _skillModel;
            Debug.Log($"Skill Selected: {_skillModel.SkillName}");
        }
    }
}