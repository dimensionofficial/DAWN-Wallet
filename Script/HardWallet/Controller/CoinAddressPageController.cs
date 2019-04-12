using UnityEngine;
using System.Collections;
namespace HardwareWallet
{
    public class CoinAddressPageController : BaseState
    {

        BaseUI ui;
        string trans = string.Empty;

        public CoinAddressPageController()
        {
            if (ui == null)
            {
                ui = FlowManager.Instance.transform.Find("CoinAddress").gameObject.AddComponent<CoinAddressPageUI>();
            }
            ui.Ini();
        }

        public override void OnEnter()
        {
            trans = string.Empty;
            if (FlowManager.Instance.globalParams.ContainsKey("addressPageMode"))
            {
                string s = FlowManager.Instance.globalParams["addressPageMode"].ToString();
                if (s == "pair")
                {
                    ui.Show(false);
                }
                else
                {
                    ui.Show(true);
                }
            }
            else
            {
                ui.Show(null);
            }
            ui.events += (o) => {
                if ((string)o == "backBtn")
                {
                    if (FlowManager.Instance.stateManager.LastState == "CoinDetailsPageController")
                    {
                        trans = "CoinDetailsPageController";
                    }
                    else
                    {
                        trans = "MainPageController";
                    }
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
