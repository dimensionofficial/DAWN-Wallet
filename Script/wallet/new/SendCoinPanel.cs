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

public class SendCoinPanel : HotBasePanel {

    public Color normalColor;
    public Color redColor;
    public Text cText;
    public Text fText;

    public Text feeMonyText;
    public Text moneyText;
    public Text currentAmount;
    public decimal totalCount;

    public NativeEditBox inputSendCount;
    public NativeEditBox commentText;
    public InputField commentInputField;

    public GameObject inputCount;
    public GameObject sendLitteRemind;

    public WalletInfoPanel walletInfoPanel;

    public SendScanSing sendCansing;

    public InputField m_inputText;
    public InputField sendNumber;

 //   public List<AddressItem> addressItems = new List<AddressItem>();

    //public AddressItem currentSelectAddressItme;
    public string currentToAddress;
    public string currentToCount;


    public Text noticeText;
    BitcoinPubKeyAddress changeScriptPubKey;
    private Dictionary<Coin, bool> unspentCoins;

    public Text coinTypeText1;
    public Text coinTypeText2;
    public Image coinIcon;
    public Text coinCountText;
    public Text rmbCountText;


    public Slider feeSlider;
    public GameObject sline;
    public Text feeText;
    public decimal feeCount;
    public decimal fee;

    private int btcTransLeng = 0;
    public GameObject bteSendNextBtn;
    
    public EthTokenItem currentTokenItem;
    public Text walletNameText;

    //多签钱包列表
    public GameObject walletPanel;
    public GameObject[] wallets;
    public PubKey curWalletPuk;
    public string curWalletAddress;
    public List<PubKey> oldwallets;
    public List<string> oldwalletsAddress;
    MultiWalletInfo curWalletInfo;
    public BTCMulSig.MulSigTransactionBuilder BtcMulsigBuilder=null;
    public int surScanCount;
    List<string> needsign=new List<string>();
    //签名参数
    Money feeM;
    HashSet<Coin> coinsToSpend = new HashSet<Coin>();
    List<decimal> toCount=new List<decimal>();
    List<BitcoinAddress> toAddress=new List<BitcoinAddress>();
    string strCommentText;
    //多签判断
    public bool isMultiAddres = false;
    bool NotBtcMulsigBuilder = true;
    public Image titleImage;
    public Color normlColor;
    public Color multiColor;

    public Image title2Image;

    public Sprite norml2Sprite;
    public Sprite multi2Sprite;

    public Image title3Image;
    public Image title4Image;
    public Sprite norml3Sprite;
    public Sprite multi3Sprite;
    public void ShowWalletList()
    {
        for (int i = 0; i < curWalletInfo.btcAddress.Count; i++)
        {
            wallets[i].GetComponent<ReturnWalletInfo1>().Rest();
            Text _address = wallets[i].GetComponent<ReturnWalletInfo1>().address;
            Text _note = wallets[i].GetComponent<ReturnWalletInfo1>().note;
            Button _btn = wallets[i].GetComponentInChildren<ReturnWalletInfo1>().button;
            _address.text ="地址:"+curWalletInfo.btcAddress[i];
            _note.text="备注:"+ curWalletInfo.walletName[i];
            wallets[i].SetActive(true);
        }
    }
    public void OnInputAddress()
    {
        string ad = GetInputTextValue(m_inputText, m_inputText.GetComponent<NativeEditBox>());
        string result = ad.Replace("我的钱包地址: ","");
        m_inputText.text = result;
        m_inputText.GetComponent<NativeEditBox>().text = result;
        currentToAddress = result;
    }

    private void JudageValue()
    {
        decimal sendCount = 0;
        if (!string.IsNullOrEmpty(currentToCount))
        {
            sendCount = decimal.Parse(currentToCount);
        }

        if (currentTokenItem != null && currentTokenItem.isToken)
        {
            sendCount = 0;
        }

        decimal tp = sendCount + fee;
        if (tp <= totalCount)
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

    private decimal GetRmb(decimal count, bool isGas = false)
    {
        decimal tempRMB = 0;
        if (NewWalletManager._Intance.currentCoinType == NewWalletManager.CoinType.BTC)
        {
            if (HttpManager._Intance.btc_RMB <= 0)
                HttpManager._Intance.btc_RMB = 45000;

            tempRMB = count * HttpManager._Intance.btc_RMB;
        }
        else if (NewWalletManager._Intance.currentCoinType == NewWalletManager.CoinType.USDT)
        {
            tempRMB = count * HttpManager._Intance.current_usdt;
        }
        else if (NewWalletManager._Intance.currentCoinType == NewWalletManager.CoinType.ETH)
        {
            if (HttpManager._Intance.eth_RMB <= 0)
                HttpManager._Intance.eth_RMB = 3000;
            if (isGas)
            {
                tempRMB = count * HttpManager._Intance.eth_RMB;
            }
            else
            {
                if (currentTokenItem != null && currentTokenItem.isToken)
                {
                    tempRMB = (count * currentTokenItem.erc_Token.tokenService.TokenInfo.rmbValue);
                }
                else
                {
                    tempRMB = count * HttpManager._Intance.eth_RMB;
                }
            }

        }
        return tempRMB;
    }


    public void OnClickAllSendBtn()
    {
        decimal tempHasCount = totalCount;
      //decimal tempHasCount = decimal.Parse(NewWalletManager._Intance.ShowCount(null, totalCount, 6));
        decimal tempSendCountValue = 0;

        if (NewWalletManager._Intance.currentCoinType == NewWalletManager.CoinType.BTC)
        {
            if (!string.IsNullOrEmpty(currentToAddress))
            {
                currentToAddress = currentToAddress.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "");
            }

            BitcoinAddress tempD = JudgeAddress(currentToAddress);
            if (tempD == null)
            {
                ShowNotice(12);
                return;
            }

            if (tempHasCount < decimal.Parse("0.000006"))
            {
                ShowNotice(13);
                return;
            }

            currentToCount = tempHasCount.ToString();
            SendAllBTCCallBack = ShowAllSendBTC;
            GetBTCGasFee();
        }else if (NewWalletManager._Intance.currentCoinType == NewWalletManager.CoinType.USDT)
        {
            Debug.Log("OnClickAllSendBtn()===NewWalletManager._Intance.currentCoinType == NewWalletManager.CoinType.USDT");
            if (!string.IsNullOrEmpty(currentToAddress))
            {
                currentToAddress = currentToAddress.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "");
            }
            //---------------------------!?!
            BitcoinAddress tempD = JudgeAddress(currentToAddress);
            if (tempD == null)
            {
                ShowNotice(12);
                return;
            }

            if (tempHasCount <= decimal.Parse("0"))
            {
                ShowNotice(22);
                return;
            }

            currentToCount = tempHasCount.ToString();
            SendAllBTCCallBack = ShowAllSendBTC;
            HttpManager._Intance.loadingPanel.SetActive(true);
            GetTransaction();
        }
        else if (NewWalletManager._Intance.currentCoinType == NewWalletManager.CoinType.ETH)
        {
            if (JudgeEthAddress(currentToAddress) == false)
            {
                ShowNotice(12);
                return;
            }

            if (currentTokenItem != null && currentTokenItem.isToken)
            {
                if (decimal.Parse(walletInfoPanel.currentItem.coinInfo.money.ToString()) < fee)// 转代币时 ETH不够矿工费
                {
                    ShowNotice(21);
                    return;
                }

                sendNumber.text = NewWalletManager._Intance.ShowCount(null, tempHasCount);

                return;
            }
            else
            {
                tempSendCountValue = tempHasCount - fee;
            }

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
    }

    public void ShowCount()
    {
        decimal tempRMB = 0;
        totalCount = 0;
        if (NewWalletManager._Intance.currentCoinType == NewWalletManager.CoinType.BTC)
        {
            tempRMB = walletInfoPanel.currentItem.coinInfo.money * HttpManager._Intance.btc_RMB;
            totalCount = walletInfoPanel.currentItem.coinInfo.money;

            NewWalletManager._Intance.ShowCount(coinCountText, totalCount);
            coinCountText.text = coinCountText.text + " BTC";

            NewWalletManager._Intance.ShowCount(currentAmount, totalCount);
            currentAmount.text = currentAmount.text + " BTC";

        }
        else if (NewWalletManager._Intance.currentCoinType == NewWalletManager.CoinType.USDT)
        {
            //---------------------------!?!
            tempRMB = walletInfoPanel.currentItem.coinInfo.money * HttpManager._Intance.current_usdt;
            totalCount = walletInfoPanel.currentItem.coinInfo.money;

            NewWalletManager._Intance.ShowCount(coinCountText, totalCount);
            coinCountText.text = coinCountText.text + " USDT";

            NewWalletManager._Intance.ShowCount(currentAmount, totalCount);
            currentAmount.text = currentAmount.text + " USDT";
        }
        else if (NewWalletManager._Intance.currentCoinType == NewWalletManager.CoinType.ETH)
        {
            if (currentTokenItem!=null && currentTokenItem.isToken)
            {
                totalCount = currentTokenItem.erc_Token.tokenNumber;

                tempRMB = (totalCount * currentTokenItem.erc_Token.tokenService.TokenInfo.rmbValue);

                NewWalletManager._Intance.ShowCount(coinCountText, currentTokenItem.erc_Token.tokenNumber);
                coinCountText.text = coinCountText.text + " " + currentTokenItem.erc_Token.tokenService.TokenInfo.symbol;

                NewWalletManager._Intance.ShowCount(currentAmount, currentTokenItem.erc_Token.tokenNumber);
                currentAmount.text = currentAmount.text + " " + currentTokenItem.erc_Token.tokenService.TokenInfo.symbol;
            }
            else
            {
                tempRMB = walletInfoPanel.currentItem.coinInfo.money * HttpManager._Intance.eth_RMB;
                totalCount = walletInfoPanel.currentItem.coinInfo.money;

                NewWalletManager._Intance.ShowCount(coinCountText, totalCount);
                coinCountText.text = coinCountText.text + " ETH";

                NewWalletManager._Intance.ShowCount(currentAmount, totalCount);
                currentAmount.text = currentAmount.text + " ETH";
            } 
        }

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
        decimal rmb = 0;
        rmb = GetRmb(temp);

        NewWalletManager._Intance.ShowProperty(moneyText, rmb);
        JudageValue();
    }

    public void ActiveNativeEditBox(bool isActive)
    {
        m_inputText.GetComponent<NativeEditBox>().enabled = isActive;
        m_inputText.gameObject.SetActive(isActive);
        commentText.gameObject.SetActive(isActive);
    }
    
    public void OnSliderValueChange()
    {
        //---------------------------!?!
        if (NewWalletManager._Intance.currentCoinType == NewWalletManager.CoinType.BTC)
        {
            Money money = new Money(decimal.Parse((HttpManager._Intance.minBTCGasPrice + feeSlider.value * HttpManager._Intance.btcGaspercentage).ToString()) * btcTransLeng, MoneyUnit.Satoshi);
            feeCount = decimal.Parse(money.ToString());
     //       feeCount += decimal.Parse("0.000006");
            fee = feeCount;
            fee = decimal.Parse(fee.ToString("f8"));
            feeText.text = fee  + " btc";
        }
        else if (NewWalletManager._Intance.currentCoinType == NewWalletManager.CoinType.USDT)
        {
            Debug.Log("btcTransLeng:"+ btcTransLeng);
            Debug.Log(HttpManager._Intance.btcGaspercentage);
            Money money = new Money(decimal.Parse((6 + feeSlider.value*5).ToString()) * btcTransLeng, MoneyUnit.Satoshi);
            //feeCount = decimal.Parse("0.000038");
            feeCount= decimal.Parse(money.ToString());
            fee = feeCount;
            fee = decimal.Parse(fee.ToString("f8"));
            feeText.text = fee + " btc";
        }
        else
        {
            decimal differ;
            if (currentTokenItem != null && currentTokenItem.isToken)
            {
                differ = HttpManager._Intance.ethTokenMaxFee - HttpManager._Intance.ethTokenMinFee;
                feeCount = (HttpManager._Intance.ethTokenMinFee + (differ * decimal.Parse(feeSlider.value.ToString())));
            }
            else
            {
                differ = HttpManager._Intance.ethMaxFee - HttpManager._Intance.ethMinFee;
                feeCount = (HttpManager._Intance.ethMinFee + (differ * decimal.Parse(feeSlider.value.ToString())));
            }
          
            feeCount = decimal.Parse(feeCount.ToString("f8"));
            fee = getEthFee(feeCount);
            feeText.text = fee + " eth";
        }

        decimal rmb =0;
        Debug.Log(NewWalletManager._Intance.currentCoinType);
        if (NewWalletManager._Intance.currentCoinType == NewWalletManager.CoinType.USDT)
        {
            
            rmb = fee * HttpManager._Intance.btc_RMB;
        }
        else
        {
            rmb = GetRmb(fee, true);
        }
        NewWalletManager._Intance.ShowProperty(feeMonyText, rmb);

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
        walletPanel.SetActive(false);
        for (int i = 0; i < wallets.Length; i++)
        {
            wallets[i].SetActive(false);
        }
        curWalletAddress = null;
        NotBtcMulsigBuilder = true;
        BtcMulsigBuilder = null;


        btcTransLeng = 0;
        noticeText.text = "";
        changeScriptPubKey = null;
        feeCount = 0;
        feeSlider.value = 0.15F;
        feeSlider.gameObject.SetActive(true);
        sline.SetActive(true);
        bteSendNextBtn.SetActive(false);
        coinCountText.text = walletInfoPanel.currentItem.coinInfo.money.ToString();
        if (NewWalletManager._Intance.currentCoinType == NewWalletManager.CoinType.BTC)
        {
            feeSlider.gameObject.SetActive(false);
            sline.SetActive(false);
            coinIcon.overrideSprite = TextureUIAsset._Instance.btcIcon;
            coinTypeText1.text = "BTC";
            coinTypeText2.text = "BTC";
        }
        else if (NewWalletManager._Intance.currentCoinType == NewWalletManager.CoinType.USDT)
        {
            feeSlider.gameObject.SetActive(false);
            sline.SetActive(false);
            coinIcon.overrideSprite = TextureUIAsset._Instance.usdtIcon;
            coinTypeText1.text = "USDT";
            coinTypeText2.text = "USDT";
        }
        else if (NewWalletManager._Intance.currentCoinType == NewWalletManager.CoinType.ETH)
        {
            decimal differ;
            if (currentTokenItem != null && currentTokenItem.isToken)
            {
                differ = HttpManager._Intance.ethTokenMaxFee - HttpManager._Intance.ethTokenMinFee;
                feeCount = (HttpManager._Intance.ethTokenMinFee + (differ * decimal.Parse(feeSlider.value.ToString())));
                coinTypeText1.text = currentTokenItem.erc_Token.tokenService.TokenInfo.symbol;
                coinTypeText2.text = coinTypeText1.text;
            }
            else
            {
                differ = HttpManager._Intance.ethMaxFee - HttpManager._Intance.ethMinFee;
                feeCount = (HttpManager._Intance.ethMinFee + (differ * decimal.Parse(feeSlider.value.ToString())));
                coinTypeText1.text = "ETH";
                coinTypeText2.text = "ETH";
            }
            feeCount = decimal.Parse(feeCount.ToString("F8"));
            fee = getEthFee(feeCount);
            feeText.text = fee + " eth";

            if (PanelManager._Instance._WalletInfoPanel.currentTokenItem != null)
            {
                coinIcon.overrideSprite = PanelManager._Instance._WalletInfoPanel.currentTokenItem.image.overrideSprite;
            }
            else
            {
                coinIcon.overrideSprite = TextureUIAsset._Instance.ethIcon;
            }
        }

        decimal rmb = GetRmb(fee, true);

        NewWalletManager._Intance.ShowProperty(feeMonyText, rmb);

        ShowCount();

        cText.color = normalColor;
        fText.color = normalColor;
    }


    public void SetETHTokenItem(EthTokenItem tokeItem)
    {
        currentTokenItem = tokeItem;
    }

    public override void Open()
    {
        oldwallets = new List<PubKey>();
        oldwalletsAddress = new List<string>();
        isMultiAddres = MultiJSData.instance.IsMultiSigAddress(walletInfoPanel.currentItem.coinInfo.address);
        if (isMultiAddres)
        {
            titleImage.color = multiColor;
            title2Image.sprite = multi2Sprite;
            title3Image.sprite = multi3Sprite;
            title4Image.sprite = multi3Sprite;
        }
        else
        {
            titleImage.color = normlColor;
            title2Image.sprite = norml2Sprite;
            title3Image.sprite = norml3Sprite;
            title4Image.sprite = norml3Sprite;
        }
        SendAllBTCCallBack = null;

        gameObject.SetActive(true);
        ActiveNativeEditBox(true);
        Reset();
        
        m_inputText.GetComponent<NativeEditBox>().text = "";
        m_inputText.text = "";
        sendNumber.text = "";
 //     sendNumber.GetComponent<NativeEditBox>().text = "";
        commentText.text = "";
        commentInputField.text = "";
        moneyText.text = "";

        walletNameText.text = PanelManager._Instance._mainPanel.walletInfoPanel.currentItem.coinname;
    }


    public void OnClickBackBtn()
    {
        Closed();
    }

    public void OnClickScanBtn()
    {
        sendCansing.OnClickScanSendAddress(SetBTCSendAddress);
    }

    public void SetBTCSendAddress(string str)
    {
        ActiveNativeEditBox(true);
        sendCansing.inputInfoObject.SetActive(true);
        sendCansing.scanObject.SetActive(false);
        sendCansing.gameObject.SetActive(false);
        try
        {
            Hashtable has = Json.jsonDecode(str) as Hashtable;
            NewWalletManager.CoinType coinType = (NewWalletManager.CoinType)(int.Parse(has["CoinType"].ToString()));
           
            if (coinType != NewWalletManager._Intance.currentCoinType)
            {
                PopupLine.Instance.Show("录入的地址不符法");
                return;
            }
            
            string toAddress = has["receiveAddress"].ToString();
            string tocount = has["receiveCount"].ToString();

            noticeText.text = "";

            currentToAddress = toAddress;
            m_inputText.GetComponent<NativeEditBox>().text = toAddress;
            m_inputText.text = toAddress;

//            sendNumber.GetComponent<NativeEditBox>().text = tocount;
            sendNumber.text = tocount;

        }
        catch
        {
            if (NewWalletManager._Intance.currentCoinType == NewWalletManager.CoinType.BTC|| NewWalletManager._Intance.currentCoinType == NewWalletManager.CoinType.USDT)
            {
                BitcoinAddress temp = JudgeAddress(str);
                if (temp == null)
                {
                    ShowNotice(12);
                }
                else
                {
                    noticeText.text = "";
                    currentToAddress = str;
                    m_inputText.GetComponent<NativeEditBox>().text = str;
                    m_inputText.text = str;
                }
            }
            else if (NewWalletManager._Intance.currentCoinType == NewWalletManager.CoinType.ETH)
            {
                currentToAddress = str;
                m_inputText.GetComponent<NativeEditBox>().text = str;
                m_inputText.text = str;
            }
        }
    }

    public void SendCoin()
    {
        //StartCoroutine(SendFunds(walletInfoPanel.currentItem.coinInfo.address, currentToAddress, ""));
        //return;
        if (!string.IsNullOrEmpty(currentToAddress))
        {
            currentToAddress = currentToAddress.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "");
        }

        if (NewWalletManager._Intance.currentCoinType == NewWalletManager.CoinType.BTC)
        {
            BitcoinAddress tempD = JudgeAddress(currentToAddress);
            if (tempD == null)
            {
                ShowNotice(12);
                return;
            }
            fee = 0;
            feeCount = 0;
        }
        if (NewWalletManager._Intance.currentCoinType == NewWalletManager.CoinType.USDT)
        {
            //----------------------------!?!
            BitcoinAddress tempD = JudgeAddress(currentToAddress);
            if (tempD == null)
            {
                ShowNotice(12);
                return;
            }
            fee = 0;
            feeCount = 0;
        }
        if (NewWalletManager._Intance.currentCoinType == NewWalletManager.CoinType.ETH)
        {

            if (JudgeEthAddress(currentToAddress) == false)
            {
                ShowNotice(12);
                return;
            }
        }

        noticeText.text = "";
        string sendCount = "";
        if (!string.IsNullOrEmpty(sendNumber.text))
        {
            sendCount = sendNumber.text;
//            inputSendCount.text = sendCount;
        }
        else if (string.IsNullOrEmpty(sendNumber.text))
        {
            sendCount = sendNumber.text;
//            inputSendCount.text = sendCount;
        }

        if (string.IsNullOrEmpty(sendCount))
        {
            ShowNotice(15);
            return;
        }


        if (NewWalletManager._Intance.currentCoinType == NewWalletManager.CoinType.ETH)
        {
            if (currentTokenItem != null && currentTokenItem.isToken)
            {
                if (decimal.Parse(sendCount) > currentTokenItem.erc_Token.tokenNumber) //币不足
                {
                    ShowNotice(13);
                    return;
                }
                else if (!NewWalletManager._Intance.isTestSend  && decimal.Parse(walletInfoPanel.currentItem.coinInfo.money.ToString()) < fee)// 转代币时 ETH不够矿工费
                {
                    ShowNotice(21);
                    return;
                }
            }
            else
            {
                decimal tempNumber = decimal.Parse(sendCount) + fee;

                decimal total = decimal.Parse(walletInfoPanel.currentItem.coinInfo.money.ToString());
                if (tempNumber > total)
                {
                    ShowNotice(13);
                    return;
                }
            }
        }
        else
        {
            decimal tempNumber = decimal.Parse(sendCount) + fee;

            decimal total = decimal.Parse(walletInfoPanel.currentItem.coinInfo.money.ToString());
            if (tempNumber > total)
            {
                ShowNotice(13);
                return;
            }
        }

        if (NewWalletManager._Intance.currentCoinType == NewWalletManager.CoinType.BTC)
        {

            if (decimal.Parse(sendCount) < decimal.Parse("0.000006"))
            {
                inputCount.GetComponent<NativeEditBox>().enabled = false;
                sendLitteRemind.SetActive(true);
                return;
            }

            GetBTCGasFee();
        }
        else if (NewWalletManager._Intance.currentCoinType == NewWalletManager.CoinType.ETH)
        {
            StartCoroutine(SendFunds(walletInfoPanel.currentItem.coinInfo.address, currentToAddress, sendCount));
        }
        else if (NewWalletManager._Intance.currentCoinType == NewWalletManager.CoinType.USDT)
        {
            if (decimal.Parse(sendCount) < decimal.Parse("0.000006"))
            {
                inputCount.GetComponent<NativeEditBox>().enabled = false;
                sendLitteRemind.SetActive(true);
                return;
            }
            HttpManager._Intance.loadingPanel.SetActive(true);
            GetTransaction();
        }
    }


    /// <summary>
    /// 获取显示BTC矿工费
    /// </summary>
    private void GetBTCGasFee()
    {
        HttpManager._Intance.loadingPanel.SetActive(true);
        BitcoinAddress myAddress = BitcoinAddress.Create(walletInfoPanel.currentItem.coinInfo.address, NewWalletManager._Intance.btcNetwork);

        //Debug.Log(walletInfoPanel.currentItem.coinInfo.address+ " : " +  NewWalletManager._Intance.btcNetwork);

    //    changeScriptPubKey = new BitcoinPubKeyAddress(walletInfoPanel.currentItem.coinInfo.address, NewWalletManager._Intance.btcNetwork);
    //    Debug.Log(changeScriptPubKey);
        //StartCoroutine(HttpManager._Intance.GetBTCUnspentBlance(walletInfoPanel.currentItem.coinInfo.address, BTCSend1));
        
        QBitNinja4Unity.QBitNinjaClient.GetBalance(myAddress, NewWalletManager._Intance.btcNetwork, Send1_1, false, true, true, (ex) =>
        {
            PopupLine.Instance.Show(ex.ToString());
            HttpManager._Intance.loadingPanel.SetActive(false);
        });
    }




    public void OnClickLitteRemind()
    {
        ActiveNativeEditBox(true);
        sendLitteRemind.SetActive(false);
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
    public IEnumerator SendFunds(string addressFrom, string addressTo, string transferAmount, bool istoken = false)
    {
        decimal temp = feeCount * 100000000;
        int tempDes = 18;
        if (currentTokenItem != null && currentTokenItem.isToken)
        {
            tempDes = currentTokenItem.erc_Token.tokenService.tokenDecimal;
        }
        System.Numerics.BigInteger gas = System.Numerics.BigInteger.Parse(temp.ToString("f0"));
        System.Numerics.BigInteger gasPrice = System.Numerics.BigInteger.Parse(HttpManager._Intance.ethGasPrice_int64.ToString());

        System.Numerics.BigInteger toCount = Nethereum.Util.UnitConversion.Convert.ToWei(decimal.Parse(transferAmount), tempDes);

        System.Numerics.BigInteger tokenNumber = 0;
        string tokenData = null;
        if (currentTokenItem != null && currentTokenItem.isToken)
        {
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
            addressTo = currentTokenItem.erc_Token.tokenService.tokenContractAddress;
        }
		string tempUnit = "ETH";
        sendCansing.contractAddress = "";
        if (tokenData != null)
		{
			tempUnit = currentTokenItem.erc_Token.tokenService.TokenInfo.symbol;
            sendCansing.contractAddress = currentTokenItem.erc_Token.tokenService.tokenContractAddress;

        }
        QRPayTools.CreateNoSignPayQRInfo_ETH(addressFrom, addressTo, toCount, gas, gasPrice, this.gameObject, ETHInfo._url, tokenData, delegate (string qrinfo)
        {
            if (qrinfo == null)
                qrinfo = "";

            System.Numerics.BigInteger free = gas * gasPrice;
            System.Numerics.BigInteger showSendNumber = toCount;
            bool isSameUnit = true;
            string tokenFullName = "";
           
            if (currentTokenItem != null && currentTokenItem.isToken)
            {
                showSendNumber = tokenNumber;
                tokenFullName = currentTokenItem.erc_Token.tokenService.TokenInfo.fullName;
                isSameUnit = false;
            }

            string str = commentText.text;
            if (string.IsNullOrEmpty(str))
            {
                str = commentInputField.text;
            }

            sendCansing.SetSendInfo(Nethereum.Util.UnitConversion.Convert.FromWei(free, 18), showSendNumber.ToString(), Nethereum.Util.UnitConversion.Convert.FromWei(showSendNumber, tempDes), addressFrom, addressTo, "ETH", tempUnit, isSameUnit, str, tokenFullName);

            if (NewWalletManager._Intance.IsNeedColdWallet)
            {
                sendCansing.ShowQR(qrinfo);
            }
            else
            {
                PanelManager._Instance._pingCodeInputBox.OpenMe(qrinfo, PingCodeInputBox.SingType.ETH, addressFrom, delegate() 
                {
                    sendCansing.sendPanel.ActiveNativeEditBox(true);
                });
            }
//            Debug.Log("发送大数 = " + showSendNumber + " 数量 = " + Nethereum.Util.UnitConversion.Convert.FromWei(showSendNumber, tempDes));
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
        var function = currentTokenItem.erc_Token.tokenService.contract.GetFunction("transfer");
        return function.CreateTransactionInput(addressFrom, gas, gasPrice, valueAmount, addressTo, transferAmount);
    }




    #endregion


    #region BTC发送

    //void BTCSend1(ArrayList resultList)
    //{
    //    if (resultList == null)
    //    {
    //        return;
    //    }
        
    //    for (int i = 0; i < resultList.Count; i++)
    //    {
    //        Hashtable h = resultList[i] as Hashtable;
    //        Debug.Log(h["value"]);
    //    }
    //    return;

    //    //JsonData jd = JsonMapper.ToObject(result);
    //    //QBitNinja4Unity.Models.BalanceModel model = new QBitNinja4Unity.Models.BalanceModel();
    //    //object te = jd["continuation"];
    //    //if (te != null)
    //    //{
    //    //    model.continuation = (string)jd["continuation"];
    //    //}
    //    //else
    //    //{
    //    //    model.continuation = "";
    //    //}
    //    //List<QBitNinja4Unity.Models.BalanceOperation> bopList = new List<QBitNinja4Unity.Models.BalanceOperation>();

    //    //ArrayList strs = new ArrayList(jd["operations"]);
    //    //long textAmount = 0;
    //    //for (int i = 0; i < strs.Count; i++)
    //    //{
    //    //    QBitNinja4Unity.Models.BalanceOperation bop = new QBitNinja4Unity.Models.BalanceOperation();
    //    //    JsonData t = (JsonData)strs[i];
    //    //    string tempAmount = t["amount"].ToString();
    //    //    bop.Amount = long.Parse(tempAmount);
    //    //    textAmount += bop.Amount;
    //    //    bop.confirmations = (int)t["confirmations"];
    //    //    bop.height = (int)t["height"];
    //    //    object tempBlockID = t["blockId"];
    //    //    if (tempBlockID != null)
    //    //        bop.blockId = (string)t["blockId"];
    //    //    else
    //    //        bop.blockId = "";

    //    //    object tempTransID = t["transactionId"];
    //    //    if (tempTransID != null)
    //    //        bop.transactionId = (string)t["transactionId"];
    //    //    else
    //    //        bop.transactionId = "";

    //    //    ArrayList strs1 = new ArrayList(t["receivedCoins"]);
    //    //    CoinJson[] coinJson = new CoinJson[strs1.Count];
    //    //    for (int j = 0; j < strs1.Count; j++)
    //    //    {
    //    //        JsonData tj = (JsonData)strs1[j];
    //    //        CoinJson cj = new CoinJson();
    //    //        object tempTransactionId = tj["transactionId"];
    //    //        if (tempTransactionId != null)
    //    //            cj.transactionId = (string)tj["transactionId"];
    //    //        cj.index = uint.Parse(tj["index"].ToString());
    //    //        cj.value = long.Parse(tj["value"].ToString());
    //    //        object tempScripPk = tj["scriptPubKey"];
    //    //        if (tempScripPk != null)
    //    //            cj.scriptPubKey = (string)tj["scriptPubKey"];
    //    //        object tempO = tj["redeemScript"];
    //    //        if (tempO != null)
    //    //            cj.redeemScript = (string)tj["redeemScript"];
    //    //        else
    //    //            cj.redeemScript = "";

    //    //        coinJson[j] = cj;
    //    //    }
    //    //    bop.receivedCoins = coinJson;

    //    //    ArrayList strs2 = new ArrayList(t["spentCoins"]);
    //    //    CoinJson[] coinJson2 = new CoinJson[strs2.Count];
    //    //    for (int j = 0; j < strs2.Count; j++)
    //    //    {
    //    //        JsonData tj = (JsonData)strs2[j];
    //    //        CoinJson cj = new CoinJson();
    //    //        object tempTransactionId = tj["transactionId"];
    //    //        if (tempTransactionId != null)
    //    //            cj.transactionId = (string)tj["transactionId"];
    //    //        else
    //    //            cj.transactionId = "";
    //    //        cj.index = uint.Parse(tj["index"].ToString());
    //    //        cj.value = long.Parse(tj["value"].ToString());
    //    //        object tempScripPk = tj["scriptPubKey"];
    //    //        if (tempScripPk != null)
    //    //            cj.scriptPubKey = (string)tj["scriptPubKey"];
    //    //        else
    //    //            cj.scriptPubKey = "";

    //    //        object tempO = tj["redeemScript"];
    //    //        if (tempO != null)
    //    //            cj.redeemScript = (string)tj["redeemScript"];
    //    //        else
    //    //            cj.redeemScript = "";

    //    //        coinJson2[j] = cj;
    //    //    }
    //    //    bop.spentCoins = coinJson2;
    //    //    bop.firstSeen = (string)t["firstSeen"];

    //    //    bopList.Add(bop);
    //    //}

    //    //model.operations = bopList.ToArray();
    //    //model.conflictedOperations = new List<QBitNinja4Unity.Models.BalanceOperation>().ToArray();
    //    //QBitNinja.Client.Models.BalanceModel r = model.Result();

    //    //unspentCoins = new Dictionary<Coin, bool>();
    //    //foreach (var operation in r.Operations)
    //    //{
    //    //    foreach (var elem in operation.ReceivedCoins.Select(coin => coin as Coin))
    //    //    {
    //    //        unspentCoins.Add(elem, operation.Confirmations > 0);
    //    //    }
    //    //}
    //    //GetTransaction();
    //}

    //解析json 获取信息
    void Send1_1(string result)
    {
        JsonData jd = JsonMapper.ToObject(result);
        QBitNinja4Unity.Models.BalanceModel model = new QBitNinja4Unity.Models.BalanceModel();
        object te = jd["continuation"];
        if (te != null)
        {
            model.continuation = (string)jd["continuation"];
        }
        else
        {
            model.continuation = "";
        }
        List<QBitNinja4Unity.Models.BalanceOperation> bopList = new List<QBitNinja4Unity.Models.BalanceOperation>();

        ArrayList strs = new ArrayList(jd["operations"]);
        long textAmount = 0;
        for (int i = 0; i < strs.Count; i++)
        {
            QBitNinja4Unity.Models.BalanceOperation bop = new QBitNinja4Unity.Models.BalanceOperation();
            JsonData t = (JsonData)strs[i];
            string tempAmount = t["amount"].ToString();
            bop.Amount = long.Parse(tempAmount);
            textAmount += bop.Amount;
            bop.confirmations = (int)t["confirmations"];
            bop.height = (int)t["height"];
            object tempBlockID = t["blockId"];
            if (tempBlockID != null)
                bop.blockId = (string)t["blockId"];
            else
                bop.blockId = "";

            object tempTransID = t["transactionId"];
            if (tempTransID != null)
                bop.transactionId = (string)t["transactionId"];
            else
                bop.transactionId = "";

            ArrayList strs1 = new ArrayList(t["receivedCoins"]);
            CoinJson[] coinJson = new CoinJson[strs1.Count];
            for (int j = 0; j < strs1.Count; j++)
            {
                JsonData tj = (JsonData)strs1[j];
                CoinJson cj = new CoinJson();
                object tempTransactionId = tj["transactionId"];
                if (tempTransactionId != null)
                    cj.transactionId = (string)tj["transactionId"];
                cj.index = uint.Parse(tj["index"].ToString());
                cj.value = long.Parse(tj["value"].ToString());
                object tempScripPk = tj["scriptPubKey"];
                if (tempScripPk != null)
                    cj.scriptPubKey = (string)tj["scriptPubKey"];
                object tempO = tj["redeemScript"];
                if (tempO != null)
                    cj.redeemScript = (string)tj["redeemScript"];
                else
                    cj.redeemScript = "";

                coinJson[j] = cj;
            }
            bop.receivedCoins = coinJson;

            ArrayList strs2 = new ArrayList(t["spentCoins"]);
            CoinJson[] coinJson2 = new CoinJson[strs2.Count];
            for (int j = 0; j < strs2.Count; j++)
            {
                JsonData tj = (JsonData)strs2[j];
                CoinJson cj = new CoinJson();
                object tempTransactionId = tj["transactionId"];
                if (tempTransactionId != null)
                    cj.transactionId = (string)tj["transactionId"];
                else
                    cj.transactionId = "";
                cj.index = uint.Parse(tj["index"].ToString());
                cj.value = long.Parse(tj["value"].ToString());
                object tempScripPk = tj["scriptPubKey"];
                if (tempScripPk != null)
                    cj.scriptPubKey = (string)tj["scriptPubKey"];
                else
                    cj.scriptPubKey = "";

                object tempO = tj["redeemScript"];
                if (tempO != null)
                    cj.redeemScript = (string)tj["redeemScript"];
                else
                    cj.redeemScript = "";

                coinJson2[j] = cj;
            }
            bop.spentCoins = coinJson2;
            bop.firstSeen = (string)t["firstSeen"];

            bopList.Add(bop);
        }

        model.operations = bopList.ToArray();
        model.conflictedOperations = new List<QBitNinja4Unity.Models.BalanceOperation>().ToArray();
        QBitNinja.Client.Models.BalanceModel r = model.Result();

        unspentCoins = new Dictionary<Coin, bool>();
        foreach (var operation in r.Operations)
        {
            foreach (var elem in operation.ReceivedCoins.Select(coin => coin as Coin))
            {
                unspentCoins.Add(elem, operation.Confirmations > 0);
            }
        }
        GetTransaction();
    }



    //广播交易
    public void ChooseSend()
    {
        if (walletInfoPanel.currentItem.type== NewWalletManager.CoinType.USDT)
        {
            Debug.Log("walletInfoPanel.currentItem.type== NewWalletManager.CoinType.USDT=" + (walletInfoPanel.currentItem.type == NewWalletManager.CoinType.USDT));
            SendUSDT();
        }
        else
        {
            NotBtcMulsigBuilder = true;
            if (isMultiAddres)
            {
                Debug.Log(isMultiAddres);
                ShowWalletList();
                commentText.gameObject.SetActive(false);
                walletPanel.SetActive(true);
            }
            else
            {
                Debug.Log(isMultiAddres);
                Send2();
            }
        }
    }
    public void Send2()
    {
        
        if (isMultiAddres&&(!NotBtcMulsigBuilder))
        {

        }
        else
        {
            BitcoinAddress tempD = JudgeAddress(currentToAddress);
            if (tempD == null)
            {
                ShowNotice(12);
                return;
            }

            feeM = new Money(fee, MoneyUnit.BTC);

            
            //decimal tempNumber = decimal.Parse(inputSendCount.text) + fee;
            decimal tempNumber = decimal.Parse(sendNumber.text) + fee;

            decimal total = decimal.Parse(walletInfoPanel.currentItem.coinInfo.money.ToString());
            if (tempNumber > total)
            {
                ShowNotice(13);
                return;
            }

            // 5. How much money we can spend?
            Money availableAmount = Money.Zero;
            Money unconfirmedAvailableAmount = Money.Zero;
            foreach (var elem in unspentCoins)
            {
                if (elem.Value)
                {
                    availableAmount += elem.Key.Amount;
                }
            }

            decimal seedCount = 0;


            toCount = new List<decimal>();
            string v = GetInputTextValue(sendNumber, inputSendCount);
            toCount.Add(decimal.Parse(v));

            toAddress = new List<BitcoinAddress>();
            toAddress.Add(tempD);

            seedCount = decimal.Parse(v);

            Money amountToSend = new Money((decimal)seedCount, MoneyUnit.BTC);

            Money totalOutAmount = amountToSend + feeM;

            // 7. Do some checks
            if (availableAmount < totalOutAmount)
            {
                ShowNotice(20);
                return;
            }


            // 8. Select coins
            
            var unspentConfirmedCoins = new List<Coin>();
            var unspentUnconfirmedCoins = new List<Coin>();
            foreach (var elem in unspentCoins)
                if (elem.Value) unspentConfirmedCoins.Add(elem.Key);
                else unspentUnconfirmedCoins.Add(elem.Key);

            bool haveEnough = SelectCoins(ref coinsToSpend, totalOutAmount, unspentConfirmedCoins);
            if (!haveEnough)
                haveEnough = SelectCoins(ref coinsToSpend, totalOutAmount, unspentUnconfirmedCoins);
            if (!haveEnough)
            {
                ShowNotice(16);
                return;
            }

            strCommentText = commentText.text;
            if (string.IsNullOrEmpty(strCommentText))
            {
                strCommentText = commentInputField.text;
            }

            bteSendNextBtn.SetActive(false);
            //feeM = new Money(fee - decimal.Parse("0.000006"), MoneyUnit.BTC);
            feeM = new Money(fee, MoneyUnit.BTC);
        }
        //判断是否是多签地址
        string qrinfo;
        if (isMultiAddres)
        {
            if (NotBtcMulsigBuilder)
            {
                //Script mulSig = PayToMultiSigTemplate.Instance.GenerateScriptPubKey(2, curWalletInfo.GetPukList().ToArray());
                NotBtcMulsigBuilder = false;
                surScanCount = curWalletInfo.MultiSig_M;
                //Debug.Log("curWalletInfo.GetPukList==");
                //foreach (var item in curWalletInfo.GetPukList())
                //{
                //    Debug.Log("=" + item);
                //}
                //Debug.Log(curWalletInfo.MultiSig_M + "coinsToSpend : " + coinsToSpend.Count + "toAddress : " + toAddress[0].ToString() + " toCount: " + toCount[0]);
                BitcoinAddress temp = BitcoinAddress.Create(walletInfoPanel.currentItem.coinInfo.address, NBitcoin.Network.Main);
                if (temp==null)
                {
                    PopupLine.Instance.Show("网络异常");
                    return;
                }
                BtcMulsigBuilder = new BTCMulSig.MulSigTransactionBuilder(curWalletInfo.GetPukList(),
                curWalletInfo.MultiSig_M, coinsToSpend.ToArray(), toAddress[0].ToString(), new Money(toCount[0],MoneyUnit.BTC), feeM, temp);
                needsign = BtcMulsigBuilder.GetSignatureHash();
            }
            curWalletPuk = curWalletInfo.GetPuk(curWalletAddress);
            //for (int i = 0; i < needsign.Count; i++)
            //{
            //    Debug.Log("old:"+needsign[i]);
            //}
            //needsign.Clear();
            //needsign.Add("76a914ffab1e7a6972adaa1b637ce86a50ab8886a4a64088ac");
            //for (int i = 0; i < needsign.Count; i++)
            //{
            //    Debug.Log("cur:" + needsign[i]);
            //}
            qrinfo = QRPayTools.MulitCreateNoSignPayQRInfo(needsign);
        }
        else
        {
            qrinfo = QRPayTools.CreateNoSignPayQRInfo(feeM, toCount, toAddress, walletInfoPanel.currentItem.coinInfo.address, coinsToSpend);

        }

        sendCansing.SetSendInfo(decimal.Parse(feeM.ToString()), toCount[0].ToString(), toCount[0], walletInfoPanel.currentItem.coinInfo.address, toAddress[0].ToString(), "BTC", "BTC", true, strCommentText, "");
        if (NewWalletManager._Intance.IsNeedColdWallet)
        {
            //Debug.Log(toCount.Count + " : " + toAddress.Count);
            sendCansing.ShowQR(qrinfo);
        }
        else
        {
            PanelManager._Instance._pingCodeInputBox.OpenMe(qrinfo, PingCodeInputBox.SingType.BTC, walletInfoPanel.currentItem.coinInfo.address, delegate () 
            {
                sendCansing.sendPanel.ActiveNativeEditBox(true);
            });
        }
      
    }
    #region USDT发送
    string usdtAddressFrom;
    string usdtAddressTo;
    string usdtSpendMoney;
    List<string> signatureAll = new List<string>();
    public  List<HistoryManager.InputVo> inputVos = new List<HistoryManager.InputVo>();
    public void SendUSDT()
    {
        HttpManager._Intance.loadingPanel.SetActive(true);
        BitcoinAddress tempD = JudgeAddress(currentToAddress);
        if (tempD == null)
        {
            ShowNotice(12);
            return;
        }
        string v = GetInputTextValue(sendNumber, inputSendCount);
        toCount.Add(decimal.Parse(v));
        HistoryManager.Instance.PostUSDTInput(walletInfoPanel.currentItem.coinInfo.address, tempD.ToString(), (toCount[0] * 100000000).ToString("f0"), inputVos, SendUSDTOK, (fee * 100000000).ToString("f0"));
        
    }
    public void SendUSDTOK(List<HistoryManager.InputVo> inputs,bool isOk,string message)
    {
        if (isOk)
        {
            //Debug.Log("-----------SendPost1---------");
            HttpManager._Intance.loadingPanel.SetActive(false);
            inputVos = inputs;
            needsign.Clear();
            foreach (var inputVo in inputVos)
            {
                needsign.Add(inputVo.signature);
                //Debug.Log("inputVo.signature" + inputVo.signature);
                inputVo.pubKey = PlayerPrefs.GetString("myUsdtPubkey" + walletInfoPanel.currentItem.coinInfo.address);
                //Debug.Log("pbk:" + inputVo.pubKey + "walletInfoPanel.currentItem.coinInfo.address" + walletInfoPanel.currentItem.coinInfo.address);
            }
            string qrinfo = QRPayTools.MulitCreateNoSignPayQRInfo(needsign);
            //参数设置
            feeM = new Money(fee, MoneyUnit.BTC);
            BitcoinAddress tempD = JudgeAddress(currentToAddress);
            if (tempD == null)
            {
                ShowNotice(12);
                return;
            }
            sendCansing.SetSendInfo(decimal.Parse(feeM.ToString()), toCount[0].ToString(), toCount[0], walletInfoPanel.currentItem.coinInfo.address, tempD.ToString(), "BTC", "USDT", true, strCommentText, "");
            if (NewWalletManager._Intance.IsNeedColdWallet)
            {
                //Debug.Log(toCount.Count + " : " + toAddress.Count);
                sendCansing.ShowQR(qrinfo);
            }
            else
            {
                
                PanelManager._Instance._pingCodeInputBox.OpenMe(qrinfo, PingCodeInputBox.SingType.BTC, walletInfoPanel.currentItem.coinInfo.address, delegate ()
                {
                    sendCansing.sendPanel.ActiveNativeEditBox(true);
                });
            }
        }
        else
        {
            HttpManager._Intance.loadingPanel.SetActive(false);
            PopupLine.Instance.Show(message);
        }
    }
    public void GetUSDTInput(string usdtAddress,string spendMoney)
    {

        Debug.Log("-----------GetUSDTFee---------");
        BitcoinAddress tempD = JudgeAddress(currentToAddress);
        if (tempD == null)
        {
            ShowNotice(12);
            return;
        }
        //HistoryManager.Instance.GetUSDTInput(usdtAddress,tempD.ToString(),spendMoney,"3800", GetUsdtInput, PlayerPrefs.GetString("myUsdtPubkey" + walletInfoPanel.currentItem.coinInfo.address));

        HistoryManager.Instance.GetUSDTFee(usdtAddress, tempD.ToString(),(decimal.Parse(spendMoney)*100000000).ToString("f0"), GetUsdtInput);
    }
    public decimal GetUsdtFee()
    {
        Debug.Log("-----------GetUsdtFee---------");
        GetUSDTInput(walletInfoPanel.currentItem.coinInfo.address, currentToCount);
        decimal usdtFee = (decimal)(0.000018 + (inputVos.Count * 0.00002));
        return usdtFee;
    }
    public void GetUsdtInput(List<HistoryManager.InputVo> inputs,bool isOk,string message)
    {
        if (isOk)
        {
            inputVos.Clear();
            Debug.Log("-----------GetUsdtSig---------");
            inputVos = inputs;
            foreach (var inputVo in inputVos)
            {
                inputVo.pubKey = PlayerPrefs.GetString("myUsdtPubkey" + walletInfoPanel.currentItem.coinInfo.address);
                Debug.Log("pbk:" + inputVo.pubKey + "walletInfoPanel.currentItem.coinInfo.address" + walletInfoPanel.currentItem.coinInfo.address);
            }
            Debug.Log("inputVos.Count:" + inputVos.Count);
            btcTransLeng = inputVos.Count * 200 + 180;
            bteSendNextBtn.SetActive(true);
            feeSlider.gameObject.SetActive(true);
            OnSliderValueChange();
            if (SendAllBTCCallBack != null)
            {
                SendAllBTCCallBack();
                SendAllBTCCallBack = null;
            }
        }
        else
        {
            PopupLine.Instance.Show(message);
        }
        HttpManager._Intance.loadingPanel.SetActive(false);
    }

    public void PostInputSig(Action<string,bool> sucCallBack)
    {
        BitcoinAddress tempD = JudgeAddress(currentToAddress);
        if (tempD == null)
        {
            ShowNotice(12);
            return;
        }
        Debug.Log("-----------PostInputSig---------");
        HistoryManager.Instance.PostUSDTTx(walletInfoPanel.currentItem.coinInfo.address, tempD.ToString(), (toCount[0]* 100000000).ToString("f0"), inputVos, sucCallBack,(fee*100000000).ToString("f0"));
    }
   
    #endregion

    //获得交易结果
    void Send3(QBitNinja.Client.Models.BroadcastResponse broadcastResponse, NBitcoin.Network network)
    {
        if (!broadcastResponse.Success)
        {
            ShowNotice(16);
            UnityEngine.Debug.Log(string.Format("ErrorCode: {0}", broadcastResponse.Error.ErrorCode));
            UnityEngine.Debug.Log("Error message: " + broadcastResponse.Error.Reason);

        }
        else
        {
            ShowNotice(18);
            UnityEngine.Debug.Log("Success! You can check out the hash of the transaciton in any block explorer:");
            //UnityEngine.Debug.Log(transaction.GetHash());
        }
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
        Debug.Log("SendNumber:"+ v);
        return v;
    }

    private Action SendAllBTCCallBack;

    private void ShowAllSendBTC()
    {
        //feeSlider.value = 0;
        //OnSliderValueChange();
        
        decimal tempHasCount = 0;
        if (!string.IsNullOrEmpty(currentAmount.text))
        {
            string c = currentAmount.text.Split(' ')[0];
            tempHasCount = decimal.Parse(c);
        }
        decimal tc;
        if (NewWalletManager._Intance.currentCoinType == NewWalletManager.CoinType.USDT)
        {
            tc = tempHasCount;
        }
        else
        {
            tc = tempHasCount - fee;
        }
        if (tc > 0)
        {
//            inputSendCount.text = tc.ToString();
            sendNumber.text = tc.ToString();
        }
        else
        {
//            inputSendCount.text = "0";
            sendNumber.text = "0";
            ShowNotice(13);
        }
    }
    private void GetTransaction()
    {
        if (NewWalletManager._Intance.currentCoinType == NewWalletManager.CoinType.USDT)
        {
            BitcoinAddress tempD = JudgeAddress(currentToAddress);
            if (tempD == null)
            {
                ShowNotice(12);
                return;
            }
            GetUsdtFee();
        }
        else
        {
            curWalletInfo = MultiJSData.instance.GetMultiWalletInfo(walletInfoPanel.currentItem.coinInfo.address);
            Money availableAmount = Money.Zero;
            Money unconfirmedAvailableAmount = Money.Zero;
            foreach (var elem in unspentCoins)
            {
                if (elem.Value)
                {
                    availableAmount += elem.Key.Amount;
                }
            }

            decimal seedCount = 0;

            // List<BitcoinAddress> toAddress = new List<BitcoinAddress>();
            BitcoinAddress tempD = JudgeAddress(currentToAddress);
            if (tempD == null)
            {
                ShowNotice(12);
                return;
            }
            List<decimal> toCount = new List<decimal>();
            toCount.Add(decimal.Parse(currentToCount));

            seedCount = decimal.Parse(currentToCount);

            Money amountToSend = new Money(seedCount, MoneyUnit.BTC);

            var totalOutAmount = amountToSend;

            // 8. Select coins
            var coinsToSpend = new HashSet<Coin>();
            var unspentConfirmedCoins = new List<Coin>();
            var unspentUnconfirmedCoins = new List<Coin>();
            foreach (var elem in unspentCoins)
                if (elem.Value) unspentConfirmedCoins.Add(elem.Key);
                else unspentUnconfirmedCoins.Add(elem.Key);

            bool haveEnough = SelectCoins(ref coinsToSpend, totalOutAmount, unspentConfirmedCoins);
            if (!haveEnough)
                haveEnough = SelectCoins(ref coinsToSpend, totalOutAmount, unspentUnconfirmedCoins);
            int tranSize;
            if (isMultiAddres)
            {
                
                tranSize = coinsToSpend.Count * 100 * curWalletInfo.MultiSig_N + 150;
                //Debug.Log("tranSize = coinsToSpend.Count * 100 * curWalletInfo.MultiSig_N + 150;"+ tranSize+"="+ coinsToSpend.Count+"*"+100+"*"+ curWalletInfo.MultiSig_N + "+"+ 150);
            }
            else
            {
                tranSize = coinsToSpend.Count * 200;
            }
            btcTransLeng = tranSize;
            bteSendNextBtn.SetActive(true);
            feeSlider.gameObject.SetActive(true);
            OnSliderValueChange();
            HttpManager._Intance.loadingPanel.SetActive(false);


            if (SendAllBTCCallBack != null)
            {
                SendAllBTCCallBack();
                SendAllBTCCallBack = null;
            }
        }
    }

    //获得交易结果
    //void Send3(QBitNinja.Client.Models.BroadcastResponse broadcastResponse, NBitcoin.Network network)
    //{
    //    if (!broadcastResponse.Success)
    //    {
    //        ShowNotice(16);
    //        UnityEngine.Debug.Log(string.Format("ErrorCode: {0}", broadcastResponse.Error.ErrorCode));
    //        UnityEngine.Debug.Log("Error message: " + broadcastResponse.Error.Reason);
    //    }
    //    else
    //    {
    //        ShowNotice(18);
    //        UnityEngine.Debug.Log("Success! You can check out the hash of the transaciton in any block explorer:");
    //        //UnityEngine.Debug.Log(transaction.GetHash());
    //    }
    //}
    bool SelectCoins(ref HashSet<Coin> coinsToSpend, Money totalOutAmount, List<Coin> unspentCoins)
    {
        var haveEnough = false;
        foreach (var coin in unspentCoins.OrderByDescending(x => x.Amount))
        {
            coinsToSpend.Add(coin);
            // if doesn't reach amount, continue adding next coin
            if (coinsToSpend.Sum(x => x.Amount) < totalOutAmount) continue;
            else
            {
                haveEnough = true;
                break;
            }
        }
     //   Debug.Log(totalOutAmount.ToString() + " : " + temp.ToString() + " 数量 = " + coinsToSpend.Count);
        return haveEnough;
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
    public void RefreshWallerList()
    {
        for (int j = 0; j < oldwalletsAddress.Count; j++)
        {
            for (int i = 0; i < curWalletInfo.btcAddress.Count; i++)
            {
                if (oldwalletsAddress[j] == curWalletInfo.btcAddress[i])
                {
                    wallets[i].SetActive(false);
                }
            }

        }
    }
    public void OnChangeSeedToValue(string v)
    {
        currentToCount = GetInputTextValue(sendNumber, inputSendCount);
    }

}
