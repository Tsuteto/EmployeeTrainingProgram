using System;
using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace EmployeeTraining
{
    public static class CashierPatch
    {
        [HarmonyPatch(typeof(EmployeeManager), "SpawnCashier")]
        [HarmonyPostfix]
        public static void EmployeeManager_SpawnCashier_Postfix(EmployeeManager __instance, List<Cashier> ___m_ActiveCashiers, int cashierID)
        {
            List<Cashier> cashiers = ___m_ActiveCashiers;
            CashierSkillManager.Instance.Spawn(cashiers, cashierID);
        }

        [HarmonyPatch(typeof(EmployeeManager), "GetAvailableCashier")]
        [HarmonyPostfix]
        public static void EmployeeManager_GetAvailableCashier_Postfix(EmployeeManager __instance, List<Cashier> ___m_ActiveCashiers, Cashier __result)
        {
            if (__result != null)
            {
                List<Cashier> cashiers = ___m_ActiveCashiers;
                CashierSkillManager.Instance.Spawn(cashiers, __result.CashierID);
            }
        }

        // [HarmonyPatch(typeof(EmployeeManager), "HireCashier")]
        // [HarmonyPostfix]
        // public static void EmployeeManager_HireCashier_Postfix(EmployeeManager __instance, int cashierID)
        // {
        //     CashierSkillManager.Instance.HireCashier(cashierID);
        // }

        [HarmonyPatch(typeof(EmployeeManager), "FireCashier")]
        [HarmonyPostfix]
        public static void EmployeeManager_FireCashier_Postfix(EmployeeManager __instance, int cashierID)
        {
            CashierSkillManager.Instance.Fire(cashierID);
        }

        [HarmonyPatch(typeof(CheckoutManager), "RemoveCashier")]
        [HarmonyPrefix]
        public static bool CheckoutManager_RemoveCashier_Prefix(CheckoutManager __instance, Cashier cashier)
        {
            CashierSkillManager.Instance.Despawn(cashier);
            return true;
        }

        /***** LOGIC *****/

        [HarmonyPatch(typeof(AutomatedCheckout), "StartCashierCheckout")]
        [HarmonyPrefix]
        public static bool AutomatedCheckout_StartCashierCheckout_Prefix(AutomatedCheckout __instance, ref GameObject ___m_ShoppingBag, ref Checkout ___m_Checkout, ref SFXInstance ___m_CashierSFX)
        {
            CashierLogic.PerformScanning(__instance, ___m_ShoppingBag, ___m_Checkout, ___m_CashierSFX);
            return false;
        }

        [HarmonyPatch(typeof(AutomatedCheckout), "FinishCheckout", new Type[] { typeof(bool) })]
        [HarmonyPrefix]
        public static bool AutomatedCheckout_FinishCheckout_Prefix(
                AutomatedCheckout __instance,
                ref GameObject ___m_ShoppingBag,
                ref float ___m_IntervalAfterScanningAll,
                ref float ___m_TakingPaymentInterval,
                ref float ___m_FinishingPaymentDuration
            )
        {
            // VANILLA:
            // m_IntervalAfterScanningAll: 1
            // m_TakingPaymentInterval: 0.4
            // m_FinishingPaymentDuration: 2
            PaymentDuration duration = CashierLogic.FinishCheckout(__instance);
            ___m_IntervalAfterScanningAll = duration.IntervalAfterScanningAll;
            ___m_TakingPaymentInterval = duration.TakingPaymentInterval;
            ___m_FinishingPaymentDuration = duration.FinishingPaymentDuration;
            var m_IntervalAfterScanningAll = ___m_IntervalAfterScanningAll;
            var m_TakingPaymentInterval = ___m_TakingPaymentInterval;
            var m_FinishingPaymentDuration = ___m_FinishingPaymentDuration;

            // 経験値を与えるだけ
            // アクションはせず本処理と並行して実行する
            __instance.StartCoroutine(GiveExp());
            IEnumerator GiveExp()
            {
                yield return new WaitForSeconds(m_IntervalAfterScanningAll + m_TakingPaymentInterval + m_FinishingPaymentDuration);
                CashierLogic.GiveExpAfterFinishingCheckout(__instance);
            }
            return true;
        }


    }
}