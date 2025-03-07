using System.Collections.Generic;
using EmployeeTraining.Employee;
using EmployeeTraining.EmployeeCsHelper;
using HarmonyLib;
using MyBox;
using UnityEngine;
using UnityEngine.UI;

namespace EmployeeTraining.TrainingApp
{
    public class EmployeeCsHelperAppUI : EmployeeAppUI<CsHelperTrainingProgressItem, CsHelperSkill>, IEmployeeAppUI
    {
        public void ComposeTabButton(GameObject baseTabBtnObj, GameObject taskbarBtnsObj)
        {
            this.ComposeTabButton(baseTabBtnObj, taskbarBtnsObj,
                "Customer Helper Tab Button", "icon_shopping_52", "Customer Helpers");
        }

        void IEmployeeAppUI.CreateStatusPanel(GameObject panelObj, GameObject panelTmpl)
        {
            this.CreateStatusPanel(panelObj, panelTmpl);
        }

        public void ComposeStatusPanel()
        {
            this.ComposeStatusPanel(
                "icon_shopping_52", "Customer Helper Name",
                bgColorPanel: new Color(0.349f, 0.6039f, 0.549f, 0.9412f),
                bgColorIntr: new Color32(68, 200, 182, 255),
                bgColorExp: new Color32(33, 108, 99, 255),
                bgColorRmSlider: new Color32(33, 108, 99, 255),
                bgColorRmSeal: new Color32(64, 95, 89, 255),
                detailCellSize: new Vector2(95, 25),
                detailParamCount: 3,
                detailParams: new DetailParam[]
                {
                    new DetailParam() { Name="SPM", LabelKey="Scans Per Minute", ValueKey="SPM Range", ValueArgs=new string[]{"<MIN>", "<MAX>"} },
                    new DetailParam() { Name="Rapidity", LabelKey="Rapidity", ValueKey="Speed", ValueArgs=new string[]{"0"} },
                    new DetailParamWage(),
                }
            );
        }

        void IEmployeeAppUI.CreateTabScreen(GameObject baseTabObj, GameObject tabsObj)
        {
            this.CreateTabScreen(baseTabObj, tabsObj);
        }

        public void ComposeTabScreen()
        {
            this.ComposeTabScreen("Customer Helpers Tab", "cshelpers", new Color32(228, 255, 251, 255));
            this.ComposeNoEmployee("No Customer Helpers", "NO CUSTOMER HELPERS HIRED", new Color32(150, 212, 206, 255));
        }

        public void AddTabEvent(GameObject taskbarObj, TabManager tabMgr)
        {
            this.AddTabBtnEvent(taskbarObj, tabMgr, "Buttons/Customer Helper Tab Button", "cshelpers");
        }

        public void RegisterEmployee(IEmployeeSkill skill, GameObject panelTmpl, GameObject unlockApprWindowObj)
        {
            this.RegisterEmployee("Customer Helper Status Panel", (CsHelperSkill)skill, unlockApprWindowObj);
        }

        public void DeleteEmployee(IEmployeeSkill skill)
        {
            List<int> employeeData = Traverse.Create(Singleton<EmployeeManager>.Instance).Field<List<int>>("m_CustomerHelpersData").Value;
            this.DeleteEmployee((CsHelperSkill)skill, employeeData);
        }
    }
}