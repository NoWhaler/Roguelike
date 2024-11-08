using System.Collections.Generic;
using Game.Technology.Model;
using UnityEngine;

namespace Game.Technology.Controller
{
    public class TechnologiesController
    {
        private Dictionary<string, TechnologyModel> _allTechnologies = new Dictionary<string, TechnologyModel>();
        
        public void InitializeResearch(Dictionary<string, TechnologyModel> technologies)
        {
            _allTechnologies = technologies;
        }
        
        public void StartTechnology(TechnologyModel technologyModel)
        {
            Debug.Log("Tech started");
        }

        public void CompleteTechnology(TechnologyModel technologyModel)
        {
            technologyModel.IsResearched = true;
            Debug.Log("Tech completed");
        }

        public Dictionary<string, TechnologyModel> GetAllTechnologies()
        {
            return _allTechnologies;
        }
    }
}