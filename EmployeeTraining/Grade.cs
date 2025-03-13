using UnityEngine;

namespace EmployeeTraining
{
    public class Grade
    {
        public static readonly Grade Rookie = new Grade(
                0,  1,   9,
                100, "Rookie", new Color32(0, 175, 240, 255));
        public static readonly Grade Middle = new Grade(
                1, 10,  24,
                500, "Middle", new Color32(0, 200, 0, 255));
        public static readonly Grade Adv = new Grade(
                2, 25,  44,
                2000, "Advance", new Color32(255, 160, 0, 255));
        public static readonly Grade Pro = new Grade(
                3, 45,  74,
                5000, "Pro", new Color32(224, 0, 128, 255));
        public static readonly Grade Ninja = new Grade(
                4, 75, 100,
                null, "Ninja", new Color32(196, 128, 255, 255));

        public static readonly Grade[] List = new Grade[]{Rookie, Middle, Adv, Pro, Ninja};

        private Grade(int order, int lvlMin, int lvlMax,
                float? cost, string name, Color32 color)
        {
            this.Order = order;
            this.LvlMin = lvlMin;
            this.LvlMax = lvlMax;
            this.Cost = cost;
            this.Name = name;
            this.Color = color;
        }

        public readonly int Order;
        public readonly int LvlMin;
        public readonly int LvlMax;

        public readonly float? Cost;
        public readonly string Name;
        public readonly Color32 Color;

        public static bool operator <(Grade a, Grade b) => a.Order < b.Order;
        public static bool operator >(Grade a, Grade b) => a.Order > b.Order;
        public static bool operator <=(Grade a, Grade b) => a.Order <= b.Order;
        public static bool operator >=(Grade a, Grade b) => a.Order >= b.Order;
    }

}