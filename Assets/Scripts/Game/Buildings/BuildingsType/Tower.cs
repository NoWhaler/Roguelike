using Game.Buildings.BuildingActions;

namespace Game.Buildings.BuildingsType
{
    public class Tower: Building
    {
        protected override void SetupActions()
        {
             _availableActions.Add(new ResearchAction("Enhanced Range", 180, 3, _researchesController, this));
             _availableActions.Add(new ResearchAction("Reinforced Structure", 200, 2, _researchesController, this));   
        }
    }
}