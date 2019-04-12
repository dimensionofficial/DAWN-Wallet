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
using Nethereum.Contracts;
using Nethereum.Util;

public class EOSPaymentPanel : MonoBehaviour
{

    public EthcoinInfoItem walletItem;

    public GoodsInfo dstInfo;

    public Color normalColor;
    public Color redColor;
    public Text cText;
    public Text fText;

    public Text feeMonyText;
    public Text moneyText;
    public Text currentAmount;
    public decimal totalCount;

    public InputField commentInputField;
    public SendScanSing sendCansing;

    public Text m_inputText;
    public Text sendNumber;

    public Text noticeText;
    BitcoinPubKeyAddress changeScriptPubKey;
    private Dictionary<Coin, bool> unspentCoins;

    public Slider feeSlider;
    public GameObject sline;
    public Text feeText;
    public decimal feeCount;
    public decimal fee;

    private decimal ethTotalCount;
    public decimal tokenTotalCount;

    private void JudageValue()
    {
        if (ethTotalCount > fee || tokenTotalCount > dstInfo.price)
        {
            cText.color = normalColor;
            fText.color = normalColor;
        }
        else
        {
            cText.color = redColor;
            fText.color = redColor;
        }
    }


    public void ActiveNativeEditBox(bool isActive)
    {

   //   m_inputText.GetComponent<NativeEditBox>().enabled = isActive;
        m_inputText.gameObject.SetActive(isActive);
  //    inputSendCount.enabled = isActive;
  //    inputSendCount.gameObject.SetActive(isActive);
 //     commentText.gameObject.SetActive(isActive);
    }

    public void OnSliderValueChange()
    {

        decimal differ = HttpManager._Intance.ethTokenMaxFee - HttpManager._Intance.ethTokenMinFee;
        feeCount = (HttpManager._Intance.ethTokenMinFee + (differ * decimal.Parse(feeSlider.value.ToString())));

        feeCount = decimal.Parse(feeCount.ToString("f8"));
        fee = getEthFee(feeCount);
        feeText.text = fee + " eth";

        JudageValue();
    }

    private decimal getEthFee(decimal feecount)
    {
        decimal temp = feecount * 100000000;
        System.Numerics.BigInteger gas = System.Numerics.BigInteger.Parse(temp.ToString("f0"));
        System.Numerics.BigInteger gasPrice = System.Numerics.BigInteger.Parse(HttpManager._Intance.ethGasPrice_int64.ToString());

        System.Numerics.BigInteger tempfree = gas * gasPrice;
        decimal tempV =  Nethereum.Util.UnitConversion.Convert.FromWei(tempfree, 18);
        tempV = decimal.Parse(tempV.ToString("f8"));
        return tempV;
    }

    private void Reset()
    {
        noticeText.text = "";
        changeScriptPubKey = null;
        feeCount = 0;
        feeSlider.value = 0.15F;
        feeSlider.gameObject.SetActive(true);
        sline.SetActive(true);
       
        cText.color = normalColor;
        fText.color = normalColor;
    }

    private EOSWalletInfo eosWalletInfo;
    private decimal haveValue;
    public void Open(EthcoinInfoItem _walletItem, EOSWalletInfo _eosWalletInfo, decimal _haveValue)
    {
        eosWalletInfo = _eosWalletInfo;
        walletItem = _walletItem;
        dstInfo = PanelManager._Instance._eosRegisterPanel.paymentOrderInfo;
        gameObject.SetActive(true);
        ActiveNativeEditBox(true);
        Reset();
        haveValue = _haveValue;


        m_inputText.text = dstInfo.toAddress;
        sendNumber.text = dstInfo.price.ToString() + " " + dstInfo.symbol;
        //sendNumber.GetComponent<NativeEditBox>().text = "";
   //     commentText.text = "EOS 注册支付";
        commentInputField.text = "EOS 注册支付";
        moneyText.text = "";

        ethTotalCount = decimal.Parse(walletItem.coinInfo.ethmoney);
        tokenTotalCount = GetTokenCount();

        currentAmount.text = tokenTotalCount.ToString() + " " + dstInfo.symbol;

        OnSliderValueChange();

    }

    private decimal GetTokenCount()
    {
        return haveValue;
    }

    public void OnClickBackBtn()
    {
        gameObject.SetActive(false);
    }

    public void SendCoin()
    {
        noticeText.text = "";
        if (tokenTotalCount < dstInfo.price) //币不足
        {
            ShowNotice(13);
            return;
        }

        if (ethTotalCount < fee)// 转代币时 ETH不够矿工费
        {
            ShowNotice(21);
            return;
        }

        StartCoroutine(SendFunds(walletItem.coinInfo.address, dstInfo.toAddress));
    }



    public void OnClickLitteRemind()
    {
        ActiveNativeEditBox(true);
    }


    private bool JudgeEthAddress(string eth)
    {
        if (string.IsNullOrEmpty(eth))
        {
            return false;
        }

        if (eth.Length != 42)
            return false;

        string on = eth.Substring(0, 2);
        if (!on.Equals("0x"))
        {
            return false;
        }

        return true;
    }

    #region ETH发送
    public IEnumerator SendFunds(string addressFrom, string addressTo, bool istoken = false)
    {
        decimal temp = feeCount * 100000000;
        int tempDes = dstInfo.tokenDecimal;
        System.Numerics.BigInteger gas = System.Numerics.BigInteger.Parse(temp.ToString("f0"));
        System.Numerics.BigInteger gasPrice = System.Numerics.BigInteger.Parse(HttpManager._Intance.ethGasPrice_int64.ToString());

        System.Numerics.BigInteger toCount = Nethereum.Util.UnitConversion.Convert.ToWei(dstInfo.price, tempDes);

        System.Numerics.BigInteger tokenNumber = 0;
        string tokenData = null;

        var transactionInput = CreateTransferFundsTransactionInput(
                addressFrom,
                addressTo,
                toCount,
                new HexBigInteger(gas),
                new HexBigInteger(gasPrice),
                null
            );
        tokenData = transactionInput.Data;
        tokenNumber = toCount;
        toCount = 0;
        addressTo = dstInfo.contactAddress;

		string tempUnit = dstInfo.symbol;
        sendCansing.contractAddress = dstInfo.contactAddress; 

        QRPayTools.CreateNoSignPayQRInfo_ETH(addressFrom, addressTo, toCount, gas, gasPrice, this.gameObject, ETHInfo._url, tokenData, delegate (string qrinfo)
        {
            if (qrinfo == null)
                qrinfo = "";

            System.Numerics.BigInteger free = gas * gasPrice;
            System.Numerics.BigInteger showSendNumber = toCount;
            string tokenFullName = dstInfo.fullName;
            showSendNumber = tokenNumber;
            string str = commentInputField.text;

			sendCansing.SetSendInfo(Nethereum.Util.UnitConversion.Convert.FromWei(free, 18), showSendNumber.ToString(), Nethereum.Util.UnitConversion.Convert.FromWei(showSendNumber, tempDes), addressFrom, addressTo, "ETH", tempUnit, false, str, tokenFullName);

            if (NewWalletManager._Intance.IsNeedColdWallet)
            {

                sendCansing.ShowQR(qrinfo, true);
            }
            else
            {
                PanelManager._Instance._pingCodeInputBox.OpenMe(qrinfo, PingCodeInputBox.SingType.ETH, addressFrom, delegate ()
                {
                    sendCansing.sendPanel.ActiveNativeEditBox(true);
                });
            }
           

            sendCansing.SetPayInfo(eosWalletInfo.adminAddress, dstInfo.id);
        });

        yield return 0;

       
    }

    public TransactionInput CreateTransferFundsTransactionInput(
       string addressFrom,
       string addressTo,
       System.Numerics.BigInteger transferAmount,
       HexBigInteger gas = null,
       HexBigInteger gasPrice = null,
       HexBigInteger valueAmount = null)
    {
        Contract ct =  new Contract(null, EthTokenManager._Intance.ABI, dstInfo.contactAddress);
        var function = ct.GetFunction("transfer");
        return function.CreateTransactionInput(addressFrom, gas, gasPrice, valueAmount, addressTo, transferAmount);
    }




    #endregion

    private BitcoinAddress JudgeAddress(string btcToAddress)
    {
        try
        {
            BitcoinAddress temp = BitcoinAddress.Create(btcToAddress, NewWalletManager._Intance.btcNetwork);
            return temp;
        }
        catch (Exception ex)
        {
            return null;
        }
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

    public void OnPaymentOver()
    {
        gameObject.SetActive(false);
        PanelManager._Instance._eosRegisterPanel.gameObject.SetActive(false);
        PanelManager._Instance._eosOrderPanel.gameObject.SetActive(false);
    }

}
