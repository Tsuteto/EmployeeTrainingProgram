using EmployeeTraining.Localization;

namespace EmployeeTraining
{
    public class CsHelperTrainingProgressItem : EmployeeTrainingProgressItem<CsHelperSkill>
    {
        private StringLocalizeTranslator spm;
        private StringLocalizeTranslator rapidity;

        internal override void SetupDetailParams()
        {
            this.spm = this.transform.Find("Elements/Info/Detail Params/SPM/Value").GetComponent<StringLocalizeTranslator>();
            // Plugin.LogDebug($"spm: {spm}");
            this.rapidity = this.transform.Find("Elements/Info/Detail Params/Rapidity/Value").GetComponent<StringLocalizeTranslator>();
            // Plugin.LogDebug($"rapidity: {rapidity}");
        }

        internal override void UpdateExp()
        {
            base.UpdateExp();

            // Plugin.LogDebug($"spm: {this.spm}");
            this.spm.Translate($"{60f / skill.IntervalMax:0.#}", $"{60f / skill.IntervalMin:0.#}");
            // Plugin.LogDebug($"rapidity: {this.rapidity}");
            this.rapidity.Translate($"{skill.Rapidity:0.0#}");
        }
    }
}