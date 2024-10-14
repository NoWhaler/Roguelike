using Game.Buildings.BuildingActions;

namespace Game.Buildings.BuildingsType
{
    public class Tower: Building
    {
        protected override void SetupActions()
        {
             _availableActions.Add(new ResearchAction("Increased Range", 180, 25));
             _availableActions.Add(new ResearchAction("Bonus Damage", 200, 30));   
        }
    }
}