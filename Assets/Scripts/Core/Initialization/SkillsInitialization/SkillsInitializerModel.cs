using System.Collections.Generic;
using Core.Initialization.Interfaces;
using Core.Repository.Interfaces;
using Game.Skills.SkillsBook.Models;
using Game.Skills.SkillsBook.Presenter;
using Game.Skills.SkillsBook.Views;
using Game.Skills.SkillSlots.Presenters;
using Game.Skills.SkillSlots.Views;
using Game.Skills.SkillsSelection.Models;
using UnityEngine;
using Zenject;

namespace Core.Initialization.SkillsInitialization
{
    public class SkillsInitializerModel: MonoBehaviour, IInitializeData
    {
        [SerializeField] private List<SkillBookView> _allSkillsInBook;

        [SerializeField] private List<SkillSlotView> _allSkillSlots;

        private Queue<SkillModel> _skillModels;

        private IRepository _repository;

        private SkillSelectorModel _skillSelectorModel;

        [Inject]
        private void Constructor(IRepository repository, SkillSelectorModel skillSelectorModel)
        {
            _repository = repository;
            _skillSelectorModel = skillSelectorModel;
        }

        private void Awake()
        {
            Init();
        }

        public void Init()
        {
            InitSkillsBook();
            InitSkillSlots();
        }

        private void InitSkillsBook()
        {
            _skillModels = _repository.LoadSkills();

            foreach (var skillView in _allSkillsInBook)
            {
                SkillModel skillModel = new SkillModel(skillView);
                SkillBookPresenter skillBookPresenter = new SkillBookPresenter(skillModel, _skillSelectorModel);

                skillModel.SkillName = _skillModels.Peek().SkillName;
                skillModel.ItemSpriteName = _skillModels.Peek().ItemSpriteName;
                skillModel.Description = _skillModels.Peek().Description;

                _skillModels.Dequeue();

                skillView.Init(skillBookPresenter);
                skillView.SetSkill();
            }
        }

        private void InitSkillSlots()
        {
            foreach (var skillSlot in _allSkillSlots)
            {
                SkillModel skillModel = new SkillModel(skillSlot);
                SkillSlotPresenter skillSlotPresenter = new SkillSlotPresenter(_skillSelectorModel);
                
                skillSlot.Init(skillSlotPresenter);
            }
        }
    }
}