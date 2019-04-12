using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NBitcoin;
using System.Linq;
using LitJson;
using System;
using QBitNinja4Unity.Models;
using NBitcoin.BouncyCastle.Math;
using Nethereum.RPC.Eth.DTOs;
using System.Numerics;
using Nethereum.JsonRpc.UnityClient;
using Nethereum.Hex.HexTypes;
using UnityEngine.Networking;
using System.Text;
using HardwareWallet;

public class EOSSendCoinPanel : MonoBehaviour
{

    public Color normalColor;
    public Color redColor;
    public Text cText;

    public Text moneyText;
    public Text currentAmount;
    public decimal totalCount;

    public NativeEditBox inputSendCount;
    public NativeEditBox commentText;
    public InputField commentInputField;

    public GameObject inputCount;

    public EOSWalletInfoPanel walletInfoPanel;

    public EOSSendScanSing eossendCansing;

    public InputField m_inputText;
    public InputField sendNumber;

    public string currentToAddress;
    public string currentToCount;


    public Text noticeText;


    public Text coinTypeText1;
    public Text coinTypeText2;
    public Image coinIcon;
    public Text coinCountText;
    public Text rmbCountText;

    public Text walletNameText;
    private EOSWalletInfo eosWalletInfo;

    private void JudageValue()
    {
        decimal sendCount = 0;
        if (!string.IsNullOrEmpty(currentToCount))
        {
            sendCount = decimal.Parse(currentToCount);
        }


        decimal tp = sendCount;
        if (tp <= totalCount)
        {
            cText.color = normalColor;
        }
        else
        {
            cText.color = redColor;
        }
    }

    private decimal GetRmb(decimal count, bool isGas = false)
    {
        decimal tempRMB = count * HttpManager._Intance.eos_RMB;
        //if (NewWalletManager._Intance.currentCoinType == NewWalletManager.CoinType.BTC)
        //{
        //    if (HttpManager._Intance.btc_RMB <= 0)
        //        HttpManager._Intance.btc_RMB = 45000;

        //    tempRMB = count * HttpManager._Intance.btc_RMB;

        //}
        //else if (NewWalletManager._Intance.currentCoinType == NewWalletManager.CoinType.ETH)
        //{
        //    if (HttpManager._Intance.eth_RMB <= 0)
        //        HttpManager._Intance.eth_RMB = 3000;
        //    if (isGas)
        //    {
        //        tempRMB = count * HttpManager._Intance.eth_RMB;
        //    }
        //    else
        //    {
        //        if (currentTokenItem != null && currentTokenItem.isToken)
        //        {
        //            tempRMB = (count * currentTokenItem.erc_Token.tokenService.TokenInfo.rmbValue);
        //        }
        //        else
        //        {
        //            tempRMB = count * HttpManager._Intance.eth_RMB;
        //        }
        //    }

        //}
        return tempRMB;
    }


    public void OnClickAllSendBtn()
    {
        decimal tempHasCount = totalCount;
      //decimal tempHasCount = decimal.Parse(NewWalletManager._Intance.ShowCount(null, totalCount, 6));
        decimal tempSendCountValue = 0;

        tempSendCountValue = tempHasCount;

        if (tempSendCountValue > 0)
        {
            sendNumber.text = NewWalletManager._Intance.ShowCount(null, tempSendCountValue);
        }
        else
        {
            sendNumber.text = "0";
            ShowNotice(13);
        }
    }

    public void ShowCount()
    {
        decimal tempRMB = 0;
        tempRMB = eosWalletInfo.balance * HttpManager._Intance.eos_RMB;
        totalCount = eosWalletInfo.balance;

        NewWalletManager._Intance.ShowCount(coinCountText, totalCount);
        coinCountText.text = coinCountText.text + " EOS";

        NewWalletManager._Intance.ShowCount(currentAmount, totalCount);
        currentAmount.text = currentAmount.text + " EOS";

        NewWalletManager._Intance.ShowProperty(rmbCountText, tempRMB);
    }

    public void OnSendCountChange()
    {
        currentToCount = GetInputTextValue(sendNumber,inputSendCount);

        if (string.IsNullOrEmpty(currentToCount))
        {
            currentToCount = "0";
            return;
        }

        decimal temp = decimal.Parse(currentToCount);
        decimal rmb = GetRmb(temp);
        NewWalletManager._Intance.ShowProperty(moneyText, rmb);
        JudageValue();
    }

    public void ActiveNativeEditBox(bool isActive)
    {
        m_inputText.gameObject.SetActive(isActive);
        commentText.gameObject.SetActive(isActive);
    }

    private void Reset()
    {
        noticeText.text = "";
        coinCountText.text = eosWalletInfo.balance.ToString();
        
        ShowCount();

        cText.color = normalColor;

    }

    public void Open(EOSWalletInfo eosInfo)
    {
        eosWalletInfo = eosInfo;
        gameObject.SetActive(true);
        ActiveNativeEditBox(true);
        Reset();
        
        m_inputText.GetComponent<NativeEditBox>().text = "";
        m_inputText.text = "";
        sendNumber.text = "";
        commentText.text = "";
        commentInputField.text = "";
        moneyText.text = "";

        walletNameText.text = eosWalletInfo.walletName;
    }


    public void OnClickBackBtn()
    {
        gameObject.SetActive(false);
    }

    public void OnClickScanBtn()
    {
        eossendCansing.OnClickScanSendAddress(SetSendAccount);
    }

    public void SetSendAccount(string str)
    {
        ActiveNativeEditBox(true);
        eossendCansing.inputInfoObject.SetActive(true);
        eossendCansing.scanObject.SetActive(false);
        eossendCansing.gameObject.SetActive(false);
        currentToAddress = str;
        m_inputText.GetComponent<NativeEditBox>().text = str;
        m_inputText.text = str;
    }

    public void SendCoin()
    {
        currentToAddress = m_inputText.text;

        if (string.IsNullOrEmpty(currentToAddress))
        {
            currentToAddress = m_inputText.GetComponent<NativeEditBox>().text;
        }

        if (!string.IsNullOrEmpty(currentToAddress))
        {
            currentToAddress = currentToAddress.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "");
        }
        else
        {
            PopupLine.Instance.Show("账户不能为空");
            return;
        }

        noticeText.text = "";
        string sendCount = "";
        if (!string.IsNullOrEmpty(sendNumber.text))
        {
            sendCount = sendNumber.text;
        }
        else if (string.IsNullOrEmpty(sendNumber.text))
        {
            sendCount = sendNumber.text;
        }

        if (string.IsNullOrEmpty(sendCount))
        {
            ShowNotice(15);
            return;
        }

        decimal tempNumber = decimal.Parse(sendCount);

        decimal total = eosWalletInfo.balance;
        if (tempNumber > total)
        {
            ShowNotice(13);
            return;
        }

        string meno = commentText.text;
        if (string.IsNullOrEmpty(meno))
        {
            meno = commentInputField.text;
        }
        EosGetSingInfoPanel.Instance.Gettransfer(eosWalletInfo.adminAddress, EOSSendScanSing.LastPanel.transfer, eosWalletInfo, currentToAddress, tempNumber.ToString(), meno);

        
    }
    public string jsonText;

    private void Ttext()
    {
        //Hashtable tb = Json.jsonDecode(jsonText) as Hashtable;

        //Debug.Log(Json.jsonEncode(tb));

        //string strHex = tb["signHex"].ToString();
        //Debug.Log("Hex : " + strHex);

        //eossendCansing.SetSendInfo(tb,"0.01", currentToAddress, eosWalletInfo.account, "");


        //Hashtable ht = new Hashtable();
        //ht["path"] = WalletTools.eospath_admin;
        //ht["sign"] = "EOS";
        //strHex = Encry.ToBase64String(strHex);
        //ht["data"] = strHex;

        //string signStr = Json.jsonEncode(ht);

        
        //eossendCansing.ShowQR(signStr);
    }

    public void OnClickLitteRemind()
    {
        ActiveNativeEditBox(true);
    }

    Timer m_timer;
    public void ShowNotice(int id)
    {
        LanguageSetting.Intance.SetText(noticeText, id);
        if (m_timer != null)
        {
            TimerManager.Instance.RemoveTimer(m_timer);
        }
        m_timer = TimerManager.Instance.AddTimer(3, () => 
        {
            noticeText.text = "";
        });
    }

    public void OnChangeSeedToValue(string v)
    {
        currentToCount = GetInputTextValue(sendNumber, inputSendCount);
    }
    private string GetInputTextValue(InputField text, NativeEditBox box)
    {
        string v = "";
        if (text != null && !string.IsNullOrEmpty(text.text))
        {
            v = text.text;
        }
        else if (box != null && !string.IsNullOrEmpty(box.text))
        {
            v = box.text;
        }

        return v;
    }
}
