using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using UnityEngine.AI;

namespace EmployeeTraining.EmployeeSecurity
{
    [HarmonyPatch]
    public static class SecurityPatch
    {
        [HarmonyPatch(typeof(EmployeeManager), "SpawnSecurityGuard")]
        [HarmonyPostfix]
        public static void EmployeeManager_SpawnSecurityGuard_Postfix(EmployeeManager __instance, List<SecurityGuard> ___m_ActiveSecurityGuards, int securityGuardID)
        {
            List<SecurityGuard> security = ___m_ActiveSecurityGuards;
            SecuritySkillManager.Instance.Spawn(security, securityGuardID);
        }

        [HarmonyPatch(typeof(EmployeeManager), "FireSecurityGuard")]
        [HarmonyPostfix]
        public static void EmployeeManager_FireSecurityGuard_Postfix(EmployeeManager __instance, int SecurityGuardID)
        {
            SecuritySkillManager.Instance.Fire(SecurityGuardID);
        }

        [HarmonyPatch(typeof(EmployeeManager), "DespawnSecurityGuard")]
        [HarmonyPrefix]
        public static bool CheckoutManager_DespawnSecurityGuard_Prefix(EmployeeManager __instance, List<SecurityGuard> ___m_ActiveSecurityGuards, int SecurityGuardID)
        {
            SecurityGuard security = ___m_ActiveSecurityGuards.FirstOrDefault(r => r.ID == SecurityGuardID);
            if (security == null)
            {
                return true;
            }
            SecuritySkillManager.Instance.Despawn(security);
            return true;
        }

        /***** LOGIC *****/
        [HarmonyPatch(typeof(SecurityGuardAnimationController), "SetSpeed")]
        [HarmonyPrefix]
        public static bool SecurityGuardAnimationController_SetSpeed_Prefix(SecurityGuardAnimationController __instance, int _speedLevel, NavMeshAgent ___m_Agent)
        {
            SecurityLogic.SetSpeed(__instance, _speedLevel, ___m_Agent);
            return false;
        }

        [HarmonyPatch(typeof(SecurityGuardAnimationController), "Move")]
        [HarmonyPrefix]
        public static bool SecurityGuardAnimationController_Move_Prefix(SecurityGuardAnimationController __instance, Vector3 target, int speedLevel, NavMeshAgent ___m_Agent, ref IEnumerator __result)
        {
            __result = SecurityLogic.Move(__instance, target, speedLevel, ___m_Agent);
            return false;
        }

        [HarmonyPatch(typeof(ChaseState), "OnEnter")]
        [HarmonyPostfix]
        public static void ChaseState_OnEnter_Postfix(ChaseState __instance)
        {
            SecurityLogic.OnShoplifterDetected(__instance);
        }


        [HarmonyPatch(typeof(ShoplifterTutorialCustomer), "RunAway")]
        [HarmonyPostfix]
        public static void ShoplifterTutorialCustomer_RunAway(Customer __instance, bool isHitByGuard, SecurityGuard securityGuard)
        {
            SecurityLogic.OnShoplifterBeaten(__instance, isHitByGuard, securityGuard);
        }

        [HarmonyPatch(typeof(Customer), "RunAway")]
        [HarmonyPostfix]
        public static void Customer_RunAway(Customer __instance, bool isHitByGuard, SecurityGuard securityGuard)
        {
            SecurityLogic.OnShoplifterBeaten(__instance, isHitByGuard, securityGuard);
        }

        [HarmonyPatch(typeof(CollectingState), "FindNearbyProducts")]
        [HarmonyPrefix]
        public static bool CollectingState_FindNearbyProducts_Prefix(CollectingState __instance, float ___m_DetectRadius)
        {
            SecurityLogic.FindNearbyProducts(__instance, ___m_DetectRadius);
            return false;
        }

        [HarmonyPatch(typeof(RestockingState), "ProductRestockLoop")]
        [HarmonyPrefix]
        public static bool RestockingState_ProductRestockLoop_Prefix(RestockingState __instance, Restocker ___m_Restocker, Crate ___m_Crate, ref IEnumerator __result)
        {
            __result = SecurityLogic.ProductRestockLoop(__instance, ___m_Restocker, ___m_Crate);
            return false;
        }
    }
}