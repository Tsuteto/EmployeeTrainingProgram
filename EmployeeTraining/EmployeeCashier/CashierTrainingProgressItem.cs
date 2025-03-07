using EmployeeTraining.Employee;
using EmployeeTraining.Localization;

namespace EmployeeTraining.EmployeeCashier
{
    public class CashierTrainingProgressItem : EmployeeTrainingProgressItem<CashierSkill>
    {
        private StringLocalizeTranslator spm;
        private StringLocalizeTranslator payment;

        internal override void SetupDetailParams()
        {
            spm = transform.Find("Elements/Info/Detail Params/SPM/Value").GetComponent<StringLocalizeTranslator>();
            // Plugin.LogDebug($"spm: {spm}");
            payment = transform.Find("Elements/Info/Detail Params/Payment Time/Value").GetComponent<StringLocalizeTranslator>();
            // Plugin.LogDebug($"payment: {payment}");
        }

        internal override void UpdateExp()
        {
            base.UpdateExp();

            // Plugin.LogDebug($"spm: {spm}");
            spm.Translate($"{60f / skill.IntervalMax:0.#}", $"{60f / skill.IntervalMin:0.#}");
            // Plugin.LogDebug($"payment: {payment}");
            payment.Translate($"{skill.TotalCheckoutDuration:0.0#}");
        }
    }
}