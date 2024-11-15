using System.Collections.Generic;
using System.Linq;
using MyBox;

namespace EmployeeTraining
{
    public class RestockerSkillManager : EmployeeSkillManager<RestockerSkill, RestockerSkillTier, RestockerSkillData, EmployeeRestocker, Restocker>
    {
        public static RestockerSkillManager Instance { get; private set; }

        public static void CreateInstance()
        {
            Instance = new RestockerSkillManager();
        }

        public override RestockerSkill Register(int id)
        {
            // Plugin.LogDebug($"Registering Restocker {cashierId}");
            var newData = new RestockerSkillData{Id=id};
            this.TrainingData.Add(newData);
            Singleton<PCTrainingApp>.Instance.RegisterEmployee(newData.Skill);
            return newData.Skill;
        }

        public List<RestockerLogic> GetActiveLogics()
        {
            return this.TrainingData.Where(d => d.Skill.IsAssigned()).Select(d => d.Skill.Logic).ToList();
        }

        internal override List<RestockerSkillData> TrainingData => ETSaveManager.Data.RestockerSkills;

        public override int GetId(Restocker employee)
        {
            return employee.RestockerID;
        }
    }
}