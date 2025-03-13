using System;
using System.Collections.Generic;
using EmployeeTraining.Employee;
using MyBox;

namespace EmployeeTraining.EmployeeJanitor
{
    public class JanitorSkill : EmployeeSkill<JanitorSkill, JanitorSkillTier, EmplJanitor, Janitor>
    {
        private static readonly Dictionary<Grade, float> WAGE = new Dictionary<Grade, float>(){
            {Grade.Rookie, 80f},
            {Grade.Middle, 100f},
            {Grade.Adv, 140f},
            {Grade.Pro, 200f},
            {Grade.Ninja, 280f}
        };

        private static readonly Dictionary<Grade, float> HIRING_COST = new Dictionary<Grade, float>(){
            {Grade.Rookie, 150f},
            {Grade.Middle, 200f},
            {Grade.Adv, 270f},
            {Grade.Pro, 350f},
            {Grade.Ninja, 450f}
        };

        private static readonly float[] EXTRA_HIRING_COST = {0f, 50f, 100f};

        private static readonly JanitorSkillTier[] SKILL_TABLE = {
            new JanitorSkillTier{Lvl=1, Exp=0, Rapidity=1.388889f, Dexterity=70},
            new JanitorSkillTier{Lvl=2, Exp=10, Rapidity=1.444444f, Dexterity=73},
            new JanitorSkillTier{Lvl=3, Exp=22, Rapidity=1.500000f, Dexterity=76},
            new JanitorSkillTier{Lvl=4, Exp=36, Rapidity=1.555556f, Dexterity=79},
            new JanitorSkillTier{Lvl=5, Exp=52, Rapidity=1.638889f, Dexterity=82},
            new JanitorSkillTier{Lvl=6, Exp=72, Rapidity=1.722222f, Dexterity=85},
            new JanitorSkillTier{Lvl=7, Exp=96, Rapidity=1.805556f, Dexterity=88},
            new JanitorSkillTier{Lvl=8, Exp=126, Rapidity=1.888889f, Dexterity=91},
            new JanitorSkillTier{Lvl=9, Exp=162, Rapidity=2.000000f, Dexterity=100},
            new JanitorSkillTier{Lvl=10, Exp=242, Rapidity=2.222222f, Dexterity=100},
            new JanitorSkillTier{Lvl=11, Exp=274, Rapidity=2.277778f, Dexterity=100},
            new JanitorSkillTier{Lvl=12, Exp=316, Rapidity=2.333333f, Dexterity=100},
            new JanitorSkillTier{Lvl=13, Exp=371, Rapidity=2.388889f, Dexterity=100},
            new JanitorSkillTier{Lvl=14, Exp=439, Rapidity=2.444444f, Dexterity=100},
            new JanitorSkillTier{Lvl=15, Exp=523, Rapidity=2.500000f, Dexterity=100},
            new JanitorSkillTier{Lvl=16, Exp=623, Rapidity=2.583333f, Dexterity=100},
            new JanitorSkillTier{Lvl=17, Exp=742, Rapidity=2.666667f, Dexterity=100},
            new JanitorSkillTier{Lvl=18, Exp=883, Rapidity=2.750000f, Dexterity=100},
            new JanitorSkillTier{Lvl=19, Exp=1049, Rapidity=2.833333f, Dexterity=100},
            new JanitorSkillTier{Lvl=20, Exp=1246, Rapidity=2.916667f, Dexterity=100},
            new JanitorSkillTier{Lvl=21, Exp=1480, Rapidity=3.000000f, Dexterity=100},
            new JanitorSkillTier{Lvl=22, Exp=1757, Rapidity=3.111111f, Dexterity=100},
            new JanitorSkillTier{Lvl=23, Exp=2086, Rapidity=3.222222f, Dexterity=100},
            new JanitorSkillTier{Lvl=24, Exp=2476, Rapidity=3.333333f, Dexterity=100},
            new JanitorSkillTier{Lvl=25, Exp=3276, Rapidity=3.472222f, Dexterity=100},
            new JanitorSkillTier{Lvl=26, Exp=3496, Rapidity=3.527778f, Dexterity=103},
            new JanitorSkillTier{Lvl=27, Exp=3751, Rapidity=3.583333f, Dexterity=106},
            new JanitorSkillTier{Lvl=28, Exp=4056, Rapidity=3.638889f, Dexterity=109},
            new JanitorSkillTier{Lvl=29, Exp=4426, Rapidity=3.694444f, Dexterity=112},
            new JanitorSkillTier{Lvl=30, Exp=4876, Rapidity=3.777778f, Dexterity=117},
            new JanitorSkillTier{Lvl=31, Exp=5421, Rapidity=3.861111f, Dexterity=122},
            new JanitorSkillTier{Lvl=32, Exp=6081, Rapidity=3.944444f, Dexterity=127},
            new JanitorSkillTier{Lvl=33, Exp=6876, Rapidity=4.027778f, Dexterity=132},
            new JanitorSkillTier{Lvl=34, Exp=7826, Rapidity=4.111111f, Dexterity=137},
            new JanitorSkillTier{Lvl=35, Exp=8951, Rapidity=4.222222f, Dexterity=144},
            new JanitorSkillTier{Lvl=36, Exp=10276, Rapidity=4.333333f, Dexterity=151},
            new JanitorSkillTier{Lvl=37, Exp=11826, Rapidity=4.444444f, Dexterity=158},
            new JanitorSkillTier{Lvl=38, Exp=13626, Rapidity=4.555556f, Dexterity=165},
            new JanitorSkillTier{Lvl=39, Exp=15701, Rapidity=4.666667f, Dexterity=172},
            new JanitorSkillTier{Lvl=40, Exp=18081, Rapidity=4.805556f, Dexterity=181},
            new JanitorSkillTier{Lvl=41, Exp=20796, Rapidity=4.944444f, Dexterity=190},
            new JanitorSkillTier{Lvl=42, Exp=23876, Rapidity=5.083333f, Dexterity=199},
            new JanitorSkillTier{Lvl=43, Exp=27361, Rapidity=5.222222f, Dexterity=208},
            new JanitorSkillTier{Lvl=44, Exp=31291, Rapidity=5.361111f, Dexterity=217},
            new JanitorSkillTier{Lvl=45, Exp=39291, Rapidity=5.555556f, Dexterity=250},
            new JanitorSkillTier{Lvl=46, Exp=43426, Rapidity=5.611111f, Dexterity=260},
            new JanitorSkillTier{Lvl=47, Exp=47796, Rapidity=5.666667f, Dexterity=270},
            new JanitorSkillTier{Lvl=48, Exp=52451, Rapidity=5.722222f, Dexterity=280},
            new JanitorSkillTier{Lvl=49, Exp=57441, Rapidity=5.777778f, Dexterity=290},
            new JanitorSkillTier{Lvl=50, Exp=62816, Rapidity=5.833333f, Dexterity=300},
            new JanitorSkillTier{Lvl=51, Exp=68636, Rapidity=5.916667f, Dexterity=310},
            new JanitorSkillTier{Lvl=52, Exp=74961, Rapidity=6.000000f, Dexterity=320},
            new JanitorSkillTier{Lvl=53, Exp=81851, Rapidity=6.083333f, Dexterity=330},
            new JanitorSkillTier{Lvl=54, Exp=89376, Rapidity=6.166667f, Dexterity=340},
            new JanitorSkillTier{Lvl=55, Exp=97606, Rapidity=6.250000f, Dexterity=355},
            new JanitorSkillTier{Lvl=56, Exp=106611, Rapidity=6.333333f, Dexterity=370},
            new JanitorSkillTier{Lvl=57, Exp=116471, Rapidity=6.444444f, Dexterity=385},
            new JanitorSkillTier{Lvl=58, Exp=127266, Rapidity=6.555556f, Dexterity=400},
            new JanitorSkillTier{Lvl=59, Exp=139086, Rapidity=6.666667f, Dexterity=415},
            new JanitorSkillTier{Lvl=60, Exp=152021, Rapidity=6.777778f, Dexterity=430},
            new JanitorSkillTier{Lvl=61, Exp=166171, Rapidity=6.888889f, Dexterity=445},
            new JanitorSkillTier{Lvl=62, Exp=181636, Rapidity=7.000000f, Dexterity=460},
            new JanitorSkillTier{Lvl=63, Exp=198526, Rapidity=7.111111f, Dexterity=475},
            new JanitorSkillTier{Lvl=64, Exp=216951, Rapidity=7.222222f, Dexterity=490},
            new JanitorSkillTier{Lvl=65, Exp=237031, Rapidity=7.361111f, Dexterity=510},
            new JanitorSkillTier{Lvl=66, Exp=258886, Rapidity=7.500000f, Dexterity=530},
            new JanitorSkillTier{Lvl=67, Exp=282656, Rapidity=7.638889f, Dexterity=550},
            new JanitorSkillTier{Lvl=68, Exp=308481, Rapidity=7.777778f, Dexterity=570},
            new JanitorSkillTier{Lvl=69, Exp=336501, Rapidity=7.916667f, Dexterity=590},
            new JanitorSkillTier{Lvl=70, Exp=366876, Rapidity=8.055556f, Dexterity=610},
            new JanitorSkillTier{Lvl=71, Exp=399766, Rapidity=8.194444f, Dexterity=630},
            new JanitorSkillTier{Lvl=72, Exp=435331, Rapidity=8.333333f, Dexterity=650},
            new JanitorSkillTier{Lvl=73, Exp=473751, Rapidity=8.472222f, Dexterity=670},
            new JanitorSkillTier{Lvl=74, Exp=515206, Rapidity=8.611111f, Dexterity=690},
            new JanitorSkillTier{Lvl=75, Exp=595206, Rapidity=10.000000f, Dexterity=800},
            new JanitorSkillTier{Lvl=76, Exp=624041, Rapidity=10.111111f, Dexterity=820},
            new JanitorSkillTier{Lvl=77, Exp=655311, Rapidity=10.222222f, Dexterity=840},
            new JanitorSkillTier{Lvl=78, Exp=689516, Rapidity=10.333333f, Dexterity=860},
            new JanitorSkillTier{Lvl=79, Exp=727406, Rapidity=10.444444f, Dexterity=880},
            new JanitorSkillTier{Lvl=80, Exp=769981, Rapidity=10.555556f, Dexterity=905},
            new JanitorSkillTier{Lvl=81, Exp=818491, Rapidity=10.666667f, Dexterity=930},
            new JanitorSkillTier{Lvl=82, Exp=874436, Rapidity=10.777778f, Dexterity=955},
            new JanitorSkillTier{Lvl=83, Exp=939816, Rapidity=10.888889f, Dexterity=980},
            new JanitorSkillTier{Lvl=84, Exp=1017131, Rapidity=11.000000f, Dexterity=1005},
            new JanitorSkillTier{Lvl=85, Exp=1109381, Rapidity=11.111111f, Dexterity=1030},
            new JanitorSkillTier{Lvl=86, Exp=1220066, Rapidity=11.277778f, Dexterity=1055},
            new JanitorSkillTier{Lvl=87, Exp=1353186, Rapidity=11.444444f, Dexterity=1080},
            new JanitorSkillTier{Lvl=88, Exp=1513741, Rapidity=11.611111f, Dexterity=1105},
            new JanitorSkillTier{Lvl=89, Exp=1707731, Rapidity=11.777778f, Dexterity=1130},
            new JanitorSkillTier{Lvl=90, Exp=1943156, Rapidity=11.944444f, Dexterity=1160},
            new JanitorSkillTier{Lvl=91, Exp=2230016, Rapidity=12.166667f, Dexterity=1190},
            new JanitorSkillTier{Lvl=92, Exp=2580311, Rapidity=12.388889f, Dexterity=1220},
            new JanitorSkillTier{Lvl=93, Exp=3009041, Rapidity=12.611111f, Dexterity=1250},
            new JanitorSkillTier{Lvl=94, Exp=3536206, Rapidity=12.833333f, Dexterity=1280},
            new JanitorSkillTier{Lvl=95, Exp=4191806, Rapidity=13.055556f, Dexterity=1320},
            new JanitorSkillTier{Lvl=96, Exp=5025841, Rapidity=13.333333f, Dexterity=1360},
            new JanitorSkillTier{Lvl=97, Exp=6118311, Rapidity=13.611111f, Dexterity=1400},
            new JanitorSkillTier{Lvl=98, Exp=7589216, Rapidity=13.888889f, Dexterity=1450},
            new JanitorSkillTier{Lvl=99, Exp=9608556, Rapidity=14.166667f, Dexterity=1500},
            new JanitorSkillTier{Lvl=100, Exp=12406331, Rapidity=14.444444f, Dexterity=1600},
        };

        public int CurrentBoostLevel { get => fldCurrentBoostLevel.Value; set => fldCurrentBoostLevel.Value = value; }
        private readonly PrivateFld<int> fldCurrentBoostLevel = new PrivateFld<int>(typeof(Janitor), "m_CurrentBoostLevel");

        public List<float> WalkingSpeeds { get => fldWalkingSpeeds.Value; set => fldWalkingSpeeds.Value = value; }
        private readonly PrivateFld<List<float>> fldWalkingSpeeds = new PrivateFld<List<float>>(typeof(Janitor), "m_JanitorWalkingSpeeds");

        public JanitorSkill(JanitorSkillData data) : base(data)
        {
        }

        public JanitorLogic Logic { get; set; }

        public override Janitor Employee {
            get => base.Employee;
            set
            {
                base.Employee = value;
                fldCurrentBoostLevel.Instance = Employee;
                fldWalkingSpeeds.Instance = Employee;
            }
        }
        public override string JobName => "Janitor";

        public float Rapidity => Tier.Rapidity * 3.6f;
        public int Dexterity => Tier.Dexterity;
        public float CleaningDuration => 3f / (Tier.Dexterity / 100f); // [s] Vanilla: 3
        public float AgentSpeed => Tier.Rapidity; // [m/s], Vanilla: 2 .. max: 10
        public float AgentAngularSpeed => Math.Max(0, Tier.Rapidity - 2f) * 240; // [degree/s] Vanilla: 0 .. max: 1200
        public float AgentAcceleration => Tier.Rapidity < 2 ? Tier.Rapidity * 4 : 8 + (Tier.Rapidity - 2) * 6; // [m/s^2] Vanilla: 8 .. max: 60
        public float TurningSpeed => 5f * Tier.Rapidity; // Vanilla: 5

        internal override JanitorSkillTier[] SkillTable => SKILL_TABLE;
        protected override float CostRateToLevelUp => 5f;

        public override float Wage => WAGE[Grade];
        public override float GetWage(Grade g) => WAGE[g];
        public override float HiringCost => HIRING_COST[Grade] + EXTRA_HIRING_COST[this.Id - 1];
        internal override void ApplyWageToGame(float dailyWage, float hiringCost)
        {
            Singleton<IDManager>.Instance.JanitorSO(Id).DailyWage = dailyWage;
            Singleton<IDManager>.Instance.JanitorSO(Id).HiringCost = hiringCost;
        }

        public override void Setup()
        {
            InitialWage = Singleton<IDManager>.Instance.JanitorSO(Id).DailyWage;
            InitialHiringCost = Singleton<IDManager>.Instance.JanitorSO(Id).HiringCost;
            UpdateStatus(true);

            OnLevelChanged += lvlup => JanitorLogic.ApplyRapidity(Employee);
            // Plugin.LogDebug(this);
        }

        public override void Despawn()
        {
            Employee = null;
        }

    }

    [Serializable]
    public class JanitorSkillData : SkillData<JanitorSkill>
    {
        public JanitorSkillData() : base()
        {
            Skill = new JanitorSkill(this);
        }
    }

    public struct JanitorSkillTier : ISkillTier
    {
        public int Lvl { get; set; }
        public int Exp { get; set; }
        public float Rapidity { get; set; }
        public int Dexterity { get; set; }
    }


    public class EmplJanitor : Employee<Janitor>
    {
        public override int ID {
            get => Instance.JanitorID;
        }
    }
}