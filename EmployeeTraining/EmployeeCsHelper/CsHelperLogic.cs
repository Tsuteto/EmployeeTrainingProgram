using System;
using System.Collections;
using MyBox;
using UnityEngine;
using UnityEngine.AI;

namespace EmployeeTraining.EmployeeCsHelper
{
    public static class CsHelperLogic
    {
        private static readonly PrivateMtdStatic mtdFireForceFinish = new PrivateMtdStatic(typeof(SelfCheckout), "FireForceFinish");
        private static readonly PrivateFldStatic<NavMeshAgent> fldAgent = new PrivateFldStatic<NavMeshAgent>(typeof(CustomerHelper), "m_Agent");

        public static void Init()
        {
            Plugin.Instance.GameLoadedEvent += () =>
            {
                Singleton<ScaleManager>.Instance.ScaleBarcodeApplied += GiveExpOnScaleBarcodeApplied;
            };
            Plugin.Instance.GameQuitEvent += () =>
            {
                Singleton<ScaleManager>.Instance.ScaleBarcodeApplied -= GiveExpOnScaleBarcodeApplied;
            };
        }

        public static void ApplyRapidity(CustomerHelper cshelper)
        {
            var agent = fldAgent.GetValue(cshelper);

            CsHelperSkill skill = CsHelperSkillManager.Instance.GetSkill(cshelper);
            var boost = skill.CustomerHelperWalkingSpeeds[skill.CurrentBoostLevel] / 2f;
            var speed = skill.AgentSpeed * boost;
            agent.speed = speed;
            agent.angularSpeed = skill.AgentAngularSpeed * boost;
            agent.acceleration = skill.AgentAcceleration * boost;
        }

        public static void PerformScanning(SelfCheckout __instance, Checkout m_Checkout, SFXInstance m_CashierSFX, GameObject m_RepairIndicator)
        {

            // Plugin.LogDebug("Called CsHelper PerformScanning");
            // Logger.LogDebug($"__instance={__instance}, m_Checkout={m_Checkout}, m_CashierSFX={m_CashierSFX}, m_RepairIndicator={m_RepairIndicator}");
            CustomerHelper cshelper = __instance.ControlledBy;

            // Logger.LogDebug($"cshelper: {cshelper}");
            CsHelperSkill skill = CsHelperSkillManager.Instance.GetOrAssignSkill(cshelper);
            // Logger.LogDebug($"skill: {skill}");
            float intervalMin = skill.IntervalMin * ((1.5f + (skill.CustomerHelperScanIntervals[skill.CurrentBoostLevel] - 1.5f) * 0.6f) / 1.5f);
            float intervalMax = skill.IntervalMax * (skill.CustomerHelperScanIntervals[skill.CurrentBoostLevel] / 1.5f);
            float scanInterval = UnityEngine.Random.Range(intervalMin, intervalMax);
            // Logger.LogDebug($"valueMin={valueMin} valueMax={valueMax} scanInterval={scanInterval}");

            cshelper.isHelping = true;
            cshelper.isBusy = true;
            __instance.StartCoroutine(Scanning());

            IEnumerator Scanning()
            {
                bool scanned = false;
                while (m_Checkout.Belt.Products.Count > 0 && !m_Checkout.CurrentCustomerGotHit)
                {
                    if (cshelper)
                    {
                        cshelper.ScanAnimation();
                        yield return new WaitForSeconds(scanInterval);
                    }
                    else
                    {
                        mtdFireForceFinish.Invoke(__instance);
                    }
                    if (m_Checkout.Belt.Products.Count <= 0)
                    {
                        break;
                    }
                    m_Checkout.ProductScanned(m_Checkout.Belt.Products[0], true);
                    m_CashierSFX.PlayScanningProductSFX();
                    if (cshelper)
                    {
                        scanInterval = UnityEngine.Random.Range(intervalMin, intervalMax);
                        skill?.AddExp(1);
                        scanned = true;
                    }
                }
                yield return new WaitForSeconds(1f);
                m_RepairIndicator.gameObject.SetActive(false);
                if (cshelper != null)
                {
                    if (scanned) skill?.AddExp(5);

                    cshelper.isHelping = false;
                    cshelper.isBusy = false;
                    cshelper.AssignControllable(null);
                    cshelper.HelpControllable(null);
                }

                Singleton<CheckoutManager>.Instance.SelfCheckoutsNeedHelp.SafeRemove(__instance);
                CustomerHelperControllableHelper.SyncAll();
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

        public static void GiveExpOnScaleBarcodeApplied(object sender, EventArgs e)
        {
            Scale scale = sender as Scale;
            if (scale == null) return;

            if (scale.ControlledBy != null)
            {
                CsHelperSkill skill = CsHelperSkillManager.Instance.GetOrAssignSkill(scale.ControlledBy);
                skill?.AddExp(5);
            }
        }
    }
}