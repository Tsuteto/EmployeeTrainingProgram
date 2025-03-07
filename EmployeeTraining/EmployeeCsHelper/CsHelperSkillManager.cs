using System.Collections.Generic;
using System.Linq;
using EmployeeTraining.Employee;
using MyBox;

namespace EmployeeTraining.EmployeeCsHelper
{
    public class CsHelperSkillManager : EmployeeSkillManager<CsHelperSkill, CsHelperSkillTier, CsHelperSkillData, EmplCsHelper, CustomerHelper>
    {
        public static CsHelperSkillManager Instance;

        static CsHelperSkillManager()
        {
            Instance = new CsHelperSkillManager();
        }

        internal override List<CsHelperSkillData> TrainingData => ETSaveManager.Data.CsHelperSkills;

        public override int GetId(CustomerHelper employee)
        {
            return employee.CustomerHelperID;
        }
        
        public override CustomerHelper Spawn(List<CustomerHelper> employees, int employeeID)
        {
            var cshelper = base.Spawn(employees, employeeID);
            CsHelperLogic.ApplyRapidity(cshelper);
            return cshelper;
        }
    }
}