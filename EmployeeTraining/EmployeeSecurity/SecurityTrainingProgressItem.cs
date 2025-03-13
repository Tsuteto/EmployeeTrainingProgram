using EmployeeTraining.Employee;
using EmployeeTraining.Localization;

namespace EmployeeTraining.EmployeeSecurity
{
    public class SecurityTrainingProgressItem : EmployeeTrainingProgressItem<SecuritySkill>
    {
        private StringLocalizeTranslator repidity;
        private StringLocalizeTranslator dexterity;

        internal override void SetupDetailParams()
        {
            repidity = transform.Find("Elements/Info/Detail Params/Rapidity/Value").GetComponent<StringLocalizeTranslator>();
            // Plugin.LogDebug($"this.repidity: {this.repidity}");
            dexterity = transform.Find("Elements/Info/Detail Params/Dexterity/Value").GetComponent<StringLocalizeTranslator>();
            // Plugin.LogDebug($"this.strength: {this.strength}");
        }

        internal override void UpdateExp()
        {
            base.UpdateExp();

            repidity.Translate($"{skill.Rapidity:0.0#}");
            // Plugin.LogDebug($"Rapidity: {skill.Rapidity}");
            dexterity.Translate($"{skill.Dexterity}");
            // Plugin.LogDebug($"Dexterity: {skill.dexterity}");
        }
    }
}