using System;
using System.Linq;
using UnityEngine;

namespace EmployeeTraining.Employee
{
    public interface IEmployeeSkill
    {
        int Id { get; }
        string JobName { get; }
        int Exp { get; }
        int TotalExp { get; }
        int Lvl { get; }
        Grade Grade { get; }
        float Wage { get; }

        GameObject TrainingStatusPanelObj { get; set; }
        GameObject ExpGaugeObj { get; set; }
        bool IsGaugeDisplayed { get; set; }

        Action<int, bool> OnExpChanged { get; set; }
        Action<bool> OnLevelChanged { get; set; }

        bool IsAssigned();
        void AddExp(int exp);
        void UpdateStatus(bool init);
        void UnlockGrade();
        int? GetExpForNext();
        int? GetTotalExpForNext();
        bool IsUnlockNeeded();
        float? GetCostToLevelup();
        float? GetCostToUpgrade();
        string GetExpDisplay();
        void TrainToLevelup();

        void Setup();

        ExpRange GetExpRangeOfGrade(Grade g);
    }

    public abstract class EmployeeSkill<S, ST, E, T> : IEmployeeSkill
            where S : IEmployeeSkill
            where ST : ISkillTier
            where E : Employee<T>, new()
            where T : MonoBehaviour
    {
        public virtual T Employee {
            get {
                return employee.Instance;
            }
            set {
                employee.Instance = value;
            }
        }
        private readonly E employee = new E();

        public int Id {
            get {
                return data.Id;
            }
        }
        public virtual string JobName => Employee.GetType().Name;

        public GameObject ExpGaugeObj { get; set; }
        public GameObject TrainingStatusPanelObj { get; set; }
        public Action<int, bool> OnExpChanged { get; set; }
        public Action<bool> OnLevelChanged { get; set; }

        public bool IsAssigned()
        {
            return Employee != null;
        }

        internal abstract ST[] SkillTable { get; }

        public int Exp
        {
            get {
                return TotalExp - Tier.Exp;
            }
        }

        public int TotalExp {
            get {
                return data.Exp;
            }
            private set {
                data.Exp = value;
                UpdateStatus(false);
            }
        }
        public Grade Grade {
            get {
                return Grade.List[data.Grade];
            }
            set {
                data.Grade = value.Order;
            }
        }
        public bool IsGaugeDisplayed {
            get {
                return data.IsGaugeDisplayed;
            }
            set {
                data.IsGaugeDisplayed = value;
            }
        }

        internal readonly SkillData<S> data;

        protected EmployeeSkill(SkillData<S> data)
        {
            this.data = data;
            Tier = SkillTable[0];
        }

        internal ST Tier { get; set; }

        internal abstract void ApplyWageToGame(float dailyWage, float hiringCost);

        public abstract float Wage { get; }
        public float InitialWage { get; internal set; }
        public abstract float HiringCost { get; }
        public float InitialHiringCost { get; internal set; }

        public int Lvl {
            get {
                return Tier.Lvl;
            }
        }

        public abstract void Setup();

        public void AddExp(int exp)
        {
            TotalExp += exp;
            // Plugin.LogDebug($"{typeof(T)}[{this.Id}] is now {this.TotalExp}exp");
            OnExpChanged?.Invoke(exp, true);
        }

        public void UpdateStatus(bool init = false)
        {
            // Never use TotalExp setter within UpdateStatus!
            bool leveledUp = UpdateLvl();

            if (!init && leveledUp)
            {
                // Plugin.LogDebug($"{typeof(T)}[{this.Id}] is now level {this.Tier.Lvl}");
                Grade = Grade.List.First(g => Lvl <= g.LvlMax);
                OnLevelChanged?.Invoke(true);
            }

            ApplyWageToGame(Wage, HiringCost);
        }

        private bool UpdateLvl()
        {
            int prevLvl = Lvl;
            if (Lvl >= Grade.LvlMax)
            {
                return false;
            }
            else
            {
                for (int i = prevLvl - 1; i < SkillTable.Length; i++)
                {
                    if (data.Exp < SkillTable[i].Exp) break;
                    Tier = SkillTable[i];
                }
                if (Lvl > Grade.LvlMax)
                {
                    Tier = SkillTable[Grade.LvlMax - 1];
                }
                return Lvl > prevLvl;
            }
        }

        public void UnlockGrade()
        {
            var nextGrade = Grade.List.FirstOrDefault(g => g.Order == Grade.Order + 1);
            if (nextGrade != null)
            {
                Grade = nextGrade;
                UpdateStatus();
                OnExpChanged?.Invoke(0, true);
                ApplyWageToGame(Wage, HiringCost);
            }
        }

        public int? GetExpForNext()
        {
            if (Tier.Lvl < SkillTable.Length)
            {
                return SkillTable[Tier.Lvl].Exp - Tier.Exp;
            }
            return null;
        }

        public int? GetTotalExpForNext()
        {
            if (Tier.Lvl < SkillTable.Length)
            {
                return SkillTable[Tier.Lvl].Exp;
            }
            return null;
        }

        public bool IsUnlockNeeded()
        {
            var expForNext = GetTotalExpForNext();
            if (expForNext != null)
            {
                return Lvl >= Grade.LvlMax && TotalExp >= expForNext;
            }
            return false;
        }

        public float? GetCostToUpgrade()
        {
            return Grade.Cost;
        }

        public string GetExpDisplay()
        {
            int? expForNext = GetExpForNext();
            if (expForNext != null)
            {
                return $"{Exp}<size=70%> / {expForNext}</size>";
            }
            else
            {
                return $"{TotalExp}";
            }
        }

        public ExpRange GetExpRangeOfGrade(Grade g)
        {
            return new ExpRange{
                Start=SkillTable[g.LvlMin - 1].Exp,
                End=SkillTable[Math.Min(g.LvlMax, SkillTable.Length - 1)].Exp - 1
            };
        }

        public override string ToString()
        {
            return $"{typeof(T)}[{Id}] exp={TotalExp}, lvl={Lvl}, grade={Grade.Name}";
        }

        public float? GetCostToLevelup()
        {
            int? expForNext = GetExpForNext();
            if (expForNext == null || Grade.Order > Grade.Adv.Order)
            {
                return null;
            }
            return (expForNext - Exp) * 2;
        }

        public void TrainToLevelup()
        {
            int? expForNext = GetExpForNext();
            if (expForNext != null)
            {
                AddExp(expForNext.Value - Exp);
            }
        }

        public void OnFired()
        {
            ExpGaugeObj = null;
            Employee = null;
        }

        public abstract void Despawn();

    }

    public abstract class Employee<T>
    {
        public T Instance;
        public abstract int ID { get; }
        
        public Employee()
        {
        }
    }

    public interface ISkillTier
    {
        int Lvl { get; set; }
        int Exp { get; set; }
    }

    public struct ExpRange
    {
        public int Start;
        public int End;
    }

}