using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using EmployeeTraining.Employee;
using MyBox;
using UnityEngine;
using UnityEngine.AI;

namespace EmployeeTraining.EmployeeSecurity
{
    public class SecurityLogic
    {

        // RestockingState
        private static readonly PrivateMtdStatic<bool> mtdIsDisplayAvailable = new PrivateMtdStatic<bool>(typeof(RestockingState), "IsDisplayAvailable", new System.Type[]{typeof(int)});

        public static void SetSpeed(SecurityGuardAnimationController controller, int speedLevel, NavMeshAgent agent)
        {
            SecurityGuard security = controller.GetComponent<SecurityGuard>();

            SecuritySkill skill = SecuritySkillManager.Instance.GetSkill(security);
            speedLevel = Mathf.Clamp(speedLevel, 0, skill.RunningSpeeds.Count - 1);
            var boost = skill.RunningSpeeds[speedLevel] / 2f; // 2f when not boosted
            var speed = skill.AgentSpeed * boost;
            agent.speed = speed;
            agent.angularSpeed = skill.AgentAngularSpeed * boost;
            agent.acceleration = skill.AgentAcceleration * boost;
        }

        public static IEnumerator Move(SecurityGuardAnimationController controller, Vector3 target, int speedLevel, NavMeshAgent agent)
        {
            SecurityGuard security = controller.GetComponent<SecurityGuard>();
            SecuritySkill skill = SecuritySkillManager.Instance.GetSkill(security);
            speedLevel = Mathf.Clamp(speedLevel, 0, skill.RunningSpeeds.Count - 1);
            controller.SetSpeed(speedLevel);
            var boost = skill.RunningSpeeds[speedLevel] / 2f;

            yield return controller.StartCoroutine(
                EmployeeLogicHelper.MoveTo(security, target, agent, boost, skill.TurningSpeed, 5f));
        }

        public static void OnShoplifterDetected(ChaseState state)
        {
                SecuritySkill skill = SecuritySkillManager.Instance.GetSkill(state.securityGuard);
                skill.AddExp(2);
        }

        public static void OnShoplifterBeaten(Customer customer, bool isHitByGuard, SecurityGuard security)
        {
            if (isHitByGuard && security != null)
            {
                SecuritySkill skill = SecuritySkillManager.Instance.GetSkill(security);
                skill.AddExp(4);
            }
        }

    public static void FindNearbyProducts(CollectingState state, float m_DetectRadius)
    {
        SecuritySkill skill = SecuritySkillManager.Instance.GetSkill(state.securityGuard);
        var securityGuard = state.securityGuard;
        var sc = state.sc;
        var crateOpeningTime = skill.CrateOpeningTime;
        var collectingIntv = skill.CollectingIntv;

        List<Product> _SpreadedProducts = securityGuard.ProductsToCollect.ToList();
        if (_SpreadedProducts.Count == 0)
        {
            Collider[] array = Physics.OverlapSphere(sc.transform.position, m_DetectRadius);
            foreach (Collider collider in array)
            {
                if (collider.transform.gameObject.layer == 27 && !(collider.transform.position.y > 0.5f) && collider.TryGetComponent<Product>(out var component))
                {
                    _SpreadedProducts.Add(component);
                }
            }
        }

        securityGuard.StartCoroutine(Delay());
        IEnumerator Delay()
        {
            yield return OpenCrate();
            foreach (Product item in _SpreadedProducts)
            {
                securityGuard.AddProductIntoCrate(item);
                yield return new WaitForSeconds(collectingIntv);
            }

            securityGuard.ProductsToCollect.Clear();
            yield return null;
            securityGuard.StateRestocking();
        }
        IEnumerator OpenCrate()
        {
            securityGuard.GuardCrate.IsEnabled = true;
            yield return new WaitForSeconds(crateOpeningTime);
        }
    }

        public static IEnumerator ProductRestockLoop(RestockingState state, Restocker m_Restocker, Crate m_Crate, List<DisplaySlot> slots)
        {
            SecuritySkill skill = SecuritySkillManager.Instance.GetSkill(state.securityGuard);

            var sc = state.sc;
            var m_SecurityGuard = state.securityGuard;
            var productPlacingIntv = skill.ProductPlacingIntv;
            var rotationTime = skill.RotationTime;

            List<Product> list = new List<Product>();
            list.AddRange(m_Crate.UnlimitedProducts);
            list.AddRange(m_Crate.Products);
            foreach (Product item in list)
            {
                if (item == null)
                {
                    Debug.LogWarning("Guard: Product was null");
                    continue;
                }

                int productID = item.ProductSO.ID;
                if (Singleton<DisplayManager>.Instance.GetDisplaySlots(productID, false, slots) <= 0)
                {
                    Debug.LogWarning("Guard: No display slot!");
                    continue;
                }

                DisplaySlot targetDisplaySlot = slots.Where(x => !x.Full).FirstOrDefault();
                if (!targetDisplaySlot)
                {
                    Debug.LogWarning("Guard: No empty display slot!");
                    continue;
                }

                targetDisplaySlot.OccupiedRestocker = m_Restocker;
                if (Vector3.Distance(targetDisplaySlot.InteractionPosition, m_SecurityGuard.transform.position) > 0.4f)
                {
                    IEnumerator routine = sc.SecurityGuard.Controller.Move(targetDisplaySlot.InteractionPosition - targetDisplaySlot.InteractionPositionForward * 0.3f, 0);
                    yield return sc.SecurityGuard.StartCoroutine(routine);
                }

                sc.SecurityGuard.transform.DOKill();
                sc.SecurityGuard.transform.DORotateQuaternion(targetDisplaySlot.InteractionRotation, 0.4f);
                yield return new WaitForSeconds(rotationTime);
                if (!mtdIsDisplayAvailable.Invoke(state, productID))
                {
                    Debug.LogWarning("Guard: No available display slot!");
                    continue;
                }

                Product product = m_Crate.RemoveProduct(productID);
                if (!product)
                {
                    Debug.LogWarning("Guard: Remove product failed!");
                    continue;
                }

                product.gameObject.SetActive(value: true);
                targetDisplaySlot.AddProduct(productID, product);                
                skill.AddExp(1);
                yield return new WaitForSeconds(productPlacingIntv);
                targetDisplaySlot.OccupiedRestocker = null;
            }

            if (m_SecurityGuard.ShouldChase)
            {
                m_SecurityGuard.StateChase();
            }
            else
            {
                m_SecurityGuard.StateIdle();
            }
        }


    }
}