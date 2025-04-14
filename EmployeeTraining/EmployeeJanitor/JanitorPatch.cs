using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MyBox;
using UnityEngine;
using UnityEngine.AI;

namespace EmployeeTraining.EmployeeJanitor
{
    [HarmonyPatch]
    public static class JanitorPatch
    {
        [HarmonyPatch(typeof(EmployeeManager), "SpawnJanitor")]
        [HarmonyPostfix]
        public static void EmployeeManager_SpawnJanitor_Postfix(EmployeeManager __instance, List<CustomerHelper> ___m_ActiveCustomerHelpers, int janitorID)
        {
            List<Janitor> janitor = Singleton<EmployeeManager>.Instance.ActiveJanitor;
            JanitorSkillManager.Instance.Spawn(janitor, janitorID);
        }

        [HarmonyPatch(typeof(EmployeeManager), "FireJanitor")]
        [HarmonyPostfix]
        public static void EmployeeManager_FireJanitor_Postfix(EmployeeManager __instance, int janitorID)
        {
            JanitorSkillManager.Instance.Fire(janitorID);
        }

        [HarmonyPatch(typeof(EmployeeManager), "DespawnJanitor")]
        [HarmonyPrefix]
        public static bool CheckoutManager_DespawnJanitor_Prefix(EmployeeManager __instance, int janitorID)
        {
            Janitor janitor = Singleton<EmployeeManager>.Instance.ActiveJanitor.FirstOrDefault(r => r.JanitorID == janitorID);
            if (janitor == null)
            {
                return true;
            }
            JanitorSkillManager.Instance.Despawn(janitor);
            return true;
        }

        /***** LOGIC *****/
        [HarmonyPatch(typeof(Janitor), "SetJanitorBoost")]
        [HarmonyPriority(Priority.Last)]
        [HarmonyPrefix]
        public static bool Janitor_SetJanitorBoost_Prefix(Janitor __instance)
        {
            JanitorLogic.ApplyRapidity(__instance);
            return false;
        }

        [HarmonyPatch(typeof(Janitor), "CleaningDuration", MethodType.Getter)]
        [HarmonyPriority(Priority.Last)]
        [HarmonyPrefix]
        public static bool Janitor_GetCleaningDuration_Prefix(Janitor __instance, ref float __result)
        {
            __result = JanitorLogic.GetCleanDuration(__instance);
            return false;
        }


        [HarmonyPatch(typeof(Dust), "CleaningForJanitor")]
        [HarmonyPriority(Priority.Last)]
        [HarmonyPrefix]
        public static bool Dust_CleaningForJanitor_Prefix(Dust __instance, Janitor janitor,
            List<GameObject> ___m_DustList, List<ParticleSystem> ___m_BubbleParticles, ParticleSystem ___m_BubbleParticle,
            float ___m_DustCleaningMultiplier, int ___m_DustExp,
            float ___m_AlphaMax, float ___m_AlphaMin, int ___AlphaClip)
        {
            JanitorLogic.CleaningForJanitor(janitor, __instance, ___m_DustList, ___m_BubbleParticles, ___m_BubbleParticle,
                ___m_DustCleaningMultiplier, ___m_DustExp, ___m_AlphaMax, ___m_AlphaMin, ___AlphaClip);

            return false;
        }

        [HarmonyPatch(typeof(Janitor), "FinishCleaningMopRoutine")]
        [HarmonyPrefix]
        public static bool Janitor_FinishCleaningMopRoutine_Postfix(Janitor __instance)
        {
            JanitorLogic.OnDirtCleaned(__instance);
            return true;
        }

        [HarmonyPatch(typeof(CleanGarbageAction), "OnExecute")]
        [HarmonyPostfix]
        public static void CleanGarbageAction_OnExecute_Postfix(CleanGarbageAction __instance)
        {
            JanitorLogic.OnGarbageCleaned(__instance);
        }
    }
}