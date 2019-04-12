using UnityEngine;
using System.Collections;
namespace HardwareWallet
{
    public class MainPageController : BaseState
    {

        BaseUI ui;
        string trans = string.Empty;

        public MainPageController()
        {
            if (ui == null)
            {
                ui = FlowManager.Instance.transform.Find("MainPanel").gameObject.AddComponent<MainPageUI>();
            }
            ui.Ini();
        }

        public override void OnEnter()
        {
            trans = string.Empty;
            ui.Show(null);
            ui.events += (o) => {
                if ((string)o == "SendCoin")
                {
                    //扫描二维码
                    trans = "ScanQRPageController";
                }
                else if((string)o == "Setting")
                {
                    trans = "BackupPageController";
                }
                else 
                {
                    PopBox.Instance.ShowMsg("error option");
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
