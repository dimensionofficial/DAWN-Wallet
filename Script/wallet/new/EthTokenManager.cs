using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nethereum.JsonRpc.UnityClient;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts;
using UnityEngine.Networking;
using System.Text;
using System;

[System.Serializable]
public struct TokenInfo
{
    public string fullName;
 //   public string name;
    public string symbol;
    //    public int decimals;
    //    public BigInteger totalSupply;

    private decimal m_usdvalue;
    public decimal rmbValue
    {
        get
        {
            return m_usdvalue * HttpManager._Intance.current_usdt;
        }
        set
        {
            m_usdvalue = value;
        }
    }

    public int isShow;
    public string iconPath;
};
[System.Serializable]
public class TokenService
{
    public string tokenContractAddress;

    public int tokenDecimal = -1;

    public Contract contract;

    public TokenInfo TokenInfo = new TokenInfo();

    public TokenService(string ABI, string tokenAddress, bool isHaveTokenInfo)
    {
        tokenContractAddress = tokenAddress;
        this.contract = new Contract(null, ABI, tokenContractAddress);
        if (isHaveTokenInfo)
        {
            List<KeyValuePair<string, string>> ws = new List<KeyValuePair<string, string>>();
            ws.Add(new KeyValuePair<string, string>("ercaddress", tokenAddress));
            HttpManager._Intance.StartCoroutine(HttpManager._Intance.GetNodeJsRequest("gettokeninfo", ws, (Hashtable data) =>
            {
                if (data != null)
                {
                    int decimals = int.Parse(data["decimals"].ToString());
                    string fullname = data["fullname"].ToString();
                    string symbol = data["symbol"].ToString();
                    tokenDecimal = decimals;
                    TokenInfo.symbol = symbol;
                    TokenInfo.fullName = fullname;
                }
            }));
        }
        
    }

    public CallInput CreateCallInput(string variableName, params object[] p)
    {
        var function = contract.GetFunction(variableName);
        return function.CreateCallInput(p);
    }

    public T DecodeVariable<T>(string variableName, string result)
    {
        var function = contract.GetFunction(variableName);
        try
        {
            return function.DecodeSimpleTypeOutput<T>(result); // this results in an error if BigInteger is 0
        }
        catch
        {
            return default(T);
        }
    }

    public string GetTokenRPC_Json(string data)
    {
        Hashtable myJsonData = new Hashtable();
        myJsonData["id"] = 1;
        myJsonData["jsonrpc"] = "2.0";
        myJsonData["method"] = "eth_call";
        ArrayList arrayList = new ArrayList();
        Hashtable json1 = new Hashtable();
        json1["data"] = data;
        json1["to"] = tokenContractAddress;
        arrayList.Add(json1);
        arrayList.Add("latest");
        myJsonData["params"] = arrayList;
        return Json.jsonEncode(myJsonData);
    }
   
        //public IEnumerator GetTokenInfo ()
        //{
        //    yield return 0;

        //    //Create a unity call request (we have a request for each type of rpc operation)
        //    var currencyInfoRequest = new EthCallUnityRequest(ETHInfo._url);
        //    //{"jsonrpc":"2.0","method":"eth_call","params":[{see above}],"id":1}
        //    UnityWebRequest unityRequest = QRPayTools.GetUnityWebRequest("0x95d89b41"); //ABI symbol
        //    yield return unityRequest.SendWebRequest();
        //    if (unityRequest.error != null)
        //    {
        //        Debug.Log(unityRequest.error);
        //    }
        //    else
        //    {
        //        byte[] results = unityRequest.downloadHandler.data;
        //        string responseJson = Encoding.UTF8.GetString(results).ToString();
        //        Hashtable ht = Json.jsonDecode(responseJson) as Hashtable;
        //        string reslt = ht["result"].ToString();
        //        TokenInfo.symbol = DecodeVariable<string>("symbol", reslt);
        //    }
        //    CallInput input = CreateCallInput("transfer", "", "");

        //    CallInput input1 = CreateCallInput("symbol"); //0x95d89b41 0x313ce567 0x06fdde03 0x18160ddd
        //    CallInput input2= CreateCallInput("decimals");
        //    CallInput input3 = CreateCallInput("name");
        //    CallInput input4 = CreateCallInput("totalSupply");

        //    CallInput input5 = CreateCallInput("balanceOf");
        //    //// get token symbol (string)
        //    //yield return currencyInfoRequest.SendRequest(CreateCallInput("symbol"), BlockParameter.CreateLatest());
        //    //Debug.Log("AAAAAAAAA1 = " + currencyInfoRequest.Result);
        //    //TokenInfo.symbol = DecodeVariable<string>("symbol", currencyInfoRequest.Result);
        //    //// get token decimal places (uint 8)
        //    //yield return currencyInfoRequest.SendRequest(CreateCallInput("decimals"), BlockParameter.CreateLatest());
        //    //TokenInfo.decimals = DecodeVariable<int>("decimals", currencyInfoRequest.Result);

        //    //// get token name (string)
        //    //yield return currencyInfoRequest.SendRequest(CreateCallInput("name"), BlockParameter.CreateLatest());
        //    //TokenInfo.name = DecodeVariable<string>("name", currencyInfoRequest.Result);

        //    //// get token totalSupply (uint 256)
        //    //yield return currencyInfoRequest.SendRequest(CreateCallInput("totalSupply"), BlockParameter.CreateLatest());
        //    //TokenInfo.totalSupply = DecodeVariable<BigInteger>("totalSupply", currencyInfoRequest.Result);

        //}
}

public class EthTokenManager : MonoBehaviour
{
    public static EthTokenManager _Intance;

    public string ABI;


    public Dictionary<string, Sprite> tokenIconDic = new Dictionary<string, Sprite>();

    public Dictionary<string, TokenService> tokenServiceDic = new Dictionary<string, TokenService>();

    void Awake()
    {
        _Intance = this;
    }

    public TokenService GetTokenService(string contractAddress, string symbol, bool isHaveToken)
    {
		TokenService tokenService = GetTokenService(contractAddress, isHaveToken);
        tokenService.TokenInfo.symbol = symbol;
        return tokenService;
    }

	public TokenService GetTokenService(string contractAddress, bool isHaveToken)
	{
		return new TokenService(ABI, contractAddress, isHaveToken);
	}

    public void InitTokenList(ArrayList tokenList)
    {
        Dictionary<string, string> tempDic = new Dictionary<string, string>();
        for (int i = 0; i < tokenList.Count; i++)
        {
            Hashtable temp = tokenList[i] as Hashtable;
           
            //Debug.Log(temp["tokenName"].ToString() + " : " + temp["tokenAdress"].ToString() + " -> " + temp["isShow"].ToString());
            string tokenName = temp["tokenName"].ToString();
            string tokenAddress = temp["tokenAdress"].ToString().ToLower();
            TokenService tokenService = GetTokenService(tokenAddress, tokenName, true);
            tokenService.TokenInfo.isShow = int.Parse(temp["isShow"].ToString());
            tokenService.TokenInfo.fullName = temp["fullName"].ToString();
            if (temp["iconPath"] == null)
            {
                tokenService.TokenInfo.iconPath = "";
            }
            else
            {
                string path = temp["iconPath"].ToString();
                if (!string.IsNullOrEmpty(path) && !path.StartsWith("http"))
                {
                    path = "https://etherscan.io" + path;
                }
                tokenService.TokenInfo.iconPath = path;
            }

            if (tempDic.ContainsKey(tokenName))
            {
          //      Debug.Log("相同名字 = " + tokenName);
            }
            else
            {
                tempDic.Add(tokenName, tokenName);
            }

            if (tokenServiceDic.ContainsKey(tokenAddress))
            {
         //       Debug.Log("相同地址 = " + tokenAddress);
            }
            else
            {
                tokenServiceDic.Add(tokenAddress, tokenService);
            } 
        }
    }

    //private IEnumerator Get_Usdt()
    //{
    //    UnityWebRequest unityRequest = new UnityWebRequest("http://data.gateio.io/api2/1/ticker/usdt_cny");
    //    unityRequest.SetRequestHeader("Content-Type", "application/json");
    //    unityRequest.downloadHandler = new DownloadHandlerBuffer();
    //    yield return unityRequest.SendWebRequest();
    //    if (unityRequest.error != null)
    //    {
    //        Debug.Log(unityRequest.error);
    //    }
    //    else
    //    {
    //        byte[] results = unityRequest.downloadHandler.data;
    //        string responseJson = Encoding.UTF8.GetString(results).ToString();
    //        Hashtable table = Json.jsonDecode(responseJson) as Hashtable;
    //        string str = "0";
    //        if (table["last"] != null)
    //        {
    //            str = table["last"].ToString();               
    //        }

    //    }
    //}

    //public IEnumerator GetTokenBalance(string ethAddress, TokenService tokenService)
    //{
    //    var tokenBalanceRequest = new EthCallUnityRequest(ETHInfo._url);

    //    // get custom token balance (uint 256)
    //    yield return tokenBalanceRequest.SendRequest(tokenService.CreateCallInput("balanceOf", ethAddress),
    //        BlockParameter.CreateLatest());

    //    string customTokenBalance = UnitConversion.Convert.FromWei(
    //        tokenService.DecodeVariable<BigInteger>("balanceOf", tokenBalanceRequest.Result),18).ToString();

    //    Debug.Log(tokenService.TokenInfo.symbol + ":" + customTokenBalance);
    //}
    public IEnumerator Packaging(string address, string hash, Action<int> callback)
    {
        Hashtable myJsonData = new Hashtable();
        myJsonData["id"] = 83;
        myJsonData["jsonrpc"] = "2.0";
        myJsonData["method"] = "eth_blockNumber";

        ArrayList arrayList = new ArrayList();
        myJsonData["params"] = arrayList;
        string rpcRequestJson = Json.jsonEncode(myJsonData);

        bool isStop = true;
        int firstBlockNumber = 0;
        if (PlayerPrefs.HasKey(address.ToLower() + hash.ToLower() + "BlockNumber"))
        {
            firstBlockNumber = PlayerPrefs.GetInt(address.ToLower() + hash.ToLower() + "BlockNumber");
        }
        WaitForSeconds wfs = new WaitForSeconds(0.8F);

        while (isStop)
        {
            UnityWebRequest unityRequest = GetUnityWebRequest(rpcRequestJson);
            yield return unityRequest.SendWebRequest();
            if (unityRequest.error != null)
            {
                PopupLine.Instance.Show("当前网络不可用，请检查网络配置");
            }
            else
            {
                string r = unityRequest.downloadHandler.text;
                Hashtable h = Json.jsonDecode(r) as Hashtable;
                string result = h["result"].ToString();
                int bn = Convert.ToInt32(result, 16);

                if (firstBlockNumber == 0)
                {
                    firstBlockNumber = bn;
                    PlayerPrefs.SetInt(address.ToLower() + hash.ToLower() + "BlockNumber", bn);
                }
                else
                {
                    if (bn - firstBlockNumber >= RecordPrefab.ethpackageHeight)
                    {
                        isStop = false;
                        PlayerPrefs.SetInt(address.ToLower() + hash.ToLower() + "Over", bn);
                        PlayerPrefs.DeleteKey(address.ToLower() + hash.ToLower() + "Packing");
                        PlayerPrefs.DeleteKey(address.ToLower() + hash.ToLower() + "BlockNumber");
                    }
                }
                if (callback != null)
                    callback(bn - firstBlockNumber);

            }
            yield return wfs;
        }
    }

    private UnityWebRequest GetUnityWebRequest(string jsonString)
    {
        byte[] requestBytes = Encoding.UTF8.GetBytes(jsonString);
        UnityWebRequest unityRequest = new UnityWebRequest(ETHInfo._url, "POST");
        UploadHandlerRaw uploadHandler = new UploadHandlerRaw(requestBytes);
        unityRequest.SetRequestHeader("Content-Type", "application/json");
        uploadHandler.contentType = "application/json";
        unityRequest.uploadHandler = uploadHandler;
        unityRequest.downloadHandler = new DownloadHandlerBuffer();
        return unityRequest;
    }
}


