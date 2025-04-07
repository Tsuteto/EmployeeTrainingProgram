using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MyBox;
using UnityEngine;

namespace EmployeeTraining.EmployeeRestocker
{
    [HarmonyPatch(typeof(EmployeeManager))]
    public static class EmployeeManagerPatch
    {
        [HarmonyPatch("SpawnRestocker")]
        [HarmonyPostfix]
        public static void EmployeeManager_SpawnRestocker_Postfix(EmployeeManager __instance, List<Restocker> ___m_ActiveRestockers, int restockerID)
        {
            List<Restocker> restocker = ___m_ActiveRestockers;
            RestockerSkillManager.Instance.Spawn(restocker, restockerID);
        }

        [HarmonyPatch("FireRestocker")]
        [HarmonyPostfix]
        public static void EmployeeManager_FireRestocker_Postfix(EmployeeManager __instance, int restockerID)
        {
            RestockerSkillManager.Instance.Fire(restockerID);
        }

        [HarmonyPatch("DespawnRestocker")]
        [HarmonyPrefix]
        public static bool CheckoutManager_RemoveRestocker_Prefix(EmployeeManager __instance, List<Restocker> ___m_ActiveRestockers, int restockerID)
        {
            Restocker restocker = ___m_ActiveRestockers.FirstOrDefault(r => r.RestockerID == restockerID);
            if (restocker == null)
            {
                return true;
            }
            RestockerSkillManager.Instance.Despawn(restocker);
            return true;
        }
    }

    [HarmonyPatch(typeof(Restocker))]
    public static class RestockerPatch
    {
        [HarmonyPatch("ResetRestocker")]
        [HarmonyPostfix]
        public static void ResetRestocker_Postfix(Restocker __instance)
        {
            RestockerSkill skill = RestockerSkillManager.Instance.GetSkill(__instance);
            skill.Logic.AfterResetRestocker();
        }

        [HarmonyPatch("FreeTargetDisplaySlot")]
        [HarmonyPostfix]
        public static void FreeTargetDisplaySlot_Postfix(Restocker __instance)
        {
            RestockerSkill skill = RestockerSkillManager.Instance.GetSkill(__instance);
            skill.Logic.AfterFreeTargetDisplaySlot();
        }

        [HarmonyPatch("TryRestocking")]
        [HarmonyPrefix]
        public static bool TryRestocking_Prefix(Restocker __instance, ref IEnumerator __result)
        {
            RestockerSkill skill = RestockerSkillManager.Instance.GetSkill(__instance);
            __result = skill.Logic.Internal_TryRestocking();
            return false;
        }

        [HarmonyPatch("PlaceBoxFromVehicle")]
        [HarmonyPrefix]
        public static bool PlaceBoxFromVehicle_Prefix(Restocker __instance, ref IEnumerator __result)
        {
            RestockerSkill skill = RestockerSkillManager.Instance.GetSkill(__instance);
            __result = skill.Logic.Internal_PlaceBoxFromVehicle();
            return false;
        }

        [HarmonyPatch("PlaceBoxFromStreet")]
        [HarmonyPrefix]
        public static bool PlaceBoxFromStreet_Prefix(Restocker __instance, ref IEnumerator __result)
        {
            RestockerSkill skill = RestockerSkillManager.Instance.GetSkill(__instance);
            __result = skill.Logic.Internal_PlaceBoxFromStreet();
            return false;
        }

        [HarmonyPatch("PlaceBox")]
        [HarmonyPrefix]
        public static bool PlaceBox_Prefix(Restocker __instance)
        {
            RestockerSkill skill = RestockerSkillManager.Instance.GetSkill(__instance);
            skill.Logic.Internal_PlaceBox();
            return false;
        }

        [HarmonyPatch("DropTheBox")]
        [HarmonyPrefix]
        public static bool DropTheBox_Prefix(Restocker __instance)
        {
            RestockerSkill skill = RestockerSkillManager.Instance.GetSkill(__instance);
            skill.Logic.Internal_DropTheBox();
            return false;
        }

        [HarmonyPatch("DropBoxToGround")]
        [HarmonyPrefix]
        public static bool DropBoxToGround_Prefix(Restocker __instance)
        {
            RestockerSkill skill = RestockerSkillManager.Instance.GetSkill(__instance);
            skill.Logic.Internal_DropBoxToGround();
            return false;
        }

        [HarmonyPatch("PickUpBox")]
        [HarmonyPrefix]
        public static bool PickUpBox_Prefix(Restocker __instance, bool isFromRack, ref IEnumerator __result)
        {
            RestockerSkill skill = RestockerSkillManager.Instance.GetSkill(__instance);
            __result = skill.Logic.Internal_PickUpBox(isFromRack);
            return false;
        }

        [HarmonyPatch("DropBox")]
        [HarmonyPrefix]
        public static bool DropBox_Prefix(Restocker __instance, ref IEnumerator __result)
        {
            RestockerSkill skill = RestockerSkillManager.Instance.GetSkill(__instance);
            __result = skill.Logic.Internal_DropBox();
            return false;
        }

        [HarmonyPatch("PlaceBoxToRack")]
        [HarmonyPrefix]
        public static bool PlaceBoxToRack_Prefix(Restocker __instance, ref IEnumerator __result)
        {
            RestockerSkill skill = RestockerSkillManager.Instance.GetSkill(__instance);
            __result = skill.Logic.Internal_PlaceBoxToRack();
            return false;
        }

        [HarmonyPatch("ThrowBoxToTrashBin")]
        [HarmonyPrefix]
        public static bool ThrowBoxToTrashBin_Prefix(Restocker __instance, ref IEnumerator __result)
        {
            RestockerSkill skill = RestockerSkillManager.Instance.GetSkill(__instance);
            __result = skill.Logic.Internal_ThrowBoxToTrashBin();
            return false;
        }

        [HarmonyPatch("PerformRestocking")]
        [HarmonyPrefix]
        public static bool PerformRestocking_Prefix(Restocker __instance, ref IEnumerator __result)
        {
            RestockerSkill skill = RestockerSkillManager.Instance.GetSkill(__instance);
            __result = skill.Logic.Internal_PerformRestocking();
            return false;
        }

        [HarmonyPatch("PlaceProducts")]
        [HarmonyPrefix]
        public static bool PlaceProducts_Prefix(Restocker __instance, ref IEnumerator __result)
        {
            RestockerSkill skill = RestockerSkillManager.Instance.GetSkill(__instance);
            __result = skill.Logic.Internal_PlaceProducts();
            return false;
        }

        // [HarmonyPatch("CheckForAvailableRackSlotToPlaceBox")]
        // [HarmonyPrefix]
        // public static bool CheckForAvailableRackSlotToPlaceBox_Prefix(Restocker __instance)
        // {
        //     Plugin.LogDebug($"Called CheckForAvailableRackSlotToPlaceBox");
        //     RestockerSkill skill = RestockerSkillManager.Instance.GetSkill(__instance);
        //     skill.Logic.LogStat();
        //     return true;
        // }

        [HarmonyPatch("GetAvailableDisplaySlotToRestock")]
        [HarmonyPrefix]
        public static bool GetAvailableDisplaySlotToRestock_Prefix(Restocker __instance, ref bool __result)
        {
            RestockerSkill skill = RestockerSkillManager.Instance.GetSkill(__instance);
            __result = skill.Logic.Internal_GetAvailableDisplaySlotToRestock();
            return false;
        }

        // [HarmonyPatch("IsDisplaySlotAvailableToRestock")]
        // [HarmonyPrefix]
        // public static bool IsDisplaySlotAvailableToRestock_Prefix(Restocker __instance, DisplaySlot displaySlot, ref bool __result)
        // {
        //     Plugin.LogDebug($"Called IsDisplaySlotAvailableToRestock: {displaySlot.GetComponent<Display>()?.Data.FurnitureID.ToString() ?? "NULL"}");
        //     RestockerSkill skill = RestockerSkillManager.Instance.GetSkill(__instance);
        //     __result = skill.Logic.Internal_IsDisplaySlotAvailableToRestock(displaySlot);
        //     return true;
        // }

        [HarmonyPatch("MoveTo")]
        [HarmonyPrefix]
        public static bool MoveTo_Prefix(Restocker __instance, Vector3 target, ref IEnumerator __result)
        {
            RestockerSkill skill = RestockerSkillManager.Instance.GetSkill(__instance);
            __result = skill.Logic.Internal_MoveTo(target);
            return false;
        }

        [HarmonyPatch("RotateTo")]
        [HarmonyPrefix]
        public static bool RotateTo_Prefix(Restocker __instance, Quaternion rotation, ref IEnumerator __result)
        {
            RestockerSkill skill = RestockerSkillManager.Instance.GetSkill(__instance);
            __result = skill.Logic.Internal_RotateTo(rotation);
            return false;
        }

        [HarmonyPatch("GetAvailableProductIDList")]
        [HarmonyPrefix]
        public static bool GetAvailableProductIDList_Prefix(Restocker __instance, ref List<int> __result)
        {
            RestockerSkill skill = RestockerSkillManager.Instance.GetSkill(__instance);
            __result = skill.Logic.Internal_GetAvailableProductIDList();
            return false;
        }

        // [HarmonyPatch("Update")]
        // [HarmonyPrefix]
        // public static bool DEBUG_Update(Restocker __instance, RestockerState ___m_State, bool ___m_Available, bool ___m_CheckTasks)
        // {
        //     Plugin.LogDebug($"Restocker[{__instance.RestockerID}] Called Update(): state={___m_State}, available={___m_Available}, checkTasks={___m_CheckTasks}");
        //     return true;
        // }

        // [HarmonyPatch("Start")]
        // [HarmonyPrefix]
        // public static bool DEBUG_Start(Restocker __instance, RestockerState ___m_State, bool ___m_Available, bool ___m_CheckTasks)
        // {
        //     Plugin.LogDebug($"Restocker[{__instance.RestockerID}] Called Start(): state={___m_State}, available={___m_Available}, checkTasks={___m_CheckTasks}");
        //     return true;
        // }
    }

    [HarmonyPatch(typeof(Customer))]
    public static class CustomerPatch
    {
        [HarmonyPatch("StartShopping")]
        [HarmonyPostfix]
        public static void StartShopping_Postfix(Customer __instance)
        {
            Singleton<ShoppingCustomerList>.Instance.StartShopping(__instance);
        }

        [HarmonyPatch("FinishShopping")]
        [HarmonyPostfix]
        public static void FinishShopping_Postfix(Customer __instance)
        {
            Singleton<ShoppingCustomerList>.Instance.FinishShopping(__instance);
        }

        [HarmonyPatch("OnDisable")]
        [HarmonyPostfix]
        public static void OnDisable_Postfix(Customer __instance)
        {
            Singleton<ShoppingCustomerList>.Instance?.FinishShopping(__instance);
        }
    }

    [HarmonyPatch(typeof(DisplaySlot))]
    public static class DisplaySlotPatch
    {
        [HarmonyPatch("RemoveFromDisplayManagerWhileCarrying")]
        [HarmonyPostfix]
        public static void RemoveFromDisplayManagerWhileCarrying_Postfix(DisplaySlot __instance, ItemQuantity ___m_ProductCountData)
        {
            if (___m_ProductCountData != null && ___m_ProductCountData.Products.Count > 0)
            {
                Singleton<InventoryManager>.Instance.RemoveProductFromDisplay(___m_ProductCountData);
            }
        }

        [HarmonyPatch("AddBackToDisplayManagerAfterPlaced")]
        [HarmonyPostfix]
        public static void AddBackToDisplayManagerAfterPlaced_Postfix(DisplaySlot __instance, ItemQuantity ___m_ProductCountData)
        {
            if (___m_ProductCountData != null && ___m_ProductCountData.Products.Count > 0)
            {
                Singleton<InventoryManager>.Instance.AddProductToDisplay(___m_ProductCountData);
            }
        }
    }
}