using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using NBitcoin;
using System;
using QBitNinja4Unity;
using UnityEngine.UI;
using Nethereum.Util;

public class NewWalletManager : MonoBehaviour
{
    /// <summary>
    /// 是否需要冷钱包
    /// </summary>
    public bool IsNeedColdWallet = false;

    public string versionNumber;
    public static int statusBarOffset = 0;
    public const string COIN_UNIT_TYPE = "CoinUnitType";

    //1 BTC 2 ETH 3 EOS 禁止修改， 次定义和冷钱包对应的
    public enum CoinType
    {
        BTC = 1,
        ETH = 2,
        EOS = 3,
        USDT = 4,
    }
    public CoinType currentCoinType = CoinType.BTC;

    public static NewWalletManager _Intance;

    

    public string userName;
    public string passWorld;
    public string userId;

    /// <summary>
    /// 商品列表
    /// </summary>
    public List<GoodsInfo> goodsInfoList = new List<GoodsInfo>();


    public Dictionary<string, string> usdtAddresListInfo = new Dictionary<string, string>();
    /// <summary>
    /// 比特币地址列表 key 为地址, value 是钱包名称
    /// </summary>
    public Dictionary<string,string> bitcoinAddresListInfo = new Dictionary<string, string>();

    /// <summary>
    /// 以太坊地址
    /// </summary>
    public Dictionary<string, string> ethcoinAddresListInfo = new Dictionary<string, string>();
    /// <summary>
    /// 以太坊地址对应显示的代币类型
    /// </summary>
    public Dictionary<string, List<string>> ethTokenListDic = new Dictionary<string, List<string>>();


    /// <summary>
    /// EOS地址 key 为owner地址
    /// </summary>
    public Dictionary<string, EOSWalletInfo> eoscoinAddressListInfo = new Dictionary<string, EOSWalletInfo>();

    public bool btcIsTestNetWork;

    public bool isTestSend = false; //6475469

    public bool isDelPlayerPrefs = false;

    public NBitcoin.Network btcNetwork;

    void Awake()
    {
        if (isDelPlayerPrefs)
        {
            PlayerPrefs.DeleteAll();
        }
        _Intance = this;
        btcNetwork = btcIsTestNetWork ? NBitcoin.Network.TestNet : NBitcoin.Network.Main;
    }

    public void SavaBtcPubkey(string btcAddress, string seed)
    {
        if (!PlayerPrefs.HasKey("myBtcPubkey" + btcAddress))
        {
            WalletTools tools = new WalletTools();
            string btcPubkey = tools.GetPubKeyByPath(seed, WalletTools.btcPath);
            if (!string.IsNullOrEmpty(btcPubkey))
            {
                PlayerPrefs.SetString("myBtcPubkey" + btcAddress, btcPubkey);
            }
        }
    }
    public void SavaUsdtPubkey(string usdtAddress, string seed)
    {
        if (!PlayerPrefs.HasKey("myUsdtPubkey" + usdtAddress))
        {
            WalletTools tools = new WalletTools();
            string usdtPubkey = tools.GetPubKeyByPath(seed, WalletTools.btcPath);
            if (!string.IsNullOrEmpty(usdtPubkey))
            {
                Debug.Log("myUsdtPubkey" + usdtAddress+" "+ usdtPubkey);
                PlayerPrefs.SetString("myUsdtPubkey" + usdtAddress, usdtPubkey);
            }
        }
    }
    public string GetUsdtPukey(string usdtAddress)
    {
        Debug.Log("myUsdtPubkey" + usdtAddress + " " + PlayerPrefs.GetString("myUsdtPubkey" + usdtAddress));
        return PlayerPrefs.GetString("myUsdtPubkey" + usdtAddress);
    }
    public string GetBtcPukey(string btcAddress)
    {
        return PlayerPrefs.GetString("myBtcPubkey" + btcAddress);
    }

    public void CleanData()
    {
        userName = "";
        passWorld = "";
        usdtAddresListInfo.Clear();
        bitcoinAddresListInfo.Clear();
        ethcoinAddresListInfo.Clear();
        ethTokenListDic.Clear();
        eoscoinAddressListInfo.Clear();
    }

    private void SetHttpConstValue(Hashtable hashtable)
    {
        HttpManager._Intance.ethMinFee = decimal.Parse(hashtable["ethMinFee"].ToString());
        HttpManager._Intance.ethMaxFee = decimal.Parse(hashtable["ethMaxFee"].ToString());
        HttpManager._Intance.ethTokenMinFee = decimal.Parse(hashtable["ethTokeMinFee"].ToString());
        HttpManager._Intance.ethTokenMaxFee = decimal.Parse(hashtable["ethTokeMaxFee"].ToString());
        HttpManager._Intance.current_usdt = decimal.Parse(hashtable["usdtormb"].ToString());
    }
    private decimal ChangeDataToD(string strData)
    {
        decimal dData = 0.0M;
        if (strData.Contains("E"))
        {
            dData = Convert.ToDecimal(decimal.Parse(strData.ToString(), System.Globalization.NumberStyles.Float));
        }
        return dData;
    }
    private void GetOrderList()
    {
        StartCoroutine(HttpManager._Intance.GetNodeJsRequest("getgoodslist", new List<KeyValuePair<string, string>>(), delegate (Hashtable data)
          {
              if (data != null)
              {
                  ArrayList array = data["data"] as ArrayList;
                  for (int i = 0; i < array.Count; i++)
                  {
                      Hashtable h = array[i] as Hashtable;
                      GoodsInfo payOrder = new GoodsInfo();
                      payOrder.id = int.Parse(h["id"].ToString());
                      string title = h["title"].ToString();
                      if (title.Equals("EOS_CVT"))
                      {
                          payOrder.goodsType = GoodsInfo.GoodsType.EOS_CVT;
                      }
                      string paytype = h["paytype"].ToString();
                      if (paytype.Equals("token"))
                      {
                          payOrder.payType = GoodsInfo.PayType.ETHToken;
                      }
                      payOrder.contactAddress = h["contactAddress"].ToString();

                      payOrder.toAddress = h["toAddress"].ToString();

                      payOrder.price = ChangeDataToD(h["price"].ToString());

                      payOrder.tokenDecimal = int.Parse(h["decimal"].ToString());
                      decimal cost = UnitConversion.Convert.FromWei(System.Numerics.BigInteger.Parse(payOrder.price.ToString()), payOrder.tokenDecimal);
                      payOrder.price = cost;
                      payOrder.symbol = h["coinShortName"].ToString();
                      payOrder.fullName = h["coinFullName"].ToString();

                      goodsInfoList.Add(payOrder);

                  }
              }
          }));
    }

    public GoodsInfo GetPayOrder(GoodsInfo.GoodsType orderType)
    {
        for (int i = 0; i < goodsInfoList.Count; i++)
        {
            if (goodsInfoList[i].goodsType == orderType)
                return goodsInfoList[i];
        }

        return null;
    }

    /// <summary>
    /// 登录成功 初始化数据
    /// </summary>
    /// <param name="_userName"></param>
    /// <param name="hashtable"></param>
    public void Init(string _userName,string _passWorld, Hashtable hashtable = null)
    {
        GetOrderList();
        this.userName = _userName;
        this.passWorld = _passWorld;

        if (hashtable != null)
        {
            
            QBitNinjaClient.MAIN = hashtable["btcmainnetwork"].ToString();

           
            userId = hashtable["userid"].ToString();

            if (hashtable["regId"] == null)
            {
                StartCoroutine(JPushManage.instance.GetRegIDIE());
            }
            else
            {
                HttpManager._Intance.regId = hashtable["regId"].ToString();
            }

            SetHttpConstValue(hashtable);

            EthTokenManager._Intance.InitTokenList((ArrayList)hashtable["tokenList"]);

            ArrayList list = (ArrayList)hashtable["data"];
            for (int i = 0; i < list.Count; i++)
            {

                Hashtable temp = list[i] as Hashtable;
                
                if (temp["type"].ToString() == "1")
                {
                    bitcoinAddresListInfo.Add(temp["address"].ToString(), temp["walletName"].ToString());
                }
                else if (temp["type"].ToString() == "2")
                {
                    ethcoinAddresListInfo.Add(temp["address"].ToString(), temp["walletName"].ToString());

                }
                else if (temp["type"].ToString() == "3")
                {
                    EOSWalletInfo ew = new EOSWalletInfo();
                    ew.ownerAddress = temp["address"].ToString();
                    ew.walletName = temp["walletName"].ToString();
                    ew.adminAddress = temp["eosAdminAddress"].ToString();
                    if (!string.IsNullOrEmpty(temp["eosuserName"] + ""))
                    {
                        ew.account = temp["eosuserName"].ToString();
                    }
                    eoscoinAddressListInfo.Add(temp["eosAdminAddress"].ToString(), ew);
                }
                else if (temp["type"].ToString() == "4")
                {
                    if (!MultiJSData.instance.IsMultiSigAddress(temp["address"].ToString()))
                    {
                        usdtAddresListInfo.Add(temp["address"].ToString(), temp["walletName"].ToString());
                    }
                  
                }
            }
        }
    }

    private QRCodeManager m_qRCodeManager;
    public QRCodeManager QRCodeManager
    {
        get
        {
            if (m_qRCodeManager == null)
                m_qRCodeManager = new QRCodeManager();

            return m_qRCodeManager;
        }
    }

    public void ShowProperty(Text text, decimal v)
    {

        if (PanelManager._Instance._mainPanel.currentUnitType == MainPanel.UnitType.BTC)
        {
            if (HttpManager._Intance.btc_RMB == 0)
            {
                HttpManager._Intance.btc_RMB = 45000;
            }
            decimal temp = v / HttpManager._Intance.btc_RMB;
            if (temp == 0)
            {
                text.text = "≈B0";
            }
            else
            {
                text.text = "≈B" + temp.ToString("f6");
            }

        }
        else if (PanelManager._Instance._mainPanel.currentUnitType == MainPanel.UnitType.RMB)
        {
            if (v == 0)
            {
                text.text = "≈￥0";
            }
            else
            {
                text.text = "≈￥" + v.ToString("f2");
            }
        }
        else if (PanelManager._Instance._mainPanel.currentUnitType == MainPanel.UnitType.USD)
        {
            if (HttpManager._Intance.current_usdt <= 0)
            {
                HttpManager._Intance.current_usdt = 6.4M;
            }
            decimal temp = v / HttpManager._Intance.current_usdt;
            if (temp == 0)
            {
                text.text = "≈$0";
            }
            else
            {
                text.text = "≈$" + temp.ToString("f2");
            }
        }
    }


    public string ShowCount(Text target, decimal v, int digit = 9)
    {
        if (v <= 0)
        {
            if (target != null)
                target.text = "0";

            return "0";
        }

        string s = v.ToString();
        int c = s.Length - s.IndexOf('.') - 1;
        if (c > digit)
        {
            
            string f = "f" + digit;
            
            string ts = v.ToString(f);
            s = ts.Substring(0, ts.Length - 1);
            if(target != null)
                target.text = s;


        }
        else
        {
            if (target != null)
                target.text = s;
        }

        return s;
    }

    public void DOTweenCome(Transform target, float startPos, float endPos, TweenCallback endCallBack = null)
    {


        target.localPosition = new Vector3(startPos, 0, 0);
    
        target.localPosition = new Vector3(endPos, 0,0);
        if (endCallBack != null)
        {
            endCallBack();
        }
    }

    public void DoTweenBack(Transform target, float endPos, TweenCallback endCallBack = null)
    {
      
        target.localPosition = new Vector3(endPos, 0, 0);
        if (endCallBack != null)
        {
            endCallBack();
        }
    }

    public bool IsSameAsAddress(string address, CoinType cur)
    {

        if (cur == CoinType.BTC)
        {
            return bitcoinAddresListInfo.ContainsKey(address);
        }
        else if (cur == CoinType.ETH)
        {
            return ethcoinAddresListInfo.ContainsKey(address);

        }
        else if (cur == CoinType.EOS)
        {
            return eoscoinAddressListInfo.ContainsKey(address);
        }
        else if (cur == CoinType.USDT)
        {
            return usdtAddresListInfo.ContainsKey(address);
        }
        return false;
    }

    /// <summary>
    /// 保存地址
    /// </summary>
    public void SaveAddress(string jsonstr,bool isMulit=false)
    {
        Hashtable h = Json.jsonDecode(jsonstr) as Hashtable;
        string btcAddress = h["BTC"].ToString();
        string ethAddress = h["ETH"].ToString();
        string eosAddress_owner = h["EOSowner"].ToString();
        string eosAddress_admin = h["EOSadmin"].ToString();
        string walletName = h["walletName"].ToString();

        if (!string.IsNullOrEmpty(btcAddress) && !bitcoinAddresListInfo.ContainsKey(btcAddress))
        {
            bitcoinAddresListInfo.Add(btcAddress, walletName);
        }
        if (!string.IsNullOrEmpty(btcAddress) && !usdtAddresListInfo.ContainsKey(btcAddress) && !isMulit)
        {
            usdtAddresListInfo.Add(btcAddress, walletName);
        }

        if (!string.IsNullOrEmpty(ethAddress) && !ethcoinAddresListInfo.ContainsKey(ethAddress))
        {
            ethcoinAddresListInfo.Add(ethAddress, walletName);
        }

        if (!string.IsNullOrEmpty(eosAddress_admin) && !eoscoinAddressListInfo.ContainsKey(eosAddress_admin))
        {
            EOSWalletInfo ew = new EOSWalletInfo();
            ew.ownerAddress = eosAddress_owner;
            ew.adminAddress = eosAddress_admin;
            ew.walletName = walletName;
            eoscoinAddressListInfo.Add(eosAddress_admin, ew);
        }


    }


    public BitcoinAddress JudgeBTCAddress(string btcToAddress)
    {
        try
        {
            BitcoinAddress temp = BitcoinAddress.Create(btcToAddress, btcNetwork);
            return temp;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public bool JudgeETHAddress(string address)
    {
        if (address.Length != 42)
        {
            return false;
        }

        string temp = address.Substring(0, 2);
        if (!temp.Equals("0x"))
        {
            return false;
        }

        return true;
    }

    public bool JudgeEosAddress(string address)
    {
        if (address.Length != 53)
        {
            return false;
        }

        string temp = address.Substring(0, 3);
        if (!temp.Equals("EOS"))
        {
            return false;
        }
        return true;
    }
}
