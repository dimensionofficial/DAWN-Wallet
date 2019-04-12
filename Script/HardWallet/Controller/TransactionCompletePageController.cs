using UnityEngine;
using System.Collections;
namespace HardwareWallet
{
    public class TransactionCompletePageController : BaseState
    {

        BaseUI ui;
        bool trans = false;

        public TransactionCompletePageController()
        {
            if (ui == null)
            {
                ui = FlowManager.Instance.transform.Find("TransComplete").gameObject.AddComponent<TransactionCompletePageUI>();
            }
            ui.Ini();
        }

        string[] GetQRcodeList(string d)
        {
            if (d.Contains("^"))
            {
                string[] qrcodeList = d.Split('^');
                return qrcodeList;
            }
            else
            {
                string[] qrcodeList = new string[1];
                qrcodeList[0] = d;
                return qrcodeList;
            }
        }

        public override void OnEnter()
        {
            trans = false;
            string qrcodeT = FlowManager.Instance.globalParams["qrcode"].ToString();
            string[] qrcodeList = GetQRcodeList(qrcodeT);
            PinCodeUI.Instace.Show((pincode) =>
            {
                string resultqr = "";
                foreach (var v in qrcodeList)
                {
                    IWalletDataBase walletBase = WalletDataFactory.CreateWalletData(v);
                    if (walletBase == null)
                    {
                        PopBox.Instance.ShowMsg(FlowManager.Instance.language.GetWord(Words.不能识别的二维码));
                        trans = true;
                        return;
                    }
                    string signedData = walletBase.CreateSignedPayString(walletBase.data, pincode);
                    if (string.IsNullOrEmpty(signedData))
                    {
                        PopBox.Instance.ShowMsg(FlowManager.Instance.language.GetWord(Words.不能识别的二维码));
                        trans = true;
                        return;
                    }
                    else
                    {
                        resultqr += "^" + signedData;
                    }
                }
                if (string.IsNullOrEmpty(resultqr))
                {
                    PopBox.Instance.ShowMsg(FlowManager.Instance.language.GetWord(Words.不能识别的二维码));
                    trans = true;
                    return;
                }
                else
                {
                    if (resultqr.StartsWith("^"))
                    {
                        resultqr = resultqr.Substring(1);
                    }
                    ui.Show(resultqr);
                    ui.events += (o) =>
                    {
                        trans = true;
                    };
                }

            }, FlowManager.Instance.language.GetWord(Words.确认密码), () => {
                //返回上一状态
                FlowManager.Instance.stateManager.BackToLastState();
            });
        }

        public override void OnTick()
        {

        }

        public override string CheckTransation()
        {
            if (trans)
            {
                trans = false;
                return FlowManager.Instance.GetGlobalParams(FlowManager.GlobalData_ScanFrom).ToString();
            }
            return string.Empty;
        }

        public override void OnExit()
        {
            ui.Hide();
        }

        public string GetJsonData(string data)
        {
            try
            {
                Hashtable table = Json.jsonDecode(data) as Hashtable;
                string result = Json.jsonEncode(table["data"] as Hashtable);
                return result;
            }
            catch
            {
                return "";
            }
        }
    }
}
