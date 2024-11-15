using System.Collections;
using UnityEngine;

namespace EmployeeTraining
{
    public static class CashierLogic
    {
        public static void PerformScanning(AutomatedCheckout __instance, GameObject m_ShoppingBag, Checkout m_Checkout, SFXInstance m_CashierSFX)
        {
            // Plugin.LogDebug("Called PerformScanning");
            // Logger.LogDebug($"__instance={__instance}, m_ShoppingBag={m_ShoppingBag}, m_Checkout={m_Checkout}, m_CashierSFX={m_CashierSFX}");
            Cashier m_Cashier = __instance.Cashier;

            // Logger.LogDebug($"m_Cashier: {m_Cashier}");
            CashierSkill skill = CashierSkillManager.Instance.GetOrAssignSkill(m_Cashier);
            // Logger.LogDebug($"skill: {skill}");
            float valueMin = skill.IntervalMin * ((1.5f + (skill.CashierScanIntervals[skill.CurrentBoostLevel] - 1.5f) * 0.6f) / 1.5f);
            float valueMax = skill.IntervalMax * (skill.CashierScanIntervals[skill.CurrentBoostLevel] / 1.5f);
            float scanInterval = UnityEngine.Random.Range(valueMin, valueMax);
            // Logger.LogDebug($"valueMin={valueMin} valueMax={valueMax} scanInterval={scanInterval}");
            m_ShoppingBag.SetActive(true);
            __instance.StartCoroutine(Scanning());

            IEnumerator Scanning()
            {
                while (m_Checkout.Belt.Products.Count > 0)
                {
                    yield return new WaitForSeconds(scanInterval);
                    if (m_Checkout.Belt.Products.Count <= 0)
                    {
                        break;
                    }
                    Product currentProduct = m_Checkout.Belt.Products[0];
                    m_Checkout.ProductScanned(currentProduct, true);
                    m_Cashier.ScanAnimation();
                    m_CashierSFX.PlayScanningProductSFX();
                    scanInterval = UnityEngine.Random.Range(valueMin, valueMax);
                    skill.AddExp(1);
                }
            }
        }

        public static PaymentDuration FinishCheckout(AutomatedCheckout instance)
        {
            Cashier m_Cashier = instance.Cashier;
            CashierSkill skill = CashierSkillManager.Instance.GetOrAssignSkill(m_Cashier);

            var dur = new PaymentDuration{
                IntervalAfterScanningAll = skill.OperationSpd / 3,
                TakingPaymentInterval = skill.OperationSpd / 15 * 2,
                FinishingPaymentDuration = skill.OperationSpd / 3 * 2
            };
            // Plugin.LogDebug($"IntervalAfterScanningAll: {dur.IntervalAfterScanningAll}s");
            // Plugin.LogDebug($"TakingPaymentInterval: {dur.TakingPaymentInterval}s");
            // Plugin.LogDebug($"FinishingPaymentDuration: {dur.FinishingPaymentDuration}s");
            return dur;
        }

        public static void GiveExpAfterFinishingCheckout(AutomatedCheckout instance)
        {
            CashierSkillManager.Instance.GetOrAssignSkill(instance.Cashier).AddExp(2);
        }
    }
}