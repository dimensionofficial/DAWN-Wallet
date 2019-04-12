using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NBitcoin;
using NBitcoin.Crypto;
using Nethereum.Hex.HexConvertors.Extensions;

namespace HardwareWallet
{
    public class CoinAddressPageUI : BaseUI
    {
        public Image addressQR;
        public Text address;
        public Button backBtn;
        public Text title;

        public override void Ini()
        {
            base.Ini();
            smooth = true;
            listenBack = true;
            address = transform.Find("Address").GetComponent<Text>();
            title = transform.Find("Text (1)").GetComponent<Text>();
            addressQR = transform.Find("QR").GetComponent<Image>();
            backBtn = transform.Find("BackBtn").GetComponent<Button>();
            backBtn.onClick.AddListener(() => {
                if (events != null)
                {
                    events("backBtn");
                }
            });
        }

        public override void Show(object param)
        {
            bool addressMode = true;
            if (param != null && typeof(bool) == param.GetType())
            {
                addressMode = (bool)param;
            }
            string titleStr = addressMode ? FlowManager.Instance.language.GetWord(Words.地址)
                : FlowManager.Instance.language.GetWord(Words.配对);
            title.text = titleStr;
           
            if (!addressMode)
            {
                var addressStr = FlowManager.Instance.GetMasterPublicKey();
                ExtPubKey masterKey = FlowManager.Instance.masterPuKey;
                addressStr = PlayerPrefs.GetString(FlowManager.SNCODE) + "&" + masterKey.ToBytes().ToHex() + "&" + SystemInfo.deviceUniqueIdentifier + "&" + FlowManager.Instance.version;
                byte[] addr = System.Text.UTF8Encoding.UTF8.GetBytes(addressStr);
                byte[] key = Hashes.SHA256(System.Text.UTF8Encoding.UTF8.GetBytes("fdsamcldi123sawqa"));
                byte[] result = Encry.EncryData(addr, key);
                string data = QRPayTools.ToHexString(result);
                FlowManager.Instance.qRCodeManager.EncodeQRCode(addressQR, data);
                address.text = FlowManager.Instance.language.GetWord(Words.请打开DRACO应用扫一扫);
            }
            else
            {
                string publicKey = FlowManager.Instance.GetPublicKey(FlowManager.Instance.walletType);
                string addressStr = publicKey;
                address.text = addressStr.Substring(0, 6) + "..." + addressStr.Substring(addressStr.Length - 7, 7);
                FlowManager.Instance.qRCodeManager.EncodeQRCode(addressQR, addressStr);
            }
            base.Show(param);
        }

        public override void Hide()
        {
            base.Hide();
        }
    }
}
