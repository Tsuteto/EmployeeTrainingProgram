using System;
using System.Collections.Generic;
using EmployeeTraining.Employee;
using MyBox;

namespace EmployeeTraining.EmployeeSecurity
{
    public class SecuritySkill : EmployeeSkill<SecuritySkill, SecuritySkillTier, EmplSecurity, SecurityGuard>
    {
        private static readonly Dictionary<Grade, float> WAGE = new Dictionary<Grade, float>(){
            {Grade.Rookie, 120f},
            {Grade.Middle, 140f},
            {Grade.Adv, 180f},
            {Grade.Pro, 240f},
            {Grade.Ninja, 320f}
        };

        private static readonly Dictionary<Grade, float> HIRING_COST = new Dictionary<Grade, float>(){
            {Grade.Rookie, 200f},
            {Grade.Middle, 250f},
            {Grade.Adv, 320f},
            {Grade.Pro, 400f},
            {Grade.Ninja, 500f}
        };

        private static readonly float[] EXTRA_HIRING_COST = {0f, 100f};

        private static readonly SecuritySkillTier[] SKILL_TABLE = {
            new SecuritySkillTier{Lvl=1, Exp=0, Rapidity=1.388889f, Dexterity=70},
            new SecuritySkillTier{Lvl=2, Exp=10, Rapidity=1.444444f, Dexterity=73},
            new SecuritySkillTier{Lvl=3, Exp=22, Rapidity=1.500000f, Dexterity=76},
            new SecuritySkillTier{Lvl=4, Exp=36, Rapidity=1.555556f, Dexterity=79},
            new SecuritySkillTier{Lvl=5, Exp=52, Rapidity=1.638889f, Dexterity=82},
            new SecuritySkillTier{Lvl=6, Exp=72, Rapidity=1.722222f, Dexterity=85},
            new SecuritySkillTier{Lvl=7, Exp=96, Rapidity=1.805556f, Dexterity=88},
            new SecuritySkillTier{Lvl=8, Exp=126, Rapidity=1.888889f, Dexterity=91},
            new SecuritySkillTier{Lvl=9, Exp=162, Rapidity=2.000000f, Dexterity=100},
            new SecuritySkillTier{Lvl=10, Exp=242, Rapidity=2.222222f, Dexterity=100},
            new SecuritySkillTier{Lvl=11, Exp=274, Rapidity=2.277778f, Dexterity=100},
            new SecuritySkillTier{Lvl=12, Exp=316, Rapidity=2.333333f, Dexterity=100},
            new SecuritySkillTier{Lvl=13, Exp=371, Rapidity=2.388889f, Dexterity=100},
            new SecuritySkillTier{Lvl=14, Exp=439, Rapidity=2.444444f, Dexterity=100},
            new SecuritySkillTier{Lvl=15, Exp=523, Rapidity=2.500000f, Dexterity=100},
            new SecuritySkillTier{Lvl=16, Exp=623, Rapidity=2.583333f, Dexterity=100},
            new SecuritySkillTier{Lvl=17, Exp=742, Rapidity=2.666667f, Dexterity=100},
            new SecuritySkillTier{Lvl=18, Exp=883, Rapidity=2.750000f, Dexterity=100},
            new SecuritySkillTier{Lvl=19, Exp=1049, Rapidity=2.833333f, Dexterity=100},
            new SecuritySkillTier{Lvl=20, Exp=1246, Rapidity=2.916667f, Dexterity=100},
            new SecuritySkillTier{Lvl=21, Exp=1480, Rapidity=3.000000f, Dexterity=100},
            new SecuritySkillTier{Lvl=22, Exp=1757, Rapidity=3.111111f, Dexterity=100},
            new SecuritySkillTier{Lvl=23, Exp=2086, Rapidity=3.222222f, Dexterity=100},
            new SecuritySkillTier{Lvl=24, Exp=2476, Rapidity=3.333333f, Dexterity=100},
            new SecuritySkillTier{Lvl=25, Exp=3276, Rapidity=3.472222f, Dexterity=100},
            new SecuritySkillTier{Lvl=26, Exp=3496, Rapidity=3.527778f, Dexterity=103},
            new SecuritySkillTier{Lvl=27, Exp=3751, Rapidity=3.583333f, Dexterity=106},
            new SecuritySkillTier{Lvl=28, Exp=4056, Rapidity=3.638889f, Dexterity=109},
            new SecuritySkillTier{Lvl=29, Exp=4426, Rapidity=3.694444f, Dexterity=112},
            new SecuritySkillTier{Lvl=30, Exp=4876, Rapidity=3.777778f, Dexterity=117},
            new SecuritySkillTier{Lvl=31, Exp=5421, Rapidity=3.861111f, Dexterity=122},
            new SecuritySkillTier{Lvl=32, Exp=6081, Rapidity=3.944444f, Dexterity=127},
            new SecuritySkillTier{Lvl=33, Exp=6876, Rapidity=4.027778f, Dexterity=132},
            new SecuritySkillTier{Lvl=34, Exp=7826, Rapidity=4.111111f, Dexterity=137},
            new SecuritySkillTier{Lvl=35, Exp=8951, Rapidity=4.222222f, Dexterity=144},
            new SecuritySkillTier{Lvl=36, Exp=10276, Rapidity=4.333333f, Dexterity=151},
            new SecuritySkillTier{Lvl=37, Exp=11826, Rapidity=4.444444f, Dexterity=158},
            new SecuritySkillTier{Lvl=38, Exp=13626, Rapidity=4.555556f, Dexterity=165},
            new SecuritySkillTier{Lvl=39, Exp=15701, Rapidity=4.666667f, Dexterity=172},
            new SecuritySkillTier{Lvl=40, Exp=18081, Rapidity=4.805556f, Dexterity=181},
            new SecuritySkillTier{Lvl=41, Exp=20796, Rapidity=4.944444f, Dexterity=190},
            new SecuritySkillTier{Lvl=42, Exp=23876, Rapidity=5.083333f, Dexterity=199},
            new SecuritySkillTier{Lvl=43, Exp=27361, Rapidity=5.222222f, Dexterity=208},
            new SecuritySkillTier{Lvl=44, Exp=31291, Rapidity=5.361111f, Dexterity=217},
            new SecuritySkillTier{Lvl=45, Exp=39291, Rapidity=5.555556f, Dexterity=250},
            new SecuritySkillTier{Lvl=46, Exp=43426, Rapidity=5.611111f, Dexterity=260},
            new SecuritySkillTier{Lvl=47, Exp=47796, Rapidity=5.666667f, Dexterity=270},
            new SecuritySkillTier{Lvl=48, Exp=52451, Rapidity=5.722222f, Dexterity=280},
            new SecuritySkillTier{Lvl=49, Exp=57441, Rapidity=5.777778f, Dexterity=290},
            new SecuritySkillTier{Lvl=50, Exp=62816, Rapidity=5.833333f, Dexterity=300},
            new SecuritySkillTier{Lvl=51, Exp=68636, Rapidity=5.916667f, Dexterity=310},
            new SecuritySkillTier{Lvl=52, Exp=74961, Rapidity=6.000000f, Dexterity=320},
            new SecuritySkillTier{Lvl=53, Exp=81851, Rapidity=6.083333f, Dexterity=330},
            new SecuritySkillTier{Lvl=54, Exp=89376, Rapidity=6.166667f, Dexterity=340},
            new SecuritySkillTier{Lvl=55, Exp=97606, Rapidity=6.250000f, Dexterity=355},
            new SecuritySkillTier{Lvl=56, Exp=106611, Rapidity=6.333333f, Dexterity=370},
            new SecuritySkillTier{Lvl=57, Exp=116471, Rapidity=6.444444f, Dexterity=385},
            new SecuritySkillTier{Lvl=58, Exp=127266, Rapidity=6.555556f, Dexterity=400},
            new SecuritySkillTier{Lvl=59, Exp=139086, Rapidity=6.666667f, Dexterity=415},
            new SecuritySkillTier{Lvl=60, Exp=152021, Rapidity=6.777778f, Dexterity=430},
            new SecuritySkillTier{Lvl=61, Exp=166171, Rapidity=6.888889f, Dexterity=445},
            new SecuritySkillTier{Lvl=62, Exp=181636, Rapidity=7.000000f, Dexterity=460},
            new SecuritySkillTier{Lvl=63, Exp=198526, Rapidity=7.111111f, Dexterity=475},
            new SecuritySkillTier{Lvl=64, Exp=216951, Rapidity=7.222222f, Dexterity=490},
            new SecuritySkillTier{Lvl=65, Exp=237031, Rapidity=7.361111f, Dexterity=510},
            new SecuritySkillTier{Lvl=66, Exp=258886, Rapidity=7.500000f, Dexterity=530},
            new SecuritySkillTier{Lvl=67, Exp=282656, Rapidity=7.638889f, Dexterity=550},
            new SecuritySkillTier{Lvl=68, Exp=308481, Rapidity=7.777778f, Dexterity=570},
            new SecuritySkillTier{Lvl=69, Exp=336501, Rapidity=7.916667f, Dexterity=590},
            new SecuritySkillTier{Lvl=70, Exp=366876, Rapidity=8.055556f, Dexterity=610},
            new SecuritySkillTier{Lvl=71, Exp=399766, Rapidity=8.194444f, Dexterity=630},
            new SecuritySkillTier{Lvl=72, Exp=435331, Rapidity=8.333333f, Dexterity=650},
            new SecuritySkillTier{Lvl=73, Exp=473751, Rapidity=8.472222f, Dexterity=670},
            new SecuritySkillTier{Lvl=74, Exp=515206, Rapidity=8.611111f, Dexterity=690},
            new SecuritySkillTier{Lvl=75, Exp=595206, Rapidity=10.000000f, Dexterity=800},
            new SecuritySkillTier{Lvl=76, Exp=624041, Rapidity=10.111111f, Dexterity=820},
            new SecuritySkillTier{Lvl=77, Exp=655311, Rapidity=10.222222f, Dexterity=840},
            new SecuritySkillTier{Lvl=78, Exp=689516, Rapidity=10.333333f, Dexterity=860},
            new SecuritySkillTier{Lvl=79, Exp=727406, Rapidity=10.444444f, Dexterity=880},
            new SecuritySkillTier{Lvl=80, Exp=769981, Rapidity=10.555556f, Dexterity=905},
            new SecuritySkillTier{Lvl=81, Exp=818491, Rapidity=10.666667f, Dexterity=930},
            new SecuritySkillTier{Lvl=82, Exp=874436, Rapidity=10.777778f, Dexterity=955},
            new SecuritySkillTier{Lvl=83, Exp=939816, Rapidity=10.888889f, Dexterity=980},
            new SecuritySkillTier{Lvl=84, Exp=1017131, Rapidity=11.000000f, Dexterity=1005},
            new SecuritySkillTier{Lvl=85, Exp=1109381, Rapidity=11.111111f, Dexterity=1030},
            new SecuritySkillTier{Lvl=86, Exp=1220066, Rapidity=11.277778f, Dexterity=1055},
            new SecuritySkillTier{Lvl=87, Exp=1353186, Rapidity=11.444444f, Dexterity=1080},
            new SecuritySkillTier{Lvl=88, Exp=1513741, Rapidity=11.611111f, Dexterity=1105},
            new SecuritySkillTier{Lvl=89, Exp=1707731, Rapidity=11.777778f, Dexterity=1130},
            new SecuritySkillTier{Lvl=90, Exp=1943156, Rapidity=11.944444f, Dexterity=1160},
            new SecuritySkillTier{Lvl=91, Exp=2230016, Rapidity=12.166667f, Dexterity=1190},
            new SecuritySkillTier{Lvl=92, Exp=2580311, Rapidity=12.388889f, Dexterity=1220},
            new SecuritySkillTier{Lvl=93, Exp=3009041, Rapidity=12.611111f, Dexterity=1250},
            new SecuritySkillTier{Lvl=94, Exp=3536206, Rapidity=12.833333f, Dexterity=1280},
            new SecuritySkillTier{Lvl=95, Exp=4191806, Rapidity=13.055556f, Dexterity=1320},
            new SecuritySkillTier{Lvl=96, Exp=5025841, Rapidity=13.333333f, Dexterity=1360},
            new SecuritySkillTier{Lvl=97, Exp=6118311, Rapidity=13.611111f, Dexterity=1400},
            new SecuritySkillTier{Lvl=98, Exp=7589216, Rapidity=13.888889f, Dexterity=1450},
            new SecuritySkillTier{Lvl=99, Exp=9608556, Rapidity=14.166667f, Dexterity=1500},
            new SecuritySkillTier{Lvl=100, Exp=12406331, Rapidity=14.444444f, Dexterity=1600},
        };

        // public int CurrentBoostLevel { get => fldCurrentBoostLevel.Value; set => fldCurrentBoostLevel.Value = value; }
        // private readonly PrivateFld<int> fldCurrentBoostLevel = new PrivateFld<int>(typeof(SecurityGuard), "m_CurrentBoostLevel");

        public List<float> RunningSpeeds { get => fldRunnigSpeeds.Value; set => fldRunnigSpeeds.Value = value; }
        private readonly PrivateFld<List<float>> fldRunnigSpeeds = new PrivateFld<List<float>>(typeof(SecurityGuardAnimationController), "m_RunnigSpeeds");

        public SecuritySkill(SecuritySkillData data) : base(data)
        {
        }

        public SecurityLogic Logic { get; set; }

        public override SecurityGuard Employee {
            get => base.Employee;
            set
            {
                base.Employee = value;
                if (value != null)
                {
                    // fldCurrentBoostLevel.Instance = Employee;
                    fldRunnigSpeeds.Instance = Employee.Controller;
                }
            }
        }
        public override string JobName => "Security Guard";

        public float Rapidity => Tier.Rapidity * 3.6f;
        public int Dexterity => Tier.Dexterity;
        public float AgentSpeed => Tier.Rapidity; // [m/s], Vanilla: 2 .. max: 10
        public float AgentAngularSpeed => Math.Max(0, Tier.Rapidity - 2f) * 240; // [degree/s] Vanilla: 0 .. max: 1200
        public float AgentAcceleration => Tier.Rapidity < 2 ? Tier.Rapidity * 4 : 8 + (Tier.Rapidity - 2) * 6; // [m/s^2] Vanilla: 8 .. max: 60
        public float ProductPlacingIntv => 0.2f / (Tier.Dexterity / 100f);
        public float TurningSpeed => 5f * Tier.Rapidity; // Vanilla: 5
        public float RotationTime => 0.4f / (Tier.Dexterity / 100f);
        public float CrateOpeningTime => 1f / (Tier.Dexterity / 100f);
        public float CollectingIntv => 0.3f / (Tier.Dexterity / 100f);

        internal override SecuritySkillTier[] SkillTable => SKILL_TABLE;
        protected override float CostRateToLevelUp => 5f;

        public override float Wage => WAGE[Grade];
        public override float HiringCost => HIRING_COST[Grade] + EXTRA_HIRING_COST[this.Id - 1];
        public override float GetWage(Grade g) => WAGE[g];
        internal override void ApplyWageToGame(float dailyWage, float hiringCost)
        {
            Singleton<IDManager>.Instance.SecurityGuardSO(Id).DailyWage = dailyWage;
            Singleton<IDManager>.Instance.SecurityGuardSO(Id).HiringCost = hiringCost;
        }

        public override void Setup()
        {
            InitialWage = Singleton<IDManager>.Instance.SecurityGuardSO(Id).DailyWage;
            InitialHiringCost = Singleton<IDManager>.Instance.SecurityGuardSO(Id).HiringCost;
            UpdateStatus(true);

            // OnLevelChanged += lvlup => SecurityLogic.SetSpeed(Employee.Controller, 0);
            // Plugin.LogDebug(this);
        }

        public override void Despawn()
        {
            Employee = null;
        }

    }

    [Serializable]
    public class SecuritySkillData : SkillData<SecuritySkill>
    {
        public SecuritySkillData() : base()
        {
            Skill = new SecuritySkill(this);
        }
    }

    public struct SecuritySkillTier : ISkillTier
    {
        public int Lvl { get; set; }
        public int Exp { get; set; }
        public float Rapidity { get; set; }
        public int Dexterity { get; set; }
    }


    public class EmplSecurity : Employee<SecurityGuard>
    {
        public override int ID {
            get => Instance.ID;
        }
    }
}