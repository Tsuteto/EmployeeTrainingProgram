using System;
using System.Linq;
using __Project__.Scripts.Computer;
using EmployeeTraining.TrainingApp;
using HarmonyLib;
using MyBox;

namespace EmployeeTraining
{
    [HarmonyPatch]
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

        [HarmonyPatch(typeof(DailyStatisticsScreen), "StartNewGame")]
        [HarmonyPrefix]
        public static bool BankruptcyManager_CheckForBankruptcy_Postfix()
        {
            ETSaveManager.Clear();
            Plugin.Instance.GameQuitEvent?.Invoke();
            return true;
        }

        [HarmonyPatch(typeof(SaveManager), "Awake")]
        [HarmonyPostfix]
        public static void SaveManager_Awake_Postfix(SaveManager __instance, string ___m_CurrentSaveFilePath)
        {
            ETSaveManager.Load(___m_CurrentSaveFilePath);
        }

        [HarmonyPatch(typeof(VehicleDataLoader), "ApplyVehicleData")]
        [HarmonyPrefix]
        public static bool VehicleDataLoader_ApplyVehicleData_Prefix(VehicleDataLoader __instance, IPlacementArea ___m_PlacementArea)
        {
            // Addressing a suspected vanilla bug where if there was an empty box in the vehicle load when the game loaded,
            // an error would occur and a transparent load would occupy the vehicle forever.
            // This is a temporary fix until the issue is resolved in the game,
            // and also addresses the effects of a bug in this mod,
            // where the box does not detach from the vehicle when a restocker picks up the box from the vehicle.
            __instance.VehicleData.Boxes.RemoveAll(box => box.ProductID == -1);
            return true;
        }
    }
}