using System.Collections.Generic;
using MyBox;

namespace EmployeeTraining
{
    public class CashierSkill : EmployeeSkill<CashierSkill, CashierSkillTier, EmployeeCashier, Cashier>
    {
        private static readonly CashierSkillTier[] SKILL_TABLE = {
            new CashierSkillTier{Lvl=1, Exp=0, IntervalMax=2.400000f, IntervalMin=2.000000f, Payment=4.2f},
            new CashierSkillTier{Lvl=2, Exp=50, IntervalMax=1.818182f, IntervalMin=1.500000f, Payment=4.2f},
            new CashierSkillTier{Lvl=3, Exp=110, IntervalMax=1.666667f, IntervalMin=1.333333f, Payment=4.2f},
            new CashierSkillTier{Lvl=4, Exp=180, IntervalMax=1.578947f, IntervalMin=1.200000f, Payment=4f},
            new CashierSkillTier{Lvl=5, Exp=260, IntervalMax=1.500000f, IntervalMin=1.153846f, Payment=4f},
            new CashierSkillTier{Lvl=6, Exp=360, IntervalMax=1.363636f, IntervalMin=1.111111f, Payment=3.7f},
            new CashierSkillTier{Lvl=7, Exp=480, IntervalMax=1.276596f, IntervalMin=1.071429f, Payment=3.7f},
            new CashierSkillTier{Lvl=8, Exp=620, IntervalMax=1.200000f, IntervalMin=1.034483f, Payment=3.5f},
            new CashierSkillTier{Lvl=9, Exp=780, IntervalMax=1.132075f, IntervalMin=1.000000f, Payment=3.5f},
            new CashierSkillTier{Lvl=10, Exp=1080, IntervalMax=1.090909f, IntervalMin=0.857143f, Payment=3f},
            new CashierSkillTier{Lvl=11, Exp=1280, IntervalMax=1.052632f, IntervalMin=0.833333f, Payment=3f},
            new CashierSkillTier{Lvl=12, Exp=1500, IntervalMax=0.983607f, IntervalMin=0.810811f, Payment=3f},
            new CashierSkillTier{Lvl=13, Exp=1745, IntervalMax=0.952381f, IntervalMin=0.789474f, Payment=2.9f},
            new CashierSkillTier{Lvl=14, Exp=2015, IntervalMax=0.902256f, IntervalMin=0.764331f, Payment=2.9f},
            new CashierSkillTier{Lvl=15, Exp=2315, IntervalMax=0.869565f, IntervalMin=0.740741f, Payment=2.9f},
            new CashierSkillTier{Lvl=16, Exp=2645, IntervalMax=0.816327f, IntervalMin=0.718563f, Payment=2.8f},
            new CashierSkillTier{Lvl=17, Exp=3010, IntervalMax=0.774194f, IntervalMin=0.693642f, Payment=2.8f},
            new CashierSkillTier{Lvl=18, Exp=3415, IntervalMax=0.727273f, IntervalMin=0.670391f, Payment=2.8f},
            new CashierSkillTier{Lvl=19, Exp=3865, IntervalMax=0.701754f, IntervalMin=0.648649f, Payment=2.7f},
            new CashierSkillTier{Lvl=20, Exp=4370, IntervalMax=0.674157f, IntervalMin=0.625000f, Payment=2.7f},
            new CashierSkillTier{Lvl=21, Exp=4940, IntervalMax=0.648649f, IntervalMin=0.603015f, Payment=2.7f},
            new CashierSkillTier{Lvl=22, Exp=5585, IntervalMax=0.625000f, IntervalMin=0.582524f, Payment=2.6f},
            new CashierSkillTier{Lvl=23, Exp=6320, IntervalMax=0.600000f, IntervalMin=0.560748f, Payment=2.6f},
            new CashierSkillTier{Lvl=24, Exp=7160, IntervalMax=0.576923f, IntervalMin=0.540541f, Payment=2.6f},
            new CashierSkillTier{Lvl=25, Exp=9160, IntervalMax=0.600000f, IntervalMin=0.500000f, Payment=2.5f},
            new CashierSkillTier{Lvl=26, Exp=10005, IntervalMax=0.591133f, IntervalMin=0.493827f, Payment=2.5f},
            new CashierSkillTier{Lvl=27, Exp=10895, IntervalMax=0.576923f, IntervalMin=0.487805f, Payment=2.5f},
            new CashierSkillTier{Lvl=28, Exp=11855, IntervalMax=0.566038f, IntervalMin=0.480000f, Payment=2.5f},
            new CashierSkillTier{Lvl=29, Exp=12910, IntervalMax=0.550459f, IntervalMin=0.472441f, Payment=2.4f},
            new CashierSkillTier{Lvl=30, Exp=14090, IntervalMax=0.540541f, IntervalMin=0.465116f, Payment=2.4f},
            new CashierSkillTier{Lvl=31, Exp=15425, IntervalMax=0.528634f, IntervalMin=0.456274f, Payment=2.4f},
            new CashierSkillTier{Lvl=32, Exp=16950, IntervalMax=0.512821f, IntervalMin=0.447761f, Payment=2.4f},
            new CashierSkillTier{Lvl=33, Exp=18700, IntervalMax=0.502092f, IntervalMin=0.439560f, Payment=2.3f},
            new CashierSkillTier{Lvl=34, Exp=20715, IntervalMax=0.485830f, IntervalMin=0.430108f, Payment=2.3f},
            new CashierSkillTier{Lvl=35, Exp=23035, IntervalMax=0.474308f, IntervalMin=0.421053f, Payment=2.3f},
            new CashierSkillTier{Lvl=36, Exp=25705, IntervalMax=0.458015f, IntervalMin=0.410959f, Payment=2.3f},
            new CashierSkillTier{Lvl=37, Exp=28770, IntervalMax=0.446097f, IntervalMin=0.401338f, Payment=2.2f},
            new CashierSkillTier{Lvl=38, Exp=32280, IntervalMax=0.431655f, IntervalMin=0.392157f, Payment=2.2f},
            new CashierSkillTier{Lvl=39, Exp=36285, IntervalMax=0.419580f, IntervalMin=0.382166f, Payment=2.2f},
            new CashierSkillTier{Lvl=40, Exp=40845, IntervalMax=0.405405f, IntervalMin=0.372671f, Payment=2.2f},
            new CashierSkillTier{Lvl=41, Exp=46020, IntervalMax=0.394737f, IntervalMin=0.363636f, Payment=2.1f},
            new CashierSkillTier{Lvl=42, Exp=51880, IntervalMax=0.379747f, IntervalMin=0.352941f, Payment=2.1f},
            new CashierSkillTier{Lvl=43, Exp=58495, IntervalMax=0.365854f, IntervalMin=0.342857f, Payment=2.1f},
            new CashierSkillTier{Lvl=44, Exp=65945, IntervalMax=0.352941f, IntervalMin=0.333333f, Payment=2.1f},
            new CashierSkillTier{Lvl=45, Exp=85945, IntervalMax=0.352941f, IntervalMin=0.300000f, Payment=1.8f},
            new CashierSkillTier{Lvl=46, Exp=88960, IntervalMax=0.346821f, IntervalMin=0.295567f, Payment=1.8f},
            new CashierSkillTier{Lvl=47, Exp=92390, IntervalMax=0.338983f, IntervalMin=0.291262f, Payment=1.75f},
            new CashierSkillTier{Lvl=48, Exp=96285, IntervalMax=0.333333f, IntervalMin=0.287081f, Payment=1.75f},
            new CashierSkillTier{Lvl=49, Exp=100695, IntervalMax=0.326087f, IntervalMin=0.283019f, Payment=1.7f},
            new CashierSkillTier{Lvl=50, Exp=105680, IntervalMax=0.319149f, IntervalMin=0.277778f, Payment=1.7f},
            new CashierSkillTier{Lvl=51, Exp=111300, IntervalMax=0.310881f, IntervalMin=0.272727f, Payment=1.65f},
            new CashierSkillTier{Lvl=52, Exp=117625, IntervalMax=0.304569f, IntervalMin=0.267857f, Payment=1.65f},
            new CashierSkillTier{Lvl=53, Exp=124725, IntervalMax=0.297030f, IntervalMin=0.263158f, Payment=1.6f},
            new CashierSkillTier{Lvl=54, Exp=132680, IntervalMax=0.289855f, IntervalMin=0.257511f, Payment=1.6f},
            new CashierSkillTier{Lvl=55, Exp=141570, IntervalMax=0.281690f, IntervalMin=0.252101f, Payment=1.55f},
            new CashierSkillTier{Lvl=56, Exp=151485, IntervalMax=0.275229f, IntervalMin=0.246914f, Payment=1.55f},
            new CashierSkillTier{Lvl=57, Exp=162515, IntervalMax=0.267857f, IntervalMin=0.241935f, Payment=1.5f},
            new CashierSkillTier{Lvl=58, Exp=174760, IntervalMax=0.260870f, IntervalMin=0.236220f, Payment=1.5f},
            new CashierSkillTier{Lvl=59, Exp=188320, IntervalMax=0.253165f, IntervalMin=0.230769f, Payment=1.45f},
            new CashierSkillTier{Lvl=60, Exp=203315, IntervalMax=0.246914f, IntervalMin=0.225564f, Payment=1.45f},
            new CashierSkillTier{Lvl=61, Exp=219865, IntervalMax=0.240000f, IntervalMin=0.220588f, Payment=1.4f},
            new CashierSkillTier{Lvl=62, Exp=238110, IntervalMax=0.233463f, IntervalMin=0.215054f, Payment=1.4f},
            new CashierSkillTier{Lvl=63, Exp=258190, IntervalMax=0.226415f, IntervalMin=0.209790f, Payment=1.35f},
            new CashierSkillTier{Lvl=64, Exp=280265, IntervalMax=0.220588f, IntervalMin=0.204778f, Payment=1.35f},
            new CashierSkillTier{Lvl=65, Exp=304495, IntervalMax=0.214286f, IntervalMin=0.200000f, Payment=1.3f},
            new CashierSkillTier{Lvl=66, Exp=331060, IntervalMax=0.208333f, IntervalMin=0.194805f, Payment=1.3f},
            new CashierSkillTier{Lvl=67, Exp=360140, IntervalMax=0.202020f, IntervalMin=0.189873f, Payment=1.25f},
            new CashierSkillTier{Lvl=68, Exp=391935, IntervalMax=0.196721f, IntervalMin=0.185185f, Payment=1.25f},
            new CashierSkillTier{Lvl=69, Exp=426645, IntervalMax=0.190476f, IntervalMin=0.180180f, Payment=1.2f},
            new CashierSkillTier{Lvl=70, Exp=464500, IntervalMax=0.185185f, IntervalMin=0.175439f, Payment=1.2f},
            new CashierSkillTier{Lvl=71, Exp=505730, IntervalMax=0.179641f, IntervalMin=0.170940f, Payment=1.1f},
            new CashierSkillTier{Lvl=72, Exp=550585, IntervalMax=0.174419f, IntervalMin=0.166205f, Payment=1.1f},
            new CashierSkillTier{Lvl=73, Exp=599315, IntervalMax=0.169014f, IntervalMin=0.161725f, Payment=1.0f},
            new CashierSkillTier{Lvl=74, Exp=652190, IntervalMax=0.164384f, IntervalMin=0.157480f, Payment=1.0f},
            new CashierSkillTier{Lvl=75, Exp=852190, IntervalMax=0.166667f, IntervalMin=0.142857f, Payment=0.8f},
            new CashierSkillTier{Lvl=76, Exp=952295, IntervalMax=0.158940f, IntervalMin=0.137931f, Payment=0.8f},
            new CashierSkillTier{Lvl=77, Exp=1055105, IntervalMax=0.151899f, IntervalMin=0.133333f, Payment=0.8f},
            new CashierSkillTier{Lvl=78, Exp=1161620, IntervalMax=0.145455f, IntervalMin=0.129032f, Payment=0.75f},
            new CashierSkillTier{Lvl=79, Exp=1273840, IntervalMax=0.140515f, IntervalMin=0.125786f, Payment=0.75f},
            new CashierSkillTier{Lvl=80, Exp=1394765, IntervalMax=0.135900f, IntervalMin=0.122699f, Payment=0.75f},
            new CashierSkillTier{Lvl=81, Exp=1528395, IntervalMax=0.131579f, IntervalMin=0.119760f, Payment=0.7f},
            new CashierSkillTier{Lvl=82, Exp=1679730, IntervalMax=0.128068f, IntervalMin=0.117417f, Payment=0.7f},
            new CashierSkillTier{Lvl=83, Exp=1856270, IntervalMax=0.124740f, IntervalMin=0.115163f, Payment=0.65f},
            new CashierSkillTier{Lvl=84, Exp=2068015, IntervalMax=0.122200f, IntervalMin=0.112994f, Payment=0.65f},
            new CashierSkillTier{Lvl=85, Exp=2327465, IntervalMax=0.118812f, IntervalMin=0.110497f, Payment=0.6f},
            new CashierSkillTier{Lvl=86, Exp=2649620, IntervalMax=0.116054f, IntervalMin=0.108108f, Payment=0.6f},
            new CashierSkillTier{Lvl=87, Exp=3051980, IntervalMax=0.113422f, IntervalMin=0.105820f, Payment=0.55f},
            new CashierSkillTier{Lvl=88, Exp=3554545, IntervalMax=0.111111f, IntervalMin=0.104167f, Payment=0.55f},
            new CashierSkillTier{Lvl=89, Exp=4197315, IntervalMax=0.109290f, IntervalMin=0.102564f, Payment=0.5f},
            new CashierSkillTier{Lvl=90, Exp=5040290, IntervalMax=0.107527f, IntervalMin=0.101010f, Payment=0.5f},
            new CashierSkillTier{Lvl=91, Exp=6163470, IntervalMax=0.105820f, IntervalMin=0.099834f, Payment=0.47f},
            new CashierSkillTier{Lvl=92, Exp=7666855, IntervalMax=0.104530f, IntervalMin=0.098684f, Payment=0.47f},
            new CashierSkillTier{Lvl=93, Exp=9670445, IntervalMax=0.103270f, IntervalMin=0.097561f, Payment=0.45f},
            new CashierSkillTier{Lvl=94, Exp=12324240, IntervalMax=0.102041f, IntervalMin=0.096774f, Payment=0.42f},
            new CashierSkillTier{Lvl=95, Exp=15808240, IntervalMax=0.101180f, IntervalMin=0.096000f, Payment=0.39f},
            new CashierSkillTier{Lvl=96, Exp=20342445, IntervalMax=0.100334f, IntervalMin=0.095238f, Payment=0.36f},
            new CashierSkillTier{Lvl=97, Exp=26196855, IntervalMax=0.099502f, IntervalMin=0.094787f, Payment=0.33f},
            new CashierSkillTier{Lvl=98, Exp=33701470, IntervalMax=0.099010f, IntervalMin=0.094340f, Payment=0.3f},
            new CashierSkillTier{Lvl=99, Exp=43256290, IntervalMax=0.098684f, IntervalMin=0.094044f, Payment=0.25f},
            new CashierSkillTier{Lvl=100, Exp=55361315, IntervalMax=0.097561f, IntervalMin=0.093750f, Payment=0.2f},
        };

        public int CurrentBoostLevel { get => this.fldCurrentBoostLevel.Value; set => this.fldCurrentBoostLevel.Value = value; }
        private readonly PrivateFld<int> fldCurrentBoostLevel = new PrivateFld<int>(typeof(Cashier), "m_CurrentBoostLevel");
        public List<float> CashierScanIntervals { get => this.fldCashierScanIntervals.Value; set => this.fldCashierScanIntervals.Value = value; }
        private readonly PrivateFld<List<float>> fldCashierScanIntervals = new PrivateFld<List<float>>(typeof(Cashier), "m_CashierScanIntervals");

        public CashierSkill(CashierSkillData data) : base(data)
        {
        }

        public override Cashier Employee {
            get => base.Employee;
            set
            {
                base.Employee = value;
                this.fldCurrentBoostLevel.Instance = this.Employee;
                this.fldCashierScanIntervals.Instance = this.Employee;
            }
        }

        public float IntervalMin => this.Tier.IntervalMin;
        public float IntervalMax => this.Tier.IntervalMax;
        public float OperationSpd => this.Tier.Payment;
        public float TotalCheckoutDuration => this.Tier.Payment * 17 / 15;

        internal override CashierSkillTier[] SkillTable => SKILL_TABLE;

        public override float Wage => this.Grade.WageCashier;
        public override float HiringCost => this.Grade.HiringCostCashier;
        internal override void ApplyWageToGame(float dailyWage, float hiringCost)
        {
            Singleton<IDManager>.Instance.CashierSO(this.Id).DailyWage = dailyWage;
            Singleton<IDManager>.Instance.CashierSO(this.Id).HiringCost = hiringCost;
        }

        public override void Setup()
        {
            this.InitialWage = Singleton<IDManager>.Instance.CashierSO(this.Id).DailyWage;
            this.InitialHiringCost = Singleton<IDManager>.Instance.CashierSO(this.Id).HiringCost;
            this.UpdateStatus(true);
            // Plugin.LogDebug(this);
        }

        public override void Despawn()
        {
            this.Employee = null;
        }

    }

    public struct CashierSkillTier : ISkillTier
    {
        public int Lvl { get; set; }
        public int Exp { get; set; }
        public float IntervalMin { get; set; }
        public float IntervalMax { get; set; }
        public float Payment { get; set; }
    }


    public class EmployeeCashier : Employee<Cashier>
    {
        public override int ID {
            get => this.Instance.CashierID;
        }
    }

    public struct PaymentDuration
    {
        public float IntervalAfterScanningAll;
        public float TakingPaymentInterval;
        public float FinishingPaymentDuration;
    }
}