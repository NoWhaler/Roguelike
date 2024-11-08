using System.Collections.Generic;
using Game.Buildings.Enum;
using Game.Technology.Controller;
using Game.Technology.Model;
using Zenject;

namespace Core.Lifetime.Initialization
{
    public class TechnologiesInitialization: IInitializable
    {
        private TechnologiesController _technologiesController;

        private readonly Dictionary<TechnologyModel, string> _allTechnologies = new Dictionary<TechnologyModel, string>
        {
            
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