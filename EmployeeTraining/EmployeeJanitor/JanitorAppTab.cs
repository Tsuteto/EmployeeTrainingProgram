using System.Collections.Generic;
using EmployeeTraining.Employee;
using EmployeeTraining.TrainingApp;
using HarmonyLib;
using MyBox;
using UnityEngine;
using UnityEngine.UI;

namespace EmployeeTraining.EmployeeJanitor
{
    public class JanitorAppTab : EmployeeAppTab<JanitorTrainingProgressItem, JanitorSkill>, IEmployeeAppTab
    {
        public void ComposeTabButton(GameObject baseTabBtnObj, GameObject taskbarBtnsObj)
        {
            this.ComposeTabButton(baseTabBtnObj, taskbarBtnsObj,
                "Janitor Tab Button", "Janitor_Icon", "Janitors");
        }

        void IEmployeeAppTab.CreateStatusPanel(GameObject panelObj, GameObject panelTmpl)
        {
            this.CreateStatusPanel(panelObj, panelTmpl);
        }

        public void ComposeStatusPanel()
        {
            this.ComposeStatusPanel(
                "Janitor_Icon", "Janitor Name",
                bgColorPanel: new Color(0.349f, 0.6039f, 0.549f, 0.9412f),
                bgColorIntr: new Color32(68, 200, 182, 255),
                bgColorExp: new Color32(33, 108, 99, 255),
                bgColorRmSlider: new Color32(33, 108, 99, 255),
                bgColorRmSeal: new Color32(64, 95, 89, 255),
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
            this.ComposeTabScreen("Janitor Tab", "janitors", new Color32(228, 255, 251, 255));
            this.ComposeNoEmployee("No Janitors", "NO JANITORS HIRED", new Color32(150, 212, 206, 255));
        }

        public void AddTabEvent(GameObject taskbarObj, TabManager tabMgr)
        {
            this.AddTabBtnEvent(taskbarObj, tabMgr, "Buttons/Janitor Tab Button", "janitors");
        }

        public void RegisterEmployee(IEmployeeSkill skill, GameObject panelTmpl, GameObject unlockApprWindowObj)
        {
            this.RegisterEmployee("Janitor Status Panel", (JanitorSkill)skill, unlockApprWindowObj);
        }

        public void DeleteEmployee(IEmployeeSkill skill)
        {
            List<int> employeeData = Traverse.Create(Singleton<EmployeeManager>.Instance).Field<List<int>>("m_JanitorsData").Value;
            this.DeleteEmployee((JanitorSkill)skill, employeeData);
        }
    }
}