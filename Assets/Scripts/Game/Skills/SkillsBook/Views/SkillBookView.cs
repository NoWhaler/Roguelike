using Game.Skills.SkillsBook.Presenter;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Skills.SkillsBook.Views
{
    public class SkillBookView: SkillView
    {
        [SerializeField] private TMP_Text _skillDescription;

        [SerializeField] private TMP_Text _skillName;
        
        [SerializeField] private Image _skillImage;

        private SkillBookPresenter _skillBookPresenter;

        private void Start()
        {
            _button.onClick.AddListener(() =>
                _skillBookPresenter.OnSkillButtonClick());
        }

        public void Init(SkillBookPresenter skillBookPresenter)
        {
            _skillBookPresenter = skillBookPresenter;
        }
        
        public void SetSkill()
        {
            _skillBookPresenter.SetSkill(_skillDescription, _skillName, _skillImage);
        }
    }
}