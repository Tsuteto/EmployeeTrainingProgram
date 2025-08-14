using EmployeeTraining.Employee;
using EmployeeTraining.Localization;

namespace EmployeeTraining.EmployeeRestocker
{
    public class RestockerTrainingProgressItem : EmployeeTrainingProgressItem<RestockerSkill>
    {
        private StringLocalizeTranslator rapidity;
        private StringLocalizeTranslator capacity;
        private StringLocalizeTranslator dexterity;

        internal override void SetupDetailParams()
        {
            rapidity = transform.Find("Elements/Info/Detail Params/Rapidity/Value").GetComponent<StringLocalizeTranslator>();
            // Plugin.LogDebug($"this.repidity: {this.repidity}");
            capacity = transform.Find("Elements/Info/Detail Params/Capacity/Value").GetComponent<StringLocalizeTranslator>();
            // Plugin.LogDebug($"this.capacity: {this.capacity}");
            dexterity = transform.Find("Elements/Info/Detail Params/Dexterity/Value").GetComponent<StringLocalizeTranslator>();
            // Plugin.LogDebug($"this.efficiency: {this.dexterity}");
        }

        internal override void UpdateExp()
        {
            base.UpdateExp();

            rapidity.Translate($"{skill.Rapidity:0.0#}");
            // Plugin.LogDebug($"Rapidity: {skill.Rapidity}");
            capacity.Translate($"{skill.Capacity:0.0#}", $"{skill.CapacityMaxHeight:0.0}");
            // Plugin.LogDebug($"Capacity: {skill.Capacity}");
            dexterity.Translate($"{skill.Dexterity}");
            // Plugin.LogDebug($"Dexterity: {skill.Dexterity}");
        }
    }
}