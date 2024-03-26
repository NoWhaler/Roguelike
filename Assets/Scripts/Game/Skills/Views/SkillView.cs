using Game.Skills.Models;
using Game.Skills.Presenter;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Skills.Views
{
    public class SkillView: MonoBehaviour
    {
        [SerializeField] private TMP_Text _skillDescription;

        [SerializeField] private TMP_Text _skillName;
        
        [SerializeField] private Image _skillImage;

        private SkillPresenter _skillPresenter;

        public void Init(SkillPresenter skillPresenter)
        {
            _skillPresenter = skillPresenter;
        }
        
        public void SetSkill(SkillModel skillModel)
        {
            _skillDescription.text = skillModel.Description;
            _skillImage.sprite = skillModel.SkillIcon;
            _skillName.text = skillModel.Name;
        }
    }
}