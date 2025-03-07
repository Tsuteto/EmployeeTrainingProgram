using System.Collections.Generic;
using System.Linq;
using EmployeeTraining.Localization;
using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EmployeeTraining.Employee
{
    public abstract class EmployeeTrainingProgressItem<S> : MonoBehaviour where S : IEmployeeSkill
    {
        internal static readonly StringLocalizer Localizer = Plugin.Localizer;
        internal S skill;
        internal GameObject unlockApprovalObj;
        internal Toggle gaugeToggle;
        internal Button trainingBtn;
        internal Button unlockBtn;
        internal TextMeshProUGUI expValue;
        internal Slider expSlider;
        internal StringLocalizeTranslator level;
        internal StringLocalizeTranslator ninjaLabel;
        internal GameObject unlockBtnObj;
        internal GameObject trainBtnObj;
        internal TextMeshProUGUI priceText;
        private TextMeshProUGUI wage;
        private readonly Dictionary<Grade, RoadmapObjects> roadmap = new Dictionary<Grade, RoadmapObjects>();

        private void Awake()
        {
            skill.OnExpChanged += ExpChanged;
            skill.OnLevelChanged += LevelChanged;

            Plugin.LogDebug("Called EmployeeTrainingProgressItem.Awake");
            MoneyManager moneyMgr = Singleton<MoneyManager>.Instance;
            moneyMgr.onMoneyTransition += MoneyChanged;

            Plugin.LogDebug("- TrainingCashierItem.Setup Head Gauge Toggle");
            gaugeToggle = transform.Find("Head Gauge Toggle").GetComponent<Toggle>();
            gaugeToggle.isOn = skill.IsGaugeDisplayed;
            gaugeToggle.onValueChanged = new Toggle.ToggleEvent();
            gaugeToggle.onValueChanged.AddListener(GaugeToggleChanged);
            trainingBtn = transform.Find("Interaction Zone/Training Button").GetComponent<Button>();
            trainingBtn.onClick = new Button.ButtonClickedEvent();
            trainingBtn.onClick.AddListener(TrainingBtnClicked);
            unlockBtn = transform.Find("Interaction Zone/Unlock Button").GetComponent<Button>();
            unlockBtn.onClick = new Button.ButtonClickedEvent();
            unlockBtn.onClick.AddListener(UnlockBtnClicked);

            expValue = transform.Find("Elements/Info/Exp Value").GetComponent<TextMeshProUGUI>();
            Plugin.LogDebug($"- expValue: {expValue}");
            expSlider = transform.Find("Elements/Info/Exp Slider").GetComponent<Slider>();
            Plugin.LogDebug($"- expSlider: {expSlider}");
            level = transform.Find("Elements/Info/Level").GetComponent<StringLocalizeTranslator>();
            Plugin.LogDebug($"- level: {level}");

            wage = transform.Find("Elements/Info/Detail Params/Daily Wage/Value").GetComponent<TextMeshProUGUI>();
            Plugin.LogDebug($"- wage: {wage}");

            // Plugin.LogDebug("TrainingCashierItem.Setup Roadmap");
            foreach (Grade g in Grade.List)
            {
                var sliderObj = transform.Find($"Elements/Info/Roadmap/{g.Name}/Slider").gameObject;
                roadmap.Add(g, new RoadmapObjects{
                    slider=sliderObj.GetComponent<Slider>(),
                    sliderObj=sliderObj.gameObject,
                    sealObj= transform.Find($"Elements/Info/Roadmap/{g.Name}/Seal").gameObject,
                    checkmarkObj=sliderObj.transform.Find("Checkmark").gameObject
                });
            }

            ninjaLabel = transform.Find($"Elements/Info/Roadmap/{Grade.Ninja.Name}/Label").GetComponent<StringLocalizeTranslator>();
            Plugin.LogDebug($"- ninjaLabel: {ninjaLabel}");

            unlockBtnObj = transform.Find("Interaction Zone/Unlock Button").gameObject;
            trainBtnObj = transform.Find("Interaction Zone/Training Button").gameObject;
            priceText = transform.Find("Interaction Zone/Total Price Text").GetComponent<TextMeshProUGUI>();

            SetupDetailParams();

            Plugin.LogDebug("- Called TrainingProgressItem.UpdateExp");
            Plugin.LogDebug("- UpdateExp");
            UpdateExp();
            Plugin.LogDebug("- UpdateLevel");
            UpdateLevel();

            Plugin.LogDebug("Completed setting up TrainingCashierItem");
        }

        public void Setup(S skill, GameObject unlockApprovalObj)
        {
            // Plugin.LogDebug($"Setting up TrainingCashierItem: CashierID={skill.CashierId}");
            this.skill = skill;
            this.unlockApprovalObj = unlockApprovalObj;
        }

        internal abstract void SetupDetailParams();

        private void GaugeToggleChanged(bool toggled)
        {
            skill.IsGaugeDisplayed = toggled;
            skill.ExpGaugeObj.SetActive(toggled);
        }

        private void UnlockBtnClicked()
        {
            var nextGrade = Grade.List.FirstOrDefault(g => g.Order == skill.Grade.Order + 1);
            if (nextGrade != null)
            {
                var message = unlockApprovalObj.transform.Find("Window BG/Message").GetComponent<StringLocalizeTranslator>();
                var employeeName = Plugin.Localizer.Get($"{skill.JobName} Name").Translate(skill.Id);
                message.Translate(employeeName, Plugin.Localizer.Get(nextGrade.Name).Translate(), nextGrade.WageCashier.ToMoneyText(12));

                var approveBtn = unlockApprovalObj.transform.Find("Window BG/Approve Button").GetComponent<Button>();
                approveBtn.onClick = new Button.ButtonClickedEvent();
                approveBtn.onClick.AddListener(UnlockApproved);

                unlockApprovalObj.SetActive(true);
            }
        }

        private void UnlockApproved()
        {
            MoneyManager money = Singleton<MoneyManager>.Instance;
            float? cost = skill.GetCostToUpgrade();
            if (cost != null && money.HasMoney(cost.Value))
            {
                money.MoneyTransition(-cost.Value, MoneyManager.TransitionType.STAFF, true);
                skill.UnlockGrade();
                UpdateExp();
                UpdateLevel();
                unlockApprovalObj.SetActive(false);
            }
        }

        private void TrainingBtnClicked()
        {
            MoneyManager money = Singleton<MoneyManager>.Instance;
            float? cost = skill.GetCostToLevelup();
            if (cost != null && money.HasMoney(cost.Value))
            {
                money.MoneyTransition(-cost.Value, MoneyManager.TransitionType.STAFF, true);
                skill.TrainToLevelup();
            }
        }

        internal virtual void UpdateExp()
        {
            // Plugin.LogDebug($"skill: {skill}");
            expValue.text = skill.GetExpDisplay();
            var expForNext = skill.GetExpForNext();
            if (expForNext != null)
            {
                expSlider.value = 1f - skill.Exp / (float)expForNext.Value;
            }
            else
            {
                expSlider.value = 0;
            }
            // Plugin.LogDebug($"wage: {wage}");
            wage.text = skill.Wage.ToMoneyText(12);

            // Plugin.LogDebug($"unlockBtnObj: {unlockBtnObj}");
            unlockBtnObj.SetActive(skill.IsUnlockNeeded());
            // Plugin.LogDebug($"trainBtnObj: {trainBtnObj}");
            trainBtnObj.SetActive(!skill.IsUnlockNeeded());

            var price = skill.IsUnlockNeeded() ? skill.GetCostToUpgrade() : skill.GetCostToLevelup();

            // Plugin.LogDebug($"priceText: {priceText}");
            if (price != null)
            {
                priceText.text = price.Value.ToMoneyText(10f);
            }
            else
            {
                priceText.text = "-----";
            }

            if (expForNext != null)
            {
                var objs = roadmap[skill.Grade];
                // Plugin.LogDebug($"roadmap objs: {objs}");
                objs.sliderObj.SetActive(true);
                objs.slider.value = CalcProgress();
                objs.sealObj.SetActive(false);
                objs.checkmarkObj.SetActive(false);
            }
        }

        private void UpdateLevel()
        {
            level.Translate(skill.Lvl, (TranslateArgHandler)(() => Localizer.Get(skill.Grade.Name).Translate()));

            UpdateButtons();

            foreach (KeyValuePair<Grade, RoadmapObjects> entry in roadmap)
            {
                var g = entry.Key;
                var objs = entry.Value;
                bool isMaxLvl = skill.GetExpForNext() == null;
                if (g < skill.Grade || isMaxLvl)
                {
                    objs.sliderObj.SetActive(true);
                    objs.slider.value = 0;
                    objs.checkmarkObj.SetActive(true);
                }
                else if (g == skill.Grade)
                {
                    objs.sliderObj.SetActive(true);
                    objs.slider.value = CalcProgress();
                    objs.sealObj.SetActive(false);
                    objs.checkmarkObj.SetActive(false);
                }
                else
                {
                    objs.sliderObj.SetActive(false);
                    objs.sealObj.SetActive(true);
                }

                if (g == Grade.Ninja)
                {
                    ninjaLabel.Key = skill.Grade == Grade.Ninja ? "Ninja" : "?";
                    ninjaLabel.Translate();
                }
            }
        }

        private float CalcProgress()
        {
            var expForNext = skill.GetExpForNext();
            if (expForNext != null)
            {
                return 1f - (skill.Lvl + (float)skill.Exp / expForNext.Value - skill.Grade.LvlMin) / (skill.Grade.LvlMax - skill.Grade.LvlMin + 1);
            }
            else
            {
                return 0;
            }
        }

        private void UpdateButtons()
        {
            var trainingCost = skill.GetCostToLevelup();
            trainingBtn.interactable = trainingCost != null
                    && Singleton<MoneyManager>.Instance.HasMoney(trainingCost.Value);

            var unlockCost = skill.GetCostToUpgrade();
            unlockBtn.interactable = unlockCost != null
                    && Singleton<MoneyManager>.Instance.HasMoney(unlockCost.Value);
        }

        private void ExpChanged(int exp, bool incr)
        {
            UpdateExp();
        }

        private void LevelChanged(bool incr)
        {
            UpdateLevel();
        }

        private void MoneyChanged(float _amount, MoneyManager.TransitionType _type)
        {
            UpdateButtons();
        }

        private void OnDestroy()
        {
            // Plugin.LogDebug($"Destroying TrainingCashierItem");
            if (skill != null)
            {
                // Plugin.LogDebug($"skill: {skill}");
                skill.OnExpChanged -= ExpChanged;
                skill.OnLevelChanged -= LevelChanged;
                // this.skill = default(S);
                MoneyManager moneyMgr = Singleton<MoneyManager>.Instance;
                moneyMgr.onMoneyTransition -= MoneyChanged;
            }
        }

        struct RoadmapObjects
        {
            public GameObject sealObj;
            public GameObject sliderObj;
            public Slider slider;
            public GameObject checkmarkObj;
        }
    }
}
