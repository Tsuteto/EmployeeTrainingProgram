using System.Collections.Generic;
using EmployeeTraining.Employee;
using EmployeeTraining.EmployeeRestocker;
using HarmonyLib;
using MyBox;
using UnityEngine;

namespace EmployeeTraining.TrainingApp
{
    public class EmployeeRestockerAppUI : EmployeeAppUI<RestockerTrainingProgressItem, RestockerSkill>, IEmployeeAppUI
    {
        public void ComposeTabButton(GameObject baseTabBtnObj, GameObject taskbarBtnsObj)
        {
            this.ComposeTabButton(baseTabBtnObj, taskbarBtnsObj,
                "Restockers Tab Button", "icon_shopping_28", "Restockers");
        }

        void IEmployeeAppUI.CreateStatusPanel(GameObject panelObj, GameObject panelTmpl)
        {
            this.CreateStatusPanel(panelObj, panelTmpl);
        }

        public void ComposeStatusPanel()
        {
            this.ComposeStatusPanel(
                "icon_shopping_28", "Restocker Name",
                bgColorPanel: new Color(0.4208f, 0.3171f, 0.5377f, 0.9412f),
                bgColorIntr: new Color32(150, 134, 226, 255),
                bgColorExp: new Color32(64, 63, 105, 255),
                bgColorRmSlider: new Color32(64, 63, 105, 255),
                bgColorRmSeal: new Color32(55, 54, 87, 255),
                detailCellSize: new Vector2(71, 25),
                detailParamCount: 4,
                detailParams: new DetailParam[]
                {
                    new DetailParam() { Name="Rapidity", LabelKey="Rapidity", ValueKey="Speed", ValueArgs=new string[]{"0"} },
                    new DetailParam() { Name="Capacity", LabelKey="Capacity", ValueKey="Weight/Height", ValueArgs=new string[]{"0", "0"} },
                    new DetailParam() { Name="Dexterity", LabelKey="Dexterity", ValueKey="Percentage", ValueArgs=new string[]{"0"} },
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
            this.ComposeTabScreen("Restockers Tab", "restockers", new Color32(224, 218, 255, 255));
            this.ComposeNoEmployee("No Restockers", "NO RESTOCKERS HIRED", new Color32(192, 177, 255, 255));
        }

        public void AddTabEvent(GameObject taskbarObj, TabManager tabMgr)
        {
            this.AddTabBtnEvent(taskbarObj, tabMgr, "Buttons/Restockers Tab Button", "restockers");
        }

        public void RegisterEmployee(IEmployeeSkill skill, GameObject panelTmpl, GameObject unlockApprWindowObj)
        {
            base.RegisterEmployee("Restocker Status Panel", (RestockerSkill)skill, unlockApprWindowObj);
        }

        public void DeleteEmployee(IEmployeeSkill skill)
        {
            List<int> employeeData = Traverse.Create(Singleton<EmployeeManager>.Instance).Field<List<int>>("m_RestockersData").Value;
            this.DeleteEmployee((RestockerSkill)skill, employeeData);
        }
    }
}