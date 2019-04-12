using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SendOverPanel : MonoBehaviour
{
    public enum SendType
    {
        None,
        EOS,
        ETH,
        ERC20,
        BTC,
    }
    private SendType currentSendType = SendType.None;

    public Text toCountRMBtext;
    public Text gasRmbText;

    public SendCoinPanel senndCoinPanel;

    public WalletInfoPanel walletInfoPanel;
    public float _Y = -570;
    public GameObject totalObject;
    public GameObject stateObject;

    public GameObject eosNouseObject;

    public Text addressTitle;
    public Text toAddress;
    public Text toCount;
    public Text free;
    public Text totalCount;
    public Text stateText;
    public Text sendCountText;

    private bool isPay = false;
    private int goodsid;

    public void Open(string _toAddress, decimal _toCount, decimal _free, bool _stateText, string _hash, string _commentInfo, string _freeUnit, string _toCountUnit, bool isSameUnit)
    {
        isPay = false;
        goodsid = 0;

        addressTitle.text = "地址";
        eosNouseObject.SetActive(true);
        toCountRMBtext.text = senndCoinPanel.moneyText.text;
        gasRmbText.text = senndCoinPanel.feeMonyText.text;
        senndCoinPanel.OnClickBackBtn();
        gameObject.SetActive(true);
        string one = _toAddress.Substring(0, 10);
        string two = _toAddress.Substring(_toAddress.Length - 10, 10);
        toAddress.text = one + "..." + two;
        toCount.text = _toCount.ToString() + " " + _toCountUnit;
        if (_toCountUnit.Equals("ETH"))
        {
            currentSendType = SendType.ETH;
        }
        else if (_toCount.Equals("BTC"))
        {
            currentSendType = SendType.BTC;
        }
        else
        {
            currentSendType = SendType.ERC20;
        }
        free.text = _free.ToString() + " " + _freeUnit;
		sendCountText.text = _toCount.ToString () + " " + _toCountUnit;
        if (isSameUnit)
        {
            totalObject.SetActive(true);
            stateObject.transform.localPosition = new Vector3(stateObject.transform.localPosition.x, _Y, stateObject.transform.localPosition.z);
            totalCount.text = (_free + _toCount).ToString() + " " + _freeUnit;
        }
        else
        {
            totalObject.SetActive(false);
            stateObject.transform.localPosition = new Vector3(stateObject.transform.localPosition.x, totalObject.transform.localPosition.y, stateObject.transform.localPosition.z);
        }
        stateText.text = _commentInfo;

        if (!string.IsNullOrEmpty(_hash) && !string.IsNullOrEmpty(_commentInfo))
        {
            PlayerPrefs.SetString(_hash, _commentInfo);
        }
    }

    public void SetPayInfo(int _goodsid)
    {
        isPay = true;
        goodsid = _goodsid;
    }

    public void OpenEos(string toAccount, string _toCount, string mean)
    {
        addressTitle.text = "账户";
        currentSendType = SendType.EOS;
        sendCountText.text = _toCount.ToString() + " EOS";
        toCountRMBtext.text = "≈￥ " + decimal.Parse(_toCount) * HttpManager._Intance.eos_RMB ;
        gameObject.SetActive(true);
        eosNouseObject.SetActive(false);
        toCount.text = _toCount + " EOS";
        toAddress.text = toAccount;
        stateText.text = mean;
    }
  
    public void OnClickOk()
    {
        gameObject.SetActive(false);

        if (isPay)
        {
            PanelManager._Instance._eosPaymentPanel.OnPaymentOver();
            PanelManager._Instance._eosRegisteringPanel.Show(PanelManager._Instance._eosRegisterPanel.eosWalletInfo);
        }
        else
        {
            if (currentSendType == SendType.EOS)
            {
                PanelManager._Instance._eosWalletInfoPanel.ShowMe(PanelManager._Instance._eosWalletInfoPanel.currentItem);
            }
            else
            {
                if (walletInfoPanel.currentTokenItem != null)
                {
                    //walletInfoPanel.currentItem.GetHistory(false);
                    walletInfoPanel.recordPage.ShowRecord(walletInfoPanel.currentTokenItem);
                }
                else
                {
                    //if (walletInfoPanel.currentItem.type == NewWalletManager.CoinType.ETH)
                    //{
                    //    walletInfoPanel.ShowMe(walletInfoPanel.currentItem);
                    //    walletInfoPanel.bottomBtns.SetActive(true);
                    //    walletInfoPanel.recordPage.InitRcordItem(walletInfoPanel.currentItem.historyRcordlist, "ETH");
                    //}
                   
                    if (walletInfoPanel.currentItem.type == NewWalletManager.CoinType.BTC)
                    {
                        walletInfoPanel.ShowMe(walletInfoPanel.currentItem);
                    }

                }
            }
        }
    }
}
