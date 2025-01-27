using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MyBox;
using UnityEngine;
using UnityEngine.AI;

namespace EmployeeTraining
{
    public static class CsHelperPatch
    {
        [HarmonyPatch(typeof(EmployeeManager), "SpawnCustomerHelper")]
        [HarmonyPostfix]
        public static void EmployeeManager_SpawnCustomerHelper_Postfix(EmployeeManager __instance, List<CustomerHelper> ___m_ActiveCustomerHelpers, int customerHelperID)
        {
            List<CustomerHelper> cshelpers = Singleton<EmployeeManager>.Instance.hiredCustomerHelpers;
            CsHelperSkillManager.Instance.Spawn(cshelpers, customerHelperID);
        }

        [HarmonyPatch(typeof(EmployeeManager), "FireCustomerHelper")]
        [HarmonyPostfix]
        public static void EmployeeManager_FireCustomerHepler_Postfix(EmployeeManager __instance, int customerHelperID)
        {
            CsHelperSkillManager.Instance.Fire(customerHelperID);
        }

        [HarmonyPatch(typeof(EmployeeManager), "DespawnCustomerHelper")]
        [HarmonyPrefix]
        public static bool CheckoutManager_RemoveCustomerHelper_Prefix(EmployeeManager __instance, List<CustomerHelper> ___m_ActiveCustomerHelpers, int customerHelperID)
        {
            CustomerHelper cshelper = ___m_ActiveCustomerHelpers.FirstOrDefault(r => r.CustomerHelperID == customerHelperID);
            if (cshelper == null)
            {
                return true;
            }
            CsHelperSkillManager.Instance.Despawn(cshelper);
            return true;
        }

        /***** LOGIC *****/
        [HarmonyPatch(typeof(SelfCheckout), "StartCustomerHelperCheckout")]
        [HarmonyPrefix]
        public static bool SelfCheckout_StartCustomerHelperCheckout_Prefix(SelfCheckout __instance, ref Checkout ___m_Checkout, ref SFXInstance ___m_CashierSFX, ref GameObject ___m_RepairIndicator)
        {
            CsHelperLogic.PerformScanning(__instance, ___m_Checkout, ___m_CashierSFX, ___m_RepairIndicator);
            return false;
        }

        [HarmonyPatch(typeof(CustomerHelper), "SetCustomerHelperBoost")]
        [HarmonyPrefix]
        public static bool CustomerHelper_SetCustomerHelperBoost_Prefix(CustomerHelper __instance)
        {
            CsHelperLogic.ApplyRapidity(__instance);
            return false;
        }

        // [HarmonyPatch(typeof(CustomerHelper), "MoveTo")]
        // [HarmonyPrefix]
        // public static bool MoveTo_Prefix(CustomerHelper __instance, Vector3 target, ref IEnumerator __result, ref NavMeshAgent ___m_Agent)
        // {
        //     __result = CsHelperLogic.MoveTo(__instance, target, ___m_Agent);
        //     return false;
        // }


    }
}