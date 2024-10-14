using System;
using Game.Buildings;
using Game.Buildings.BuildingsType;
using Game.Units;

namespace Game.UI.UIGameplayScene.SelectionHandling
{
    public class UISelectionHandler
    {
        public event Action<Building> OnSelectedBuilding;

        public event Action<Unit> OnSelectedUnit;

        public void SelectBuilding(Building building)
        {
            OnSelectedBuilding?.Invoke(building);
        }

        public void SelectUnit(Unit unit)
        {
            OnSelectedUnit?.Invoke(unit);
        }
    }
}