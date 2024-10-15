using System.Collections.Generic;
using Game.Buildings.BuildingsType;
using Game.Buildings.Enum;
using Game.Researches.Controller;
using Zenject;

namespace Core.Lifetime.Initialization
{
    public class ResearchesInitialization: IInitializable
    {
        private ResearchesController _researchesController;
        
        // TODO Replace all data to separate file and load it
        private Dictionary<BuildingType, List<string>> _allPossibleResearch = new Dictionary<BuildingType, List<string>>
        {
            {
                BuildingType.MainBuilding, new List<string>
                {
                    "Improved Armor",
                    "Advanced Weaponry"
                }
            },
            {
                BuildingType.Lumber, new List<string>
                {
                    "Efficient Logging",
                    "Sustainable Forestry"
                }
            },
            {
                BuildingType.Tower, new List<string>
                {
                    "Enhanced Range",
                    "Reinforced Structure"
                }
            }
        };
        
        [Inject]
        private void Constructor(ResearchesController researchesController)
        {
            _researchesController = researchesController;
        }
        
        public void Initialize()
        {
            _researchesController.InitializeResearch(_allPossibleResearch);
        }
    }
}