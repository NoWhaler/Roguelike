using System.Collections.Generic;
using Game.Skills.Models;
using Game.Skills.Presenter;
using Game.Skills.Views;
using UnityEngine;

namespace Core.Initialization.SkillsInitialization.Model
{
    public class SkillsInitializerModel: MonoBehaviour
    {
        [SerializeField] private List<SkillView> _allSkills;

        public void Init()
        {
            foreach (var skillView in _allSkills)
            {
                SkillModel skillModel = new SkillModel(skillView);
                SkillPresenter skillPresenter = new SkillPresenter(skillModel);
                
                skillView.Init(skillPresenter);
                skillView.SetSkill();
            }
        }
    }
}