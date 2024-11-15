using System;

namespace EmployeeTraining
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
    }

    [Serializable]
    public class CashierSkillData : SkillData<CashierSkill>
    {

        public CashierSkillData()
        {
            this.Skill = new CashierSkill(this);
        }
    }

    [Serializable]
    public class RestockerSkillData : SkillData<RestockerSkill>
    {
        public RestockerSkillData()
        {
            this.Skill = new RestockerSkill(this);
        }
    }

    [Serializable]
    public class CsHelperSkillData : SkillData<CsHelperSkill>
    {
        public CsHelperSkillData()
        {
            this.Skill = new CsHelperSkill(this);
        }
    }
}