using Game.Buildings.BuildingActions;

namespace Game.Buildings.BuildingsType
{
    public class Lumber: Building
    {
        protected override void SetupActions()
        {
            _availableActions.Add(new ProductionUpgradeAction("Lumber Efficiency", 150, 20));
            _availableActions.Add(new ResearchAction("Advanced Logging", 250, 40));
        }
    }
}