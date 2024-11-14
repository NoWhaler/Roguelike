using System.Collections.Generic;

namespace Game.Buildings.Interfaces
{
    public interface IHireUnit
    {
        List<IBuildingAction> GetAvailableActions();
    }
}