using EmployeeTraining.Employee;
using UnityEngine;

namespace EmployeeTraining.TrainingApp
{
    public interface IEmployeeAppTab
    {
        void ComposeTabButton(GameObject baseTabBtnObj, GameObject taskbarBtnsObj);

        void CreateStatusPanel(GameObject cashierPanelObj, GameObject panelTmpl);
        void ComposeStatusPanel();

        void CreateTabScreen(GameObject baseTabObj, GameObject tabsObj);

        void ComposeTabScreen();

        void AddTabEvent(GameObject taskbarObj, TabManager tabMgr);

        void RegisterEmployee(IEmployeeSkill skill, GameObject panelTmpl, GameObject unlockApprWindowObj);
        void DeleteEmployee(IEmployeeSkill skill);
        
        bool Matches(IEmployeeSkill skill);
    }
}