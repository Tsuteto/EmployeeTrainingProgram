using System.Collections.Generic;
using System.Linq;
using EmployeeTraining.Localization;
using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EmployeeTraining
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
            skill.OnExpChanged += this.ExpChanged;
            skill.OnLevelChanged += this.LevelChanged;

            // Plugin.LogDebug("Called TrainingProgressItem.Awake");
            MoneyManager moneyMgr = Singleton<MoneyManager>.Instance;
            moneyMgr.onMoneyTransition += this.MoneyChanged;

            // Plugin.LogDebug("TrainingCashierItem.Setup Head Gauge Toggle");
            this.gaugeToggle = transform.Find("Head Gauge Toggle").GetComponent<Toggle>();
            this.gaugeToggle.isOn = skill.IsGaugeDisplayed;
            this.gaugeToggle.onValueChanged = new Toggle.ToggleEvent();
            this.gaugeToggle.onValueChanged.AddListener(this.GaugeToggleChanged);
            this.trainingBtn = transform.Find("Interaction Zone/Training Button").GetComponent<Button>();
            this.trainingBtn.onClick = new Button.ButtonClickedEvent();
            this.trainingBtn.onClick.AddListener(this.TrainingBtnClicked);
            this.unlockBtn = transform.Find("Interaction Zone/Unlock Button").GetComponent<Button>();
            this.unlockBtn.onClick = new Button.ButtonClickedEvent();
            this.unlockBtn.onClick.AddListener(this.UnlockBtnClicked);

            this.expValue = this.transform.Find("Elements/Info/Exp Value").GetComponent<TextMeshProUGUI>();
            // Plugin.LogDebug($"expValue: {expValue}");
            this.expSlider = this.transform.Find("Elements/Info/Exp Slider").GetComponent<Slider>();
            // Plugin.LogDebug($"expSlider: {expSlider}");
            this.level = this.transform.Find("Elements/Info/Level").GetComponent<StringLocalizeTranslator>();
            // Plugin.LogDebug($"level: {level}");

            this.wage = this.transform.Find("Elements/Info/Detail Params/Daily Wage/Value").GetComponent<TextMeshProUGUI>();
            // Plugin.LogDebug($"wage: {wage}");

            // Plugin.LogDebug("TrainingCashierItem.Setup Roadmap");
            foreach (Grade g in Grade.List)
            {
                var sliderObj = this.transform.Find($"Elements/Info/Roadmap/{g.Name}/Slider").gameObject;
                roadmap.Add(g, new RoadmapObjects{
                    slider=sliderObj.GetComponent<Slider>(),
                    sliderObj=sliderObj.gameObject,
                    sealObj=this.transform.Find($"Elements/Info/Roadmap/{g.Name}/Seal").gameObject,
                    checkmarkObj=sliderObj.transform.Find("Checkmark").gameObject
                });
            }

            this.ninjaLabel = this.transform.Find($"Elements/Info/Roadmap/{Grade.Ninja.Name}/Label").GetComponent<StringLocalizeTranslator>();
            // Plugin.LogDebug($"ninjaLabel: {ninjaLabel}");

            this.unlockBtnObj = this.transform.Find("Interaction Zone/Unlock Button").gameObject;
            this.trainBtnObj = this.transform.Find("Interaction Zone/Training Button").gameObject;
            this.priceText = this.transform.Find("Interaction Zone/Total Price Text").GetComponent<TextMeshProUGUI>();

            this.SetupDetailParams();

            // Plugin.LogDebug("Called TrainingProgressItem.UpdateExp");
            // Plugin.LogDebug("UpdateExp");
            this.UpdateExp();
            // Plugin.LogDebug("UpdateLevel");
            this.UpdateLevel();

            // Plugin.LogDebug("Completed setting up TrainingCashierItem");
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
            this.skill.IsGaugeDisplayed = toggled;
            this.skill.ExpGaugeObj.SetActive(toggled);
        }

        private void UnlockBtnClicked()
        {
            var nextGrade = Grade.List.FirstOrDefault(g => g.Order == skill.Grade.Order + 1);
            if (nextGrade != null)
            {
                var message = this.unlockApprovalObj.transform.Find("Window BG/Message").GetComponent<StringLocalizeTranslator>();
                var employeeName = Plugin.Localizer.Get($"{skill.JobName} Name").Translate(skill.Id);
                message.Translate(employeeName, Plugin.Localizer.Get(nextGrade.Name).Translate(), nextGrade.WageCashier.ToMoneyText(12));

                var approveBtn = this.unlockApprovalObj.transform.Find("Window BG/Approve Button").GetComponent<Button>();
                approveBtn.onClick = new Button.ButtonClickedEvent();
                approveBtn.onClick.AddListener(this.UnlockApproved);
                
                this.unlockApprovalObj.SetActive(true);
            }
        }

        private void UnlockApproved()
        {
            MoneyManager money = Singleton<MoneyManager>.Instance;
            float? cost = this.skill.GetCostToUpgrade();
            if (cost != null && money.HasMoney(cost.Value))
            {
                money.MoneyTransition(-cost.Value, MoneyManager.TransitionType.STAFF, true);
                this.skill.UnlockGrade();
                this.UpdateExp();
                this.UpdateLevel();
                this.unlockApprovalObj.SetActive(false);
            }
        }

        private void TrainingBtnClicked()
        {
            MoneyManager money = Singleton<MoneyManager>.Instance;
            float? cost = this.skill.GetCostToLevelup();
            if (cost != null && money.HasMoney(cost.Value))
            {
                money.MoneyTransition(-cost.Value, MoneyManager.TransitionType.STAFF, true);
                this.skill.TrainToLevelup();
            }
        }

        internal virtual void UpdateExp()
        {
            // Plugin.LogDebug($"skill: {skill}");
            this.expValue.text = skill.GetExpDisplay();
            var expForNext = skill.GetExpForNext();
            if (expForNext != null)
            {
                this.expSlider.value = 1f - skill.Exp / (float)expForNext.Value;
            }
            else
            {
                this.expSlider.value = 0;
            }
            // Plugin.LogDebug($"wage: {wage}");
            this.wage.text = skill.Wage.ToMoneyText(12);

            // Plugin.LogDebug($"unlockBtnObj: {unlockBtnObj}");
            this.unlockBtnObj.SetActive(skill.IsUnlockNeeded());
            // Plugin.LogDebug($"trainBtnObj: {trainBtnObj}");
            this.trainBtnObj.SetActive(!skill.IsUnlockNeeded());

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
                objs.slider.value = this.CalcProgress();
                objs.sealObj.SetActive(false);
                objs.checkmarkObj.SetActive(false);
            }
        }

        private void UpdateLevel()
        {
            this.level.Translate(skill.Lvl, (TranslateArgHandler)(() => Localizer.Get(skill.Grade.Name).Translate()));
            
            this.UpdateButtons();

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
                    objs.slider.value = this.CalcProgress();
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
                    this.ninjaLabel.Key = skill.Grade == Grade.Ninja ? "Ninja" : "?";
                    this.ninjaLabel.Translate();
                }
            }
        }

        private float CalcProgress()
        {
            var expForNext = this.skill.GetExpForNext();
            if (expForNext != null)
            {
                return 1f - (this.skill.Lvl + ((float)this.skill.Exp / expForNext.Value) - this.skill.Grade.LvlMin) / (this.skill.Grade.LvlMax - this.skill.Grade.LvlMin + 1);
            }
            else
            {
                return 0;
            }
        }

        private void UpdateButtons()
        {
            var trainingCost = this.skill.GetCostToLevelup();
            this.trainingBtn.interactable = trainingCost != null
                    && Singleton<MoneyManager>.Instance.HasMoney(trainingCost.Value);

            var unlockCost = this.skill.GetCostToUpgrade();
            this.unlockBtn.interactable = unlockCost != null
                    && Singleton<MoneyManager>.Instance.HasMoney(unlockCost.Value);
        }

        private void ExpChanged(int exp, bool incr)
        {
            this.UpdateExp();
        }

        private void LevelChanged(bool incr)
        {
            this.UpdateLevel();
        }

        private void MoneyChanged(float _amount, MoneyManager.TransitionType _type)
        {
            this.UpdateButtons();
        }

        private void OnDestroy()
        {
            // Plugin.LogDebug($"Destroying TrainingCashierItem");
            if (this.skill != null)
            {
                // Plugin.LogDebug($"skill: {skill}");
                this.skill.OnExpChanged -= this.ExpChanged;
                this.skill.OnLevelChanged -= this.LevelChanged;
                // this.skill = default(S);
                MoneyManager moneyMgr = Singleton<MoneyManager>.Instance;
                moneyMgr.onMoneyTransition -= this.MoneyChanged;
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
