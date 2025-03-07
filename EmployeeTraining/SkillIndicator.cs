using EmployeeTraining.Localization;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using HarmonyLib;
using Lean.Pool;
using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using EmployeeTraining.Employee;

namespace EmployeeTraining
{
    public class SkillIndicator : MonoBehaviour
    {
        private StringLocalizeTranslator lvlText;
        private TMP_Text expText;
        private float orthoCamSmoothness;
        private Transform player;
        private IEmployeeSkill skill;
        private StringLocalizer localizer;

        // === Slider control ===
        private const float FILL_SPEED = 1f;
        private Slider expSlider;
        private InGameTextIndicator storePointIndicator;
        private Transform indicatorPosition;
        private bool changingBarForNewLevel;
        private bool fillingBar;
        private Tween fillingTween;

        public void SetUp(IEmployeeSkill skill, StringLocalizer localizer)
        {
            this.enabled = true;
            this.skill = skill;
            this.localizer = localizer;
        }

        private void Start()
        {
            this.player = Singleton<PlayerController>.Instance.transform;
            this.orthoCamSmoothness = 0.7f;

            this.lvlText = this.transform.Find("Lvl Text").GetComponent<StringLocalizeTranslator>();
            // Plugin.LogDebug($"Lvl Text: {this.lvlText}");

            this.expText = this.transform.Find("Exp Text").GetComponent<TextMeshProUGUI>();
            // Plugin.LogDebug($"Exp Text: {this.expText}");

            this.storePointIndicator = Traverse.Create(
                    GameObject.Find("---UI---/Ingame Canvas/Store Point Slider").GetComponent<StorePointSlider>())
                .Field("m_StorePointIndicator").GetValue<InGameTextIndicator>();
            // Plugin.LogDebug($"m_StorePointIndicator: {this.storePointIndicator}");

            this.expSlider = this.GetComponentInChildren<Slider>(true);
            // Plugin.LogDebug($"m_ExpSlider: {this.expSlider}");
            this.indicatorPosition = this.expSlider.transform;
            // Plugin.LogDebug($"m_IndicatorPosition: {this.indicatorPosition}");
            this.skill.OnExpChanged += this.ExpChanged;
            this.skill.OnLevelChanged += this.LevelChanged;
            this.LoadStoreLevelInfo();
        }

        private void LoadStoreLevelInfo()
        {
            this.lvlText.Translate(this.skill.Lvl);
            this.expText.text = this.skill.GetExpDisplay();
            this.UpdateBar();
        }

        private void OnDisable()
        {
            // if (this.m_LvlText)
            // {
            //     this.m_LvlText.enabled = false;
            // }
        }

        private void Update()
        {
            Vector3 playerPos = this.player.position;
            playerPos.y = base.transform.position.y;
            base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.LookRotation(base.transform.position - playerPos), this.orthoCamSmoothness);
        }

        private void ExpChanged(int amount, bool increased)
        {
            this.expText.text = this.skill.GetExpDisplay();

            if (amount != 0)
            {
                var ind = LeanPool.Spawn(this.storePointIndicator, this.indicatorPosition, false);
                ind.Setup(this.localizer.Get("Popup Exp").Translate($"{(increased ? "+" : "-")}{amount}"), increased);
                ind.transform.localScale = new Vector3(0.0025f, 0.0025f, 0.0025f);
            }

            if (this.changingBarForNewLevel)
            {
                return;
            }
            if (this.fillingBar)
            {
                this.fillingTween?.Kill(false);
                this.fillingTween = null;
                this.fillingBar = false;
            }
            this.UpdateBar();
        }

        private void LevelChanged(bool levelUp)
        {
            this.lvlText.Translate(this.skill.Lvl);

            var ind = LeanPool.Spawn(this.storePointIndicator, lvlText.transform, false);
            ind.Setup("LEVEL UP!", true);
            ind.transform.localScale = new Vector3(0.003f, 0.003f, 0.003f);

            if (this.fillingBar)
            {
                this.fillingTween?.Kill(false);
                this.fillingTween = null;
                this.fillingBar = false;
            }
            this.changingBarForNewLevel = true;
            float duration;
            if (levelUp)
            {
                bool isMax = skill.GetExpForNext() == null;
                duration = (1f - this.expSlider.value) * 1f;
                TweenerCore<float, float, FloatOptions> tweenerCore = DOTween.To(
                    () => this.expSlider.value, x => this.expSlider.value = x, 1f, duration);
                tweenerCore.onComplete += () => {
                    if (!isMax)
                    {
                        this.expSlider.value = 0f;
                        this.changingBarForNewLevel = false;
                    }
                    this.UpdateBar();
                };
                return;
            }
            else
            {
                duration = this.expSlider.value * 1f;
                TweenerCore<float, float, FloatOptions> tweenerCore = DOTween.To(
                    () => this.expSlider.value, x => this.expSlider.value = x, 0f, duration);
                tweenerCore.onComplete += () => {
                    this.expSlider.value = 1f;
                    this.changingBarForNewLevel = false;
                    this.UpdateBar();
                };
            }
        }

        private void UpdateBar()
        {
            float? expForNext = this.skill.GetExpForNext();
            if (expForNext != null)
            {
                this.fillingBar = true;
                this.fillingTween = DOTween.To(
                    () => this.expSlider.value,
                    x => this.expSlider.value = x,
                    this.skill.Exp / expForNext.Value,
                    this.GetDuration()
                );
                this.fillingTween.OnComplete(() => this.fillingBar = false);
            }
            else
            {
                this.expSlider.value = 1;
            }
        }

        private float GetDuration()
        {
            return Mathf.Abs((float)this.skill.Exp / (float)this.skill.GetExpForNext() - this.expSlider.value) * 1f;
        }

        private void OnDestroy()
        {
            // Plugin.LogDebug($"Destroying SkillIndicator");
            if (this.skill != null)
            {
                this.skill.OnExpChanged -= this.ExpChanged;
                this.skill.OnLevelChanged -= this.LevelChanged;
                this.skill = null;
            }
        }
    }
}