using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReceiveCoinPanel : MonoBehaviour {

    public WalletInfoPanel walletInfoPanel;
    public Image coinIcon;
    public Text coinTypeText;

    public Image qRImage;
    private string address;
    //public InputField receiveCountText;
    //public NativeEditBox receiveText;
    public Text btnText;
    public Text m_text;
    public Text walletName;
    //多签判断
    public bool isMultiAddres = false;
    public Image colorImage;
    public Color normlColor;
    public Color multiColor;

    public Image titleImage;
    public Sprite normlSprite;
    public Sprite multiSprite;

    public Image title2Image;

    public Sprite norml2Sprite;
    public Sprite multi2Sprite;
    public void OnClickAddressBtn()
    {
        CMGE_Clipboard.CopyToClipboard(address);
        PopupLine.Instance.Show("复制成功");
    }
 
    public void Open( WalletInfoPanel walletPanel, bool isShowEos = false)
    {
        
        walletInfoPanel = walletPanel;

        gameObject.SetActive(true);
        string str = "";
        if (isShowEos)
        {
            btnText.text = "复制EOS账户";
            EOSWalletInfoPanel eoswalletInfo = PanelManager._Instance._eosWalletInfoPanel;
            walletName.text = eoswalletInfo.currentItem.coinInfo.walletname;
            coinIcon.overrideSprite = TextureUIAsset._Instance.eosIcon;
            coinTypeText.text = "EOS";
            EosItem it = eoswalletInfo.currentItem as EosItem;
            str = it.eosWalletInfo.account;
            ShareManager.init.shareStr = "我的EOS账户: " + str;

            m_text.text = "我的EOS账户：" + str;
        }
        else
        {
            isMultiAddres = MultiJSData.instance.IsMultiSigAddress(walletInfoPanel.currentItem.coinInfo.address);
            if (isMultiAddres)
            {
                colorImage.color = multiColor;
                titleImage.sprite = multiSprite;
                title2Image.sprite = multi2Sprite;
            }
            else
            {
                colorImage.color = normlColor;
                titleImage.sprite = normlSprite;
                title2Image.sprite = norml2Sprite;
            }
            btnText.text = "复制钱包地址";
            //Debug.Log(walletInfoPanel);
            //Debug.Log(walletInfoPanel.currentItem);
            str = walletInfoPanel.currentItem.coinInfo.address;

            walletName.text = walletInfoPanel.currentItem.coinInfo.walletname;
            if (NewWalletManager._Intance.currentCoinType == NewWalletManager.CoinType.BTC)
            {
                coinIcon.overrideSprite = TextureUIAsset._Instance.btcIcon;
                coinTypeText.text = "BTC";
            }else if (NewWalletManager._Intance.currentCoinType == NewWalletManager.CoinType.USDT)
            {
                coinIcon.overrideSprite = TextureUIAsset._Instance.usdtIcon;
                coinTypeText.text = "USDT";
            }
            else if (NewWalletManager._Intance.currentCoinType == NewWalletManager.CoinType.ETH)
            {
                if (walletInfoPanel.currentTokenItem != null)
                {
                    coinIcon.overrideSprite = walletInfoPanel.currentTokenItem.image.overrideSprite;
                    coinTypeText.text = walletInfoPanel.currentTokenItem.tokenNameText.text;
                }
                else
                {
                    coinIcon.overrideSprite = TextureUIAsset._Instance.ethIcon;
                    coinTypeText.text = "ETH";
                }
            }

            ShareManager.init.shareStr = "我的钱包地址: " + str;
            m_text.text = str;
        }
        
        //receiveCountText.text = "";
        //receiveText.text = "";

        address = str;

       

        var encoded = NewWalletManager._Intance.QRCodeManager.GetQRTexture2D(str);
        ShareManager.init.tex = encoded;

        NewWalletManager._Intance.QRCodeManager.EncodeQRCode(qRImage, str);
    }

    public void OnClickBackBtn()
    {
        NewWalletManager._Intance.DoTweenBack(this.transform, -1000);
        gameObject.SetActive(false);
    }

    //public void OnChangeReceiveValue()
    //{
    //    string receiveCount = "";
    //    if (string.IsNullOrEmpty(receiveCountText.text))
    //    {
    //        receiveCount = receiveText.text;
    //    }
    //    else
    //    {
    //        receiveCount = receiveCountText.text;
    //    }

    //    if (string.IsNullOrEmpty(receiveCount))
    //    {
    //        receiveCount = "0";
    //    }

    //    Hashtable has = new Hashtable();
    //    has["CoinType"] = (int)NewWalletManager._Intance.currentCoinType;
    //    has["receiveAddress"] = address;
    //    has["receiveCount"] = receiveCount;
    //    string info = Json.jsonEncode(has);

    //    NewWalletManager._Intance.QRCodeManager.EncodeQRCode(qRImage, info);
    //}
}
