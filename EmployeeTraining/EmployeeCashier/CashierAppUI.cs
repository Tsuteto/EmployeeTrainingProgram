using EmployeeTraining.Employee;
using EmployeeTraining.EmployeeCashier;
using EmployeeTraining.Localization;
using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace EmployeeTraining.TrainingApp
{
    public class EmployeeCashierAppUI : EmployeeAppUI<CashierTrainingProgressItem, CashierSkill>, IEmployeeAppUI
    {
        public void ComposeTabButton(GameObject managementApp, GameObject taskbarBtnsObj, out GameObject tabBtnObj)
        {
            var orgTabBtnObj = managementApp.transform.Find("Taskbar/Buttons").transform.GetChild(0).gameObject;
            // Plugin.LogDebug($"orgTabBtnObj: {orgTabBtnObj}");

            tabBtnObj = GameObject.Instantiate(orgTabBtnObj, taskbarBtnsObj.transform, true);
            // Plugin.LogDebug($"cashierBtnObj: {tabBtnObj}");
            tabBtnObj.name = "Cashiers Tab Button";
            GameObject.Destroy(tabBtnObj.GetComponentInChildren<LocalizeStringEvent>());
            var icon = Utils.FindResourceByName<Sprite>("icon_shopping_52");
            // Plugin.LogDebug($"icon: {icon}");
            var tabIconObj = tabBtnObj.transform.Find("Tab Icon");
            tabIconObj.GetComponent<Image>().sprite = icon;
            tabIconObj.localPosition = new Vector3(-32, 0, 0);
            tabBtnObj.transform.Find("Text (TMP)").GetOrAddComponent<StringLocalizeTranslator>().Key = "Cashiers";
            var btn = tabBtnObj.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.m_PersistentCalls = new PersistentCallGroup();
        }

        public void ComposeTabButton(GameObject baseTabBtnObj, GameObject taskbarBtnsObj)
        {
        }

        void IEmployeeAppUI.CreateStatusPanel(GameObject basePanelObj, GameObject panelTmpl)
        {
            this.StatusPanelTmpl = basePanelObj;
        }

        public void ComposeStatusPanel()
        {
            ComposeStatusPanel("icon_shopping_52", "Cashier Name",
                detailCellSize: new Vector2(95, 25),
                detailParamCount: 3,
                detailParams: new DetailParam[]
                {
                    new DetailParam() { Name="SPM", LabelKey="Scans Per Minute", ValueKey="SPM Range", ValueArgs=new string[]{"<MIN>", "<MAX>"} },
                    new DetailParam() { Name="Payment Time", LabelKey="Payment Time", ValueKey="Payment Time Sec.", ValueArgs=new string[]{"<SEC>"} },
                    new DetailParamWage(),
                }
            );

        }

        void IEmployeeAppUI.CreateTabScreen(GameObject baseTabObj, GameObject tabsObj)
        {
            this.TabObj = baseTabObj;
        }

        public void ComposeTabScreen()
        {
            this.ListObj = this.TabObj.transform.Find("Scroll View/Viewport/Status");
            this.ComposeNoEmployee("No Cashiers", "NO CASHIERS HIRED", new Color32(150, 206, 255, 255));
        }

        public void AddTabEvent(GameObject taskbarObj, TabManager tabMgr)
        {
            this.AddTabBtnEvent(taskbarObj, tabMgr, "Buttons/Cashiers Tab Button", "cashiers");
        }

        public void RegisterEmployee(IEmployeeSkill skill, GameObject panelTmpl, GameObject unlockApprWindowObj)
        {
            this.RegisterEmployee("Cashier Status Panel", (CashierSkill)skill, unlockApprWindowObj);
        }

        public void DeleteEmployee(IEmployeeSkill skill)
        {
            this.DeleteEmployee((CashierSkill)skill, Singleton<EmployeeManager>.Instance.CashiersData);
        }
    }
}