using System.Collections.Generic;
using System.Linq;
using EmployeeTraining.Employee;
using MyBox;

namespace EmployeeTraining.EmployeeJanitor
{
    public class JanitorSkillManager : EmployeeSkillManager<JanitorSkill, JanitorSkillTier, JanitorSkillData, EmplJanitor, Janitor>
    {
        public static JanitorSkillManager Instance;

        static JanitorSkillManager()
        {
            Instance = new JanitorSkillManager();
        }

        internal override List<JanitorSkillData> TrainingData => ETSaveManager.Data.JanitorSkills;

        public override int GetId(Janitor employee)
        {
            return employee.JanitorID;
        }
        
        public override Janitor Spawn(List<Janitor> employees, int employeeID)
        {
            var janitor = base.Spawn(employees, employeeID);
            JanitorLogic.ApplyRapidity(janitor);
            return janitor;
        }
    }
}