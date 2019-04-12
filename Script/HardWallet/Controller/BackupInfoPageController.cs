using UnityEngine;
using System.Collections;
namespace HardwareWallet
{
    public class BackupInfoPageController : BaseState
    {

        BaseUI ui;
        string trans = "";
        string backTo = "";

        public BackupInfoPageController()
        {
            if (ui == null)
            {
                ui = FlowManager.Instance.transform.Find("BackupInfoPanel").gameObject.AddComponent<BackupInfoPageUI>();
            }
            ui.Ini();
        }

        public override void OnEnter()
        {
            trans = "";
            ui.Show(null);
            if (FlowManager.Instance.stateManager.LastState == "MainPageController")
            {
                backTo = "MainPageController";
            }
            else if (FlowManager.Instance.stateManager.LastState == "SettingPageController")
            {
                backTo = "SettingPageController";
            }
            FlowManager.Instance.SetGlobalParams(FlowManager.GlobalData_NeverShowBackupInfeture, "true");
            ui.events += (o) => {
                string l = (string)o;
                switch (l)
                {
                    case "true":
                        FlowManager.Instance.SetGlobalParams(FlowManager.GlobalData_NeverShowBackupInfeture, "true");
                        break;
                    case "false":
                        FlowManager.Instance.SetGlobalParams(FlowManager.GlobalData_NeverShowBackupInfeture, "false");
                        break;
                    case "backBtn":
                        if (string.IsNullOrEmpty(backTo))
                            trans = "SettingPageController";
                        else
                            trans = backTo;
                        break;
                    case "nextBtn":
                        trans = "BackupPageController";
                        break;
                }
            };
        }

        public override void OnTick()
        {

        }

        public override string CheckTransation()
        {
            return trans;
        }

        public override void OnExit()
        {
            ui.Hide();
        }
    }
}
