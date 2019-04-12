using UnityEngine;
using System.Collections;
namespace HardwareWallet
{
    public class StartSettingIniPageController : BaseState
    {

        BaseUI ui;
        string trans = string.Empty;
        string tempHandCode = "";

        public StartSettingIniPageController()
        {
            if (ui == null)
            {
                ui = FlowManager.Instance.transform.Find("StartIniPanel").gameObject.AddComponent<StartSettingIniPageUI>();
            }
            ui.Ini();
        }

        public override void OnEnter()
        {
            trans = string.Empty;
            ui.Show(null);
            ui.events += (o) => {
                if ((string)o == "backBtn")
                {
                    trans = "SelectLanguagePageController";
                }
                else if ((string)o == "Next")
                {
                    trans = "RegistPageController";
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
