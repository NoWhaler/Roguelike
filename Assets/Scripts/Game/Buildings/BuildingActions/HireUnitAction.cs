using Game.Buildings.BuildingsType;
using Game.Units;
using Game.WorldGeneration.Hex;
using UnityEngine;

namespace Game.Buildings.BuildingActions
{
    public class HireUnitAction: BaseBuildingAction
    {
        public HireUnitAction(string name, float cost, float duration, Building building)
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
            Debug.Log("Started hire unit");
            IsActive = true;
        }

        public override void Complete()
        {
            Debug.Log("Hiring finished");
            IsActive = false;
        }
    }
}