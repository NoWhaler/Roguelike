using Game.Skills.SkillSlots.Presenters;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Skills.SkillSlots.Views
{
    public class SkillSlotView: SkillView
    {
        private SkillSlotPresenter _skillSlotPresenter;

        [SerializeField] private Image _skillImage;
        
        private DiContainer _diContainer;
        private void Start()
        {
            _button.onClick.AddListener(() => 
                _skillSlotPresenter.OnSkillSlotButtonClick(this)
                );
        }

        public void Init(SkillSlotPresenter skillSlotPresenter)
        {
            _skillSlotPresenter = skillSlotPresenter;
        }
    }
}