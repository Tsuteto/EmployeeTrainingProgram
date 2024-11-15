using System;
using System.Collections;
using MyBox;
using UnityEngine;
using UnityEngine.AI;

namespace EmployeeTraining
{
    public static class CsHelperLogic
    {
        public static void PerformScanning(SelfCheckout __instance, Checkout m_Checkout, SFXInstance m_CashierSFX, GameObject m_RepairIndicator)
        {

            // Plugin.LogDebug("Called CsHelper PerformScanning");
            // Logger.LogDebug($"__instance={__instance}, m_Checkout={m_Checkout}, m_CashierSFX={m_CashierSFX}, m_RepairIndicator={m_RepairIndicator}");
            CustomerHelper cshelper = __instance.CustomerHelper;

            // Logger.LogDebug($"cshelper: {cshelper}");
            CsHelperSkill skill = null;
            float valueMin = 0;
            float valueMax = 0;
            float scanInterval = 0;
            if (cshelper)
            {
                skill = CsHelperSkillManager.Instance.GetOrAssignSkill(cshelper);
                // Logger.LogDebug($"skill: {skill}");
                valueMin = skill.IntervalMin * ((1.5f + (skill.CustomerHelperScanIntervals[skill.CurrentBoostLevel] - 1.5f) * 0.6f) / 1.5f);
                valueMax = skill.IntervalMax * (skill.CustomerHelperScanIntervals[skill.CurrentBoostLevel] / 1.5f);
                scanInterval = UnityEngine.Random.Range(valueMin, valueMax);
                // Logger.LogDebug($"valueMin={valueMin} valueMax={valueMax} scanInterval={scanInterval}");

                cshelper.isHelping = true;
                cshelper.AddCheckoutToList(m_Checkout);
                cshelper.checkoutToStay = m_Checkout;
            }
            __instance.StartCoroutine(Scanning());

            IEnumerator Scanning()
            {
                bool scanned = false;
                while (m_Checkout.Belt.Products.Count > 0)
                {
                    if (cshelper) {
                        cshelper.ScanAnimation();
                        yield return new WaitForSeconds(scanInterval);
                    }
                    if (m_Checkout.Belt.Products.Count <= 0)
                    {
                        break;
                    }
                    m_Checkout.ProductScanned(m_Checkout.Belt.Products[0], true);
                    m_CashierSFX.PlayScanningProductSFX();
                    if (cshelper)
                    {
                        scanInterval = UnityEngine.Random.Range(valueMin, valueMax);
                        skill?.AddExp(1);
                        scanned = true;
                    }
                }
                yield return new WaitForSeconds(1f);
                if (cshelper)
                {
                    cshelper.isHelping = false;
                    if (scanned) skill?.AddExp(10);
                }
                m_RepairIndicator.gameObject.SetActive(false);
		        Singleton<CheckoutManager>.Instance.SelfCheckoutsNeedHelp.Remove(m_Checkout);
            }
        }

        public static IEnumerator MoveTo(CustomerHelper cshelper, Vector3 target, NavMeshAgent agent)
        {
            CsHelperSkill skill = CsHelperSkillManager.Instance.GetSkill(cshelper);
            var boost = skill.CustomerHelperWalkingSpeeds[skill.CurrentBoostLevel] / 2f;
            var speed = skill.AgentSpeed * boost;
            var linearMotion = speed >= 10;
            agent.speed = speed;
            agent.angularSpeed = skill.AgentAngularSpeed * boost;
            agent.acceleration = skill.AgentAcceleration * boost;
            // Plugin.LogDebug($"Agent: speed={this.Agent.speed}, angularSpeed={this.Agent.angularSpeed}, acceleration={this.Agent.acceleration}");

            if (NavMesh.SamplePosition(target, out NavMeshHit navMeshHit, 3.4028235E+38f, -1))
            {
                agent.SetDestination(navMeshHit.position);
            }
            while (Vector3.Distance(cshelper.transform.position, agent.destination) > agent.stoppingDistance)
            {
                if (linearMotion)
                {
                    agent.velocity = (agent.steeringTarget - cshelper.transform.position).normalized * agent.speed;
                    cshelper.transform.forward = agent.steeringTarget - cshelper.transform.position;
                }
                else
                {
                    if (agent.velocity.magnitude > 0f)
                    {
                        cshelper.transform.rotation = Quaternion.Slerp(cshelper.transform.rotation, Quaternion.LookRotation(agent.velocity), skill.TurningSpeed * boost * Time.deltaTime);
                    }
                }
                yield return null;
            }
        }
    }
}