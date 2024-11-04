using System;
using Game.Buildings;
using Game.Buildings.BuildingsType;
using Game.Units;

namespace Game.UI.UIGameplayScene.SelectionHandling
{
    public class UISelectionHandler
    {
        public event Action<Building> OnUISelectedBuilding;

        public event Action<Unit> OnUISelectedUnit;
        
        public event Action<Building> OnSelectedBuilding;

        public event Action<Unit> OnSelectedUnit;
        
        public event Action OnSelectionCleared;

        public void SelectUIBuilding(Building building)
        {
            OnUISelectedBuilding?.Invoke(building);
        }

        public void SelectUIUnit(Unit unit)
        {
            OnUISelectedUnit?.Invoke(unit);
        }
        
        public void SelectBuilding(Building building)
        {
            OnSelectedBuilding?.Invoke(building);
        }

        public void SelectUnit(Unit unit)
        {
            OnSelectedUnit?.Invoke(unit);
        }
        
        public void ClearSelection()
        {
            OnSelectionCleared?.Invoke();
        }
    }
}