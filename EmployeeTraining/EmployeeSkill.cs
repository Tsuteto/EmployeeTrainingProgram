using System;
using System.Linq;
using UnityEngine;

namespace EmployeeTraining
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
                return this.employee.Instance;
            }
            set {
                this.employee.Instance = value;
            }
        }
        private readonly E employee = new E();

        public int Id {
            get {
                return this.data.Id;
            }
        }
        public virtual string JobName => this.Employee.GetType().Name;

        public GameObject ExpGaugeObj { get; set; }
        public GameObject TrainingStatusPanelObj { get; set; }
        public Action<int, bool> OnExpChanged { get; set; }
        public Action<bool> OnLevelChanged { get; set; }

        public bool IsAssigned()
        {
            return this.Employee != null;
        }

        internal abstract ST[] SkillTable { get; }

        public int Exp
        {
            get {
                return this.TotalExp - this.Tier.Exp;
            }
        }

        public int TotalExp {
            get {
                return this.data.Exp;
            }
            private set {
                this.data.Exp = value;
                this.UpdateStatus(false);
            }
        }
        public Grade Grade {
            get {
                return Grade.List[this.data.Grade];
            }
            set {
                this.data.Grade = value.Order;
            }
        }
        public bool IsGaugeDisplayed {
            get {
                return this.data.IsGaugeDisplayed;
            }
            set {
                this.data.IsGaugeDisplayed = value;
            }
        }

        internal readonly SkillData<S> data;

        protected EmployeeSkill(SkillData<S> data)
        {
            this.data = data;
            this.Tier = this.SkillTable[0];
        }

        internal ST Tier { get; set; }

        internal abstract void ApplyWageToGame(float dailyWage, float hiringCost);

        public abstract float Wage { get; }
        public float InitialWage { get; internal set; }
        public abstract float HiringCost { get; }
        public float InitialHiringCost { get; internal set; }

        public int Lvl {
            get {
                return this.Tier.Lvl;
            }
        }

        public abstract void Setup();

        public void AddExp(int exp)
        {
            this.TotalExp += exp;
            // Plugin.LogDebug($"{typeof(T)}[{this.Id}] is now {this.TotalExp}exp");
            this.OnExpChanged?.Invoke(exp, true);
        }

        public void UpdateStatus(bool init = false)
        {
            // Never use TotalExp setter within UpdateStatus!
            bool leveledUp = this.UpdateLvl();

            if (!init && leveledUp)
            {
                // Plugin.LogDebug($"{typeof(T)}[{this.Id}] is now level {this.Tier.Lvl}");
                this.Grade = Grade.List.First(g => this.Lvl <= g.LvlMax);
                this.OnLevelChanged?.Invoke(true);
            }

            this.ApplyWageToGame(this.Wage, this.HiringCost);
        }

        private bool UpdateLvl()
        {
            int prevLvl = this.Lvl;
            if (this.Lvl >= this.Grade.LvlMax)
            {
                return false;
            }
            else
            {
                for (int i = prevLvl - 1; i < this.SkillTable.Length; i++)
                {
                    if (this.data.Exp < this.SkillTable[i].Exp) break;
                    this.Tier = this.SkillTable[i];
                }
                if (this.Lvl > this.Grade.LvlMax)
                {
                    this.Tier = this.SkillTable[this.Grade.LvlMax - 1];
                }
                return this.Lvl > prevLvl;
            }
        }

        public void UnlockGrade()
        {
            var nextGrade = Grade.List.FirstOrDefault(g => g.Order == this.Grade.Order + 1);
            if (nextGrade != null)
            {
                this.Grade = nextGrade;
                this.UpdateStatus();
                this.OnExpChanged?.Invoke(0, true);
                this.ApplyWageToGame(this.Wage, this.HiringCost);
            }
        }

        public int? GetExpForNext()
        {
            if (this.Tier.Lvl < SkillTable.Length)
            {
                return SkillTable[this.Tier.Lvl].Exp - this.Tier.Exp;
            }
            return null;
        }

        public int? GetTotalExpForNext()
        {
            if (this.Tier.Lvl < SkillTable.Length)
            {
                return SkillTable[this.Tier.Lvl].Exp;
            }
            return null;
        }

        public bool IsUnlockNeeded()
        {
            var expForNext = this.GetTotalExpForNext();
            if (expForNext != null)
            {
                return this.Lvl >= this.Grade.LvlMax && this.TotalExp >= expForNext;
            }
            return false;
        }

        public float? GetCostToUpgrade()
        {
            return this.Grade.Cost;
        }

        public string GetExpDisplay()
        {
            int? expForNext = this.GetExpForNext();
            if (expForNext != null)
            {
                return $"{this.Exp}<size=70%> / {expForNext}</size>";
            }
            else
            {
                return $"{this.TotalExp}";
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
            return $"{typeof(T)}[{this.Id}] exp={this.TotalExp}, lvl={this.Lvl}, grade={this.Grade.Name}";
        }

        public float? GetCostToLevelup()
        {
            int? expForNext = this.GetExpForNext();
            if (expForNext == null || this.Grade.Order > Grade.Adv.Order)
            {
                return null;
            }
            return (expForNext - this.Exp) * 2;
        }

        public void TrainToLevelup()
        {
            int? expForNext = this.GetExpForNext();
            if (expForNext != null)
            {
                this.AddExp(expForNext.Value - this.Exp);
            }
        }

        public void OnFired()
        {
            this.ExpGaugeObj = null;
            this.Employee = null;
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