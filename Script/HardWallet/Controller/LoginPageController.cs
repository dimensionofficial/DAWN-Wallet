using UnityEngine;
using System.Collections;
namespace HardwareWallet
{
    public class LoginPageController : BaseState
    {

        BaseUI ui;
        bool trans = false;

        public LoginPageController()
        {
            if (ui == null)
            {
                ui = FlowManager.Instance.transform.Find("LoginPanel").gameObject.AddComponent<LoginUI>();
            }
            ui.Ini();
        }

        public override void OnEnter()
        {
            ui.Show(null);
            ui.events += (o) => {
                string pincode = (string)o;
                if (FlowManager.PINCODEVERYFI == 
                    SaveAndLoad.LoadString(FlowManager.PINCODEVERYFI, System.Text.UTF8Encoding.UTF8.GetBytes(pincode)))
                {
                    string bip = SaveAndLoad.LoadString(FlowManager.BIPKEYWORDS, System.Text.UTF8Encoding.UTF8.GetBytes(pincode));
                    if (bip == null || !QRPayTools.VerifyBip(bip))
                    {
                        PopBox.Instance.ShowMsg("cant read bip from local");
                        return;
                    }
                    //FlowManager.Instance.walletInfo = new WalletInfo(bip, FlowManager.Instance.network);
                    //FlowManager.Instance.mainNetPrivateKey = 
                    //new NBitcoin.BitcoinSecret(QRPayTools.GetMnemonic(bip), FlowManager.Instance.network);
                    //FlowManager.Instance.changeScriptPubKey =
                    //FlowManager.Instance.mainNetPrivateKey.PubKey.GetAddress(FlowManager.Instance.network);
                    trans = true;
                }
                else
                {
                    PopBox.Instance.ShowMsg("error pin code");
                }
            };
        }

        public override void OnTick()
        {

        }

        public override string CheckTransation()
        {
            if (trans)
            {
                trans = false;
                return "MainPageController";
            }
            return string.Empty;
        }

        public override void OnExit()
        {
            ui.Hide();
        }
    }
}
