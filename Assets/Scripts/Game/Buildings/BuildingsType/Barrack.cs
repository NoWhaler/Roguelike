using Game.Buildings.BuildingActions;

namespace Game.Buildings.BuildingsType
{
    public class Barrack: Building
    {
        protected override void SetupActions()
        {
            _availableActions.Add(new HireUnitAction("Footman", 100, 10));
            _availableActions.Add(new HireUnitAction("Archer", 120, 12));
            _availableActions.Add(new ResearchAction("Improved Armor", 200, 30));
        }
    }
}