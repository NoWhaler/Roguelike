using Game.Buildings.Interfaces;

namespace Game.Buildings.BuildingsType
{
    public class Barrack: Building, IHireUnit
    {
        protected override void SetupActions()
        {
            // _availableActions.Add(new HireUnitAction("Footman", 100, 1, this));
            // _availableActions.Add(new HireUnitAction("Archer", 120, 2, this));
            // _availableActions.Add(new ResearchAction("Improved Armor", 200, 3, _researchesController, this));
        }
    }
}