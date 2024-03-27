using System;
using Game.Skills.Views;
using UnityEngine;

namespace Game.Skills.Models
{
    [Serializable]
    public class SkillModel
    {
        private readonly SkillView _skillView;

        public string Name { get; set; }

        public string Description { get; set; }
        
        public Sprite SkillIcon { get; set; }

        public SkillModel(SkillView skillView)
        {
            _skillView = skillView;
        }
    }
}