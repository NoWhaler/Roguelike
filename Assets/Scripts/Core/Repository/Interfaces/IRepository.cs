using System.Collections.Generic;
using Game.Skills.Models;

namespace Core.Repository.Interfaces
{
    public interface IRepository
    {
        Queue<SkillModel> LoadSkills();
    }
}