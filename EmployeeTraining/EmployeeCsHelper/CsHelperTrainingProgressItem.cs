using EmployeeTraining.Employee;
using EmployeeTraining.Localization;

namespace EmployeeTraining.EmployeeCsHelper
{
    public class CsHelperTrainingProgressItem : EmployeeTrainingProgressItem<CsHelperSkill>
    {
        private StringLocalizeTranslator spm;
        private StringLocalizeTranslator rapidity;

        internal override void SetupDetailParams()
        {
            spm = transform.Find("Elements/Info/Detail Params/SPM/Value").GetComponent<StringLocalizeTranslator>();
            // Plugin.LogDebug($"spm: {spm}");
            rapidity = transform.Find("Elements/Info/Detail Params/Rapidity/Value").GetComponent<StringLocalizeTranslator>();
            // Plugin.LogDebug($"rapidity: {rapidity}");
        }

        internal override void UpdateExp()
        {
            base.UpdateExp();

            // Plugin.LogDebug($"spm: {this.spm}");
            spm.Translate($"{60f / skill.IntervalMax:0.#}", $"{60f / skill.IntervalMin:0.#}");
            // Plugin.LogDebug($"rapidity: {this.rapidity}");
            rapidity.Translate($"{skill.Rapidity:0.0#}");
        }
    }
}