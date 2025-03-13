using BepInEx.Configuration;

namespace EmployeeTraining
{
    public class Settings
    {
		public float GaugeHeight;
        public bool CustomizeLocalization;
        public bool RestockerLog;

        public Settings(ConfigFile cfg)
        {
            this.GaugeHeight = cfg.Bind("Gauge Indicator", "Gauge Height", 1.8f, "Height of the gauge showing up on cashier's head. Tweak it when overlapping with another mod's indicator.").Value;
            this.CustomizeLocalization = cfg.Bind("Localization", "Customize Localization", false, "If true is set, Localization-x.x.x.json file is saved in Supermarket Simulator_Data/CashierTraining folder and you can customize it.").Value;
            this.RestockerLog = cfg.Bind("Debug", "Restocker verbose log", false, "Enable restocker verbose log. You also need to set the LogLevels in BepInEx.cfg to \"All\" for output. This may cause fps to drop. Note: This is not for bug reporting but for your own troubleshooting.").Value;
        }
    }
}