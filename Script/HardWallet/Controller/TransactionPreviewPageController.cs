using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace HardwareWallet
{
    public class TransactionPreviewData
    {
        public string sendFromAddress;
        public List<string> receiveAddress;
        public double amount;
        public double minnerFee;
        public bool isAvaliable = false;
        public TransactionPreviewData(string data, string pubkey, WalletType type)
        {
            IWalletDataBase walletBase = WalletDataFactory.CreateWalletData(FlowManager.Instance.walletType);
            isAvaliable = walletBase.GetPayInfo(data, pubkey, out receiveAddress, out sendFromAddress, out amount, out minnerFee);
        }

        public TransactionPreviewData(string data)
        {
            try
            {
                if (data.StartsWith("1"))
                {
                    data = QRPayTools.GetHashtableStr(data);
                }
                Hashtable table = Json.jsonDecode(data) as Hashtable;
                string coinType = table["coinType"].ToString();
                string transData = "";
                string pubkey = "";
                switch (coinType)
                {
                    case "BTC":
                      //  FlowManager.Instance.walletType = WalletType.BTC;
                        transData = Json.jsonEncode(table["data"] as Hashtable);
                        pubkey = table["address"].ToString();
                        break;
                    case "ETH":
                        FlowManager.Instance.walletType = WalletType.ETH;
                        transData = Json.jsonEncode(table["data"] as Hashtable);
                        Hashtable transDataTable = table["data"] as Hashtable;
                        pubkey = transDataTable["addressFrom"].ToString();
                        break;
                    case "EOS":
                        FlowManager.Instance.walletType = WalletType.EOS;
                        transData = Json.jsonEncode(table["data"] as Hashtable);
                        Hashtable transDataTableA = table["data"] as Hashtable;
                        pubkey = transDataTableA["addressFrom"].ToString();
                        break;
                    default:
                        isAvaliable = false;
                        return;
                }
                IWalletDataBase walletBase = WalletDataFactory.CreateWalletData(FlowManager.Instance.walletType);
                isAvaliable = walletBase.GetPayInfo(transData, pubkey, out receiveAddress, out sendFromAddress, out amount, out minnerFee);
            }
            catch
            {
                isAvaliable = false;
            }
        }
        /*
        public TransactionPreviewData(string data, bool raw)
        {
            try
            {
                string[] dataList = data.Split(',');
                string coinType = dataList[0];
                switch (coinType)
                {
                    case "BTC":
                        break;
                    case "ETH":
                        break;
                    case "EOS":
                        break;
                    default:
                        isAvaliable = false;
                        return;
                }
                IWalletDataBase walletBase = WalletDataFactory.CreateWalletData(FlowManager.Instance.walletType);
                isAvaliable = walletBase.GetPayInfo(transData, pubkey, out receiveAddress, out sendFromAddress, out amount, out minnerFee);
            }
            catch
            {
                isAvaliable = false;
            }
        }
        */
    }

    public class TransactionPreviewPageController : BaseState
    {

        BaseUI ui;
        string trans = string.Empty;

        public TransactionPreviewPageController()
        {
            if (ui == null)
            {
                ui = FlowManager.Instance.transform.Find("TransPreview").gameObject.AddComponent<TransactionPreviewPageUI>();
            }
            ui.Ini();
        }

        public override void OnEnter()
        {
            trans = string.Empty;
            TransactionPreviewData data = 
                new TransactionPreviewData(
                    (string)FlowManager.Instance.globalParams["qrcode"]);
            if (!data.isAvaliable)
            {
                PopBox.Instance.ShowMsg(FlowManager.Instance.language.GetWord(Words.不能识别的二维码), 
                    ()=> {
                        trans = FlowManager.Instance.GetGlobalParams(FlowManager.GlobalData_ScanFrom).ToString();
                    });
            }
            else
            {
                ui.Show(data);
                ui.events += (o) =>
                {
                    if ((string)o == "Sign")
                    {
                        trans = "TransactionCompletePageController";
                    }
                    else
                    {
                        trans = FlowManager.Instance.GetGlobalParams(FlowManager.GlobalData_ScanFrom).ToString();
                    }
                };
            }
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
