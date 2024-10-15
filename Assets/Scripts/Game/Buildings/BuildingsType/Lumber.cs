using Game.Buildings.BuildingActions;

namespace Game.Buildings.BuildingsType
{
    public class Lumber: Building
    {
        protected override void SetupActions()
        {
            _availableActions.Add(new ProductionUpgradeAction("Lumber Efficiency", 150, 2, this));
            _availableActions.Add(new ResearchAction("Efficient Logging", 250, 1, _researchesController, this));
            _availableActions.Add(new ResearchAction("Sustainable Forestry", 250, 1, _researchesController, this));
        }
    }
}