using System.Collections.Generic;
using EmployeeTraining.Employee;
using MyBox;

namespace EmployeeTraining.EmployeeCashier
{
    public class CashierSkillManager : EmployeeSkillManager<CashierSkill, CashierSkillTier, CashierSkillData, EmplCashier, Cashier>
    {
        public static CashierSkillManager Instance;

        static CashierSkillManager()
        {
            Instance = new CashierSkillManager();
        }

        internal override List<CashierSkillData> TrainingData => ETSaveManager.Data.CashierSkills;

        public override int GetId(Cashier employee)
        {
            return employee.CashierID;
        }
    }

}