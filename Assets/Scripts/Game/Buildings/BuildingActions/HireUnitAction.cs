using Game.Buildings.BuildingsType;
using Game.Units.Enum;
using UnityEngine;

namespace Game.Buildings.BuildingActions
{
    public class HireUnitAction: BaseBuildingAction
    {
        private UnitType _unitType;
        
        public HireUnitAction(string name, float cost, float duration, Building building, UnitType unitType)
        {
            Name = name;
            Cost = cost;
            Duration = duration;
            _building = building;
            _unitType = unitType;
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
            _building.IncreaseUnitCount(_unitType);
            IsActive = false;
        }
    }
}