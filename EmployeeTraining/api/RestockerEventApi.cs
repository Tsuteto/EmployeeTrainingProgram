using System;

namespace EmployeeTraining.api
{
    public static class RestockerEventApi
    {
        public static Action<Restocker, Box> BoxThrownIntoTrashEventRegistry { get; set; } = delegate {};
    }
}