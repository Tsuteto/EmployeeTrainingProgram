using EmployeeTraining.Employee;
using EmployeeTraining.Localization;

namespace EmployeeTraining.EmployeeJanitor
{
    public class JanitorTrainingProgressItem : EmployeeTrainingProgressItem<JanitorSkill>
    {
        private StringLocalizeTranslator dexterity;
        private StringLocalizeTranslator rapidity;

        internal override void SetupDetailParams()
        {
            rapidity = transform.Find("Elements/Info/Detail Params/Rapidity/Value").GetComponent<StringLocalizeTranslator>();
            // Plugin.LogDebug($"rapidity: {rapidity}");
            dexterity = transform.Find("Elements/Info/Detail Params/Dexterity/Value").GetComponent<StringLocalizeTranslator>();
            // Plugin.LogDebug($"dexterity: {dexterity}");
        }

        internal override void UpdateExp()
        {
            base.UpdateExp();

            // Plugin.LogDebug($"rapidity: {this.rapidity}");
            rapidity.Translate($"{skill.Rapidity:0.0#}");
            // Plugin.LogDebug($"dexterity: {this.dexterity}");
            dexterity.Translate($"{skill.Dexterity}");
        }
    }
}