using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NBitcoin;
using ZXing;

namespace HardwareWallet
{
    public class TransactionPreviewPageUI : BaseUI
    {
        public Text sendFromAddress;
        public Text sendToAddress;
        public Text minnerFee;
        public Text amount;
        public Text coinType;
        public Button signBtn;
        public Button cancelBtn;
        public Button backBtn;

        public override void Ini()
        {
            base.Ini();
            listenBack = true;
            sendFromAddress = transform.Find("SendAdd").GetComponent<Text>();
            sendToAddress = transform.Find("ReceiveAdd").GetComponent<Text>();
            minnerFee = transform.Find("Gas").GetComponent<Text>();
            amount = transform.Find("Amount").GetComponent<Text>();
            coinType = transform.Find("cointype").GetComponent<Text>();

            signBtn = transform.Find("SignBtn").GetComponent<Button>();
            signBtn.onClick.AddListener(() => {
                if (events != null)
                {
                    events("Sign");
                }
            });
            cancelBtn = transform.Find("BackBtn").GetComponent<Button>();
            cancelBtn.onClick.AddListener(() => {
                if (events != null)
                {
                    events("backBtn");
                }
            });
            backBtn = transform.Find("CancelBtn").GetComponent<Button>();
            backBtn.onClick.AddListener(() => {
                if (events != null)
                {
                    events("backBtn");
                }
            });
        }

        public override void Show(object param)
        {
            base.Show(param);
            TransactionPreviewData data = (TransactionPreviewData)param;
            if (data.isAvaliable)
            {
                sendFromAddress.text = data.sendFromAddress.Substring(0, 16) + "...";
                sendToAddress.text = data.receiveAddress[0].Substring(0, 16) + "...";
                amount.text = ((decimal)data.amount).ToString();
                minnerFee.text = ((decimal)data.minnerFee).ToString();
                coinType.text = FlowManager.Instance.coinDes[(int)FlowManager.Instance.walletType];
            }
            else
            {
                sendFromAddress.text = "";
                sendToAddress.text = "";
                amount.text = "";
                minnerFee.text = "";
                coinType.text = "";
            }
        }

        public override void Hide()
        {
            base.Hide();
        }
    }
}
