using UnityEngine;
using System.Collections;
namespace HardwareWallet
{
    public class ScanQRPageController : BaseState
    {

        BaseUI ui;
        string trans = string.Empty;

        public ScanQRPageController()
        {
            if (ui == null)
            {
                ui = FlowManager.Instance.transform.Find("ScanQR").gameObject.AddComponent<ScanQRPageUI>();
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
                    trans = FlowManager.Instance.GetGlobalParams(FlowManager.GlobalData_ScanFrom).ToString();
                }
                else
                {
                    if (FlowManager.Instance.globalParams.ContainsKey("qrcode"))
                    {
                        FlowManager.Instance.globalParams["qrcode"] = o;
                    }
                    else
                    {
                        FlowManager.Instance.globalParams.Add("qrcode", o);
                    }
                    trans = "TransactionCompletePageController";
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
