using UnityEngine;

namespace Game.Buildings.BuildingActions
{
    public class ProductionUpgradeAction: IBuildingAction
    {
        public ProductionUpgradeAction(string name, float cost, float duration)
        {
            Name = name;
            Cost = cost;
            Duration = duration;
        }

        public string Name { get; }
        public string Description { get; }
        public float Cost { get; }
        public float Duration { get; }
        
        public bool CanExecute()
        {
            return true;
        }

        public void Execute()
        {
            Debug.Log("Production updated");
        }
    }
}