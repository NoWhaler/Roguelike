using Game.Buildings.BuildingsType;
using Game.Buildings.Interfaces;
using Game.Researches.Controller;
using UnityEngine;

namespace Game.Buildings.BuildingActions
{
    public class ResearchAction: BaseBuildingAction
    {
        private ResearchController _researchController;
        
        public ResearchAction(string name, float cost, float duration, ResearchController researchController, Building building)
        {
            Name = name;
            Cost = cost;
            Duration = duration;
            _researchController = researchController;
            _building = building;
        }
        
        public override bool CanExecute()
        {
            return !_researchController.IsResearchActive(_building.BuildingType, Name);
        }

        public override void Execute()
        {
            Debug.Log("Research started");
            
            _researchController.StartResearch(_building.BuildingType, Name);
            IsActive = true;
        }

        public override void Complete()
        {
            Debug.Log("Research completed");
            
            _researchController.CompleteResearch(_building.BuildingType, Name);

            IBuildingAction buildingAction = null;
            
            foreach (var action in _building.GetAvailableActions())
            {
                if (action.Name == Name)
                {
                    buildingAction = action;
                }
            }

            _building.RemoveAction(buildingAction);
            
            IsActive = false;
        }
    }
}