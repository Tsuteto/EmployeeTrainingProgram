using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using __Project__.Scripts.Computer;
using __Project__.Scripts.ControllerInputModule;
using __Project__.Scripts.ControllerInputModule.EventHandlers;
using __Project__.Scripts.UI;
using EmployeeTraining.Localization;
using HarmonyLib;
using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Components;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace EmployeeTraining
{
    public class PCTrainingApp : Singleton<PCTrainingApp>
    {
        private AppWindow baseApp;

        private GameObject unlockApprWindowObj;
        private GameObject panelTmpl;
        private Transform CashierListObj;
        private Transform RestockerListObj;
        private Transform CsHelperListObj;
        public GameObject NoCashiersObj;
        public GameObject NoRestockersObj;
        public GameObject NoCsHelperObj;

        private void Awake()
        {
            // Plugin.LogDebug("Initializing Training App");
            //this._computerObj = GameObject.Find("---GAME---/Computer");
            var screenObject = GameObject.Find("---GAME---/Computer/Screen");
            GameObject appShortcuts = GameObject.Find("---GAME---/Computer/Screen/Desktop Canvas/App Shortcuts");
            GameObject orgApp = GameObject.Find("---GAME---/Computer/Screen/Management App");
            GameObject orgShortcutIcon = GameObject.Find("---GAME---/Computer/Screen/Desktop Canvas/App Shortcuts/Management.Exe");

            this.LoadScreen(screenObject, orgApp);
            Plugin.LogDebug("Loaded Training App");

            // Adding shortcut
            // Plugin.LogDebug($"Duplicating shortcut");
            var shortcutIconObj = UnityEngine.Object.Instantiate<GameObject>(orgShortcutIcon, appShortcuts.transform, true);
            shortcutIconObj.name = "Training.Exe";

            // Plugin.LogDebug($"Tweaking icon grid");
            var gridLayout = appShortcuts.transform.GetComponent<GridLayoutGroup>();
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedRowCount;
            gridLayout.constraintCount = 5;
            gridLayout.cellSize = new Vector2(60f, 40f);

            // Plugin.LogDebug($"Modifying training icon name");
            var iconTextObj = shortcutIconObj.transform.Find("Name").gameObject;
            iconTextObj.AddComponent<StringLocalizeTranslator>().Key = "TRAINING";

            GameObject.Destroy(iconTextObj.GetComponent<LocalizeStringEvent>());

            // Plugin.LogDebug($"Generate shortcut click event");
            UnityEvent unityEvent = new UnityEvent();
            unityEvent.AddListener(new UnityAction(PCTrainingApp.OpenApp));
            Traverse.Create(shortcutIconObj.GetComponent<ButtonHandler>())
                    .Field("m_OnClick").SetValue(unityEvent);

            // Plugin.LogDebug("Completed initializing Training App");
        }

        private void LoadScreen(GameObject screenObject, GameObject managementApp)
        {
            // Compose gameobject
            var app = Object.Instantiate(managementApp, screenObject.transform, true);
            app.name = "Training App";
            app.SetActive(false);

            this.baseApp = app.GetComponent<AppWindow>();
            // Plugin.LogDebug($"seApplication: {this.baseApp}");
            this.baseApp.WindowName = "Training";

            this.unlockApprWindowObj = this.ComposeUnlockApprovalWindow(app);

            var appTitleObj = app.transform.Find("App Title").gameObject;
            // Plugin.LogDebug($"Composing App Title");
            ComposeAppTitle(appTitleObj);

            var taskbarObj = app.transform.Find("Taskbar").gameObject;
            taskbarObj.transform.SetParent(app.transform);
            // Plugin.LogDebug($"Composing Taskbar");
            ComposeTaskbar(taskbarObj, managementApp);

            var tabsObj = app.transform.Find("Tabs").gameObject;
            // Plugin.LogDebug($"Composing Tabs");
            ComposeTabs(tabsObj, taskbarObj, managementApp);

            // Plugin.LogDebug($"Adjusting app's position");
            app.transform.position = managementApp.transform.position;
            app.transform.rotation = managementApp.transform.rotation;
            Vector3 localScale = managementApp.transform.localScale;
            app.transform.localScale = new Vector3(localScale.x, localScale.y, localScale.z);

            // Remove gamepad things
            GameObject.Destroy(app.GetComponent<GamePadUIPanel>());
            GameObject.Destroy(app.GetComponent<GamepadUIFunctionLibrary>());
            GameObject.Destroy(app.GetComponent<GamepadUIConfirm>());
            GameObject.Destroy(app.GetComponent<GamepadUIBack>());
            GameObject.Destroy(app.GetComponent<GamepadUIShoulder>());
            GameObject.Destroy(app.GetComponent<GamepadUITrigger>());
        }

        private void ComposeAppTitle(GameObject baseObj)
        {
            // Text
            var textObj = baseObj.transform.Find("Text (TMP)").gameObject;
            GameObject.Destroy(textObj.GetComponent<LocalizeStringEvent>());
            var translator = textObj.AddComponent<StringLocalizeTranslator>();
            translator.Key = "TRAINING";

            // Icon BG
            var iconBgObj = baseObj.transform.Find("Icon BG");
            var iconBg = iconBgObj.GetComponent<Image>();

            // Window icon
            var winIconObj = baseObj.transform.Find("Icon");
            var winIcon = winIconObj.GetComponent<Image>();

            // Exit button
            var exitObj = baseObj.transform.Find("Exit");
            ButtonHandler exitBtnHandler = exitObj.GetComponent<ButtonHandler>();
            MouseClickSFX mouseClickSfx = exitObj.GetComponent<MouseClickSFX>();
            FieldInfo onClick = typeof(ButtonHandler).GetField("m_OnClick", BindingFlags.Instance | BindingFlags.NonPublic);
            UnityEvent exitBtnEvent = new UnityEvent();
            exitBtnEvent.AddListener(() =>
            {
                PCTrainingApp.CloseApp();
                mouseClickSfx.Click();
            });
            onClick.SetValue(exitBtnHandler, exitBtnEvent);
        }

        private void ComposeTaskbar(GameObject taskbarObj, GameObject managementApp)
        {
            var grid = taskbarObj.GetComponentInChildren<GridLayoutGroup>();
            grid.cellSize = new Vector2(85, 20);

            var taskbarBtnsObj = taskbarObj.transform.Find("Buttons").gameObject;
            for (int i = 0; i < taskbarBtnsObj.transform.childCount; i++)
            {
                GameObject.Destroy(taskbarBtnsObj.transform.GetChild(i).gameObject);
            }

            // Cashier tab btn
            var orgTabBtnObj = managementApp.transform.Find("Taskbar/Buttons").transform.GetChild(0).gameObject;
            Plugin.LogDebug($"orgTabBtnObj: {orgTabBtnObj}");
            GameObject cashierBtnObj;
            {
                var tabBtnObj = Object.Instantiate(orgTabBtnObj, taskbarBtnsObj.transform, true);
                Plugin.LogDebug($"cashierBtnObj: {tabBtnObj}");
                tabBtnObj.name = "Cashiers Tab Button";
                GameObject.Destroy(tabBtnObj.GetComponentInChildren<LocalizeStringEvent>());
                var icon = Utils.FindResourceByName<Sprite>("icon_shopping_52");
                Plugin.LogDebug($"icon: {icon}");
                var tabIconObj = tabBtnObj.transform.Find("Tab Icon");
                tabIconObj.GetComponent<Image>().sprite = icon;
                tabIconObj.localPosition = new Vector3(-32, 0, 0);
                tabBtnObj.transform.Find("Text (TMP)").GetOrAddComponent<StringLocalizeTranslator>().Key = "Cashiers";
                var btn = tabBtnObj.GetComponent<Button>();
                btn.onClick.RemoveAllListeners();
                btn.onClick.m_PersistentCalls = new PersistentCallGroup();
                cashierBtnObj = tabBtnObj;
            }

            // Restocker tab btn
            {
                var tabBtnObj = Object.Instantiate(cashierBtnObj, taskbarBtnsObj.transform, true);
                Plugin.LogDebug($"restockerBtnObj: {tabBtnObj}");
                tabBtnObj.name = "Restockers Tab Button";
                var icon = Utils.FindResourceByName<Sprite>("icon_shopping_28");
                Plugin.LogDebug($"icon: {icon}");
                var tabIconObj = tabBtnObj.transform.Find("Tab Icon");
                tabIconObj.GetComponent<Image>().sprite = icon;
                tabIconObj.localPosition = new Vector3(-32, 0, 0);
                tabBtnObj.transform.Find("Text (TMP)").GetOrAddComponent<StringLocalizeTranslator>().Key = "Restockers";
            }

            // Customer Helper tab btn
            {
                var tabBtnObj = Object.Instantiate(cashierBtnObj, taskbarBtnsObj.transform, true);
                Plugin.LogDebug($"csHelperBtnObj: {tabBtnObj}");
                tabBtnObj.name = "Customer Helper Tab Button";
                var icon = Utils.FindResourceByName<Sprite>("icon_shopping_52");
                Plugin.LogDebug($"icon: {icon}");
                var tabIconObj = tabBtnObj.transform.Find("Tab Icon");
                tabIconObj.GetComponent<Image>().sprite = icon;
                tabIconObj.localPosition = new Vector3(-32, 0, 0);
                tabBtnObj.transform.Find("Text (TMP)").GetOrAddComponent<StringLocalizeTranslator>().Key = "Customer Helpers";
            }

            // Remove gamepad things
            GameObject.Destroy(taskbarObj.transform.Find("GPIcon (1)").gameObject);
            GameObject.Destroy(taskbarObj.transform.Find("GPIcon (2)").gameObject);
        }

        private void ComposeTabs(GameObject tabsObj, GameObject taskbarObj, GameObject managementApp)
        {
            /* === Initialize tabs === */
            for (int i = 0; i < tabsObj.transform.childCount; i++)
            {
                var tab = tabsObj.transform.GetChild(i).gameObject;
                if (tab.name != "Licenses Tab")
                {
                    GameObject.Destroy(tab);
                }
            }

            var cashiersTabObj = tabsObj.transform.Find("Licenses Tab").gameObject;
            cashiersTabObj.name = "Cashiers Tab";
            cashiersTabObj.SetActive(true);

            GameObject.Destroy(cashiersTabObj.transform.Find("Purchased License Indicator").gameObject);
            GameObject.Destroy(cashiersTabObj.transform.Find("Unlocked All Licenses").gameObject);
            GameObject.Destroy(cashiersTabObj.transform.GetComponent<LicensesTab>());
            // Remove gamepad things
            GameObject.Destroy(cashiersTabObj.transform.GetComponent<GamepadSelectableParent>());

            {
                var listObj = cashiersTabObj.transform.Find("Licenses Scroll View/Viewport/Content");
                for (int i = 0; i < listObj.childCount; i++)
                {
                    GameObject.Destroy(listObj.GetChild(i).gameObject);
                }
                var scrollViewObj = cashiersTabObj.transform.Find("Licenses Scroll View");
                scrollViewObj.name = "Scroll View";
                var img = scrollViewObj.GetComponent<Image>();
                img.color = new Color(0.8672f, 0.9057f, 0.8159f, 1);
            }


            /* === Compose Cashiers Tab === */
            {
                var winTab = cashiersTabObj.GetComponent<WindowTab>();
                winTab.TabName = "cashiers";

                /* Grid layout */
                // Plugin.LogDebug("Reforming grid layout");
                var listObj = cashiersTabObj.transform.Find("Scroll View/Viewport/Content");
                listObj.name = "Status";
                this.CashierListObj = listObj;
                var gridLayout = listObj.GetComponent<GridLayoutGroup>();
                gridLayout.cellSize = new Vector2(320, 145);
                gridLayout.constraint = GridLayoutGroup.Constraint.Flexible;
                gridLayout.spacing = new Vector2(10, 10);
                listObj.localPosition = new Vector3(0, 95);
                var scrollViewObj = cashiersTabObj.transform.Find("Scroll View");
                var img = scrollViewObj.GetComponent<Image>();
                img.color = new Color32(205, 235, 255, 255);
            }

            /* === Prepare base status panel === */
            // Plugin.LogDebug("Preparing base status panel");
            var orgPanel = managementApp.transform.Find("Tabs/Hiring Tab/Scroll View/Viewport/Content/Cashiers/Image/Scroll View/Viewport/Content/Cashier Item").gameObject;
            this.panelTmpl = new GameObject("Cashier Status Panel Templates");
            var cashierPanelObj = Object.Instantiate(orgPanel, this.panelTmpl.transform, false);
            this.ComposeBasePanel(cashierPanelObj);
            
            var restockerPanelObj = Object.Instantiate(cashierPanelObj, this.panelTmpl.transform, false);
            var csHelperPanelObj = Object.Instantiate(cashierPanelObj, this.panelTmpl.transform, false);

            /* Cashier status panel */
            // Plugin.LogDebug("Preparing cashier detail");
            this.ComposeCashierDetail(cashierPanelObj);

            /* Restocker status panel */
            // Plugin.LogDebug("Preparing restocker detail");
            this.ComposeRestockerDetail(restockerPanelObj);

            /* Customer helper status panel */
            // Plugin.LogDebug("Preparing customer helper detail");
            this.ComposeCsHelperDetail(csHelperPanelObj);

            /* === Compose Restocker Tab === */
            // Plugin.LogDebug("Composing restocker tab");
            var restockersTabObj = GameObject.Instantiate(cashiersTabObj, tabsObj.transform);
            {
                restockersTabObj.name = "Restockers Tab";
                restockersTabObj.SetActive(false);
                var winTab = restockersTabObj.GetComponent<WindowTab>();
                winTab.TabName = "restockers";
                var listObj = restockersTabObj.transform.Find("Scroll View/Viewport/Status");
                this.RestockerListObj = listObj;

                var scrollViewObj = restockersTabObj.transform.Find("Scroll View");
                var img = scrollViewObj.GetComponent<Image>();
                img.color = new Color32(224, 218, 255, 255);
            }

            /* === Compose Customer Helper Tab === */
            // Plugin.LogDebug("Composing restocker tab");
            var csHelperTabObj = GameObject.Instantiate(cashiersTabObj, tabsObj.transform);
            {
                csHelperTabObj.name = "Customer Helpers Tab";
                csHelperTabObj.SetActive(false);
                var winTab = csHelperTabObj.GetComponent<WindowTab>();
                winTab.TabName = "cshelpers";
                var listObj = csHelperTabObj.transform.Find("Scroll View/Viewport/Status");
                this.CsHelperListObj = listObj;

                var scrollViewObj = csHelperTabObj.transform.Find("Scroll View");
                var img = scrollViewObj.GetComponent<Image>();
                img.color = new Color32(228, 255, 251, 255);
            }

            /* === No Cashiers === */
            {
                // Plugin.LogDebug("Adding no cashiers label");
                var naObj = new GameObject("No Cashiers", typeof(TextMeshProUGUI), typeof(StringLocalizeTranslator));
                naObj.transform.SetParent(cashiersTabObj.transform);
                naObj.SetupText(pos: new Vector3(0, 97.9f, 0), size: new Vector2(500, 50), pivot: new Vector2(0.5f, 0.5f),
                        fontsize: 36, align: HorizontalAlignmentOptions.Center, color: new Color32(150, 206, 255, 255),
                        key: "NO CASHIERS HIRED");
                naObj.SetActive(true);
                this.NoCashiersObj = naObj;
            }

            /* === No Restockers === */
            {
                // Plugin.LogDebug("Adding no restockers label");
                var naObj = new GameObject("No Restockers", typeof(TextMeshProUGUI), typeof(StringLocalizeTranslator));
                naObj.transform.SetParent(restockersTabObj.transform);
                naObj.SetupText(pos: new Vector3(0, 97.9f, 0), size: new Vector2(500, 50), pivot: new Vector2(0.5f, 0.5f),
                        fontsize: 36, align: HorizontalAlignmentOptions.Center, color: new Color32(192, 177, 255, 255),
                        key: "NO RESTOCKERS HIRED");
                naObj.SetActive(true);
                this.NoRestockersObj = naObj;
            }

            /* === No Customer Helpers === */
            {
                // Plugin.LogDebug("Adding no restockers label");
                var naObj = new GameObject("No Customer Helpers", typeof(TextMeshProUGUI), typeof(StringLocalizeTranslator));
                naObj.transform.SetParent(csHelperTabObj.transform);
                naObj.SetupText(pos: new Vector3(0, 97.9f, 0), size: new Vector2(500, 50), pivot: new Vector2(0.5f, 0.5f),
                        fontsize: 36, align: HorizontalAlignmentOptions.Center, color: new Color32(150, 212, 206, 255),
                        key: "NO CUSTOMER HELPERS HIRED");
                naObj.SetActive(true);
                this.NoCsHelperObj = naObj;
            }

            /* === Taskbar events === */
            // Plugin.LogDebug("Adding event listeners");
            // Plugin.LogDebug("Registering events in taskbar buttons");
            var tabMgr = tabsObj.GetComponent<TabManager>();
            // Plugin.LogDebug($"tabMgr: {tabMgr}");
            var cashierTabBtn = taskbarObj.transform.Find("Buttons/Cashiers Tab Button").GetComponent<Button>();
            // Plugin.LogDebug($"cashierTabBtn: {cashierTabBtn}");
            cashierTabBtn.onClick.AddListener(() => tabMgr.OpenTab("cashiers"));
            var restockerTabBtn = taskbarObj.transform.Find("Buttons/Restockers Tab Button").GetComponent<Button>();
            // Plugin.LogDebug($"restockerTabBtn: {restockerTabBtn}");
            restockerTabBtn.onClick.AddListener(() => tabMgr.OpenTab("restockers"));
            var csHelperTabBtn = taskbarObj.transform.Find("Buttons/Customer Helper Tab Button").GetComponent<Button>();
            // Plugin.LogDebug($"csHelperTabBtn: {csHelperTabBtn}");
            csHelperTabBtn.onClick.AddListener(() => tabMgr.OpenTab("cshelpers"));
        }

        public void RegisterEmployee(IEmployeeSkill employeeSkill)
        {
            // Plugin.LogDebug($"Registering {employeeSkill} in Training App");
            if (employeeSkill is CashierSkill)
            {
                var skill = employeeSkill as CashierSkill;
                var panelObj = Object.Instantiate(this.panelTmpl.transform.GetChild(0).gameObject, this.CashierListObj.transform, false);
                panelObj.name = "Cashier Status Panel";
                panelObj.AddComponent<CashierTrainingProgressItem>();
                panelObj.SetActive(true);
                skill.TrainingStatusPanelObj = panelObj;
                
                var infoObj = panelObj.transform.Find("Elements/Info").gameObject;
                var nameObj = infoObj.transform.Find("Employee Name").gameObject;
                var nameTranslate = nameObj.GetComponent<StringLocalizeTranslator>();
                nameTranslate.Translate(new string[]{$"{skill.Id}"});

                // Plugin.LogDebug($"Setting up TrainingCashierItem");
                panelObj.GetComponent<CashierTrainingProgressItem>().Setup(skill, this.unlockApprWindowObj);

                this.NoCashiersObj.SetActive(false);
            }
            if (employeeSkill is RestockerSkill)
            {
                var skill = employeeSkill as RestockerSkill;
                var panelObj = Object.Instantiate(this.panelTmpl.transform.GetChild(1).gameObject, this.RestockerListObj.transform, false);
                panelObj.name = "Restocker Status Panel";
                panelObj.AddComponent<RestockerTrainingProgressItem>();
                panelObj.SetActive(true);
                skill.TrainingStatusPanelObj = panelObj;
                
                var infoObj = panelObj.transform.Find("Elements/Info").gameObject;
                var nameObj = infoObj.transform.Find("Employee Name").gameObject;
                var nameTranslate = nameObj.GetComponent<StringLocalizeTranslator>();
                nameTranslate.Translate(new string[]{$"{skill.Id}"});

                // Plugin.LogDebug($"Setting up TrainingCashierItem");
                panelObj.GetComponent<RestockerTrainingProgressItem>().Setup(skill, this.unlockApprWindowObj);

                this.NoRestockersObj.SetActive(false);
            }
            if (employeeSkill is CsHelperSkill)
            {
                var skill = employeeSkill as CsHelperSkill;
                var panelObj = Object.Instantiate(this.panelTmpl.transform.GetChild(2).gameObject, this.CsHelperListObj.transform, false);
                panelObj.name = "Customer Helper Status Panel";
                panelObj.AddComponent<CsHelperTrainingProgressItem>();
                panelObj.SetActive(true);
                skill.TrainingStatusPanelObj = panelObj;
                
                var infoObj = panelObj.transform.Find("Elements/Info").gameObject;
                var nameObj = infoObj.transform.Find("Employee Name").gameObject;
                var nameTranslate = nameObj.GetComponent<StringLocalizeTranslator>();
                nameTranslate.Translate(new string[]{$"{skill.Id}"});

                // Plugin.LogDebug($"Setting up TrainingCashierItem");
                panelObj.GetComponent<CsHelperTrainingProgressItem>().Setup(skill, this.unlockApprWindowObj);

                this.NoCsHelperObj.SetActive(false);
            }
        }

        public void DeleteEmployee(IEmployeeSkill skill)
        {
            if (skill is CashierSkill)
            {
                this.DeleteCashier(skill as CashierSkill);
            }
            if (skill is RestockerSkill)
            {
                this.DeleteRestocker(skill as RestockerSkill);
            }
            if (skill is CsHelperSkill)
            {
                this.DeleteCsHelper(skill as CsHelperSkill);
            }
        }

        public void DeleteCashier(CashierSkill c)
        {
            GameObject.Destroy(c.TrainingStatusPanelObj);

            if (Singleton<EmployeeManager>.Instance.CashiersData.Count() == 0)
            {
                this.NoCashiersObj.SetActive(true);
            }
        }

        public void DeleteRestocker(RestockerSkill c)
        {
            // Plugin.LogDebug($"Deleting restocker: TrainingStatusPanelObj={c.TrainingStatusPanelObj}");
            GameObject.Destroy(c.TrainingStatusPanelObj);

            var restockersData = Traverse.Create(Singleton<EmployeeManager>.Instance).Field<List<int>>("m_RestockersData").Value;
            if (restockersData.Count() == 0)
            {
                this.NoRestockersObj.SetActive(true);
            }
        }

        public void DeleteCsHelper(CsHelperSkill c)
        {
            // Plugin.LogDebug($"Deleting customer helper: TrainingStatusPanelObj={c.TrainingStatusPanelObj}");
            GameObject.Destroy(c.TrainingStatusPanelObj);

            var cshelperData = Traverse.Create(Singleton<EmployeeManager>.Instance).Field<List<int>>("m_CustomerHelpersData").Value;
            if (cshelperData.Count() == 0)
            {
                this.NoCsHelperObj.SetActive(true);
            }
        }

        private void ComposeBasePanel(GameObject panelObj)
        {
            var panelRect = panelObj.GetComponent<RectTransform>();
            panelRect.pivot = new Vector2(0, 1);
            panelRect.anchoredPosition = new Vector2(0, 0);
            panelObj.transform.localScale = new Vector3(1, 1, 1);
            panelObj.transform.localPosition = new Vector3(0, 0, 0f);

            // Adjust base Elements
            var elementsObj = panelObj.transform.Find("Elements");
            elementsObj.localPosition = new Vector3(80, -57.5f, 0);

            /*
             * === Interaction Zone ===
             */
            Plugin.LogDebug("Preparing interaction zone of base panel");
            var intrObj = new GameObject("Interaction Zone", typeof(RectTransform), typeof(Image));
            intrObj.transform.SetParent(panelObj.transform);
            intrObj.SetupObject(pos: new Vector3(155, -49.5f, 0), size: new Vector2(152, 20f), pivot: new Vector2(1, 1));
            {
                var img = intrObj.GetComponent<Image>();
                img.sprite = Utils.FindResourceByName<Sprite>("button_corner_square2_23");
                img.type = Image.Type.Sliced;
                img.pixelsPerUnitMultiplier = 2.64f;
                img.color = new Color(0.4235f, 0.5734f, 0.7451f, 1f);
            }

            var trainBtnObj = new GameObject("Training Button", typeof(RectTransform), typeof(Image), typeof(Button), typeof(MouseClickSFX));
            trainBtnObj.transform.SetParent(intrObj.transform);
            trainBtnObj.SetupObject(pos: new Vector3(-5, -10.36f, 0), size: new Vector2(80, 12.9f), pivot: new Vector2(1f, 0.5f));
            {
                var img = trainBtnObj.GetComponent<Image>();
                img.sprite = Utils.FindResourceByName<Sprite>("button_corner_rectangle3_1");
                img.type = Image.Type.Sliced;
                img.pixelsPerUnitMultiplier = 20f;
                img.color = new Color(0.0634f, 0.7075f, 0.3001f, 1);

                var btn = trainBtnObj.GetComponent<Button>();
                btn.image = img;
                btn.colors = UIHelper.COLOR_BLOCK;

                var textObj = new GameObject("Text", typeof(TextMeshProUGUI), typeof(StringLocalizeTranslator));
                textObj.transform.SetParent(trainBtnObj.transform);
                textObj.SetupText(pos: new Vector3(-40, 0, 0), size: new Vector2(75, 12.9f), pivot: new Vector2(0.5f, 0.5f),
                        fontsize: 8f, align: HorizontalAlignmentOptions.Center, key: "Train to Level Up");
                var text = textObj.GetComponent<TextMeshProUGUI>();
                text.autoSizeTextContainer = false;
                text.enableAutoSizing = true;
            }

            var unlockBtnObj = GameObject.Instantiate(trainBtnObj, intrObj.transform);
            unlockBtnObj.name = "Unlock Button";
            unlockBtnObj.SetActive(false);
            var unlockBtnTrans = unlockBtnObj.GetComponentInChildren<StringLocalizeTranslator>();
            unlockBtnTrans.Key = "Unlock Higher Grade";

            var priceObj = new GameObject("Total Price Text", typeof(TextMeshProUGUI));
            priceObj.transform.SetParent(intrObj.transform);
            priceObj.SetupText(pos: new Vector3(-88, -10.36f, 0), size: new Vector2(60, 12.9f), pivot: new Vector2(1f, 0.5f),
                fontsize: 10f, align: HorizontalAlignmentOptions.Right);
            var priceText = priceObj.GetComponent<TextMeshProUGUI>();
            priceText.autoSizeTextContainer = false;
            priceText.enableAutoSizing = true;

            var gaugeToggleObj = new GameObject("Head Gauge Toggle", typeof(RectTransform), typeof(Toggle));
            gaugeToggleObj.transform.SetParent(panelObj.transform);
            gaugeToggleObj.SetupObject(pos: new Vector3(-145, -59.5f), size: new Vector2(12, 12), pivot: new Vector2(0, 0.5f));
            {
                var bgObj = new GameObject("Background", typeof(Image));
                bgObj.transform.SetParent(gaugeToggleObj.transform);
                bgObj.SetupObject(pos: new Vector3(0, 0, 0), size: new Vector2(12, 12), pivot: new Vector2(0.5f, 0.5f));

                var bgImg = bgObj.GetComponent<Image>();
                bgImg.sprite = Utils.FindResourceByName<Sprite>("UISprite");
                bgImg.color = new Color(0.8431f, 0.9333f, 1, 1);
                bgImg.pixelsPerUnitMultiplier = 2;
                bgImg.type = Image.Type.Sliced;

                var chkObj = new GameObject("Checkmark", typeof(Image), typeof(Outline));
                chkObj.transform.SetParent(bgObj.transform);
                chkObj.SetupObject(pos: new Vector3(0, 0, 0), size: new Vector2(9, 9), pivot: new Vector2(0.5f, 0.5f));

                var chkImg = chkObj.GetComponent<Image>();
                chkImg.sprite = Utils.FindResourceByName<Sprite>("icon_check");
                chkImg.color = new Color(0, 0, 0, 1);

                var chkOutline = chkObj.GetComponent<Outline>();
                chkOutline.effectColor = new Color(0, 0, 0, 1);
                chkOutline.effectDistance = new Vector2(0.2f, 0.2f);

                var toggle = gaugeToggleObj.GetComponent<Toggle>();
                toggle.image = bgImg;
                toggle.graphic = chkImg;
            }

            var gaugeLabelObj = new GameObject("Head Gauge Option", typeof(TextMeshProUGUI), typeof(StringLocalizeTranslator));
            gaugeLabelObj.transform.SetParent(panelObj.transform);
            gaugeLabelObj.SetupText(pos: new Vector3(-135, -59.5f, 0), size: new Vector2(130, 10), pivot: new Vector2(0, 0.5f),
                    fontsize: 10f, align: HorizontalAlignmentOptions.Left, key: "Show head gauge");


            /*
             * === INFO ===
             */
            Plugin.LogDebug("Preparing info part of base panel");
            var infoObj = panelObj.transform.Find("Elements/Info").gameObject;
            var infoRect = infoObj.GetComponent<RectTransform>();
            infoRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 310);
            infoRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 115);
            infoRect.anchoredPosition = new Vector2(0, 0);
            infoRect.pivot = new Vector2(0, 1);
            infoObj.transform.localPosition = new Vector3(-75, 52.5f, 0);

            Plugin.LogDebug("- Sweeping unnecessary objects");
            // Plugin.LogDebug($"Hire Button: {panelObj.transform.Find("Hire Button")}");
            GameObject.Destroy(panelObj.GetComponent<CashierItem>());
            GameObject.Destroy(panelObj.transform.Find("Hire Button").gameObject);
            GameObject.Destroy(panelObj.transform.Find("Fire Button").gameObject);
            GameObject.Destroy(infoObj.transform.Find("Description").gameObject);
            GameObject.Destroy(infoObj.transform.Find("Daily Wage").gameObject);
            GameObject.Destroy(infoObj.transform.Find("Hiring Cost").gameObject);
            GameObject.Destroy(infoObj.transform.Find("Requirements").gameObject);
            //Remove gamepad things
            GameObject.Destroy(panelObj.transform.Find("GPIcon").gameObject);
            GameObject.Destroy(panelObj.GetComponent<Selectable>());
            GameObject.Destroy(panelObj.GetComponent<GamepadUISelectable>());
            GameObject.Destroy(panelObj.GetComponent<HiringDropdownScroller>());

            var nameObj = infoObj.transform.Find("Employee Name").gameObject;
            var nameRect = nameObj.GetComponent<RectTransform>();
            nameObj.SetupObject(
                pos: new Vector3(45, -4, 0), pivot: new Vector2(0, 1), size: new Vector2(135, 20));

            Plugin.LogDebug("- Preparing exp part");
            var expObj = new GameObject("Exp Label", typeof(TextMeshProUGUI), typeof(StringLocalizeTranslator));
            expObj.transform.SetParent(infoObj.transform);
            expObj.SetupText(
                pos: new Vector3(190f, -8f, 0f), size: new Vector2(20, 15),
                fontsize: 12f, key: "Exp.");

            var expValueObj = new GameObject("Exp Value", typeof(TextMeshProUGUI));
            expValueObj.transform.SetParent(infoObj.transform);
            expValueObj.SetupText(
                pos: new Vector3(295, -8f, -0f), size: new Vector2(80, 15), pivot: new Vector2(1f, 1f),
                fontsize: 12f, align: HorizontalAlignmentOptions.Right);

            var expSliderObj = this.ComposeSlider(infoObj, "Exp Slider",
                pos: new Vector3(242.5f, -25), sliderSize: new Vector2(105, 10), color: Color.white);
            expSliderObj.SetActive(true);

            var levelObj = new GameObject("Level", typeof(TextMeshProUGUI), typeof(StringLocalizeTranslator));
            levelObj.transform.SetParent(infoObj.transform);
            levelObj.SetupText(
                pos: new Vector3(45f, -25f, -0f), size: new Vector2(150, 15),
                fontsize: 12f, key: "Level", args: new string[]{"<LVL>", "<GRADE>"});

            // Skill detail parameters
            Plugin.LogDebug("- Preparing skill params");
            var paramsObj = new GameObject("Detail Params", typeof(GridLayoutGroup));
            paramsObj.transform.SetParent(infoObj.transform);
            paramsObj.SetupObject(pos: new Vector3(15, -32, 0), size: new Vector2(-15, 25));

            // Roadmap
            Plugin.LogDebug("- Preparing roadmap");
            var roadmapTitleObj = new GameObject("Roadmap Title");
            roadmapTitleObj.transform.SetParent(infoObj.transform);
            roadmapTitleObj.SetupObject(
                pos: new Vector3(15f, -70f, -0f), size: null);

            {
                var titleDivObj = new GameObject("Divider", typeof(RawImage));
                titleDivObj.transform.SetParent(roadmapTitleObj.transform);
                titleDivObj.SetupObject(
                    pos: new Vector3(0f, 0f, 0f), size: new Vector2(280, 1));
                var image = titleDivObj.GetComponent<RawImage>();
                image.texture = Utils.FindResourceByName<Texture2D>("UnityWhite");

                var textObj = new GameObject("Text", typeof(TextMeshProUGUI), typeof(StringLocalizeTranslator));
                textObj.transform.SetParent(roadmapTitleObj.transform);
                textObj.SetupText(
                    pos: new Vector3(0f, -3f, 0f), size: new Vector2(200, 10),
                    fontsize: 6f, key: "Mastery Roadmap");

            }

            var roadmapObj = new GameObject("Roadmap", typeof(GridLayoutGroup));
            roadmapObj.transform.SetParent(infoObj.transform);
            roadmapObj.SetupObject(
                pos: new Vector3(15f, -65f, -0f), size: new Vector2(-15, 35));
            {
                var cellWidth = 270f / 5f;
                var grid = roadmapObj.GetComponent<GridLayoutGroup>();
                grid.cellSize = new Vector2(cellWidth, 35);
                grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                grid.constraintCount = 5;
                grid.spacing = new Vector2(2, 0);

                Vector2 sliderSize = new Vector2(cellWidth, 12f);

                foreach (Grade grade in Grade.List)
                {
                    var gradeObj = new GameObject(grade.Name, typeof(RectTransform));
                    gradeObj.transform.SetParent(roadmapObj.transform);
                    gradeObj.SetupObject(pos: new Vector3(0, 0, 0));

                    var labelObj = new GameObject("Label", typeof(TextMeshProUGUI), typeof(StringLocalizeTranslator));
                    labelObj.transform.SetParent(gradeObj.transform);
                    labelObj.SetupText(pos: new Vector3(0, 0, 0), size: new Vector2(cellWidth - 5, 12), pivot: new Vector2(0.5f, 1f),
                        fontsize: 8f, align: HorizontalAlignmentOptions.Center, color: grade.Color, key: grade.Name);

                    var sliderObj = this.ComposeSlider(gradeObj, "Slider", new Vector3(0, -13, 0), sliderSize, grade.Color);

                    var checkmarkObj = new GameObject("Checkmark", typeof(Image), typeof(Outline));
                    checkmarkObj.transform.SetParent(sliderObj.transform);
                    checkmarkObj.SetupObject(pos: new Vector3(0, 0, 0), size: new Vector2(12, 12), pivot: new Vector2(0.5f, 1f));
                    var checkmarkImg = checkmarkObj.GetComponent<Image>();
                    checkmarkImg.sprite = Utils.FindResourceByName<Sprite>("icon_check");
                    var checkmarkOutline = checkmarkObj.GetComponent<Outline>();
                    checkmarkOutline.effectColor = new Color32(255, 255, 255, 255);
                    checkmarkOutline.effectDistance = new Vector2(0.2f, 0.2f);
                    checkmarkObj.SetActive(false);

                    var sealObj = new GameObject("Seal");
                    sealObj.transform.SetParent(gradeObj.transform);
                    sealObj.SetupObject(pos: new Vector3(0, -13, 0));
                    sealObj.SetActive(false);

                    {
                        var fillObj = new GameObject("Fill", typeof(RectTransform), typeof(Image));
                        fillObj.transform.SetParent(sealObj.transform);
                        fillObj.SetupObject(pos: new Vector3(0, 0, 0), size: sliderSize);
                        var sealRect = fillObj.GetComponent<RectTransform>();
                        sealRect.pivot = new Vector2(0.5f, 1f);
                        var sealImg = fillObj.GetComponent<Image>();
                        sealImg.color = new Color32(34, 74, 96, 255);

                        var textObj = new GameObject("Text", typeof(TextMeshProUGUI), typeof(StringLocalizeTranslator));
                        textObj.transform.SetParent(sealObj.transform);
                        textObj.SetupText(pos: new Vector3(0, 0, 0), size: sliderSize,
                            fontsize: 8f, align: HorizontalAlignmentOptions.Center, color: new Color32(255, 255, 255, 32),
                            pivot: new Vector2(0.5f, 1f), key: "Locked");
                    }
                }
            }
        }

        private void ComposeCashierDetail(GameObject panelObj)
        {
            var infoObj = panelObj.transform.Find("Elements/Info").gameObject;
            var nameObj = infoObj.transform.Find("Employee Name").gameObject;
            GameObject.Destroy(nameObj.GetComponent<LocalizeStringEvent>());
            var name = nameObj.AddComponent<StringLocalizeTranslator>();
            name.Key = "Cashier Name";
            name.Translate(new string[]{"<NO>"});

            var paramsObj = panelObj.transform.Find("Elements/Info/Detail Params");
            {
                var grid = paramsObj.GetComponent<GridLayoutGroup>();
                grid.cellSize = new Vector2(95, 25);
                grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                grid.constraintCount = 3;
                grid.spacing = new Vector2(0, 0);
            }
            // Scans Per Minute
            {
                var spmObj = new GameObject("SPM", typeof(RectTransform));
                spmObj.transform.SetParent(paramsObj.transform);
                spmObj.SetupObject(pos: new Vector3(0, 0, 0), size: null);
                {
                    var labelObj = new GameObject("Label", typeof(TextMeshProUGUI), typeof(StringLocalizeTranslator));
                    labelObj.transform.SetParent(spmObj.transform);
                    labelObj.SetupText(pos: new Vector3(0, 0, 0), size: new Vector2(80, 8), pivot: new Vector2(0.5f, 1),
                        fontsize: 6f, align: HorizontalAlignmentOptions.Center, key: "Scans Per Minute");
                    var valueObj = new GameObject("Value", typeof(TextMeshProUGUI), typeof(StringLocalizeTranslator));
                    valueObj.transform.SetParent(spmObj.transform);
                    valueObj.SetupText(pos: new Vector3(0, -8, 0), size: new Vector2(80, 15), pivot: new Vector2(0.5f, 1),
                        fontsize: 12f, align: HorizontalAlignmentOptions.Center, key: "SPM Range", args: new string[]{"<MIN>", "<MAX>"});
                }
            }

            // Payment Time
            {
                var spmObj = new GameObject("Payment Time", typeof(RectTransform));
                spmObj.transform.SetParent(paramsObj.transform);
                spmObj.SetupObject(pos: new Vector3(0, 0, 0), size: null);
                {
                    var labelObj = new GameObject("Label", typeof(TextMeshProUGUI), typeof(StringLocalizeTranslator));
                    labelObj.transform.SetParent(spmObj.transform);
                    labelObj.SetupText(pos: new Vector3(0, 0, 0), size: new Vector2(80, 8), pivot: new Vector2(0.5f, 1),
                        fontsize: 6f, align: HorizontalAlignmentOptions.Center, key: "Payment Time");
                    var valueObj = new GameObject("Value", typeof(TextMeshProUGUI), typeof(StringLocalizeTranslator));
                    valueObj.transform.SetParent(spmObj.transform);
                    valueObj.SetupText(pos: new Vector3(0, -8, 0), size: new Vector2(80, 15), pivot: new Vector2(0.5f, 1),
                        fontsize: 12f, align: HorizontalAlignmentOptions.Center, key: "Payment Time Sec.", args: new string[]{"<SEC>"});
                }
            }

            // Daily Wage
            {
                var spmObj = new GameObject("Daily Wage", typeof(RectTransform));
                spmObj.transform.SetParent(paramsObj.transform);
                spmObj.SetupObject(pos: new Vector3(0, 0, 0), size: null);
                {
                    var labelObj = new GameObject("Label", typeof(TextMeshProUGUI), typeof(StringLocalizeTranslator));
                    labelObj.transform.SetParent(spmObj.transform);
                    labelObj.SetupText(pos: new Vector3(0, 0, 0), size: new Vector2(80, 8), pivot: new Vector2(0.5f, 1),
                        fontsize: 6f, align: HorizontalAlignmentOptions.Center, key: "Daily Wage");
                    var valueObj = new GameObject("Value", typeof(TextMeshProUGUI));
                    valueObj.transform.SetParent(spmObj.transform);
                    valueObj.SetupText(pos: new Vector3(0, -8, 0), size: new Vector2(80, 15), pivot: new Vector2(0.5f, 1),
                        fontsize: 12f, align: HorizontalAlignmentOptions.Center);
                }
            }
        }

        private void ComposeRestockerDetail(GameObject panelObj)
        {
            var infoObj = panelObj.transform.Find("Elements/Info").gameObject;

            var iconImg = infoObj.transform.Find("Icon").GetComponent<Image>();
            iconImg.sprite = Utils.FindResourceByName<Sprite>("icon_shopping_28");

            var nameObj = infoObj.transform.Find("Employee Name").gameObject;
            GameObject.Destroy(nameObj.GetComponent<LocalizeStringEvent>());
            var name = nameObj.AddComponent<StringLocalizeTranslator>();
            name.Key = "Restocker Name";
            name.Translate(new string[]{"<NO>"});

            // BG
            {
                var img = panelObj.transform.Find("BG").gameObject.GetComponent<Image>();
                img.color = new Color(0.4208f, 0.3171f, 0.5377f, 0.9412f);
            }

            // Interaction zone BG
            {
                var img = panelObj.transform.Find("Interaction Zone").gameObject.GetComponent<Image>();
                img.color = new Color32(150, 134, 226, 255);
            }

            // Exp BG
            {
                var img = panelObj.transform.Find("Elements/Info/Exp Slider/Fill Area/Fill").gameObject.GetComponent<Image>();
                img.color = new Color32(64, 63, 105, 255);
            }

            // Roadmap BG
            foreach (Grade grade in Grade.List)
            {
                var sliderImg = panelObj.transform.Find($"Elements/Info/Roadmap/{grade.Name}/Slider/Fill Area/Fill").gameObject.GetComponent<Image>();
                sliderImg.color = new Color32(64, 63, 105, 255);
                var sealImg = panelObj.transform.Find($"Elements/Info/Roadmap/{grade.Name}/Seal/Fill").gameObject.GetComponent<Image>();
                sealImg.color = new Color32(55, 54, 87, 255);
            }

            var paramsObj = panelObj.transform.Find("Elements/Info/Detail Params");
            {
                var grid = paramsObj.GetComponent<GridLayoutGroup>();
                grid.cellSize = new Vector2(71, 25);
                grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                grid.constraintCount = 4;
                grid.spacing = new Vector2(0, 0);
            }

            // Rapidity
            {
                var spmObj = new GameObject("Rapidity", typeof(RectTransform));
                spmObj.transform.SetParent(paramsObj.transform);
                spmObj.SetupObject(pos: new Vector3(0, 0, 0), size: null);
                {
                    var labelObj = new GameObject("Label", typeof(TextMeshProUGUI), typeof(StringLocalizeTranslator));
                    labelObj.transform.SetParent(spmObj.transform);
                    labelObj.SetupText(pos: new Vector3(0, 0, 0), size: new Vector2(80, 8), pivot: new Vector2(0.5f, 1),
                        fontsize: 6f, align: HorizontalAlignmentOptions.Center, key: "Rapidity");
                    var valueObj = new GameObject("Value", typeof(TextMeshProUGUI), typeof(StringLocalizeTranslator));
                    valueObj.transform.SetParent(spmObj.transform);
                    valueObj.SetupText(pos: new Vector3(0, -8, 0), size: new Vector2(80, 15), pivot: new Vector2(0.5f, 1),
                        fontsize: 12f, align: HorizontalAlignmentOptions.Center, key: "Speed", args: new string[]{"0"});
                }
            }

            // Carrying Capacity
            {
                var capacityObj = new GameObject("Capacity", typeof(RectTransform));
                capacityObj.transform.SetParent(paramsObj.transform);
                capacityObj.SetupObject(pos: new Vector3(0, 0, 0), size: null);
                {
                    var labelObj = new GameObject("Label", typeof(TextMeshProUGUI), typeof(StringLocalizeTranslator));
                    labelObj.transform.SetParent(capacityObj.transform);
                    labelObj.SetupText(pos: new Vector3(0, 0, 0), size: new Vector2(80, 8), pivot: new Vector2(0.5f, 1),
                        fontsize: 6f, align: HorizontalAlignmentOptions.Center, key: "Capacity");
                    var valueObj = new GameObject("Value", typeof(TextMeshProUGUI), typeof(StringLocalizeTranslator));
                    valueObj.transform.SetParent(capacityObj.transform);
                    valueObj.SetupText(pos: new Vector3(0, -8, 0), size: new Vector2(80, 15), pivot: new Vector2(0.5f, 1),
                        fontsize: 12f, align: HorizontalAlignmentOptions.Center, key: "Weight/Height", args: new string[]{"0", "0"});
                }
            }

            // Efficiency
            {
                var spmObj = new GameObject("Dexterity", typeof(RectTransform));
                spmObj.transform.SetParent(paramsObj.transform);
                spmObj.SetupObject(pos: new Vector3(0, 0, 0), size: null);
                {
                    var labelObj = new GameObject("Label", typeof(TextMeshProUGUI), typeof(StringLocalizeTranslator));
                    labelObj.transform.SetParent(spmObj.transform);
                    labelObj.SetupText(pos: new Vector3(0, 0, 0), size: new Vector2(80, 8), pivot: new Vector2(0.5f, 1),
                        fontsize: 6f, align: HorizontalAlignmentOptions.Center, key: "Dexterity");
                    var valueObj = new GameObject("Value", typeof(TextMeshProUGUI), typeof(StringLocalizeTranslator));
                    valueObj.transform.SetParent(spmObj.transform);
                    valueObj.SetupText(pos: new Vector3(0, -8, 0), size: new Vector2(80, 15), pivot: new Vector2(0.5f, 1),
                        fontsize: 12f, align: HorizontalAlignmentOptions.Center, key: "Percentage", args: new string[]{"0"});
                }
            }

            // Daily Wage
            {
                var spmObj = new GameObject("Daily Wage", typeof(RectTransform));
                spmObj.transform.SetParent(paramsObj.transform);
                spmObj.SetupObject(pos: new Vector3(0, 0, 0), size: null);
                {
                    var labelObj = new GameObject("Label", typeof(TextMeshProUGUI), typeof(StringLocalizeTranslator));
                    labelObj.transform.SetParent(spmObj.transform);
                    labelObj.SetupText(pos: new Vector3(0, 0, 0), size: new Vector2(80, 8), pivot: new Vector2(0.5f, 1),
                        fontsize: 6f, align: HorizontalAlignmentOptions.Center, key: "Daily Wage");
                    var valueObj = new GameObject("Value", typeof(TextMeshProUGUI));
                    valueObj.transform.SetParent(spmObj.transform);
                    valueObj.SetupText(pos: new Vector3(0, -8, 0), size: new Vector2(80, 15), pivot: new Vector2(0.5f, 1),
                        fontsize: 12f, align: HorizontalAlignmentOptions.Center);
                }
            }
        }

        private void ComposeCsHelperDetail(GameObject panelObj)
        {
            var infoObj = panelObj.transform.Find("Elements/Info").gameObject;
            var nameObj = infoObj.transform.Find("Employee Name").gameObject;
            GameObject.Destroy(nameObj.GetComponent<LocalizeStringEvent>());
            var name = nameObj.AddComponent<StringLocalizeTranslator>();
            name.Key = "Customer Helper Name";
            name.Translate(new string[]{"<NO>"});

            // BG
            {
                var img = panelObj.transform.Find("BG").gameObject.GetComponent<Image>();
                img.color = new Color(0.349f, 0.6039f, 0.549f, 0.9412f);
            }

            // Interaction zone BG
            {
                var img = panelObj.transform.Find("Interaction Zone").gameObject.GetComponent<Image>();
                img.color = new Color32(68, 200, 182, 255);
            }

            // Exp BG
            {
                var img = panelObj.transform.Find("Elements/Info/Exp Slider/Fill Area/Fill").gameObject.GetComponent<Image>();
                img.color = new Color32(33, 108, 99, 255);
            }

            // Roadmap BG
            foreach (Grade grade in Grade.List)
            {
                var sliderImg = panelObj.transform.Find($"Elements/Info/Roadmap/{grade.Name}/Slider/Fill Area/Fill").gameObject.GetComponent<Image>();
                sliderImg.color = new Color32(33, 108, 99, 255);
                var sealImg = panelObj.transform.Find($"Elements/Info/Roadmap/{grade.Name}/Seal/Fill").gameObject.GetComponent<Image>();
                sealImg.color = new Color32(64, 95, 89, 255);
            }

            var paramsObj = panelObj.transform.Find("Elements/Info/Detail Params");
            {
                var grid = paramsObj.GetComponent<GridLayoutGroup>();
                grid.cellSize = new Vector2(95, 25);
                grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                grid.constraintCount = 3;
                grid.spacing = new Vector2(0, 0);
            }
            // Scans Per Minute
            {
                var spmObj = new GameObject("SPM", typeof(RectTransform));
                spmObj.transform.SetParent(paramsObj.transform);
                spmObj.SetupObject(pos: new Vector3(0, 0, 0), size: null);
                {
                    var labelObj = new GameObject("Label", typeof(TextMeshProUGUI), typeof(StringLocalizeTranslator));
                    labelObj.transform.SetParent(spmObj.transform);
                    labelObj.SetupText(pos: new Vector3(0, 0, 0), size: new Vector2(80, 8), pivot: new Vector2(0.5f, 1),
                        fontsize: 6f, align: HorizontalAlignmentOptions.Center, key: "Scans Per Minute");
                    var valueObj = new GameObject("Value", typeof(TextMeshProUGUI), typeof(StringLocalizeTranslator));
                    valueObj.transform.SetParent(spmObj.transform);
                    valueObj.SetupText(pos: new Vector3(0, -8, 0), size: new Vector2(80, 15), pivot: new Vector2(0.5f, 1),
                        fontsize: 12f, align: HorizontalAlignmentOptions.Center, key: "SPM Range", args: new string[]{"<MIN>", "<MAX>"});
                }
            }

            // Rapidity
            {
                var spmObj = new GameObject("Rapidity", typeof(RectTransform));
                spmObj.transform.SetParent(paramsObj.transform);
                spmObj.SetupObject(pos: new Vector3(0, 0, 0), size: null);
                {
                    var labelObj = new GameObject("Label", typeof(TextMeshProUGUI), typeof(StringLocalizeTranslator));
                    labelObj.transform.SetParent(spmObj.transform);
                    labelObj.SetupText(pos: new Vector3(0, 0, 0), size: new Vector2(80, 8), pivot: new Vector2(0.5f, 1),
                        fontsize: 6f, align: HorizontalAlignmentOptions.Center, key: "Rapidity");
                    var valueObj = new GameObject("Value", typeof(TextMeshProUGUI), typeof(StringLocalizeTranslator));
                    valueObj.transform.SetParent(spmObj.transform);
                    valueObj.SetupText(pos: new Vector3(0, -8, 0), size: new Vector2(80, 15), pivot: new Vector2(0.5f, 1),
                        fontsize: 12f, align: HorizontalAlignmentOptions.Center, key: "Speed", args: new string[]{"0"});
                }
            }

            // Daily Wage
            {
                var spmObj = new GameObject("Daily Wage", typeof(RectTransform));
                spmObj.transform.SetParent(paramsObj.transform);
                spmObj.SetupObject(pos: new Vector3(0, 0, 0), size: null);
                {
                    var labelObj = new GameObject("Label", typeof(TextMeshProUGUI), typeof(StringLocalizeTranslator));
                    labelObj.transform.SetParent(spmObj.transform);
                    labelObj.SetupText(pos: new Vector3(0, 0, 0), size: new Vector2(80, 8), pivot: new Vector2(0.5f, 1),
                        fontsize: 6f, align: HorizontalAlignmentOptions.Center, key: "Daily Wage");
                    var valueObj = new GameObject("Value", typeof(TextMeshProUGUI));
                    valueObj.transform.SetParent(spmObj.transform);
                    valueObj.SetupText(pos: new Vector3(0, -8, 0), size: new Vector2(80, 15), pivot: new Vector2(0.5f, 1),
                        fontsize: 12f, align: HorizontalAlignmentOptions.Center);
                }
            }
        }

        private GameObject ComposeUnlockApprovalWindow(GameObject appObj)
        {
            Sprite basicFrame = Utils.FindResourceByName<Sprite>("Frame_SpeechBubble01");

            var windowObj = new GameObject("Unlock Approval Window");
            windowObj.transform.SetParent(appObj.transform);
            windowObj.SetupObject(pos: new Vector3(0, 28, 0), pivot: new Vector2(0.5f, 0.5f));
            windowObj.SetActive(false);
            var backdrop = new GameObject("Backdrop", typeof(RawImage));
            backdrop.transform.SetParent(windowObj.transform);
            backdrop.SetupObject(pos: new Vector3(0, -40, 0), size: new Vector2(700, 460), pivot: new Vector2(0.5f, 0.5f));
            var backdropImg = backdrop.GetComponent<RawImage>();
            backdropImg.color = new Color(0.0254f, 0.0354f, 0.0566f, 0.9804f);
            var windowBgObj = new GameObject("Window BG", typeof(Image));
            windowBgObj.transform.SetParent(windowObj.transform);
            windowBgObj.SetupObject(pos: new Vector3(0, 12.8f, 0), size: new Vector2(350, 110), pivot: new Vector2(0.5f, 0.5f));
            var windowImg = windowBgObj.GetComponent<Image>();
            windowImg.sprite = basicFrame;
            windowImg.pixelsPerUnitMultiplier = 4;
            windowImg.color = new Color(0.9104f, 0.9654f, 1, 1);
            windowImg.type = Image.Type.Sliced;
            var msgObj = new GameObject("Message", typeof(TextMeshProUGUI), typeof(StringLocalizeTranslator));
            msgObj.transform.SetParent(windowBgObj.transform);
            msgObj.SetupText(pos: new Vector3(0, 45, 0), size: new Vector2(330, 100), pivot: new Vector2(0.5f, 1),
                    fontsize: 12, color: new Color(0, 0, 0, 1), key: "Upgrade warning", args: new string[]{"<NO>", "<GRADE_NAME>", "<WAGE>"});
            var msgText = msgObj.GetComponent<TextMeshProUGUI>();
            msgText.autoSizeTextContainer = false;
            msgText.enableAutoSizing = true;
            msgText.verticalAlignment = VerticalAlignmentOptions.Top;
            msgText.horizontalAlignment = HorizontalAlignmentOptions.Center;

            var apprBtnObj = new GameObject("Approve Button", typeof(Button), typeof(Image), typeof(Outline));
            apprBtnObj.transform.SetParent(windowBgObj.transform);
            apprBtnObj.SetupObject(pos: new Vector3(-60, -55, 0), size: new Vector2(80, 30), pivot: new Vector2(0.5f, 0.5f));
            {
                var img = apprBtnObj.GetComponent<Image>();
                img.sprite = basicFrame;
                img.pixelsPerUnitMultiplier = 4;
                img.color = new Color(0.1033f, 0.8113f, 0.2143f, 1);
                img.type = Image.Type.Sliced;

                var outline = apprBtnObj.GetComponent<Outline>();
                outline.effectColor = new Color(0, 0, 0, 1);
                outline.effectDistance = new Vector2(0.5f, 0.5f);

                var btn = apprBtnObj.GetComponent<Button>();
                btn.image = img;
                btn.colors = UIHelper.COLOR_BLOCK;

                var textObj = new GameObject("Text", typeof(TextMeshProUGUI), typeof(StringLocalizeTranslator));
                textObj.transform.SetParent(apprBtnObj.transform);
                textObj.SetupText(pos: new Vector3(0, 0, 0), size: new Vector2(80, 30), pivot: new Vector2(0.5f, 0.5f),
                        fontsize: 15, align: HorizontalAlignmentOptions.Center, color: new Color(1, 1, 1, 1),
                        key: "Approve");
                var textRect = textObj.GetComponent<RectTransform>();
                textRect.anchoredPosition = new Vector2(0, 2);
            }

            var cancelBtnObj = GameObject.Instantiate(apprBtnObj, windowBgObj.transform);
            cancelBtnObj.name = "Cancel Button";
            cancelBtnObj.SetupObject(pos: new Vector3(60, -55, 0), size: new Vector2(80, 30), pivot: new Vector2(0.5f, 0.5f));
            var cancelBtnImg = cancelBtnObj.GetComponent<Image>();
            cancelBtnImg.color = new Color(0.8118f, 0.2098f, 0.102f, 1);
            var cancelBtnTrsl = cancelBtnObj.GetComponentInChildren<StringLocalizeTranslator>();
            cancelBtnTrsl.Key = "Defer";

            // Plugin.LogDebug($"Injecting button events");
            var cancelBtn = windowObj.transform.Find("Window BG/Cancel Button").GetComponent<Button>();
            cancelBtn.onClick = new Button.ButtonClickedEvent();
            cancelBtn.onClick.AddListener(() => windowObj.SetActive(false));

            return windowObj;
        }

        private GameObject ComposeSlider(GameObject gradeObj, string name, Vector3 pos, Vector2 sliderSize, Color color)
        {
            var sliderPivot = new Vector2(0.5f, 1f);

            var sliderObj = new GameObject(name, typeof(Slider));
            sliderObj.transform.SetParent(gradeObj.transform);
            sliderObj.SetupObject(pos: pos, size: new Vector2(0, 0));
            sliderObj.SetActive(false);

            var bgObj = new GameObject("Background", typeof(RectTransform), typeof(Image));
            bgObj.transform.SetParent(sliderObj.transform);
            bgObj.SetupObject(pos: new Vector3(0, 0, 0), size: sliderSize);

            var fillAreaObj = new GameObject("Fill Area", typeof(RectTransform));
            fillAreaObj.transform.SetParent(sliderObj.transform);
            fillAreaObj.SetupObject(pos: new Vector3(0, 0, 0), size: sliderSize, pivot: new Vector2(0.5f, 0.5f));

            var fillObj = new GameObject("Fill", typeof(CanvasRenderer), typeof(Image));
            fillObj.transform.SetParent(fillAreaObj.transform);
            fillObj.SetupObject(pos: new Vector3(0, 0, 0), size: new Vector2(-2f, -2f), pivot: new Vector2(0.5f, 0.5f));

            var slider = sliderObj.GetComponent<Slider>();
            slider.fillRect = fillObj.GetComponent<RectTransform>();
            slider.direction = Slider.Direction.RightToLeft;
            slider.interactable = false;
            slider.transition = Selectable.Transition.None;

            var bgRect = bgObj.GetComponent<RectTransform>();
            bgRect.pivot = sliderPivot;
            var bgImg = bgObj.GetComponent<Image>();
            bgImg.color = color;

            var fillAreaRect = fillAreaObj.GetComponent<RectTransform>();
            fillAreaRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, sliderSize.x);
            fillAreaRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sliderSize.y);
            fillAreaRect.pivot = sliderPivot;

            var fillImg = fillObj.GetComponent<Image>();
            fillImg.color = new Color32(21, 45, 59, 255);

            return sliderObj;
        }

        private static void OpenApp()
        {
            Singleton<ComputerOperatingSystem>.Instance.OpenApp("Training");
        }

        private static void CloseApp()
        {
            Singleton<ComputerOperatingSystem>.Instance.CloseApp("Training");
        }

        public static void LoadAppToComputerOS()
        {
            Traverse appWindowsFld = Traverse.Create(Singleton<ComputerOperatingSystem>.Instance).Field("m_AppWindows");
            object value = new List<AppWindow>(appWindowsFld.GetValue() as AppWindow[]) { Instance.baseApp }.ToArray();
            appWindowsFld.SetValue(value);
            //Plugin.LogDebug($"Registered Training App: {string.Join(", ", (appWindowsFld.GetValue() as AppWindow[]).Select(w => w.name))}");
        }

    }

    static class UIHelper
    {
        public static readonly Material FONT_MATERIAL = Utils.FindResourceByName<Material>("UptownBoy Atlas Material");
        public static readonly TMP_FontAsset FONT_ASSET = Utils.FindResourceByName<TMP_FontAsset>("UptownBoy SDF");

        public static readonly ColorBlock COLOR_BLOCK = new ColorBlock
        {
            colorMultiplier = 1,
            disabledColor = new Color(0.7843f, 0.7843f, 0.7843f, 0.5f),
            fadeDuration = 0.1f,
            highlightedColor = new Color(0.9608f, 0.9608f, 0.9608f, 1),
            normalColor = new Color(1, 1, 1, 1),
            pressedColor = new Color(0.7843f, 0.7843f, 0.7843f, 1),
            selectedColor = new Color(0.9608f, 0.9608f, 0.9608f, 1)
        };

        public static void SetupObject(this GameObject obj, Vector3 pos, Vector2? size = null, Vector2? pivot = null)
        {
            // Plugin.LogDebug($"Setting up object {obj.name}");
            obj.transform.eulerAngles = new Vector3(0, 0, 0);
            obj.transform.localEulerAngles = new Vector3(0, 0, 0);
            obj.transform.localScale = new Vector3(1, 1, 1);

            var rect = obj.GetComponent<RectTransform>();
            if (rect != null)
            {
                if (size != null)
                {
                    rect.pivot = pivot ?? new Vector2(0, 1);
                    rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.Value.x);
                    rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.Value.y);
                    rect.anchoredPosition = new Vector2(0, 0);
                }
            }
            obj.transform.localPosition = pos;
        }

        public static void SetupText(this GameObject obj, Vector3 pos, Vector2 size,
            float fontsize, HorizontalAlignmentOptions align = HorizontalAlignmentOptions.Left,
            Color? color = null, Vector2? pivot = null, string key = null, string[] args = null)
        {
            // Plugin.LogDebug($"Setting up text field on {obj.name}");
            obj.SetupObject(pos, size, pivot);

            var tmpText = obj.GetComponent<TextMeshProUGUI>();
            tmpText.fontSize = fontsize;
            tmpText.fontSizeMax = fontsize;
            tmpText.fontSizeMin = 0;
            tmpText.color = color ?? new Color(1, 1, 1);
            tmpText.horizontalAlignment = align;
            tmpText.verticalAlignment = VerticalAlignmentOptions.Middle;
            tmpText.fontMaterial = FONT_MATERIAL;
            tmpText.font = FONT_ASSET;
            tmpText.autoSizeTextContainer = false;
            tmpText.enableAutoSizing = true;
            tmpText.enableWordWrapping = false;

            if (key != null)
            {
                var trans = obj.GetComponent<StringLocalizeTranslator>();
                trans.Key = key;
                if (args != null) trans.Translate(args);
            }
        }

        public static void SetRawText(this GameObject baseObj, string name, string text)
        {
            var obj = baseObj.transform.Find(name).gameObject;
            obj.GetComponent<TextMeshProUGUI>().text = text;
        }

    }

}