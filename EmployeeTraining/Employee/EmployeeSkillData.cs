using System;
using EmployeeTraining.EmployeeCashier;
using EmployeeTraining.EmployeeCsHelper;
using EmployeeTraining.EmployeeRestocker;

namespace EmployeeTraining.Employee
{
    [Serializable]
    public abstract class SkillData<S>
        where S : IEmployeeSkill
    {
        public int Id;
        public int Exp = 0;
        public int Grade = 0;
        public bool IsGaugeDisplayed = true;

        [NonSerialized]
        public S Skill;

        public SkillData()
        {
            ETSaveManager.SaveDataLoadedEvent += OnLoad;
        }

        private void OnLoad()
        {
            Skill.Setup();
            ETSaveManager.SaveDataLoadedEvent -= OnLoad;
        }
    }

    [Serializable]
    public class CashierSkillData : SkillData<CashierSkill>
    {

        public CashierSkillData() : base()
        {
            Skill = new CashierSkill(this);
        }
    }

    [Serializable]
    public class RestockerSkillData : SkillData<RestockerSkill>
    {
        public RestockerSkillData() : base()
        {
            Skill = new RestockerSkill(this);
        }
    }

    [Serializable]
    public class CsHelperSkillData : SkillData<CsHelperSkill>
    {
        public CsHelperSkillData() : base()
        {
            Skill = new CsHelperSkill(this);
        }
    }
}