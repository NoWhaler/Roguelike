using Game.Buildings.BuildingsType;

namespace Game.Buildings.BuildingActions
{
    public abstract class BaseBuildingAction: IBuildingAction
    {
        public string Name { get; set; }
        public string Description { get; set; }
        
        public float Cost { get; set; }
        public float Duration { get; set; }
        public bool IsActive { get; set; }

        protected Building _building;
        
        public abstract bool CanExecute();

        public abstract void Execute();

        public abstract void Complete();
    }
}