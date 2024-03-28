using System.Collections.Generic;
using Core.Repository.Interfaces;
using Game.Skills.Models;
using Game.Skills.Presenter;
using Game.Skills.Views;
using UnityEngine;
using Zenject;

namespace Core.Initialization.SkillsInitialization.Model
{
    public class SkillsInitializerModel: MonoBehaviour
    {
        [SerializeField] private List<SkillView> _allSkills;

        private Queue<SkillModel> _skillModels;

        private IRepository _repository;

        [Inject]
        private void Constructor(IRepository repository)
        {
            _repository = repository;
        }

        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            _skillModels = _repository.LoadSkills();
            
            foreach (var skillView in _allSkills)
            {
                SkillModel skillModel = new SkillModel(skillView);
                SkillPresenter skillPresenter = new SkillPresenter(skillModel);

                
                skillModel.SkillName = _skillModels.Peek().SkillName;
                skillModel.ItemSpriteName = _skillModels.Peek().ItemSpriteName;
                skillModel.Description = _skillModels.Peek().Description;
                
                _skillModels.Dequeue();
                
                skillView.Init(skillPresenter);
                skillView.SetSkill();
            }
        }
    }
}