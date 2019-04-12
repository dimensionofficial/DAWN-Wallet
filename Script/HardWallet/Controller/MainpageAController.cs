using UnityEngine;
using System.Collections;
namespace HardwareWallet
{
    public class MainPageAController : BaseState
    {

        BaseUI ui;
        string trans = string.Empty;

        public MainPageAController()
        {
            if (ui == null)
            {
                ui = FlowManager.Instance.transform.Find("MainPanelB").gameObject.AddComponent<MainPageBUI>();
            }
            ui.Ini();
        }

        public override void OnEnter()
        {
            trans = string.Empty;
            Battery.Instance.SetInfo("SN" + PlayerPrefs.GetString(FlowManager.SNCODE));
            ui.Show(FlowManager.Instance.viewWalletTypeList);
            ui.events += (o) => {
                if ((string)o == "AddCoin")
                {
                    //扫描二维码
                    trans = "AddCoinViewPageController";
                }
                else if ((string)o == "Setting")
                {
                    trans = "SettingPageController"; 
                }
                else if ((string)o == "BackupBtn")
                {
                    FlowManager.Instance.SetGlobalParams(FlowManager.GlobalData_BackFrom, "MainPageController");
                    trans = "BackupInfoPageController";
                }
                else if ((string)o == "Sign")
                {
                    FlowManager.Instance.SetGlobalParams(FlowManager.GlobalData_ScanFrom, "MainPageController");
                    trans = "ScanQRPageController";
                }
                else if ((string)o == "Power")
                {
                    FlowManager.Instance.ShutDown();
                }
                else if ((string)o == "Pare")
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
                    try
                    {
                        int i = int.Parse((string)o);
                        FlowManager.Instance.walletType = (WalletType)i;
                        trans = "CoinDetailsPageController";
                    }
                    catch
                    {
                        PopBox.Instance.ShowMsg("error option");
                    }
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
