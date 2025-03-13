
using System.Collections.Generic;
using EmployeeTraining.Localization;
using EmployeeTraining.TrainingApp;
using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace EmployeeTraining.Employee
{

    public abstract class EmployeeAppTab<I, S> where I : EmployeeTrainingProgressItem<S>
                                              where S : IEmployeeSkill
    {
        public GameObject TabObj { get; protected set; }
        public Transform ListObj { get; protected set; }
        public GameObject StatusPanelTmpl { get; protected set; }
        public GameObject NoEmployeeObj { get; protected set; }

        internal GameObject ComposeTabButton(GameObject templateBtnObj, GameObject taskbarBtnsObj, string name, string iconName, string labelKey)
        {
                var tabBtnObj = Object.Instantiate(templateBtnObj, taskbarBtnsObj.transform, true);
                tabBtnObj.name = name;
                var icon = Utils.FindResourceByName<Sprite>(iconName);
                // Plugin.LogDebug($"icon: {icon}");
                var tabIconObj = tabBtnObj.transform.Find("Tab Icon");
                tabIconObj.GetComponent<Image>().sprite = icon;
                tabIconObj.localPosition = new Vector3(-32, 0, 0);
                tabBtnObj.transform.Find("Text (TMP)").GetOrAddComponent<StringLocalizeTranslator>().Key = labelKey;
                tabBtnObj.GetComponent<Button>()
                    .onClick.AddListener(tabBtnObj.GetComponent<MouseClickSFX>().Click);
                return tabBtnObj;
        }

        protected void CreateStatusPanel(GameObject basePanelObj, GameObject panelTmpl)
        {
            this.StatusPanelTmpl = Object.Instantiate(basePanelObj, panelTmpl.transform, false);
        }

        protected class DetailParam
        {
            public string Name;
            public string LabelKey;
            public string ValueKey;
            public string[] ValueArgs;
        }

        protected class DetailParamWage : DetailParam
        {
            public DetailParamWage()
            {
                Name = "Daily Wage";
                LabelKey = "Daily Wage";
                ValueKey = null;
                ValueArgs = null;
            }
        }

        protected void ComposeStatusPanel(string iconName, string nameKey,
            Vector2 detailCellSize, int detailParamCount, DetailParam[] detailParams,
            Color? bgColorPanel = null, Color? bgColorIntr = null, Color? bgColorExp = null,
            Color? bgColorRmSlider = null, Color? bgColorRmSeal = null)
        {
            var panelObj = this.StatusPanelTmpl;

            var infoObj = panelObj.transform.Find("Elements/Info").gameObject;

            var iconImg = infoObj.transform.Find("Icon").GetComponent<Image>();
            iconImg.sprite = Utils.FindResourceByName<Sprite>(iconName);

            var nameObj = infoObj.transform.Find("Employee Name").gameObject;
            Object.Destroy(nameObj.GetComponent<LocalizeStringEvent>());
            var name = nameObj.AddComponent<StringLocalizeTranslator>();
            name.Key = nameKey;
            name.Translate(new string[]{"<NO>"});

            // BG
            if (bgColorPanel != null)
            {
                var img = panelObj.transform.Find("BG").gameObject.GetComponent<Image>();
                img.color = bgColorPanel.Value;
            }

            // Interaction zone BG
            if (bgColorIntr != null)
            {
                var img = panelObj.transform.Find("Interaction Zone").gameObject.GetComponent<Image>();
                img.color = bgColorIntr.Value;
            }

            // Exp BG
            if (bgColorExp != null)
            {
                var img = panelObj.transform.Find("Elements/Info/Exp Slider/Fill Area/Fill").gameObject.GetComponent<Image>();
                img.color = bgColorExp.Value;
            }

            // Roadmap BG
            if (bgColorRmSlider != null && bgColorRmSeal != null)
            {
                foreach (Grade grade in Grade.List)
                {
                    var sliderImg = panelObj.transform.Find($"Elements/Info/Roadmap/{grade.Name}/Slider/Fill Area/Fill").gameObject.GetComponent<Image>();
                    sliderImg.color = bgColorRmSlider.Value;
                    var sealImg = panelObj.transform.Find($"Elements/Info/Roadmap/{grade.Name}/Seal/Fill").gameObject.GetComponent<Image>();
                    sealImg.color = bgColorRmSeal.Value;
                }
            }

            var paramsObj = panelObj.transform.Find("Elements/Info/Detail Params");
            {
                var grid = paramsObj.GetComponent<GridLayoutGroup>();
                grid.cellSize = detailCellSize;
                grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                grid.constraintCount = detailParamCount;
                grid.spacing = new Vector2(0, 0);
            }

            foreach (DetailParam param in detailParams)
            {
                var paramObj = new GameObject(param.Name, typeof(RectTransform));
                paramObj.transform.SetParent(paramsObj.transform);
                paramObj.SetupObject(pos: new Vector3(0, 0, 0), size: null);

                var labelObj = new GameObject("Label", typeof(TextMeshProUGUI), typeof(StringLocalizeTranslator));
                labelObj.transform.SetParent(paramObj.transform);
                labelObj.SetupText(pos: new Vector3(0, 0, 0), size: new Vector2(80, 8), pivot: new Vector2(0.5f, 1),
                    fontsize: 6f, align: HorizontalAlignmentOptions.Center, key: param.LabelKey);

                if (param.ValueKey != null)
                {
                    var valueObj = new GameObject("Value", typeof(TextMeshProUGUI), typeof(StringLocalizeTranslator));
                    valueObj.transform.SetParent(paramObj.transform);
                    valueObj.SetupText(pos: new Vector3(0, -8, 0), size: new Vector2(80, 15), pivot: new Vector2(0.5f, 1),
                        fontsize: 12f, align: HorizontalAlignmentOptions.Center, key: param.ValueKey, args: param.ValueArgs);
                }
                else
                {
                    var valueObj = new GameObject("Value", typeof(TextMeshProUGUI));
                    valueObj.transform.SetParent(paramObj.transform);
                    valueObj.SetupText(pos: new Vector3(0, -8, 0), size: new Vector2(80, 15), pivot: new Vector2(0.5f, 1),
                        fontsize: 12f, align: HorizontalAlignmentOptions.Center);
                }
            }
        }

        protected void CreateTabScreen(GameObject baseTabObj, GameObject tabsObj)
        {
            this.TabObj = Object.Instantiate(baseTabObj, tabsObj.transform);
        }

        protected void ComposeTabScreen(string objName, string tabName, Color bgColor)
        {
            this.TabObj.name = objName;
            this.TabObj.SetActive(false);
            var winTab = this.TabObj.GetComponent<WindowTab>();
            winTab.TabName = tabName;
            var listObj = this.TabObj.transform.Find("Scroll View/Viewport/Status");

            var scrollViewObj = this.TabObj.transform.Find("Scroll View");
            var img = scrollViewObj.GetComponent<Image>();
            img.color = bgColor;

            this.ListObj = listObj;
        }

        protected void ComposeNoEmployee(string objName, string textKey, Color textColor)
        {
            var naObj = new GameObject(objName, typeof(TextMeshProUGUI), typeof(StringLocalizeTranslator));
            naObj.transform.SetParent(this.TabObj.transform);
            naObj.SetupText(pos: new Vector3(0, 97.9f, 0), size: new Vector2(600, 50), pivot: new Vector2(0.5f, 0.5f),
                    fontsize: 36, align: HorizontalAlignmentOptions.Center, color: textColor,
                    key: textKey);
            naObj.SetActive(true);
            this.NoEmployeeObj = naObj;
        }

        protected void AddTabBtnEvent(GameObject taskbarObj, TabManager tabMgr, string tabBtnPath, string tabName)
        {
            var tabBtn = taskbarObj.transform.Find(tabBtnPath).GetComponent<Button>();
            tabBtn.onClick.AddListener(() => tabMgr.OpenTab(tabName));
        }

        protected void RegisterEmployee(string panelName, S skill, GameObject unlockApprWindowObj)
        {
                Plugin.LogDebug($"{this.GetType()}.RegisterEmployee: skill={skill}");
                Plugin.LogDebug($"- StatusPanelTmpl={StatusPanelTmpl}, ListObj={ListObj}");
                var panelObj = Object.Instantiate(StatusPanelTmpl, this.ListObj.transform, false);
                Plugin.LogDebug($"- panelObj={panelObj}");
                panelObj.name = panelName;
                panelObj.AddComponent<I>();
                panelObj.SetActive(true);
                skill.TrainingStatusPanelObj = panelObj;
                
                var infoObj = panelObj.transform.Find("Elements/Info").gameObject;
                Plugin.LogDebug($"- infoObj={infoObj}");
                var nameObj = infoObj.transform.Find("Employee Name").gameObject;
                Plugin.LogDebug($"- nameObj={nameObj}");
                var nameTranslate = nameObj.GetComponent<StringLocalizeTranslator>();
                nameTranslate.Translate(new string[]{$"{skill.Id}"});

                Plugin.LogDebug($"- Setting up EmployeeProgressItem");
                panelObj.GetComponent<I>().Setup(skill, unlockApprWindowObj);
                Plugin.LogDebug($"- Item setup completed");

                this.NoEmployeeObj.SetActive(false);
        }

        protected void DeleteEmployee(S skill, List<int> employeeData)
        {
            Object.Destroy(skill.TrainingStatusPanelObj);

            if (employeeData.Count == 0)
            {
                NoEmployeeObj.SetActive(true);
            }
        }

        public bool Matches(IEmployeeSkill skill)
        {
            return skill is S;
        }
    }
}