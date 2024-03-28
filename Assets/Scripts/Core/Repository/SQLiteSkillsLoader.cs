using System.Collections.Generic;
using Core.Repository.Interfaces;
using Game.Skills.Models;
using UnityEngine;

namespace Core.Repository
{
    public class SQLiteSkillsLoader: MonoBehaviour, IRepository
    {
        public Queue<SkillModel> LoadSkills()
        {
            return new Queue<SkillModel>();
        }
    }
}