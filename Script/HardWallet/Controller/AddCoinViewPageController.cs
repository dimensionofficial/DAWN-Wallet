using UnityEngine;
using System.Collections;
namespace HardwareWallet
{
    public class AddCoinViewPageController : BaseState
    {

        BaseUI ui;
        string trans = string.Empty;

        public AddCoinViewPageController()
        {
            if (ui == null)
            {
                ui = FlowManager.Instance.transform.Find("CoinManager").gameObject.AddComponent<AddCoinViewUI>();
            }
            ui.Ini();
        }

        public override void OnEnter()
        {
            trans = string.Empty;
            ui.Show(FlowManager.Instance.viewWalletTypeList);
            ui.events += (o) => {
                if ((string)o == "backBtn")
                {
                    trans = "SettingPageController";
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
