using System.Collections.Generic;
using System.Linq;
using System;
using MyBox;
using UnityEngine;

namespace EmployeeTraining
{
    public abstract class EmployeeSkillManager<S, ST, D, E, T>
            where S : EmployeeSkill<S, ST, E, T>
            where ST : ISkillTier
            where D : SkillData<S>
            where E : Employee<T>, new()
            where T : MonoBehaviour
    {

        internal EmployeeSkillManager()
        {
        }

        internal abstract List<D> TrainingData { get; }

        public virtual void Hire(int id)
        {
            // Logger.LogDebug($"Hired a cashier: id={id}");
            // Called in CashierManager.GetSkill()
            // CashierSkillManager.Instance.RegisterCashier(id);
        }

        public virtual void Fire(int id)
        {
            Plugin.LogInfo($"Firing {typeof(T).Name}[{id}]");
            var data = this.TrainingData.First(c => c.Id == id);
            if (data != null)
            {
                var skill = data.Skill;
                GameObject.Destroy(skill.ExpGaugeObj);
                Singleton<PCTrainingApp>.Instance.DeleteEmployee(skill);
                skill.OnFired();
            }
        }

        public virtual T Spawn(List<T> employees, int employeeID)
        {
            Plugin.LogInfo($"Spawned a {typeof(T)}: id={employeeID}");
            // Plugin.LogDebug($"Stack trace:\n{Environment.StackTrace}");

            T employee = employees.Last(c => GetId(c) == employeeID);
            S skill = this.GetOrAssignSkill(employee);
            this.GenerateSkillIndiactor(skill);
            return employee;
        }

        public virtual void Despawn(T employee)
        {
            Plugin.LogInfo($"Despawned {typeof(T)}[{GetId(employee)}]");
            this.GetSkill(employee)?.Despawn();
        }

        public virtual void Clear()
        {
            Plugin.LogInfo($"SkillManager Clearing training data");
            this.TrainingData.Clear();
        }


        public abstract S Register(int id);

        public S GetSkill(T employee) 
        {
            return this.TrainingData.FirstOrDefault(
                c => c.Skill.Employee != null && c.Skill.Employee as object == employee as object)?.Skill;
        }

        public S GetOrAssignSkill(T employee) 
        {
            return this.GetSkill(employee) ?? this.AssignSkill(employee);
        }

        public S AssignSkill(T employee)
        {
            int id = this.GetId(employee);
            S skill = this.TrainingData.FirstOrDefault(c => !c.Skill.IsAssigned() && c.Id == id)?.Skill;
            if (skill == null) {
                skill = this.Register(id);
            }
            else if (skill.TrainingStatusPanelObj == null)
            {
                Singleton<PCTrainingApp>.Instance.RegisterEmployee(skill);
            }
            if (skill.Employee == null)
            {
                skill.Employee = employee;
            }
            Plugin.LogInfo($"{typeof(T).Name} {id} is assigned to the skill data: {skill}");
            // foreach (CashierSkillData d in CTSaveManager.Data.Skills)
            // {
            //     Plugin.LogDebug($"Skill[{d.Skill.GetHashCode(),12}] => cashierID={d.Skill.Cashier?.CashierID}, instanceID={d.Skill.Cashier?.GetInstanceID()}, hashcode={d.Skill.Cashier?.GetHashCode()}]");
            // }
            return skill;
        }

        public IEnumerable<S> GetSkills()
        {
            return this.TrainingData.Select(c => c.Skill).AsEnumerable();
        }

        public abstract int GetId(T employee);

        public void GenerateSkillIndiactor(S skill)
        {
            var employee = skill.Employee;
            if (employee.GetComponentInChildren<SkillIndicator>() == null)
            {
                Plugin.LogInfo($"Adding Skill Indicator for {typeof(T)} {this.GetId(employee)}");
                var displayObj = UnityEngine.Object.Instantiate(SkillIndicatorGenerator.SkillIndicatorTmpl, employee.transform, false);
                SkillIndicator display = displayObj.GetComponent<SkillIndicator>();
                display.transform.localPosition = new Vector3(0, Plugin.Instance.Settings.GaugeHeight, 0);
                display.gameObject.SetActive(true);
                display.SetUp(skill, Plugin.Localizer);
                skill.ExpGaugeObj = displayObj;
                displayObj.SetActive(skill.IsGaugeDisplayed);
            }
        }
    }
}