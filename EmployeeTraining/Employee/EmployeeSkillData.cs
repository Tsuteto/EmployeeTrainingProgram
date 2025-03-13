using System;

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
}