using System.Collections.Generic;
using System.Linq;
using EmployeeTraining.Employee;
using MyBox;

namespace EmployeeTraining.EmployeeSecurity
{
    public class SecuritySkillManager : EmployeeSkillManager<SecuritySkill, SecuritySkillTier, SecuritySkillData, EmplSecurity, SecurityGuard>
    {
        public static SecuritySkillManager Instance;

        static SecuritySkillManager()
        {
            Instance = new SecuritySkillManager();
        }

        internal override List<SecuritySkillData> TrainingData => ETSaveManager.Data.SecuritySkills;

        public override int GetId(SecurityGuard employee)
        {
            return employee.ID;
        }
        
        public override SecurityGuard Spawn(List<SecurityGuard> employees, int employeeID)
        {
            var security = base.Spawn(employees, employeeID);
            // SecurityLogic.SetSpeed(security.Controller, 0);
            return security;
        }
    }
}