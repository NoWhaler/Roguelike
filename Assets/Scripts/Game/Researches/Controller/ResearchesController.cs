using System.Collections.Generic;
using Game.Buildings.Enum;

namespace Game.Researches.Controller
{
    public class ResearchesController
    {
        private Dictionary<BuildingType, Dictionary<string, bool>> _researchStatus = new Dictionary<BuildingType, Dictionary<string, bool>>();

        public void InitializeResearch(Dictionary<BuildingType, List<string>> allPossibleResearch)
        {
            foreach (var buildingType in allPossibleResearch.Keys)
            {
                _researchStatus[buildingType] = new Dictionary<string, bool>();
                foreach (var research in allPossibleResearch[buildingType])
                {
                    _researchStatus[buildingType][research] = false;
                }
            }
        }

        public bool IsResearchActive(BuildingType buildingType, string researchName)
        {
            if (_researchStatus.TryGetValue(buildingType, out var buildingResearch))
            {
                if (buildingResearch.TryGetValue(researchName, out bool isActive))
                {
                    return isActive;
                }
            }

            return false;
        }

        public void StartResearch(BuildingType buildingType, string researchName)
        {
            if (_researchStatus.TryGetValue(buildingType, out var buildingResearch))
            {
                if (buildingResearch.ContainsKey(researchName))
                {
                    buildingResearch[researchName] = true;
                }
            }
        }

        public void CompleteResearch(BuildingType buildingType, string researchName)
        {
            if (_researchStatus.TryGetValue(buildingType, out var buildingResearch))
            {
                if (buildingResearch.ContainsKey(researchName))
                {
                    buildingResearch[researchName] = false;
                    buildingResearch.Remove(researchName);
                }
            }
        }
    }
}