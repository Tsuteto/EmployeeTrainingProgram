using __Project__.Scripts.Computer;
using EmployeeTraining.TrainingApp;
using HarmonyLib;
using MyBox;

namespace EmployeeTraining
{
    public static class GamePatch
    {
        public static Plugin plugin;

        [HarmonyPatch(typeof(SaveManager), "Clear")]
        [HarmonyPostfix]
        public static void SaveManager_Clear_Postfix(SaveManager __instance)
        {
            ETSaveManager.Clear();
        }

        [HarmonyPatch(typeof(Computer), "Start")]
        [HarmonyPrefix]
        private static bool Computer_Start_Prefix()
        {
            PCTrainingApp.LoadAppToComputerOS();
            return true;
        }

        [HarmonyPatch(typeof(MainMenuManager), "NewGame")]
        [HarmonyPrefix]
        public static bool MainMenuManager_NewGame_Prefix()
        {
            ETSaveManager.IsReadyToSave = true;
            return true;
        }

        [HarmonyPatch(typeof(MainMenuManager), "NewGame")]
        [HarmonyPostfix]
        public static void MainMenuManager_NewGame_Postfix()
        {
            ETSaveManager.Clear();
        }

        [HarmonyPatch(typeof(BankruptcyManager), "CheckForBankruptcy")]
        [HarmonyPostfix]
        public static void BankruptcyManager_CheckForBankruptcy_Postfix()
        {
            if (Singleton<MoneyManager>.Instance.Money < 0f)
            {
                ETSaveManager.Clear();
            }
        }
    }
}