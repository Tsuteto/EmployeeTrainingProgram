using System.Collections.Generic;
using System.Linq;
using MyBox;

namespace EmployeeTraining
{
    public class CsHelperSkillManager : EmployeeSkillManager<CsHelperSkill, CsHelperSkillTier, CsHelperSkillData, EmployeeCsHelper, CustomerHelper>
    {
        public static CsHelperSkillManager Instance { get; private set; }

        public static void CreateInstance()
        {
            Instance = new CsHelperSkillManager();
        }

        public override CsHelperSkill Register(int id)
        {
            // Plugin.LogDebug($"Registering CustomerHelper {id}");
            var newData = new CsHelperSkillData{Id=id};
            this.TrainingData.Add(newData);
            Singleton<PCTrainingApp>.Instance.RegisterEmployee(newData.Skill);
            return newData.Skill;
        }

        internal override List<CsHelperSkillData> TrainingData => ETSaveManager.Data.CsHelperSkills;

        public override int GetId(CustomerHelper employee)
        {
            return employee.CustomerHelperID;
        }
    }
}