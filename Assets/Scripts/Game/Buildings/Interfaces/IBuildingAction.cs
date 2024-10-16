namespace Game.Buildings.Interfaces
{
    public interface IBuildingAction
    {
        string Name { get; }
        
        string Description { get; }
        
        float Cost { get; }
        
        float Duration { get; set; }
        
        bool IsActive { get; set; }
        
        bool CanExecute();
        
        void Execute();

        void Complete();
    }
}