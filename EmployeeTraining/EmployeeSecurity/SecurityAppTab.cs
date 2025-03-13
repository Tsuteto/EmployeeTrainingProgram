using System.Collections.Generic;
using EmployeeTraining.Employee;
using EmployeeTraining.TrainingApp;
using HarmonyLib;
using MyBox;
using UnityEngine;
using UnityEngine.UI;

namespace EmployeeTraining.EmployeeSecurity
{
    public class SecurityAppTab : EmployeeAppTab<SecurityTrainingProgressItem, SecuritySkill>, IEmployeeAppTab
    {
        public void ComposeTabButton(GameObject baseTabBtnObj, GameObject taskbarBtnsObj)
        {
            this.ComposeTabButton(baseTabBtnObj, taskbarBtnsObj,
                "Security Guard Tab Button", "icon_media_48", "Security Guards");
        }

        void IEmployeeAppTab.CreateStatusPanel(GameObject panelObj, GameObject panelTmpl)
        {
            this.CreateStatusPanel(panelObj, panelTmpl);
        }

        public void ComposeStatusPanel()
        {
            this.ComposeStatusPanel(
                "icon_media_48", "Security Guard Name",
                bgColorPanel: new Color(0.6039f, 0.5946f, 0.349f, 0.9412f),
                bgColorIntr: new Color32(174, 200, 103, 255),
                bgColorExp: new Color32(97,107,64, 255),
                bgColorRmSlider: new Color32(97,107,64, 255),
                bgColorRmSeal: new Color32(83,87,48, 255),
                detailCellSize: new Vector2(95, 25),
                detailParamCount: 3,
                detailParams: new DetailParam[]
                {
                    new DetailParam() { Name="Rapidity", LabelKey="Rapidity", ValueKey="Speed", ValueArgs=new string[]{"0"} },
                    new DetailParam() { Name="Dexterity", LabelKey="Dexterity", ValueKey="Percentage", ValueArgs=new string[]{"0"} },
                    new DetailParamWage(),
                }
            );
        }

        void IEmployeeAppTab.CreateTabScreen(GameObject baseTabObj, GameObject tabsObj)
        {
            this.CreateTabScreen(baseTabObj, tabsObj);
        }

        public void ComposeTabScreen()
        {
            this.ComposeTabScreen("Security Guard Tab", "guards", new Color32(239, 255, 226, 255));
            this.ComposeNoEmployee("No Security Guards", "NO SECURITY GUARDS HIRED", new Color32(196, 210, 158, 255));
        }

        public void AddTabEvent(GameObject taskbarObj, TabManager tabMgr)
        {
            this.AddTabBtnEvent(taskbarObj, tabMgr, "Buttons/Security Guard Tab Button", "guards");
        }

        public void RegisterEmployee(IEmployeeSkill skill, GameObject panelTmpl, GameObject unlockApprWindowObj)
        {
            this.RegisterEmployee("Security Guard Status Panel", (SecuritySkill)skill, unlockApprWindowObj);
        }

        public void DeleteEmployee(IEmployeeSkill skill)
        {
            List<int> employeeData = Traverse.Create(Singleton<EmployeeManager>.Instance).Field<List<int>>("m_SecurityGuardsData").Value;
            this.DeleteEmployee((SecuritySkill)skill, employeeData);
        }
    }
}