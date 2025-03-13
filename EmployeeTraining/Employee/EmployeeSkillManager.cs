using System.Collections.Generic;
using System.Linq;
using System;
using MyBox;
using UnityEngine;
using EmployeeTraining.TrainingApp;

namespace EmployeeTraining.Employee
{
    public abstract class EmployeeSkillManager<S, ST, D, E, T>
            where S : EmployeeSkill<S, ST, E, T>
            where ST : ISkillTier
            where D : SkillData<S>, new()
            where E : Employee<T>, new()
            where T : MonoBehaviour
    {

        internal EmployeeSkillManager()
        {
            Plugin.Instance.GameQuitEvent += OnGameQuit;
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
            Plugin.LogDebug($"Firing {typeof(T).Name}[{id}]");
            var data = TrainingData.First(c => c.Id == id);
            if (data != null)
            {
                var skill = data.Skill;
                UnityEngine.Object.Destroy(skill.ExpGaugeObj);
                Singleton<PCTrainingApp>.Instance.DeleteEmployee(skill);
                skill.OnFired();
            }
        }

        public virtual T Spawn(List<T> employees, int employeeID)
        {
            Plugin.LogDebug($"Spawned {typeof(T).Name}[{employeeID}]");
            // Plugin.LogDebug($"Stack trace:\n{Environment.StackTrace}");

            T employee = employees.Last(c => GetId(c) == employeeID);
            S skill = GetOrAssignSkill(employee);
            GenerateSkillIndiactor(skill);
            return employee;
        }

        public virtual void Despawn(T employee)
        {
            Plugin.LogDebug($"Despawned {typeof(T).Name}[{GetId(employee)}]");
            GetSkill(employee)?.Despawn();
        }

        public virtual void OnGameQuit()
        {
            TrainingData.Clear();
            Plugin.LogInfo($"Cleared training data of {this.GetType().Name}");
        }

        public virtual S Register(int id)
        {
            D newData = new D(){Id = id};
            TrainingData.Add(newData);
            Singleton<PCTrainingApp>.Instance.RegisterEmployee(newData.Skill);
            return newData.Skill;
        }

        public S GetSkill(T employee) 
        {
            return TrainingData.FirstOrDefault(
                c => c.Skill.Employee != null && c.Skill.Employee as object == employee as object)?.Skill;
        }

        public S GetOrAssignSkill(T employee) 
        {
            return GetSkill(employee) ?? AssignSkill(employee);
        }

        public S AssignSkill(T employee)
        {
            int id = GetId(employee);
            S skill = TrainingData.FirstOrDefault(c => !c.Skill.IsAssigned() && c.Id == id)?.Skill;
            if (skill == null) {
                skill = Register(id);
            }
            else if (skill.TrainingStatusPanelObj == null)
            {
                Singleton<PCTrainingApp>.Instance.RegisterEmployee(skill);
            }
            if (skill.Employee == null)
            {
                skill.Employee = employee;
            }
            Plugin.LogInfo($"{typeof(T).Name}[{id}] loaded: {skill}");
            // foreach (CashierSkillData d in CTSaveManager.Data.Skills)
            // {
            //     Plugin.LogDebug($"Skill[{d.Skill.GetHashCode(),12}] => cashierID={d.Skill.Cashier?.CashierID}, instanceID={d.Skill.Cashier?.GetInstanceID()}, hashcode={d.Skill.Cashier?.GetHashCode()}]");
            // }
            return skill;
        }

        public IEnumerable<S> GetSkills()
        {
            return TrainingData.Select(c => c.Skill).AsEnumerable();
        }

        public abstract int GetId(T employee);

        public void GenerateSkillIndiactor(S skill)
        {
            var employee = skill.Employee;
            if (employee.GetComponentInChildren<SkillIndicator>() == null)
            {
                Plugin.LogDebug($"Adding Skill Indicator for {typeof(T)} {GetId(employee)}");
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