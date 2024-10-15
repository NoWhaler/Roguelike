using Game.Buildings.BuildingsType;
using UnityEngine;

namespace Game.Buildings.BuildingActions
{
    public class ProductionUpgradeAction: BaseBuildingAction
    {
        public ProductionUpgradeAction(string name, float cost, float duration, Building building)
        {
            Name = name;
            Cost = cost;
            Duration = duration;
            _building = building;
        }

        public override bool CanExecute()
        {
            return !IsActive;
        }

        public override void Execute()
        {
            Debug.Log("Production started updated");
            IsActive = true;
        }

        public override void Complete()
        {
            Debug.Log("Production finished");
            
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