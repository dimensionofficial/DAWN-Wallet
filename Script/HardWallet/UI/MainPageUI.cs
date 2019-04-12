using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NBitcoin;

namespace HardwareWallet
{
    public class MainPageUI : BaseUI
    {
        public Image addressQR;
        public Text address;
        public Button sendBtn;
        public Dropdown cointype;
        public Button settingBtn;

        public override void Ini()
        {
            base.Ini();
            address = transform.Find("Address").GetComponent<Text>();
            sendBtn = transform.Find("Send").GetComponent<Button>();
            settingBtn = transform.Find("Setting").GetComponent<Button>();
            addressQR = transform.Find("QR").GetComponent<Image>();
            cointype = transform.Find("CurrentCoinType").GetComponent<Dropdown>();
            sendBtn.onClick.AddListener(() => {
                if (events != null)
                {
                    events("SendCoin");
                }
            });
            settingBtn.onClick.AddListener(() => {
                if (events != null)
                {
                    events("Setting");
                }
            });
            cointype.onValueChanged.AddListener((i) => {
                Refresh((WalletType)i);
            });
        }

        public override void Show(object param)
        {
            if (!FlowManager.Instance.globalParams.ContainsKey("currentpubkey") || 
                string.IsNullOrEmpty(FlowManager.Instance.globalParams["currentpubkey"].ToString()))
            {
                PinCodeUI.Instace.Show((pincode) =>
                {
                    IWalletDataBase walletBase = WalletDataFactory.CreateWalletData(FlowManager.Instance.walletType);

                    string addressStr = walletBase.CreateAddressString(pincode);
                    FlowManager.Instance.globalParams["currentpubkey"] = addressStr;
                    address.text = addressStr.Substring(0, 10) + "...";
                    FlowManager.Instance.qRCodeManager.EncodeQRCode(addressQR, addressStr);
                    base.Show(param);
                });
            }
            else
            {
                base.Show(param);
            }
        }

        public override void Hide()
        {
            base.Hide();
        }

        public void Refresh(WalletType type)
        {
            PinCodeUI.Instace.Show((pincode) =>
            {
                IWalletDataBase walletBase = WalletDataFactory.CreateWalletData(FlowManager.Instance.walletType);
                string addressStr = walletBase.CreateAddressString(pincode);
                FlowManager.Instance.globalParams["currentpubkey"] = addressStr;
                address.text = addressStr.Substring(0, 10) + "...";
                FlowManager.Instance.qRCodeManager.EncodeQRCode(addressQR, addressStr);
                FlowManager.Instance.walletType = type;
            });
        }
    }
}
