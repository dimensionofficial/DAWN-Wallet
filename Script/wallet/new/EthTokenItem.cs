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
using UnityEngine.UI;
using System.IO;

[System.Serializable]
public class EthTokenItem : MonoBehaviour
{
    public Text tokenFullNameText;
    public Text tokenNameText;
    public Text tokenNumberText;
    public Text tokenRMBText;
    public string containsAddress;
    public Image image;
    public ERC_Token erc_Token;


    private CoinInfo m_coinInfo;

    public bool isToken = false;

    private void ShowCount(decimal v)
    {
        
    }

    public void ShowETHToken(ERC_Token _ercToken)
    {
        isToken = true;
        erc_Token = _ercToken;
        tokenNameText.text = erc_Token.tokenService.TokenInfo.symbol;
        tokenFullNameText.text = erc_Token.tokenService.TokenInfo.fullName;

        NewWalletManager._Intance.ShowCount(tokenNumberText, erc_Token.tokenNumber);
        
        decimal tempStr = (erc_Token.tokenNumber * erc_Token.tokenService.TokenInfo.rmbValue);
        if (tempStr <= 0)
        {
            tokenRMBText.text = "-";
        }
        else
        {
           // tokenRMBText.text = "≈￥" + tempStr + " CNY";
            NewWalletManager._Intance.ShowProperty(tokenRMBText, tempStr);
        }
    }

    void OnDestroy()
    {
        EventManager.Instance.RemoveEventListener(EventID.UpdateETHbalance, RefreshETHBalace);
    }

    //private void RefreshBalace(params object[] objs)
    //{
    //    if (isToken 
    //        && gameObject.activeInHierarchy 
    //        && erc_Token != null 
    //        && erc_Token.tokenService != null 
    //        && erc_Token.tokenNumber > 0)
    //    {
    //        if (EthTokenManager._Intance.tokenServiceDic.ContainsKey(erc_Token.tokenService.tokenContractAddress.ToLower()))
    //        {
    //            TokenService tokenService = EthTokenManager._Intance.tokenServiceDic[erc_Token.tokenService.tokenContractAddress.ToLower()];
    //            decimal tempStr = (erc_Token.tokenNumber * tokenService.TokenInfo.rmbValue);
    //            if (tempStr <= 0)
    //            {
    //                tokenRMBText.text = "-";
    //            }
    //            else
    //            {
    //                NewWalletManager._Intance.ShowProperty(tokenRMBText, tempStr);
    //            }
    //        }
    //    }
    //}

    public void ShowTokenInfo(ERCContractInofo ercInfo, EthcoinInfoItem item)
    {
        isToken = true;
        image.gameObject.SetActive(false);
        containsAddress = ercInfo.contractAddress;
        tokenNameText.text = ercInfo.symbol;
        tokenFullNameText.text = ercInfo.fullName;
        //if (ethItem.tokenInfoDic.ContainsKey(containsAddress))
        //{
        //    erc_Token = ethItem.tokenInfoDic[containsAddress];
        //    NewWalletManager._Intance.ShowCount(tokenNumberText, erc_Token.tokenNumber);
        //    decimal tempStr = (erc_Token.tokenNumber * erc_Token.tokenService.TokenInfo.rmbValue);
        //    if (tempStr <= 0)
        //    {
        //        tokenRMBText.text = "-";
        //    }
        //    else
        //    {
        //        //tokenRMBText.text = "≈￥" + tempStr + " CNY";
        //        NewWalletManager._Intance.ShowProperty(tokenRMBText, tempStr);
        //    }
        //    Debug.Log(erc_Token.tokenNumber + "  : " + erc_Token.tokenService.TokenInfo.rmbValue);
        //}
        StartCoroutine(GetTokenBalances(ercInfo, item));
    }

    public IEnumerator GetTokenBalances(ERCContractInofo ercInfo, EthcoinInfoItem item)
    {
		
        TokenService tokenService;

        if (!EthTokenManager._Intance.tokenServiceDic.ContainsKey(ercInfo.contractAddress.ToLower()))
        {
            tokenService = EthTokenManager._Intance.GetTokenService(ercInfo.contractAddress, ercInfo.symbol, false);
            tokenService.TokenInfo.iconPath = ercInfo.iconPath;
            tokenService.TokenInfo.symbol = ercInfo.symbol;
            tokenService.TokenInfo.fullName = ercInfo.fullName;
            EthTokenManager._Intance.tokenServiceDic.Add(ercInfo.contractAddress.ToLower(), tokenService);
        }
        else
        {
            tokenService = EthTokenManager._Intance.tokenServiceDic[ercInfo.contractAddress.ToLower()];
            tokenService.TokenInfo.iconPath = ercInfo.iconPath;
            tokenService.TokenInfo.symbol = ercInfo.symbol;
            tokenService.TokenInfo.fullName = ercInfo.fullName;
        }
        tokenNameText.text = tokenService.TokenInfo.symbol;
        tokenFullNameText.text = tokenService.TokenInfo.fullName;

        StartCoroutine(TextureUIAsset._Instance.LoadImage(image, tokenService.TokenInfo.iconPath));

		List<KeyValuePair<string, string>> ws = new List<KeyValuePair<string, string>>();
		ws.Add(new KeyValuePair<string, string>("op", "getTokenPrice"));
		ws.Add(new KeyValuePair<string, string>("address", ercInfo.contractAddress));
        
        yield return HttpManager._Intance.SendRequest(ws, delegate (Hashtable h)
			{
                if(h.ContainsKey("price"))
				    tokenService.TokenInfo.rmbValue = decimal.Parse(h["price"].ToString());

			}, null, false);
		

  //    StartCoroutine(TextureUIAsset._Instance.okenService.TokenInfo.iconPath));
        string addrs = item.coinInfo.address.Substring(2, item.coinInfo.address.Length - 2);
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
            if (tokenService.tokenDecimal < 0 || string.IsNullOrEmpty(tokenService.TokenInfo.symbol) || string.IsNullOrEmpty(tokenService.TokenInfo.symbol))
            {
                List<KeyValuePair<string, string>> ws1 = new List<KeyValuePair<string, string>>();
                ws1.Add(new KeyValuePair<string, string>("ercaddress", tokenService.tokenContractAddress));
                yield return HttpManager._Intance.GetNodeJsRequest("gettokeninfo", ws1, (Hashtable data) =>
                {
                    if (data != null)
                    {
                        string fullname = data["fullname"].ToString();
                        string symbol = data["symbol"].ToString();
                        int decimals = int.Parse(data["decimals"].ToString());
                        tokenService.tokenDecimal = decimals;
                        tokenService.TokenInfo.symbol = symbol;
                        tokenService.TokenInfo.fullName = fullname;

                        if (tokenService.tokenDecimal >= 0)
                            GetTokenBalances(reslt, tokenService);
                    }
                });
            }
            else
            {
                GetTokenBalances(reslt, tokenService);
            }

        }
    }

    private void GetTokenBalances(string reslt, TokenService tokenService)
    {
        decimal customTokenBalance = UnitConversion.Convert.FromWei(
        tokenService.DecodeVariable<BigInteger>("balanceOf", reslt), tokenService.tokenDecimal);

        NewWalletManager._Intance.ShowCount(tokenNumberText, customTokenBalance);

        decimal tempCount = customTokenBalance;
        decimal tempStr = (tempCount * tokenService.TokenInfo.rmbValue);
        if (tempStr <= 0)
        {
            tokenRMBText.text = "-";
        }
        else
        {
            //tokenRMBText.text = "≈￥" + tempStr + " CNY";
            NewWalletManager._Intance.ShowProperty(tokenRMBText, tempStr);
        }

        erc_Token = new ERC_Token();
        erc_Token.tokenNumber = customTokenBalance;
        erc_Token.cout = customTokenBalance.ToString();
        erc_Token.tokenService = tokenService;
    }


    public void ETHAddEventListener()
    {
        EventManager.Instance.AddEventListener(EventID.UpdateETHbalance, RefreshETHBalace);
    }
    private void RefreshETHBalace(params object[] objs)
    {
        if (!isToken && gameObject.activeInHierarchy && m_coinInfo != null)
        {
            ShowETHmoney();
        }
    }
    void OnDestory()
    {
        EventManager.Instance.RemoveEventListener(EventID.UpdateETHbalance, RefreshETHBalace);
    }

    private void ShowETHmoney()
    {
        NewWalletManager._Intance.ShowCount(tokenNumberText, m_coinInfo.money);
        decimal tempStr = (m_coinInfo.money * HttpManager._Intance.eth_RMB);
        if (tempStr <= 0)
        {
            tokenRMBText.text = "-";
        }
        else
        {
            NewWalletManager._Intance.ShowProperty(tokenRMBText, tempStr);
        }
    }

    public void ShowETH(CoinInfo ethInfo)
    {
        m_coinInfo = ethInfo;
        image.gameObject.SetActive(true);
        isToken = false;
        tokenNameText.text = "ETH";
        tokenFullNameText.text = "Ethereum";
        ShowETHmoney();
    }

    public void OnClickMe()
    {
        PanelManager._Instance._WalletInfoPanel.ShowMe(this);
    }

}
