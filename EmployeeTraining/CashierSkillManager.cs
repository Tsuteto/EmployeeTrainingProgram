using System.Collections.Generic;
using MyBox;

namespace EmployeeTraining
{
    public class CashierSkillManager : EmployeeSkillManager<CashierSkill, CashierSkillTier, CashierSkillData, EmployeeCashier, Cashier>
    {
        public static CashierSkillManager Instance { get; private set; }

        public static void CreateInstance()
        {
            Instance = new CashierSkillManager();
        }

        public override CashierSkill Register(int cashierId)
        {
            // Plugin.LogDebug($"Registering Cashier {cashierId}");
            var newData = new CashierSkillData{Id=cashierId};
            newData.Skill.Setup();
            ETSaveManager.Data.CashierSkills.Add(newData);
            Singleton<PCTrainingApp>.Instance.RegisterEmployee(newData.Skill);
            return newData.Skill;
        }

        internal override List<CashierSkillData> TrainingData => ETSaveManager.Data.CashierSkills;

        public override int GetId(Cashier employee)
        {
            return employee.CashierID;
        }
    }

}