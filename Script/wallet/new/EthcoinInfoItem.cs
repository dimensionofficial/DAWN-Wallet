using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System;
using Nethereum.JsonRpc.UnityClient;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Util;
using System.Numerics;
using System.Collections.Generic;

[System.Serializable]
public class ERC_Token
{
    public TokenService tokenService;
    public string cout;
    public decimal tokenNumber;
    public string ethv;
    public string rmbv;
    public string fullName;
}
public class ERCContractInofo
{
    public string contractAddress;
    public string symbol;
    public string fullName;
    public string iconPath;
}

public class EthcoinInfoItem : CoinInfoItemBase
{
    bool isFirsRefreshToken = true;

    bool isDestory = false;

    public List<ERC_Token> eRC_TokenList = new List<ERC_Token>();
    /// <summary>
    /// 代币交易记录
    /// </summary>
    public Dictionary<string, List<ETHHistoryRcord>> etherc20Dic = new Dictionary<string, List<ETHHistoryRcord>>();

    public Dictionary<string, decimal> tokenDicBalance = new Dictionary<string, decimal>();

    public override void Init(string[] v)
    {
        base.Init(v);
        HistoryManagerNew.Instance.Refresh(coinInfo.address, null, RefreshType.ETH);
 //     HistoryManagerNew.Instance.Refresh(coinInfo.address, GetTokenAllBalance, RefreshType.Token); //
    }
    decimal tokenETH = 0;
    public override void RefreshBalance()
    {
        string tempMoney = PlayerPrefs.GetString(coinInfo.address + "Balance");
        if (!string.IsNullOrEmpty(tempMoney))
        {
            coinInfo.money = decimal.Parse(tempMoney);
			coinInfo.ethmoney = tempMoney;
            ShowBalancesCount();
            isGetBalance = true;
            EventManager.Instance.SendEvent(EventID.UpdateETHbalance);
        }

        StartCoroutine(GetEthBalances());
        
    }

    public override void GetHistory()
    {
        HistoryManagerNew.Instance.Refresh(coinInfo.address, null, RefreshType.ETH);
        HistoryManagerNew.Instance.Refresh(coinInfo.address, null, RefreshType.Token);

        //if (isFirsRefreshToken)
        //{
        //    HistoryManagerNew.Instance.Refresh(coinInfo.address, GetTokenAllBalance, RefreshType.Token);
        //}
        //else
        //{
        //    HistoryManagerNew.Instance.Refresh(coinInfo.address, null, RefreshType.Token);
        //}

    }

    void OnDestroy()
    {

        StopAllCoroutines();
        isDestory = true;
    }

    private IEnumerator GetEthBalances()
    {
        // string url = "https://mainnet.infura.io";
        Hashtable myJsonData = new Hashtable();
        myJsonData["id"] = 1;
        myJsonData["jsonrpc"] = "2.0";
        myJsonData["method"] = "eth_getBalance";
        ArrayList arrayList = new ArrayList();
        arrayList.Add(coinInfo.address);
        arrayList.Add("latest");
        myJsonData["params"] = arrayList;
        string rpcRequestJson = Json.jsonEncode(myJsonData);
 //     Debug.Log(rpcRequestJson);
        UnityWebRequest unityRequest = QRPayTools.GetUnityWebRequest(rpcRequestJson);
        yield return unityRequest.SendWebRequest();
        if (unityRequest.error != null)
        {
   //       Exception Exception = new Exception(unityRequest.error);
            Debug.Log(unityRequest.error);
        }
        else
        {
            
            byte[] results = unityRequest.downloadHandler.data;
            string responseJson = Encoding.UTF8.GetString(results).ToString();
            Hashtable table = Json.jsonDecode(responseJson) as Hashtable;
            string Result = table["result"].ToString();
            Int64 ethBalance = Convert.ToInt64(Result, 16);
            coinInfo.money = Nethereum.Util.UnitConversion.Convert.FromWei(ethBalance, 18);
			coinInfo.ethmoney = coinInfo.money.ToString ();
            // PanelManager._Instance._mainPanel.ethCount += coinInfo.money;
            PlayerPrefs.SetString(coinInfo.address + "Balance", coinInfo.money.ToString());
            isGetBalance = true;
            EventManager.Instance.SendEvent(EventID.UpdateETHbalance);
        }
    }
    private IEnumerator GetTokenBalance(string reslt, TokenService tokenService, int i, int totalCount)
    {
        
        decimal customTokenBalance = UnitConversion.Convert.FromWei(
                       tokenService.DecodeVariable<BigInteger>("balanceOf", reslt), tokenService.tokenDecimal); //tokenService.tokenDecimal);

        
        string contractAddress = tokenService.tokenContractAddress;

        decimal tempCount = customTokenBalance;

        if (tokenDicBalance.ContainsKey(contractAddress))
        {
            tokenDicBalance[contractAddress] = tempCount;
        }
        else
        {
            tokenDicBalance.Add(contractAddress, tempCount);
        }

        int index = -1;

        if (!JudegeContrainErcToken(eRC_TokenList, contractAddress, ref index))
        {
            
            List <KeyValuePair<string, string>> ws = new List<KeyValuePair<string, string>>();
            ws.Add(new KeyValuePair<string, string>("op", "getTokenPrice"));
            ws.Add(new KeyValuePair<string, string>("address", contractAddress));
            HttpManager._Intance.StartCoroutine(HttpManager._Intance.SendRequest(ws, delegate (Hashtable h)
            {
                int refIndex = -1; 
                    
                tokenService.TokenInfo.rmbValue = decimal.Parse(h["price"].ToString());
                decimal tempEth = decimal.Parse((tempCount * tokenService.TokenInfo.rmbValue * 1.0M / HttpManager._Intance.eth_RMB).ToString("F5"));
                if (!JudegeContrainErcToken(eRC_TokenList, contractAddress, ref refIndex))
                {
                    ERC_Token ercToken = new ERC_Token();
                    ercToken.ethv = tempEth.ToString();
                    ercToken.rmbv = tokenService.TokenInfo.rmbValue.ToString();
                    ercToken.cout = customTokenBalance.ToString();
                    ercToken.tokenNumber = customTokenBalance;
                    ercToken.tokenService = tokenService;
                    eRC_TokenList.Add(ercToken);
                }
                else
                {
                    if (refIndex >= 0)
                    {
                        if (eRC_TokenList.Count != 0 && i < eRC_TokenList.Count)
                        {
                            eRC_TokenList[i].ethv = tempEth.ToString();
                            eRC_TokenList[i].ethv = tempEth.ToString();
                            eRC_TokenList[i].rmbv = tokenService.TokenInfo.rmbValue.ToString();
                            eRC_TokenList[i].cout = customTokenBalance.ToString();
                            eRC_TokenList[i].tokenNumber = customTokenBalance;
                            eRC_TokenList[i].tokenService = tokenService;
                        }
                           
                    }
                }

            }, null, false));

            yield return new WaitForSeconds(0.5F);

        }
        else
        {
            if (eRC_TokenList.Count != 0 && index < eRC_TokenList.Count)
            {
                eRC_TokenList[index].tokenNumber = customTokenBalance;
            }
        }

        if (i == totalCount - 1 && eRC_TokenList.Count == totalCount)
        {
            tokenETH = 0;
            for (int k = 0; k < eRC_TokenList.Count; k++)
            {
                decimal d = decimal.Parse(eRC_TokenList[k].ethv);
                tokenETH += d;
            }
            BalancesLater();

            isFirsRefreshToken = false;
        }
    }
    private void BalancesLater()
    {
        if (tokenETH > 0)
        {
            PlayerPrefs.SetString(coinInfo.address + "TokenBalance", tokenETH.ToString("f5"));
        }
        else
        {
            PlayerPrefs.DeleteKey(coinInfo.address + "TokenBalance");
        }
        EventManager.Instance.SendEvent(EventID.UpdateETHbalance);
        ShowBalancesCount();
    }

    //public void GetTokenAllBalance(bool b)
    //{
    //    GetTokenContractAddress();
    //}

    public void GetTokenContractAddress()
    {
        HistoryManagerNew.Instance.GetAllTokenInfo(coinInfo.address.ToLower(), delegate (List<ETHHistoryRcord> r)
        {
            tokenContractAddress = new List<string>();
            for (int i = 0; i < r.Count; i++)
            {
                tokenContractAddress.Add(r[i].contractAddress);
            }

            StartCoroutine(GetTokensInfos(r));

            //GetAllTokenBlances(r);
        });
    }

    //private IEnumerator GetAllTokenBlances(List<ETHHistoryRcord> tokenRcordlist)
    //{
    //    List<string> tokenAddressList = GetTokenAddressKeyList(); //不需要显示的token
    //    int totalCount = tokenRcordlist.Count;
    //    for (int i = 0; i < totalCount; i++)
    //    {
    //        TokenService tokenService = EthTokenManager._Intance.GetTokenService(tokenRcordlist[i].contractAddress, true);

    //        if (!EthTokenManager._Intance.tokenServiceDic.ContainsKey(tokenRcordlist[i].contractAddress.ToLower()))
    //        {
    //            EthTokenManager._Intance.tokenServiceDic.Add(tokenRcordlist[i].contractAddress.ToLower(), tokenService);
    //        }
    //        int index;
    //        if (!ContainsTokenAddress(tokenAddressList, tokenRcordlist[i].contractAddress.ToLower(), out index))
    //        {
    //            SetTokenAddress(tokenRcordlist[i].contractAddress.ToLower(), tokenRcordlist[i].tokenSymbol, "", tokenRcordlist[i].fullName, true);
    //        }

    //        StartCoroutine(GetTokenBlance(tokenService, i, totalCount));

    //        yield return new WaitForSeconds(0.5F);
    //    }
    //}
   
    private IEnumerator GetTokensInfos(List<ETHHistoryRcord> rcord)
    {
        int totalCount = rcord.Count;
        if (totalCount <= 0)
        {
            tokenETH = 0;
            BalancesLater();
        }
        else
        {
            List<string> tokenAddressList = GetTokenAddressKeyList(); //需要显示的代币信息token

            for (int n = 0; n < totalCount; n++)
            {
                int i = n;
                if (string.IsNullOrEmpty(rcord[i].contractAddress))
                    continue;

                bool finishedTag = false;

                if (EthTokenManager._Intance.tokenServiceDic.ContainsKey(rcord[i].contractAddress.ToLower()))
                {
                    TokenService tokenService = EthTokenManager._Intance.tokenServiceDic[rcord[i].contractAddress.ToLower()];
                    StartCoroutine(GetTokenBlance(tokenService, i, totalCount));
                    finishedTag = true;
                }
                else
                {
                    List<KeyValuePair<string, string>> ws1 = new List<KeyValuePair<string, string>>();
                    ws1.Add(new KeyValuePair<string, string>("ercaddress", rcord[i].contractAddress));
                    HttpManager._Intance.StartCoroutine(HttpManager._Intance.GetNodeJsRequest("gettokeninfo", ws1, (Hashtable data) =>
                    {
                        if (data != null)
                        {
                            TokenService tokenService = EthTokenManager._Intance.GetTokenService(rcord[i].contractAddress, false);
                            string fullname = data["fullname"].ToString();
                            string symbol = data["symbol"].ToString();
                            int decimals = int.Parse(data["decimals"].ToString());
                            tokenService.tokenDecimal = decimals;
                            tokenService.TokenInfo.fullName = fullname;
                            tokenService.TokenInfo.symbol = symbol;


                            if (!EthTokenManager._Intance.tokenServiceDic.ContainsKey(rcord[i].contractAddress.ToLower()))
                            {
                                EthTokenManager._Intance.tokenServiceDic.Add(rcord[i].contractAddress.ToLower(), tokenService);
                            }

                            if (!isHideToken(rcord[i].contractAddress))
                            {
                                if (!ContainsTokenAddress(tokenAddressList, rcord[i].contractAddress))
                                {
                                    tokenService.TokenInfo.isShow = 0;
                                    SetTokenAddress(tokenService.tokenContractAddress, tokenService.TokenInfo.symbol, "", tokenService.TokenInfo.fullName, true);
                                    //AddTokenService(tokenService);
                                }

                            }

                            StartCoroutine(GetTokenBlance(tokenService, i, totalCount));
                            finishedTag = true;
                        }
                    }));
                }

                while (finishedTag == false)
                {
                    yield return new WaitForFixedUpdate();
                }
            }
        }
    }

    public bool isHideToken(string tokenContractAddress)
    {
        int isHide = PlayerPrefs.GetInt(coinInfo.address + tokenContractAddress + "HideToken");

        if (isHide > 0)
        {
            return true;
        }
        return false;
    }


    private IEnumerator GetTokenBlance(TokenService tokenService, int i, int totalCount)
    {
        string contractAddress = tokenService.tokenContractAddress;
        string addrs = coinInfo.address.Substring(2, coinInfo.address.Length - 2);
        string rpc_json = tokenService.GetTokenRPC_Json("0x70a08231000000000000000000000000" + addrs);
        UnityWebRequest unityRequest = QRPayTools.GetUnityWebRequest(rpc_json);
        yield return unityRequest.SendWebRequest();
        if (unityRequest.error != null)
        {
            Debug.Log(unityRequest.error);
        }
        else
        {
            byte[] results = unityRequest.downloadHandler.data;
            string responseJson = Encoding.UTF8.GetString(results).ToString();
            Hashtable ht = Json.jsonDecode(responseJson) as Hashtable;
            string reslt = ht["result"].ToString();
            if (tokenService.tokenDecimal > 0)
            {
                StartCoroutine(GetTokenBalance(reslt, tokenService, i, totalCount));
            }
        }
    }

    private bool JudegeContrainErcToken(List<ERC_Token> ercList, string conaddress, ref int index)
    {
        for (int i = 0; i < ercList.Count; i++)
        {
            if (ercList[i].tokenService.tokenContractAddress.ToLower().Equals(conaddress.ToLower()))
            {
                index = i;
                return true;
            }
        }

        return false;
    }


    
    public void ShowBalancesCount()
    {
        //TODO
        string str = "";

        string tempTokeMoney = PlayerPrefs.GetString(coinInfo.address + "TokenBalance");
        if (!string.IsNullOrEmpty(tempTokeMoney))
        {
            tokenETH = decimal.Parse(tempTokeMoney);
            string v = NewWalletManager._Intance.ShowCount(null, tokenETH);
            str = "  (Token≈" + v + " ETH)";
        }
        coinInfo.ethTokenMoney = tokenETH;
		coinInfo.tokenmoney = tokenETH.ToString ();
        NewWalletManager._Intance.ShowCount(countText, coinInfo.money);
        if (PanelManager._Instance._mainPanel.IsHideAsset())
        {
            countText.text = "*****";
        }
        else
        {
            countText.text = countText.text + " ETH" + str;
        }
            
    }
    public List<string> tokenContractAddress = new List<string>();

    //void AddTokenService(TokenService tokenService)
    //{
    //    HttpManager._Intance.UpLoadContractAddress(tokenService.tokenContractAddress, tokenService.TokenInfo.symbol, tokenService.TokenInfo.fullName, delegate (Hashtable h)
    //    {
    //        int isHide = PlayerPrefs.GetInt(coinInfo.address + tokenService.tokenContractAddress + "HideToken");
    //        if (isHide > 0)
    //        {
    //            // 被隐藏
    //        }
    //        else
    //        {
    //            string iconPath = "";
    //            if (int.Parse(h["error"].ToString()) > 0)
    //            {
    //                if(h["tokenIcon"] != null)
    //                    iconPath = h["tokenIcon"].ToString();
    //            }
    //            tokenService.TokenInfo.isShow = 0;
    //            tokenService.TokenInfo.iconPath = iconPath;
    //            SetTokenAddress(tokenService.tokenContractAddress, tokenService.TokenInfo.symbol, iconPath, tokenService.TokenInfo.fullName, true);
    //        }
    //    });
    //}

	public void SetTokenAddressList(List<SaveSwitchTokenInfo> saveTokenInfo)
	{
		List<string> tokenAddressList = GetTokenAddressKeyList();
		for (int i = 0; i < tokenAddressList.Count; i++) 
		{
			ERCContractInofo erc = GetERContractInfo(tokenAddressList[i]);
			PlayerPrefs.DeleteKey(coinInfo.address + erc.contractAddress + "HideToken");
		}

		if (saveTokenInfo.Count > 0)
		{
			List<string> savaList = new List<string> ();
			for (int i = 0; i < saveTokenInfo.Count; i++)
			{
				Hashtable conToken = new Hashtable();
				conToken["contractAddress"] = saveTokenInfo[i].conAddress;
				conToken["symbol"] = saveTokenInfo[i].symbol;
				if (string.IsNullOrEmpty(saveTokenInfo[i].fullName))
				{
					saveTokenInfo[i].fullName = "";
				}
				conToken["fullName"] = saveTokenInfo[i].fullName;
				if (string.IsNullOrEmpty(saveTokenInfo[i].iconPath))
				{
					saveTokenInfo[i].iconPath = "";
				}
				conToken["iconPath"] = saveTokenInfo[i].iconPath;
				string jonstr = Json.jsonEncode(conToken);

                if (!ContainsTokenAddress(savaList, saveTokenInfo[i].conAddress))
				{
					savaList.Add (jonstr);
				}

			}

			if (savaList.Count > 0)
				PlayerPrefsX.SetStringArray(coinInfo.address + "NEW", savaList.ToArray());
			else
				PlayerPrefs.DeleteKey(coinInfo.address + "NEW");
			
		} else
		{
			PlayerPrefs.DeleteKey(coinInfo.address + "NEW");
		}
	}

    /// <summary>
    /// 本地保存的代币开启
    /// </summary>
    /// <param name="代币合约地址"></param>
    /// <param name="是否添加"></param>
    public void SetTokenAddress(string conAddress, string symbol, string iconPath, string fullName, bool isAdd)
    {
        List<string> tokenAddressList = GetTokenAddressKeyList();

        Hashtable conToken = new Hashtable();
        conToken["contractAddress"] = conAddress.ToLower();
        conToken["symbol"] = symbol;
        if (string.IsNullOrEmpty(fullName))
        {
            fullName = "";
        }
        conToken["fullName"] = fullName;
        if (string.IsNullOrEmpty(iconPath))
        {
            iconPath = "";
        }
        conToken["iconPath"] = iconPath;
        string jonstr = Json.jsonEncode(conToken);

        string key = coinInfo.address + conAddress;
        if (!isAdd)
        {
            PlayerPrefs.SetInt(key + "HideToken", 1);
            if (ContainsTokenAddress(tokenAddressList, conAddress))
            {
                RemoveTokenAddress(tokenAddressList, conAddress);
            }
        }
        else
        {
            PlayerPrefs.DeleteKey(key + "HideToken");
            if (!ContainsTokenAddress(tokenAddressList, conAddress))
            {
                //Debug.Log(jonstr);
                tokenAddressList.Add(jonstr);
            }
            else
            {
                if (!string.IsNullOrEmpty(fullName) && !string.IsNullOrEmpty(iconPath))
                {
                    for (int i = tokenAddressList.Count - 1; i >= 0; i--)
                    {
                        Hashtable localToken = Json.jsonDecode(tokenAddressList[i]) as Hashtable;
                        if (localToken["contractAddress"].ToString().ToLower() == conAddress.ToLower())
                        {
                            tokenAddressList[i] = jonstr;
                            break;
                        }
                    }
                }
            }
        }

        RemoveDuplicateToken(tokenAddressList);

        if (tokenAddressList.Count > 0)
            PlayerPrefsX.SetStringArray(coinInfo.address + "NEW", tokenAddressList.ToArray());
        else
            PlayerPrefs.DeleteKey(coinInfo.address + "NEW");

    }

    private void RemoveDuplicateToken(List<string> jsondata)
    {
        for (int i = jsondata.Count - 1; i >= 0; i--)
        {
            Hashtable localToken = Json.jsonDecode(jsondata[i]) as Hashtable;
            if (ContainsTokenAddressCount(jsondata, localToken["contractAddress"].ToString()) > 1)
            {
                jsondata.RemoveAt(i);
            }
        }
    }


    private bool ContainsTokenAddress(List<string> jsondata, string contractAddress)
    {
        for (int i = 0; i < jsondata.Count; i++)
        {
            try
            {
                Hashtable localToken = Json.jsonDecode(jsondata[i]) as Hashtable;
                if (localToken["contractAddress"].ToString().ToLower() == contractAddress.ToLower())
                {
                    return true;
                }
            }
            catch
            {
                continue;
            }
        }


        return false;
    }

    private int ContainsTokenAddressCount(List<string> jsondata, string address)
    {
        int i = 0;
        foreach (var v in jsondata)
        {
            try
            {
                Hashtable localToken = Json.jsonDecode(v) as Hashtable;
                if (localToken["contractAddress"].ToString().ToLower() == address.ToLower())
                {
                    i++;
                }
            }
            catch
            {
                continue;
            }
        }
        return i;
    }

    private void RemoveTokenAddress(List<string> jsondata, string address)
    {
        for (int i = jsondata.Count - 1; i >= 0; i--)
        {
            try
            {
                Hashtable localToken = Json.jsonDecode(jsondata[i]) as Hashtable;
                if (localToken["contractAddress"].ToString().ToLower() == address.ToLower())
                {
                    jsondata.RemoveAt(i);
                }
            }
            catch
            {
                continue;
            }
        }
    }

	/// <summary>
	/// 本地保存的需要显示的代币信息
	/// </summary>
	/// <returns>The token address key list.</returns>
    public List<string> GetTokenAddressKeyList()
    {
        tempConAddress = PlayerPrefsX.GetStringArray(coinInfo.address + "NEW");
        List<string> tokenNameList = new List<string>(tempConAddress);
        RemoveDuplicateToken(tokenNameList);
        return tokenNameList;
    }
    public ERCContractInofo GetERContractInfo(string contranJsonStr)
    {
        ERCContractInofo c = new ERCContractInofo();
        Hashtable has = Json.jsonDecode(contranJsonStr) as Hashtable;
        c.contractAddress = has["contractAddress"].ToString();
        c.symbol = has["symbol"].ToString();

        if (has.ContainsKey("fullName"))
            c.fullName = has["fullName"].ToString();

        c.iconPath = has["iconPath"].ToString();
        return c;
    }
}
