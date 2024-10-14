namespace Game.Buildings
{
    public interface IBuildingAction
    {
        string Name { get; }
        string Description { get; }
        float Cost { get; }
        float Duration { get; }
        bool CanExecute();
        void Execute();
    }
}