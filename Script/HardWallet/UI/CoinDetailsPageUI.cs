using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NBitcoin;

namespace HardwareWallet
{
    public class CoinDetailsPageUI : BaseUI
    {
        public Image icon;
        public Button crossWalletBtn;
        public Button addressBtn;
        public Button signBtn;
        public Button recordBtn;
        public Text coinName;
        public Button backBtn;
        public Text coinFullName;

        public override void Ini()
        {
            base.Ini();
            smooth = true;
            listenBack = true;
            coinName = transform.Find("CoinType").GetComponent<Text>();
            coinFullName = transform.Find("CoinTypeDes").GetComponent<Text>();
            crossWalletBtn = transform.Find("CrossBtn").GetComponent<Button>();
            addressBtn = transform.Find("AddressBtn").GetComponent<Button>();
            signBtn = transform.Find("SignBtn").GetComponent<Button>();
            recordBtn = transform.Find("RecordBtn").GetComponent<Button>();
            icon = transform.Find("Icon").GetComponent<Image>();
            backBtn = transform.Find("BackBtn").GetComponent<Button>();
            crossWalletBtn.onClick.AddListener(() => {
                if (events != null)
                {
                    events("crossWalletBtn");
                }
            });
            addressBtn.onClick.AddListener(() => {
                if (events != null)
                {
                    events("addressBtn");
                }
            });
            signBtn.onClick.AddListener(() => {
                if (events != null)
                {
                    events("signBtn");
                }
            });
            recordBtn.onClick.AddListener(() => {
                if (events != null)
                {
                    events("recordBtn");
                }
            });
            backBtn.onClick.AddListener(() => {
                if (events != null)
                {
                    events("backBtn");
                }
            });
        }

        public override void Show(object param)
        {
            WalletType type = FlowManager.Instance.walletType;
            icon.sprite = FlowManager.Instance.coinIcon[(int)type];
            coinName.text = FlowManager.Instance.coinDes[(int)type];
            coinFullName.text = FlowManager.Instance.coinFullDes[(int)type];
            base.Show(param);
        }

        public override void Hide()
        {
            base.Hide();
        }
    }
}
