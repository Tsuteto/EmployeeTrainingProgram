using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace EmployeeTraining.Employee
{
    class EmployeeLogicHelper
    {
        public static IEnumerator MoveTo(MonoBehaviour employee, Vector3 target, NavMeshAgent Agent, float boost, float turningSpeed, float maxDist)
        {
            var linearMotion = Agent.speed >= 10;

            if (NavMesh.SamplePosition(target, out NavMeshHit navMeshHit, maxDist, -1))
            {
                Agent.SetDestination(navMeshHit.position);
            }
            else
            {
                Agent.SetDestination(target);
            }
            while (Vector3.Distance(employee.transform.position, Agent.destination) > Agent.stoppingDistance)
            {
                if (linearMotion)
                {
                    Agent.velocity = (Agent.steeringTarget - employee.transform.position).normalized * Agent.speed;
                    employee.transform.forward = Agent.steeringTarget - employee.transform.position;
                }
                else
                {
                    if (Agent.velocity.magnitude > 0f)
                    {
                        employee.transform.rotation = Quaternion.Slerp(employee.transform.rotation, Quaternion.LookRotation(Agent.velocity), turningSpeed * boost * Time.deltaTime);
                    }
                }
                yield return null;
            }
        }

    }
}