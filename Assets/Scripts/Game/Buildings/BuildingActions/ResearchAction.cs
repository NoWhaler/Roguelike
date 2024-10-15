using Game.Buildings.BuildingsType;
using Game.Researches.Controller;
using UnityEngine;

namespace Game.Buildings.BuildingActions
{
    public class ResearchAction: BaseBuildingAction
    {
        private ResearchesController _researchesController;
        
        public ResearchAction(string name, float cost, float duration, ResearchesController researchesController, Building building)
        {
            Name = name;
            Cost = cost;
            Duration = duration;
            _researchesController = researchesController;
            _building = building;
        }
        
        public override bool CanExecute()
        {
            return !_researchesController.IsResearchActive(_building.BuildingType, Name);
        }

        public override void Execute()
        {
            Debug.Log("Research started");
            
            _researchesController.StartResearch(_building.BuildingType, Name);
            IsActive = true;
        }

        public override void Complete()
        {
            Debug.Log("Research completed");
            
            _researchesController.CompleteResearch(_building.BuildingType, Name);

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