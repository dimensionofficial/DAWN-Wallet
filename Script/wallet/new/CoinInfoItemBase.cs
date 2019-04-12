using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CoinInfo
{
    public string walletname;
    public string address;
    public decimal money;
	public string ethmoney;
	public string tokenmoney;
    public decimal ethTokenMoney;
}

public class CoinInfoItemBase : MonoBehaviour
{
    public bool isGetBalance = false;

    public NewWalletManager.CoinType type = NewWalletManager.CoinType.BTC;
    public CoinInfo coinInfo;
    public Text iconName;
    public Text nameText;
    public Text countText;
    [System.NonSerialized]
    public string coinname;

    private System.DateTime startTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);

    public string[] tempConAddress;

    ///// <summary>
    ///// BTC/ETH交易记录
    ///// </summary>
    //public List<ETHHistoryRcord> historyRcordlist = new List<ETHHistoryRcord>();

    //public List<ETHHistoryRcord> sendRecordList = new List<ETHHistoryRcord>();
    //public List<ETHHistoryRcord> receiveRecordList = new List<ETHHistoryRcord>();
    //public List<ETHHistoryRcord> myselRecordList = new List<ETHHistoryRcord>();
//    public List<ETHHistoryRcord> 


    public virtual void Init(string[] v)
    {
        coinname = v[1];
        nameText.text = coinname;

        iconName.text = coinname.Substring(0, 1);
        coinInfo = new CoinInfo();
        coinInfo.walletname = coinname;
        coinInfo.address = v[0];
        RefreshBalance();
    }

    private void InitCoinInfo(string[] v)
    {

    }

    void OnEnable()
    {
        StartCoroutine(Refresh());
    }

    IEnumerator Refresh()
    {
        while (true)
        {
            yield return new WaitForSeconds(90);
            RefreshBalance();
   //       GetHistory(false);
        }
    }

    public virtual void RefreshBalance()
    {

    }

    public virtual void GetHistory()
    {

    }


    //protected void GetHistroyRecordInfoNet(List<ETHHistoryRcord> rcordlist)
    //{
    //    if (rcordlist.Count == 0)
    //    {
    //        return;
    //    }
    //    GetHistroyRecordInfo(rcordlist);
    //}

    //protected void GetHistroyRecordInfo(List<ETHHistoryRcord> rcordlist)
    //{
       
    //    List<ETHHistoryRcord> tempRecord = AddLocalData(rcordlist, false);

    //    historyRcordlist.Clear();

    //    if (tempRecord.Count > 0)
    //    {
    //        for (int i = 0; i < tempRecord.Count; i++)
    //        {
                
    //            if (!ContainsHash(historyRcordlist, tempRecord[i].hash))
    //            {
    //                if (type == NewWalletManager.CoinType.ETH)
    //                {
    //                     historyRcordlist.Add(tempRecord[i]);
    //                }
    //                else
    //                {
    //                    historyRcordlist.Add(tempRecord[i]);
    //                }
    //            }
    //        }

    //        //if (type == NewWalletManager.CoinType.BTC)
    //        //{
    //        //    historyRcordlist.Sort(
    //        //    delegate (ETHHistoryRcord p1, ETHHistoryRcord p2)
    //        //    {
    //        //        return p1.timeStamp.CompareTo(p2.timeStamp);//升序
    //        //    });
    //        //}
    //    }


    //    //if (PanelManager._Instance._WalletInfoPanel.recordPage.gameObject.activeInHierarchy 
    //    //    && PanelManager._Instance._WalletInfoPanel.recordPage.currentAddress.Equals(coinInfo.address.ToLower()))
    //    //{
    //    //    if (PanelManager._Instance._WalletInfoPanel.recordPage.type == HistroyRecord.CurrentHistroyType.BTC)
    //    //    {
    //    //        PanelManager._Instance._WalletInfoPanel.recordPage.RefreshItem(historyRcordlist, "BTC");
    //    //    }
    //    //    else if (PanelManager._Instance._WalletInfoPanel.recordPage.type == HistroyRecord.CurrentHistroyType.ETH)
    //    //    {
    //    //        PanelManager._Instance._WalletInfoPanel.recordPage.RefreshItem(historyRcordlist, "ETH");
    //    //    }
    //    //}
        
    //}

 
    protected bool ContainsHash(List<ETHHistoryRcord> data, string hash)
    {
        foreach (var v in data)
        {
            if (v.hash == hash)
            {
                return true;
            }
        }
        return false;
    }

    protected bool ContainsKey(Dictionary<string, TokenService> dic, string address)
    {
        foreach (var v in dic.Keys)
        {
            if (v.ToLower() == address.ToLower())
            {
                return true;
            }
        }
        return false;
    }

    protected bool ContainsKey(Dictionary<string, List<ETHHistoryRcord>> dic, string address)
    {
        foreach (var v in dic.Keys)
        {
            if (v.ToLower() == address.ToLower())
            {
                return true;
            }
        }
        return false;
    }

    protected bool IsInList(List<ETHHistoryRcord> tempList, ETHHistoryRcord ethRcord)
    {
        for (int i = 0; i < tempList.Count; i++)
        {
            if (tempList[i].hash == ethRcord.hash)
                return true;
        }
        return false;
    }


    public List<ETHHistoryRcord> AddLocalData(List<ETHHistoryRcord> _rcordlist, bool isETHToken, string tokenSymbol)
    {
        string[] temp;
        if (type == NewWalletManager.CoinType.USDT)
        {
            temp = PlayerPrefsX.GetStringArray(coinInfo.address + "USDT" + "LocalHistroy");
        }
        temp = PlayerPrefsX.GetStringArray(coinInfo.address + "LocalHistroy");
        if (temp.Length > 0)
        {
            List<string> tempList = new List<string>(temp);

            List<ETHHistoryRcord> tempNeedAdd = new List<ETHHistoryRcord>();

            for (int j = 0; j < temp.Length; j++)
            {
               
                Hashtable hs = Json.jsonDecode(temp[j]) as Hashtable;
                string tempSymbol = hs["tokenSymbol"].ToString();

                if (isETHToken && ( !tempSymbol.Equals(tokenSymbol) || ((string.IsNullOrEmpty(tempSymbol) || tempSymbol.Equals("ETH")))) )
                {
                    continue;
                }

                if (isETHToken == false && ((!string.IsNullOrEmpty(tempSymbol) && !tempSymbol.Equals("ETH"))))
                {
                    continue;
                }


                if (!ContainsHash(_rcordlist, hs["hash"].ToString()))
                {
                    ETHHistoryRcord tempHistory = new ETHHistoryRcord();
                    long time = long.Parse(hs["timeStamp"].ToString());

                    TimeSpan cha = (DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)));
                    long t = (long)cha.TotalSeconds;

                    long tempTime = t - time;

                    tempHistory.hash = hs["hash"].ToString();
                    tempHistory.tokenSymbol = hs["tokenSymbol"].ToString();
                    DateTime dt = startTime.AddSeconds(time).ToLocalTime();
                    tempHistory.timeStamp = dt.ToString("yyyy-MM-dd HH:mm:ss");
                    tempHistory.timeTick = time;
                    tempHistory.from = hs["from"].ToString();
                    tempHistory.to = hs["to"].ToString();
                    tempHistory.value = hs["value"].ToString();
                    tempHistory.gas = hs["gas"].ToString();
                    tempHistory.contractAddress = hs["contractAddress"].ToString();
                    tempHistory.confixmations = hs["confixmations"].ToString();
                    tempHistory.fullName = hs["fullName"].ToString();
                    if (hs.ContainsKey("input"))
                    {
                        tempHistory.input = hs["input"].ToString();
                    }


                    if (tempTime >= 12 * 60 * 60)
                    {
                        //tempList.Remove(temp[j]);
                        tempHistory.isOverTime = true;
                        if (!tempSymbol.Equals("BTC"))
                        {
                            PlayerPrefs.DeleteKey(coinInfo.address.ToLower() + tempHistory.hash.ToLower() + "Over");
                        }
                    }

                    if (!tempSymbol.Equals("BTC"))
                    {
                        if (PlayerPrefs.HasKey(coinInfo.address.ToLower() + hs["hash"].ToString().ToLower() + "Over"))
                        {
                            tempHistory.confixmations = "1";
                            PlayerPrefs.DeleteKey(coinInfo.address.ToLower() + hs["hash"].ToString().ToLower() + "Packing");
                            PlayerPrefs.DeleteKey(coinInfo.address.ToLower() + hs["hash"].ToString().ToLower() + "BlockNumber");
                        }
                    }
                    tempNeedAdd.Add(tempHistory);
                }
                else
                {
                    PlayerPrefs.DeleteKey(coinInfo.address.ToLower() + hs["hash"].ToString().ToLower() + "Over");
                    tempList.Remove(temp[j]);
                }
            }

            if (tempList.Count > 0)
                PlayerPrefsX.SetStringArray(coinInfo.address + "LocalHistroy", tempList.ToArray());
            else
                PlayerPrefs.DeleteKey(coinInfo.address + "LocalHistroy");
            if (tempNeedAdd.Count > 0)
            {
                if (tempNeedAdd.Count > 1)
                {
                    tempNeedAdd.Sort(
                    delegate (ETHHistoryRcord p1, ETHHistoryRcord p2)
                    {
                        return p1.timeTick.CompareTo(p2.timeTick);//升序
                    });

                    for (int i = 0; i < tempNeedAdd.Count; i++)
                    {
                        _rcordlist.Insert(0, tempNeedAdd[i]);
                    }
                }
                else
                {
                    _rcordlist.Insert(0, tempNeedAdd[0]);
                }
            }
        }

        return _rcordlist;
    }

    //private void GetTokenHistroyRecord(List<ETHHistoryRcord> rcordlist)
    //{

    //    rcordlist = AddLocalData(rcordlist, true);
    //    etherc20Dic.Clear();
    //    if (rcordlist.Count > 0)
    //    {
    //        for (int i = 0; i < rcordlist.Count; i++)
    //        {
    //            ETHHistoryRcord ethRcord = rcordlist[i];
    //            string contractAddress = ethRcord.contractAddress;
    //            if (ContainsKey(etherc20Dic, contractAddress))
    //            {
    //                if (!IsInList(etherc20Dic[contractAddress], ethRcord))
    //                    etherc20Dic[contractAddress].Add(ethRcord);
    //            }
    //            else
    //            {
    //                List<ETHHistoryRcord> tempList = new List<ETHHistoryRcord>();
    //                tempList.Add(ethRcord);
    //                if (!ContainsKey(EthTokenManager._Intance.tokenServiceDic, contractAddress))
    //                {
    //                    if (string.IsNullOrEmpty(ethRcord.tokenSymbol))
    //                    {
    //                        HttpManager._Intance.StartCoroutine(HttpManager._Intance.GetTokenSymbol(contractAddress, (fullname, symbol, decimals) =>
    //                        {
    //                            ethRcord.tokenSymbol = symbol;
    //                            ethRcord.fullName = fullname;
    //                            ethRcord.tokendecimals = decimals;
    //                            AddTokenService(ethRcord);
    //                        }));
    //                    }
    //                    else
    //                    {
    //                        AddTokenService(ethRcord);
    //                    }
    //                }
    //                if (!ContainsKey(etherc20Dic, contractAddress))
    //                {
    //                    etherc20Dic.Add(contractAddress, tempList);
    //                }
    //            }
    //        }
    //    }
    //    //if (coinInfo.address.ToLower().Equals(PanelManager._Instance._WalletInfoPanel.recordPage.currentAddress))
    //    //{
    //    //    if(PanelManager._Instance._WalletInfoPanel.recordPage.type == HistroyRecord.CurrentHistroyType.TOKen)
    //    //    {
    //    //        if(PanelManager._Instance._WalletInfoPanel.currentTokenItem != null)
    //    //        {
    //    //            PanelManager._Instance._WalletInfoPanel.recordPage.RefreshTokenItem(PanelManager._Instance._WalletInfoPanel.currentTokenItem);
    //    //        }
    //    //    }
    //    //}

    //}

    //void AddTokenService(ETHHistoryRcord ethRcord)
    //{
    //    HttpManager._Intance.UpLoadContractAddress(ethRcord.contractAddress, ethRcord.tokenSymbol, ethRcord.fullName, delegate (Hashtable h)
    //    {
    //        int isHide = PlayerPrefs.GetInt(ethRcord.contractAddress + "HideToken");
    //        if (isHide > 0)
    //        {
    //            // 被隐藏
    //        }
    //        else
    //        {
    //            if (EthTokenManager._Intance.tokenServiceDic.ContainsKey(ethRcord.contractAddress))
    //            {
    //                return;
    //            }
    //            string iconPath = "";
    //            if (int.Parse(h["error"].ToString()) > 0)
    //            {
    //                iconPath = h["tokenIcon"].ToString();
    //                //                  Debug.Log(ethRcord.tokenSymbol + "  iconPath = " + iconPath);
    //            }
    //            TokenService tokenServic = EthTokenManager._Intance.GetTokenService(ethRcord.contractAddress, ethRcord.tokenSymbol);
    //            tokenServic.TokenInfo.isShow = 0;
    //            SetTokenAddress(ethRcord.contractAddress, ethRcord.tokenSymbol, iconPath, ethRcord.fullName, true);
    //            EthTokenManager._Intance.tokenServiceDic.Add(ethRcord.contractAddress, tokenServic);
    //        }
    //    });
    //}
    //private void GetTokenHistroyRecordInfoNet(List<ETHHistoryRcord> rcordlist)
    //{
    //    if (rcordlist.Count == 0)
    //    {
    //        return;
    //    }
    //    GetTokenHistroyRecord(rcordlist);
    //}

    ///// <summary>
    ///// 本地保存的代币开启
    ///// </summary>
    ///// <param name="代币合约地址"></param>
    ///// <param name="是否添加"></param>
    //public void SetTokenAddress(string conAddress, string symbol, string iconPath, string fullName, bool isAdd)
    //{
    //    List<string> tokenAddressList = GetTokenAddressKeyList();
    //    Hashtable conToken = new Hashtable();
    //    conToken["contractAddress"] = conAddress;
    //    conToken["symbol"] = symbol;
    //    if (string.IsNullOrEmpty(fullName))
    //    {
    //        fullName = "";
    //    }
    //    conToken["fullName"] = fullName;
    //    if (string.IsNullOrEmpty(iconPath))
    //    {
    //        iconPath = "";
    //    }
    //    conToken["iconPath"] = iconPath;
    //    string jonstr = Json.jsonEncode(conToken);

    //    if (!isAdd)
    //    {
    //        PlayerPrefs.SetInt(conAddress + "HideToken", 1);
    //        if (ContainsTokenAddress(tokenAddressList, conAddress))
    //        {
    //            RemoveTokenAddress(tokenAddressList, conAddress);
    //        }
    //    }
    //    else
    //    {
    //        PlayerPrefs.DeleteKey(conAddress + "HideToken");
    //        if (!ContainsTokenAddress(tokenAddressList, conAddress))
    //        {
    //            //                Debug.Log(jonstr);
    //            tokenAddressList.Add(jonstr);
    //        }
    //    }

    //    RemoveDuplicateToken(tokenAddressList);

    //    if (tokenAddressList.Count > 0)
    //        PlayerPrefsX.SetStringArray(coinInfo.address + "NEW", tokenAddressList.ToArray());
    //    else
    //        PlayerPrefs.DeleteKey(coinInfo.address + "NEW");

    //}

    //private void RemoveDuplicateToken(List<string> jsondata)
    //{
    //    for (int i = jsondata.Count - 1; i >= 0; i--)
    //    {
    //        Hashtable localToken = Json.jsonDecode(jsondata[i]) as Hashtable;
    //        if (ContainsTokenAddressCount(jsondata, localToken["contractAddress"].ToString()) > 1)
    //        {
    //            jsondata.RemoveAt(i);
    //        }
    //    }
    //}
    

    //private bool ContainsTokenAddress(List<string> jsondata, string address)
    //{
    //    foreach (var v in jsondata)
    //    {
    //        try
    //        {
    //            Hashtable localToken = Json.jsonDecode(v) as Hashtable;
    //            if (localToken["contractAddress"].ToString().ToLower() == address.ToLower())
    //            {
    //                return true;
    //            }
    //        }
    //        catch
    //        {
    //            continue;
    //        }
    //    }
    //    return false;
    //}

    //private int ContainsTokenAddressCount(List<string> jsondata, string address)
    //{
    //    int i = 0;
    //    foreach (var v in jsondata)
    //    {
    //        try
    //        {
    //            Hashtable localToken = Json.jsonDecode(v) as Hashtable;
    //            if (localToken["contractAddress"].ToString().ToLower() == address.ToLower())
    //            {
    //                i++;
    //            }
    //        }
    //        catch
    //        {
    //            continue;
    //        }
    //    }
    //    return i;
    //}

    //private void RemoveTokenAddress(List<string> jsondata, string address)
    //{
    //    for(int i = jsondata.Count - 1; i >= 0; i--)
    //    {
    //        try
    //        {
    //            Hashtable localToken = Json.jsonDecode(jsondata[i]) as Hashtable;
    //            if (localToken["contractAddress"].ToString().ToLower() == address.ToLower())
    //            {
    //                jsondata.RemoveAt(i);
    //            }
    //        }
    //        catch
    //        {
    //            continue;
    //        }
    //    }
    //}

    //public List<string> GetTokenAddressKeyList()
    //{
    //    tempConAddress = PlayerPrefsX.GetStringArray(coinInfo.address + "NEW");
    //    List<string> tokenNameList = new List<string>(tempConAddress);
    //    RemoveDuplicateToken(tokenNameList);
    //    return tokenNameList;
    //}

    //public ERCContractInofo GetERContractInfo(string contranJsonStr)
    //{
    //    ERCContractInofo c = new ERCContractInofo();
    //    Hashtable has = Json.jsonDecode(contranJsonStr) as Hashtable;
    //    c.contractAddress = has["contractAddress"].ToString();
    //    c.symbol = has["symbol"].ToString();

    //    if(has.ContainsKey("fullName"))
    //        c.fullName = has["fullName"].ToString();

    //    c.iconPath = has["iconPath"].ToString();
    //    return c;
    //}
}


