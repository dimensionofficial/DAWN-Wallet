using UnityEngine;
using System.Collections;
namespace HardwareWallet
{
    public class AddCoinViewAPageController : BaseState
    {

        BaseUI ui;
        string trans = string.Empty;

        public AddCoinViewAPageController()
        {
            if (ui == null)
            {
                ui = FlowManager.Instance.transform.Find("AddCoinViewA").gameObject.AddComponent<AddCoinViewAUI>();
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
                    trans = "MainPageController";
                }
                else if ((string)o == "viewaddress")
                {
                    if (FlowManager.Instance.globalParams.ContainsKey("addressPageMode"))
                    {
                        FlowManager.Instance.globalParams["addressPageMode"] = "pair";
                    }
                    else
                    {
                        FlowManager.Instance.globalParams.Add("addressPageMode", "pair");
                    }
                    trans = "CoinAddressPageController";
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
