using Game.Buildings.BuildingActions;
using Game.Units.Enum;

namespace Game.Buildings.BuildingsType
{
    public class MainBuilding: Building
    {
        protected override void SetupActions()
        {
            _availableActions.Add(new HireUnitAction("Footman", 100, 1, this, UnitType.Swordsman));
            _availableActions.Add(new HireUnitAction("Archer", 120, 2, this, UnitType.Archer));
            _availableActions.Add(new ResearchAction("Improved Armor", 200, 3, _researchesController, this));
            _availableActions.Add(new ResearchAction("Advanced Weaponry", 200, 3, _researchesController, this));
        }
    }
}