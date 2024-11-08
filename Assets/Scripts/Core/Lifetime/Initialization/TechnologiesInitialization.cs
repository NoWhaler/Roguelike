using System.Collections.Generic;
using Game.Technology.Controller;
using Game.Technology.Model;
using Zenject;

namespace Core.Lifetime.Initialization
{
    public class TechnologiesInitialization: IInitializable
    {
        private TechnologiesController _technologiesController;

        private readonly Dictionary<string, TechnologyModel> _allTechnologies = new Dictionary<string, TechnologyModel>
        {
            {
                "Tech1", new TechnologyModel
                {
                    Name = "Agriculture",
                    Description = "Unlocks basic farming techniques.",
                    IsResearched = false,
                    TurnsRequired = 5,
                    TurnsLeft = 5
                }
                },
                {
                    "Tech2", new TechnologyModel
                    {
                        Name = "Mining",
                        Description = "Enables the extraction of valuable minerals.",
                        IsResearched = false,
                        TurnsRequired = 7,
                        TurnsLeft = 7
                    }
                },
                {
                    "Tech3", new TechnologyModel
                    {
                        Name = "Writing",
                        Description = "Introduces written communication and record-keeping.",
                        IsResearched = false,
                        TurnsRequired = 4,
                        TurnsLeft = 4
                    }
                },
                {
                    "Tech4", new TechnologyModel
                    {
                        Name = "Bronze Working",
                        Description = "Allows the creation of tools and weapons from bronze.",
                        IsResearched = false,
                        TurnsRequired = 8,
                        TurnsLeft = 8
                    }
                }
        };
        
        [Inject]
        private void Constructor(TechnologiesController technologiesController)
        {
            _technologiesController = technologiesController;
        }
        
        public void Initialize()
        {
            _technologiesController.InitializeResearch(_allTechnologies);
        }
    }
}