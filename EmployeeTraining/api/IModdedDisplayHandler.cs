namespace EmployeeTraining.api
{
    public interface IModdedDisplayHandler
    {
        bool IsTargetDisplay(DisplaySlot displaySlot);
        int GetProductCountOfGridLayout(DisplaySlot displaySlot);
    }

}