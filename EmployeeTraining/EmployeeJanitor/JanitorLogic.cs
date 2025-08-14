using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DG.Tweening;
using Lean.Pool;
using MyBox;
using UnityEngine;
using UnityEngine.AI;

namespace EmployeeTraining.EmployeeJanitor
{
    public class JanitorLogic
    {
        // Janitor
        private static readonly PrivateFldStatic<NavMeshAgent> fldJanitorAgent = new PrivateFldStatic<NavMeshAgent>(typeof(Janitor), "m_Agent");

        // Dust
        private static readonly PrivatePropStatic<Coroutine> propDustCleaningCoroutine = new PrivatePropStatic<Coroutine>(typeof(Dust), "CleaningCoroutine", BindingFlags.Instance | BindingFlags.Public);
        private static readonly PrivateMtdStatic mtdDustPlayCleanEffect = new PrivateMtdStatic(typeof(Dust), "PlayCleanEffect");
        private static readonly PrivateFldStatic<bool> fldDustIsClean = new PrivateFldStatic<bool>(typeof(Dust), "m_IsClean");
        private static readonly PrivateFldStatic<bool> fldDustIsCleaning = new PrivateFldStatic<bool>(typeof(Dust), "m_IsCleaning");

        public static void ApplyRapidity(Janitor janitor)
        {
            var agent = fldJanitorAgent.GetValue(janitor);

            JanitorSkill skill = JanitorSkillManager.Instance.GetSkill(janitor);
            var boost = skill.WalkingSpeeds[skill.CurrentBoostLevel] / 2f; // 2f when not boosted
            var speed = skill.AgentSpeed * boost;
            agent.speed = speed;
            agent.angularSpeed = skill.AgentAngularSpeed * boost;
            agent.acceleration = skill.AgentAcceleration * boost;
        }

        public static void CleaningForJanitor(Janitor janitor, Dust dust,
            List<GameObject> m_DustList, List<ParticleSystem> m_BubbleParticles, ParticleSystem m_BubbleParticle,
            float m_DustCleaningMultiplier, int m_DustExp,
            float m_AlphaMax, float m_AlphaMin, int AlphaClip)
        {
            JanitorSkill skill = JanitorSkillManager.Instance.GetOrAssignSkill(janitor);

            if (fldDustIsClean.GetValue(dust))
            {
                janitor.isCleaning = false;
                janitor.TargetObject = null;
            }
            else
            {
                propDustCleaningCoroutine.Set(dust, janitor.StartCoroutine(CleaningForJanitorCoroutine(
                    janitor, dust, skill,
                    m_DustList, m_BubbleParticles, m_BubbleParticle,
                    m_DustCleaningMultiplier, m_DustExp,
                    m_AlphaMax, m_AlphaMin, AlphaClip)));
            }

        }

        public static float GetCleanDuration(Janitor janitor)
        {
            JanitorSkill skill = JanitorSkillManager.Instance.GetOrAssignSkill(janitor);
            return skill.CleaningDuration;
        }

        // Dirt on the floor
        public static void OnDirtCleaned(Janitor janitor)
        {
            if (janitor.TargetObject != null)
            {
                JanitorSkill skill = JanitorSkillManager.Instance.GetOrAssignSkill(janitor);
                skill.AddExp(2);
            }
        }

        // Garbage on the floor
        public static void OnGarbageCleaned(CleanGarbageAction action)
        {
            var janitor = action.janitorParam.value;
            if (janitor.isGarbageCollected)
            {
                JanitorSkill skill = JanitorSkillManager.Instance.GetOrAssignSkill(janitor);
                skill.AddExp(1);
            }
        }

        // Dust on the window
        private static IEnumerator CleaningForJanitorCoroutine(Janitor janitor, Dust dust, JanitorSkill skill,
            List<GameObject> m_DustList, List<ParticleSystem> m_BubbleParticles, ParticleSystem m_BubbleParticle,
            float m_DustCleaningMultiplier, int m_DustExp,
            float m_AlphaMax, float m_AlphaMin, int AlphaClip)
        {
            float cleaningDuration = skill.CleaningDuration;
            float elapsedTime = dust.CurrentDustPercentage * cleaningDuration;
            while (elapsedTime < cleaningDuration && dust.m_CurrentAlpha < 1f)
            {
                elapsedTime = dust.CurrentDustPercentage * cleaningDuration;
                float num = Mathf.Clamp01(elapsedTime / cleaningDuration);
                float num2 = Time.deltaTime * m_DustCleaningMultiplier * Mathf.Min(m_AlphaMax / Mathf.Max(m_AlphaMax - num, 0.05f), 3f);
                num += num2;
                dust.m_CurrentAlpha = num * (m_AlphaMax - m_AlphaMin) + m_AlphaMin;
                foreach (GameObject gameObject in m_DustList)
                {
                    gameObject.GetComponent<MeshRenderer>().material.DOKill(false);
                    gameObject.GetComponent<MeshRenderer>().material.SetFloat(AlphaClip, dust.m_CurrentAlpha);
                }
                dust.dustingSaveData.DustingAlpha = dust.m_CurrentAlpha;
                Singleton<GarbageGenerator>.Instance.SaveDustingData(dust.dustingSaveData);
                Singleton<GarbageGenerator>.Instance.OnDirtLevelChange?.Invoke();
                if (Mathf.Abs(num - m_AlphaMax) < 0.01f)
                {
                    fldDustIsClean.SetValue(dust, true);
                    if (m_BubbleParticles.Count > 0)
                    {
                        dust.StopCleaningEffect();
                    }
                    mtdDustPlayCleanEffect.Invoke(dust);
                    Singleton<StoreLevelManager>.Instance.AddPoint(m_DustExp);
                    Singleton<SaveManager>.Instance.Progression.CleanedGlassCount++;
                    Singleton<GarbageGenerator>.Instance.OnGlassCleaned?.Invoke(false);
                    skill.AddExp(2);
                }
                else if (Mathf.Abs(num - m_AlphaMax) >= 0.01f && m_BubbleParticles.Count < 1)
                {
                    foreach (GameObject dust2 in m_DustList)
                    {
                        ParticleSystem particleSystem = LeanPool.Spawn(
                            m_BubbleParticle, dust2.transform.position,
                            Quaternion.Euler(m_BubbleParticle.transform.eulerAngles.x, m_BubbleParticle.transform.eulerAngles.y, 90f), null);
                        m_BubbleParticles.Add(particleSystem);
                        Vector3 size = dust2.GetComponent<Renderer>().bounds.size;
                        size = new Vector3(size.y, size.z, size.x);
                        ParticleSystem.ShapeModule shape = particleSystem.shape;
                        shape.scale = size;
                        ParticleSystem.ShapeModule shape2 = particleSystem.transform.GetChild(0).GetComponent<ParticleSystem>().shape;
                        shape2.scale = size;
                        ParticleSystem.ShapeModule shape3 = particleSystem.transform.GetChild(1).GetComponent<ParticleSystem>().shape;
                        shape3.scale = size;
                        particleSystem.Play();
                    }
                    if (m_BubbleParticles.Count < 1)
                    {
                        Debug.Log("Bubble particle could not be spawned");
                        yield break;
                    }
                }
                yield return null;
            }
            fldDustIsClean.SetValue(dust, true);
            if (m_BubbleParticles.Count > 0)
            {
                dust.StopCleaningEffect();
            }
            mtdDustPlayCleanEffect.Invoke(dust);
            dust.m_CurrentAlpha = m_AlphaMax;
            Singleton<SaveManager>.Instance.Progression.CleanedGlassCount++;
            Singleton<GarbageGenerator>.Instance.OnGlassCleaned?.Invoke(false);
            fldDustIsCleaning.SetValue(dust, false);
            janitor.DustCleaningAnimation(false);
            janitor.Sponge.SetActive(false);
            janitor.isCleaning = false;
            janitor.TargetObject = null;
        }

    }
}