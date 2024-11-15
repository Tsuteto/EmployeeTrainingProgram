using UnityEngine;

namespace EmployeeTraining
{
    public class Grade
    {
        public static readonly Grade Rookie = new Grade(
                0,  1,   9,
                80,  90,  80,
                100, 100, 150,
                100, "Rookie", new Color32(0, 175, 240, 255));
        public static readonly Grade Middle = new Grade(
                1, 10,  24,
                100, 110, 100,
                150, 150, 200,
                500, "Middle", new Color32(0, 200, 0, 255));
        public static readonly Grade Adv = new Grade(
                2, 25,  44,
                140, 155, 140,
                220, 220, 270,
                2000, "Advance", new Color32(255, 160, 0, 255));
        public static readonly Grade Pro = new Grade(
                3, 45,  74,
                200, 215, 200,
                300, 300, 350,
                5000, "Pro", new Color32(224, 0, 128, 255));
        public static readonly Grade Ninja = new Grade(
                4, 75, 100,
                280, 300, 280,
                400, 400, 450,
                null, "Ninja", new Color32(196, 128, 255, 255));

        public static readonly Grade[] List = new Grade[]{Rookie, Middle, Adv, Pro, Ninja};

        private Grade(int order, int lvlMin, int lvlMax,
                float wageCashier, float wageRestocker, float wageCsHelper,
                float hiringCostCashier, float hiringCostBaseRestocker, float hiringCostCsHelper,
                float? cost, string name, Color32 color)
        {
            this.Order = order;
            this.LvlMin = lvlMin;
            this.LvlMax = lvlMax;
            this.WageCashier = wageCashier;
            this.WageRestocker = wageRestocker;
            this.WageCsHelper = wageCsHelper;
            this.HiringCostCashier = hiringCostCashier;
            this.HiringCostBaseRestocker = hiringCostBaseRestocker;
            this.HiringCostCsHelper = hiringCostCsHelper;
            this.Cost = cost;
            this.Name = name;
            this.Color = color;
        }

        public readonly int Order;
        public readonly int LvlMin;
        public readonly int LvlMax;

        public readonly float WageCashier;
        public readonly float WageRestocker;
        public readonly float WageCsHelper;
        public readonly float HiringCostCashier;
        public readonly float HiringCostBaseRestocker;
        public readonly float HiringCostCsHelper;

        public readonly float? Cost;
        public readonly string Name;
        public readonly Color32 Color;

        public static bool operator <(Grade a, Grade b) => a.Order < b.Order;
        public static bool operator >(Grade a, Grade b) => a.Order > b.Order;
        public static bool operator <=(Grade a, Grade b) => a.Order <= b.Order;
        public static bool operator >=(Grade a, Grade b) => a.Order >= b.Order;
    }

}