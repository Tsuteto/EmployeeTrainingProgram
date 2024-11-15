using EmployeeTraining.Localization;

namespace EmployeeTraining
{
    public class RestockerTrainingProgressItem : EmployeeTrainingProgressItem<RestockerSkill>
    {
        private StringLocalizeTranslator repidity;
        private StringLocalizeTranslator capacity;
        private StringLocalizeTranslator dexterity;

        internal override void SetupDetailParams()
        {
            this.repidity = this.transform.Find("Elements/Info/Detail Params/Rapidity/Value").GetComponent<StringLocalizeTranslator>();
            // Plugin.LogDebug($"this.repidity: {this.repidity}");
            this.capacity = this.transform.Find("Elements/Info/Detail Params/Capacity/Value").GetComponent<StringLocalizeTranslator>();
            // Plugin.LogDebug($"this.capacity: {this.capacity}");
            this.dexterity = this.transform.Find("Elements/Info/Detail Params/Dexterity/Value").GetComponent<StringLocalizeTranslator>();
            // Plugin.LogDebug($"this.efficiency: {this.dexterity}");
        }

        internal override void UpdateExp()
        {
            base.UpdateExp();

            this.repidity.Translate($"{skill.Rapidity:0.0#}");
            // Plugin.LogDebug($"Rapidity: {skill.Rapidity}");
            this.capacity.Translate($"{skill.Capacity:0.0#}", $"{skill.CapacityMaxHeight:0.0}");
            // Plugin.LogDebug($"Capacity: {skill.Capacity}");
            this.dexterity.Translate($"{skill.Dexterity}");
            // Plugin.LogDebug($"Dexterity: {skill.Dexterity}");
        }
    }
}