using System;
using System.Collections.Generic;
using System.Linq;
using EmployeeTraining;

namespace CashierTraining
{
    [Serializable]
    public class CashiersData
    {
        public List<CashierSkillData> Skills = new List<CashierSkillData>();

        public TrainingData Migrate()
        {
            TrainingData rev2 = new TrainingData{
                CashierSkills = this.Skills.Select(s => 
                new EmployeeTraining.CashierSkillData{
                    Id=s.Id,
                    Exp=s.Exp,
                    Grade=s.Grade,
                    IsGaugeDisplayed=s.IsGaugeDisplayed
                }).ToList()
            };
            return rev2;
        }
    }

    [Serializable]
    public class CashierSkillData
    {
        public int Id;
        public int Exp = 0;
        public int Grade = 0;
        public bool IsGaugeDisplayed = true;
    }
}