using System.Collections.Generic;
using EmployeeTraining.api;

namespace EmployeeTraining
{
    public static class ModdedDisplayManager 
    {
        public static readonly List<IModdedDisplayHandler> Registry = new List<IModdedDisplayHandler>();
    }
}