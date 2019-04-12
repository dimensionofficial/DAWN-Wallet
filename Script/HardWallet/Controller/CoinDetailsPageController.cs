using UnityEngine;
using System.Collections;
namespace HardwareWallet
{
    public class CoinDetailsPageController : BaseState
    {

        BaseUI ui;
        string trans = string.Empty;

        public CoinDetailsPageController()
        {
            if (ui == null)
            {
                ui = FlowManager.Instance.transform.Find("CoinDetails").gameObject.AddComponent<CoinDetailsPageUI>();
            }
            ui.Ini();
        }

        public override void OnEnter()
        {
            trans = string.Empty;
            ui.Show(null);
            ui.events += (o) => {
                if ((string)o == "crossWalletBtn")
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
                else if ((string)o == "addressBtn")
                {
                    if (FlowManager.Instance.globalParams.ContainsKey("addressPageMode"))
                    {
                        FlowManager.Instance.globalParams["addressPageMode"] = "address";
                    }
                    else
                    {
                        FlowManager.Instance.globalParams.Add("addressPageMode", "address");
                    }
                    trans = "CoinAddressPageController";
                }
                else if ((string)o == "signBtn")
                {
                    FlowManager.Instance.SetGlobalParams(FlowManager.GlobalData_ScanFrom, "CoinDetailsPageController");
                    string publicKey = FlowManager.Instance.GetPublicKey(FlowManager.Instance.walletType);
                    if (string.IsNullOrEmpty(publicKey))
                    {
                        PinCodeUI.Instace.Show((pincode) =>
                        {
                            IWalletDataBase walletBase = WalletDataFactory.CreateWalletData(FlowManager.Instance.walletType);

                            string addressStr = walletBase.CreateAddressString(pincode);
                            FlowManager.Instance.globalParams["currentpubkey"] = addressStr;
                            trans = "ScanQRPageController";
                        });
                    }
                    else
                    {
                        FlowManager.Instance.globalParams["currentpubkey"] = publicKey;
                        trans = "ScanQRPageController";
                    }
                }
                else if ((string)o == "recordBtn")
                {
                    trans = "";
                }
                else if ((string)o == "backBtn")
                {
                    trans = "MainPageController";
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
