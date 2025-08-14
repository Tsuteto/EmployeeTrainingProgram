using System;
using System.Collections.Generic;
using EmployeeTraining.Employee;
using MyBox;

namespace EmployeeTraining.EmployeeRestocker
{
    public class RestockerSkill : EmployeeSkill<RestockerSkill, RestockerSkillTier, EmplRestocker, Restocker>
    {
        private static readonly Dictionary<Grade, float> WAGE = new Dictionary<Grade, float>(){
            {Grade.Rookie, 90f},
            {Grade.Middle, 110f},
            {Grade.Adv, 155f},
            {Grade.Pro, 215f},
            {Grade.Ninja, 300f}
        };

        private static readonly Dictionary<Grade, float> HIRING_COST = new Dictionary<Grade, float>(){
            {Grade.Rookie, 100f},
            {Grade.Middle, 150f},
            {Grade.Adv, 220f},
            {Grade.Pro, 300f},
            {Grade.Ninja, 400f}
        };

        private static readonly float[] EXTRA_HIRING_COST = {0f, 0f, 10f, 50f, 50f, 50f};

        private static readonly RestockerSkillTier[] SKILL_TABLE = {
            new RestockerSkillTier{Lvl=1, Exp=0, Rapidity=1.388889f, Capacity=10000, Height=200, Dexterity=70},
            new RestockerSkillTier{Lvl=2, Exp=50, Rapidity=1.444444f, Capacity=11000, Height=200, Dexterity=73},
            new RestockerSkillTier{Lvl=3, Exp=110, Rapidity=1.500000f, Capacity=12000, Height=200, Dexterity=76},
            new RestockerSkillTier{Lvl=4, Exp=180, Rapidity=1.555556f, Capacity=13000, Height=300, Dexterity=79},
            new RestockerSkillTier{Lvl=5, Exp=260, Rapidity=1.638889f, Capacity=14000, Height=300, Dexterity=82},
            new RestockerSkillTier{Lvl=6, Exp=360, Rapidity=1.722222f, Capacity=15000, Height=400, Dexterity=85},
            new RestockerSkillTier{Lvl=7, Exp=480, Rapidity=1.805556f, Capacity=16000, Height=400, Dexterity=88},
            new RestockerSkillTier{Lvl=8, Exp=620, Rapidity=1.888889f, Capacity=17000, Height=500, Dexterity=91},
            new RestockerSkillTier{Lvl=9, Exp=780, Rapidity=2.000000f, Capacity=18000, Height=500, Dexterity=100},
            new RestockerSkillTier{Lvl=10, Exp=1080, Rapidity=2.222222f, Capacity=20000, Height=1000, Dexterity=100},
            new RestockerSkillTier{Lvl=11, Exp=1280, Rapidity=2.277778f, Capacity=20800, Height=1000, Dexterity=100},
            new RestockerSkillTier{Lvl=12, Exp=1500, Rapidity=2.333333f, Capacity=21600, Height=1000, Dexterity=100},
            new RestockerSkillTier{Lvl=13, Exp=1745, Rapidity=2.388889f, Capacity=22400, Height=1500, Dexterity=100},
            new RestockerSkillTier{Lvl=14, Exp=2015, Rapidity=2.444444f, Capacity=23400, Height=1500, Dexterity=100},
            new RestockerSkillTier{Lvl=15, Exp=2315, Rapidity=2.500000f, Capacity=24400, Height=1500, Dexterity=100},
            new RestockerSkillTier{Lvl=16, Exp=2645, Rapidity=2.583333f, Capacity=25400, Height=2000, Dexterity=100},
            new RestockerSkillTier{Lvl=17, Exp=3010, Rapidity=2.666667f, Capacity=26600, Height=2000, Dexterity=100},
            new RestockerSkillTier{Lvl=18, Exp=3415, Rapidity=2.750000f, Capacity=27800, Height=2000, Dexterity=100},
            new RestockerSkillTier{Lvl=19, Exp=3865, Rapidity=2.833333f, Capacity=29000, Height=2500, Dexterity=100},
            new RestockerSkillTier{Lvl=20, Exp=4370, Rapidity=2.916667f, Capacity=30200, Height=2500, Dexterity=100},
            new RestockerSkillTier{Lvl=21, Exp=4940, Rapidity=3.000000f, Capacity=31600, Height=2500, Dexterity=100},
            new RestockerSkillTier{Lvl=22, Exp=5585, Rapidity=3.111111f, Capacity=33000, Height=3000, Dexterity=100},
            new RestockerSkillTier{Lvl=23, Exp=6320, Rapidity=3.222222f, Capacity=34400, Height=3000, Dexterity=100},
            new RestockerSkillTier{Lvl=24, Exp=7160, Rapidity=3.333333f, Capacity=35800, Height=3000, Dexterity=100},
            new RestockerSkillTier{Lvl=25, Exp=9160, Rapidity=3.472222f, Capacity=40000, Height=4000, Dexterity=100},
            new RestockerSkillTier{Lvl=26, Exp=10005, Rapidity=3.527778f, Capacity=41200, Height=4000, Dexterity=103},
            new RestockerSkillTier{Lvl=27, Exp=10895, Rapidity=3.583333f, Capacity=42400, Height=4000, Dexterity=106},
            new RestockerSkillTier{Lvl=28, Exp=11855, Rapidity=3.638889f, Capacity=43600, Height=4000, Dexterity=109},
            new RestockerSkillTier{Lvl=29, Exp=12910, Rapidity=3.694444f, Capacity=44800, Height=4000, Dexterity=112},
            new RestockerSkillTier{Lvl=30, Exp=14090, Rapidity=3.777778f, Capacity=46000, Height=5000, Dexterity=117},
            new RestockerSkillTier{Lvl=31, Exp=15425, Rapidity=3.861111f, Capacity=47300, Height=5000, Dexterity=122},
            new RestockerSkillTier{Lvl=32, Exp=16950, Rapidity=3.944444f, Capacity=48600, Height=5000, Dexterity=127},
            new RestockerSkillTier{Lvl=33, Exp=18700, Rapidity=4.027778f, Capacity=49900, Height=5000, Dexterity=132},
            new RestockerSkillTier{Lvl=34, Exp=20715, Rapidity=4.111111f, Capacity=51200, Height=5000, Dexterity=137},
            new RestockerSkillTier{Lvl=35, Exp=23035, Rapidity=4.222222f, Capacity=52500, Height=6000, Dexterity=144},
            new RestockerSkillTier{Lvl=36, Exp=25705, Rapidity=4.333333f, Capacity=53900, Height=6000, Dexterity=151},
            new RestockerSkillTier{Lvl=37, Exp=28770, Rapidity=4.444444f, Capacity=55300, Height=6000, Dexterity=158},
            new RestockerSkillTier{Lvl=38, Exp=32280, Rapidity=4.555556f, Capacity=56700, Height=6000, Dexterity=165},
            new RestockerSkillTier{Lvl=39, Exp=36285, Rapidity=4.666667f, Capacity=58100, Height=6000, Dexterity=172},
            new RestockerSkillTier{Lvl=40, Exp=40845, Rapidity=4.805556f, Capacity=59500, Height=7000, Dexterity=181},
            new RestockerSkillTier{Lvl=41, Exp=46020, Rapidity=4.944444f, Capacity=61000, Height=7000, Dexterity=190},
            new RestockerSkillTier{Lvl=42, Exp=51880, Rapidity=5.083333f, Capacity=62500, Height=7000, Dexterity=199},
            new RestockerSkillTier{Lvl=43, Exp=58495, Rapidity=5.222222f, Capacity=64000, Height=7000, Dexterity=208},
            new RestockerSkillTier{Lvl=44, Exp=65945, Rapidity=5.361111f, Capacity=65500, Height=7000, Dexterity=217},
            new RestockerSkillTier{Lvl=45, Exp=85945, Rapidity=5.555556f, Capacity=70000, Height=8000, Dexterity=250},
            new RestockerSkillTier{Lvl=46, Exp=88960, Rapidity=5.611111f, Capacity=71250, Height=8000, Dexterity=260},
            new RestockerSkillTier{Lvl=47, Exp=92390, Rapidity=5.666667f, Capacity=72500, Height=8000, Dexterity=270},
            new RestockerSkillTier{Lvl=48, Exp=96285, Rapidity=5.722222f, Capacity=73750, Height=8000, Dexterity=280},
            new RestockerSkillTier{Lvl=49, Exp=100695, Rapidity=5.777778f, Capacity=75000, Height=8000, Dexterity=290},
            new RestockerSkillTier{Lvl=50, Exp=105680, Rapidity=5.833333f, Capacity=76250, Height=9000, Dexterity=300},
            new RestockerSkillTier{Lvl=51, Exp=111300, Rapidity=5.916667f, Capacity=77500, Height=9000, Dexterity=310},
            new RestockerSkillTier{Lvl=52, Exp=117625, Rapidity=6.000000f, Capacity=78750, Height=9000, Dexterity=320},
            new RestockerSkillTier{Lvl=53, Exp=124725, Rapidity=6.083333f, Capacity=80000, Height=9000, Dexterity=330},
            new RestockerSkillTier{Lvl=54, Exp=132680, Rapidity=6.166667f, Capacity=81250, Height=9000, Dexterity=340},
            new RestockerSkillTier{Lvl=55, Exp=141570, Rapidity=6.250000f, Capacity=82750, Height=10000, Dexterity=355},
            new RestockerSkillTier{Lvl=56, Exp=151485, Rapidity=6.333333f, Capacity=84250, Height=10000, Dexterity=370},
            new RestockerSkillTier{Lvl=57, Exp=162515, Rapidity=6.444444f, Capacity=85750, Height=10000, Dexterity=385},
            new RestockerSkillTier{Lvl=58, Exp=174760, Rapidity=6.555556f, Capacity=87250, Height=10000, Dexterity=400},
            new RestockerSkillTier{Lvl=59, Exp=188320, Rapidity=6.666667f, Capacity=88750, Height=10000, Dexterity=415},
            new RestockerSkillTier{Lvl=60, Exp=203315, Rapidity=6.777778f, Capacity=90250, Height=11000, Dexterity=430},
            new RestockerSkillTier{Lvl=61, Exp=219865, Rapidity=6.888889f, Capacity=91750, Height=11000, Dexterity=445},
            new RestockerSkillTier{Lvl=62, Exp=238110, Rapidity=7.000000f, Capacity=93250, Height=11000, Dexterity=460},
            new RestockerSkillTier{Lvl=63, Exp=258190, Rapidity=7.111111f, Capacity=94750, Height=11000, Dexterity=475},
            new RestockerSkillTier{Lvl=64, Exp=280265, Rapidity=7.222222f, Capacity=96250, Height=11000, Dexterity=490},
            new RestockerSkillTier{Lvl=65, Exp=304495, Rapidity=7.361111f, Capacity=98000, Height=12000, Dexterity=510},
            new RestockerSkillTier{Lvl=66, Exp=331060, Rapidity=7.500000f, Capacity=99750, Height=12000, Dexterity=530},
            new RestockerSkillTier{Lvl=67, Exp=360140, Rapidity=7.638889f, Capacity=101500, Height=12000, Dexterity=550},
            new RestockerSkillTier{Lvl=68, Exp=391935, Rapidity=7.777778f, Capacity=103250, Height=12000, Dexterity=570},
            new RestockerSkillTier{Lvl=69, Exp=426645, Rapidity=7.916667f, Capacity=105000, Height=12000, Dexterity=590},
            new RestockerSkillTier{Lvl=70, Exp=464500, Rapidity=8.055556f, Capacity=106750, Height=13000, Dexterity=610},
            new RestockerSkillTier{Lvl=71, Exp=505730, Rapidity=8.194444f, Capacity=108500, Height=13000, Dexterity=630},
            new RestockerSkillTier{Lvl=72, Exp=550585, Rapidity=8.333333f, Capacity=110250, Height=13000, Dexterity=650},
            new RestockerSkillTier{Lvl=73, Exp=599315, Rapidity=8.472222f, Capacity=112000, Height=13000, Dexterity=670},
            new RestockerSkillTier{Lvl=74, Exp=652190, Rapidity=8.611111f, Capacity=113750, Height=13000, Dexterity=690},
            new RestockerSkillTier{Lvl=75, Exp=852190, Rapidity=10.000000f, Capacity=120000, Height=15000, Dexterity=800},
            new RestockerSkillTier{Lvl=76, Exp=952295, Rapidity=10.111111f, Capacity=122500, Height=15000, Dexterity=820},
            new RestockerSkillTier{Lvl=77, Exp=1055105, Rapidity=10.222222f, Capacity=125000, Height=15000, Dexterity=840},
            new RestockerSkillTier{Lvl=78, Exp=1161620, Rapidity=10.333333f, Capacity=127500, Height=15000, Dexterity=860},
            new RestockerSkillTier{Lvl=79, Exp=1273840, Rapidity=10.444444f, Capacity=130000, Height=15000, Dexterity=880},
            new RestockerSkillTier{Lvl=80, Exp=1394765, Rapidity=10.555556f, Capacity=132500, Height=16000, Dexterity=905},
            new RestockerSkillTier{Lvl=81, Exp=1528395, Rapidity=10.666667f, Capacity=135000, Height=16000, Dexterity=930},
            new RestockerSkillTier{Lvl=82, Exp=1679730, Rapidity=10.777778f, Capacity=137500, Height=16000, Dexterity=955},
            new RestockerSkillTier{Lvl=83, Exp=1856270, Rapidity=10.888889f, Capacity=140000, Height=16000, Dexterity=980},
            new RestockerSkillTier{Lvl=84, Exp=2068015, Rapidity=11.000000f, Capacity=143000, Height=16000, Dexterity=1005},
            new RestockerSkillTier{Lvl=85, Exp=2327465, Rapidity=11.111111f, Capacity=146000, Height=17000, Dexterity=1030},
            new RestockerSkillTier{Lvl=86, Exp=2649620, Rapidity=11.277778f, Capacity=149000, Height=17000, Dexterity=1055},
            new RestockerSkillTier{Lvl=87, Exp=3051980, Rapidity=11.444444f, Capacity=152000, Height=17000, Dexterity=1080},
            new RestockerSkillTier{Lvl=88, Exp=3554545, Rapidity=11.611111f, Capacity=155000, Height=17000, Dexterity=1105},
            new RestockerSkillTier{Lvl=89, Exp=4197315, Rapidity=11.777778f, Capacity=158000, Height=17000, Dexterity=1130},
            new RestockerSkillTier{Lvl=90, Exp=5040290, Rapidity=11.944444f, Capacity=161000, Height=18000, Dexterity=1160},
            new RestockerSkillTier{Lvl=91, Exp=6163470, Rapidity=12.166667f, Capacity=164000, Height=18000, Dexterity=1190},
            new RestockerSkillTier{Lvl=92, Exp=7666855, Rapidity=12.388889f, Capacity=168000, Height=18000, Dexterity=1220},
            new RestockerSkillTier{Lvl=93, Exp=9670445, Rapidity=12.611111f, Capacity=172000, Height=18000, Dexterity=1250},
            new RestockerSkillTier{Lvl=94, Exp=12324240, Rapidity=12.833333f, Capacity=176000, Height=18000, Dexterity=1280},
            new RestockerSkillTier{Lvl=95, Exp=15808240, Rapidity=13.055556f, Capacity=180000, Height=19000, Dexterity=1320},
            new RestockerSkillTier{Lvl=96, Exp=20342445, Rapidity=13.333333f, Capacity=184000, Height=19000, Dexterity=1360},
            new RestockerSkillTier{Lvl=97, Exp=26196855, Rapidity=13.611111f, Capacity=188000, Height=19000, Dexterity=1400},
            new RestockerSkillTier{Lvl=98, Exp=33701470, Rapidity=13.888889f, Capacity=192000, Height=19000, Dexterity=1450},
            new RestockerSkillTier{Lvl=99, Exp=43256290, Rapidity=14.166667f, Capacity=196000, Height=19000, Dexterity=1500},
            new RestockerSkillTier{Lvl=100, Exp=55361315, Rapidity=14.444444f, Capacity=200000, Height=20000, Dexterity=1600},
        };

        public RestockerLogic Logic { get; set; }

        public Restocker Restocker
        {
            get => Employee;
            set => Employee = value;
        }

        public override Restocker Employee {
            get => base.Employee;
            set
            {
                base.Employee = value;
                Logic = new RestockerLogic(this, value);
            }
        }

        public override float Wage => WAGE[Grade];
        public override float HiringCost => HIRING_COST[Grade] + EXTRA_HIRING_COST[this.Id - 1];
        public float Rapidity => Tier.Rapidity * 3.6f;
        public float Capacity => Tier.Capacity / 1000f;
        public float CapacityMaxHeight => Tier.Height / 1000f;
        public int Dexterity => Tier.Dexterity;

        public int CarryingCapacity => Tier.Capacity;
        public int CarryingMaxHeight => Tier.Height;
        public float AgentSpeed => Tier.Rapidity; // [m/s], Vanilla: 2 .. max: 10
        public float AgentAngularSpeed => Math.Max(0, Tier.Rapidity - 2f) * 240; // [degree/s] Vanilla: 0 .. max: 1200
        public float AgentAcceleration => Tier.Rapidity < 2 ? Tier.Rapidity * 4 : 8 + (Tier.Rapidity - 2) * 6; // [m/s^2] Vanilla: 8 .. max: 60
        public float UnpackingTime => 0.7f / (Tier.Dexterity / 100f);
        public float ProductPlacingIntv => 0.2f / (Tier.Dexterity / 100f);
        public float TakingBoxTime => 0.3f / (Tier.CappedDexterity / 100f);
        public float ThrowingBoxTime => 0.7f / (Tier.Dexterity / 100f);
        public float TurningSpeed => 5f * Tier.Rapidity; // Vanilla: 5
        public float RotationTime => 0.3f / (Tier.Dexterity / 100f);

        internal override RestockerSkillTier[] SkillTable => SKILL_TABLE;
        protected override float CostRateToLevelUp => 2f;

        public RestockerSkill(RestockerSkillData data) : base(data)
        {
        }

        public override float GetWage(Grade g) => WAGE[g];
        internal override void ApplyWageToGame(float dailyWage, float hiringCost)
        {
            Singleton<IDManager>.Instance.RestockerSO(Id).DailyWage = dailyWage;
            Singleton<IDManager>.Instance.RestockerSO(Id).HiringCost = hiringCost;
        }

        public override void Setup()
        {
            InitialWage = Singleton<IDManager>.Instance.RestockerSO(Id).DailyWage;
            InitialHiringCost = Singleton<IDManager>.Instance.RestockerSO(Id).HiringCost;
            UpdateStatus(true);
            // Plugin.LogDebug(this);
        }

        public override void Despawn()
        {
            // called only when it is fired, the employee is not detached here
        }


        // public static WaitForSeconds GetIntervalOnPlacingProducts(Restocker restocker)
        // {
        //     Plugin.LogDebug($"GetIntervalOnPlacingProducts called! restocker={restocker}");
        //     RestockerSkill skill = RestockerSkillManager.Instance.GetSkill(restocker);
        //     return new WaitForSeconds(skill.GetProductPlacingInterval);
        // }

    }

    [Serializable]
    public class RestockerSkillData : SkillData<RestockerSkill>
    {
        public RestockerSkillData() : base()
        {
            Skill = new RestockerSkill(this);
        }
    }

    public class EmplRestocker : Employee<Restocker>
    {
        public override int ID => Instance.RestockerID;
    }

    public class RestockerSkillTier : ISkillTier
    {
        public int Lvl { get; set; }
        public int Exp { get; set; }
        public float Rapidity { get; set; }
        public int Capacity { get; set; }
        public int Height { get; set; }
        public int Dexterity { get; set; }

        public int CappedDexterity => Math.Min(Dexterity, 100);
    }

}