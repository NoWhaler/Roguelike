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
        
        public void SetSkill()
        {
            _skillPresenter.SetSkill(_skillDescription, _skillName, _skillImage);
        }
    }
}