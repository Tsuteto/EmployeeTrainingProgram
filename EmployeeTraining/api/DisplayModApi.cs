namespace EmployeeTraining.api
{
    public static class DisplayModApi
    {
        public static void RegisterHandler(IModdedDisplayHandler handler)
        {
            ModdedDisplayManager.Registry.Add(handler);
        }
    }
}