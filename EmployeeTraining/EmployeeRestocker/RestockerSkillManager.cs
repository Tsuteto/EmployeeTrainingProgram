using System.Collections.Generic;
using System.Linq;
using EmployeeTraining.Employee;
using HarmonyLib;
using MyBox;

namespace EmployeeTraining.EmployeeRestocker
{
    public class RestockerSkillManager : EmployeeSkillManager<RestockerSkill, RestockerSkillTier, RestockerSkillData, EmplRestocker, Restocker>
    {
        public static RestockerSkillManager Instance;

        static RestockerSkillManager()
        {
            Instance = new RestockerSkillManager();
        }

        public List<RestockerLogic> GetActiveLogics()
        {
            return TrainingData.Where(d => d.Skill.IsAssigned()).Select(d => d.Skill.Logic).ToList();
        }

        internal override List<RestockerSkillData> TrainingData => ETSaveManager.Data.RestockerSkills;

        public override int GetId(Restocker employee)
        {
            return employee.RestockerID;
        }

        public override Restocker Spawn(List<Restocker> employees, int employeeID)
        {
            var restocker = base.Spawn(employees, employeeID);
            // Force to set true to m_Available flag since it doesn't work after respawn. Vanilla's bug?
            Traverse.Create(restocker).Field("m_Available").SetValue(true);
            return restocker;
        }
    }
}