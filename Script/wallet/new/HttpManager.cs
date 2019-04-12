using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System;
using Newtonsoft.Json;
using System.Text;
using UnityEngine.Networking;
using System.Security.Cryptography;
public class HttpManager : MonoBehaviour
{
    
    /// <summary>
    /// 钱包服务器地址
    /// </summary>
    public string url_walletServer = "http://47.96.131.169:8090/walletInterfacenew.php";
    private string m_ttt_ustt = "3213213213";

    /// <summary>
    /// 二哥那边nodejs 服务器地址
    /// </summary>
    public string url_nodeJs = "https://vedio3.clawclaw.cn:8888/";
    private string m_node_tt = "3213213123123";

    /// <summary>
    /// 老段那边 eos node js 服务器地址
    /// </summary>
    public string url_eos_nodeJs = "http://47.96.131.169:3000/";

    /// <summary>
    /// rjj那边 usdt  
    /// </summary>
    //public static string url_usdt_nodeJs = "http://192.168.1.113:9002/usdt/";
    public static string url_usdt_nodeJs = "http://47.96.145.254:9002/usdt/";
    public static string LoginMark = "221321312";
    public string[] loginInfo;

    public GameObject loadingPanel;
    public static HttpManager _Intance;

    public decimal current_usdt;
    private decimal btc_usdt;
    private decimal eth_usdt;

    public string regId;


    public float btcRMB;
    private decimal m_btcRMB;
    public decimal btc_RMB
    {
        get { return m_btcRMB * current_usdt; }
        set
        {
            m_btcRMB = value;
            btcRMB = (float)value;
        }
    }

    private decimal m_eosRMB = 5;
    public decimal eos_RMB
    {
        get
        {
            return m_eosRMB * current_usdt;
        }
        set
        {
            m_eosRMB = value;
        }
    }

    private decimal m_ethRMB;
    public decimal eth_RMB
    {
        get
        {
            return m_ethRMB * current_usdt;
        }
        set
        {
            m_ethRMB = value;
        }
    }

    /// <summary>
    /// 比特币最低矿工费价格
    /// </summary>
    public int minBTCGasPrice;
    /// <summary>
    /// 比特币矿工费滑动条百分比
    /// </summary>
    public float btcGaspercentage;

    /// <summary>
    /// 以太坊最低矿工费
    /// </summary>
    public decimal ethMinFee;
    /// <summary>
    /// 以太坊最高矿工费
    /// </summary>
    public decimal ethMaxFee;
    /// <summary>
    /// 以太坊代币最低矿工费
    /// </summary>
    public decimal ethTokenMinFee;
    /// <summary>
    /// 以太坊代币最高矿工费
    /// </summary>
    public decimal ethTokenMaxFee;
    /// <summary>
    /// 以太坊矿工费价格
    /// </summary>
    public Int64 ethGasPrice_int64;

    public decimal kyberGas_Limit;
    public decimal ethGas_Limit;
    public decimal ethTokenGas_Limit;

    void Awake()
    {
        _Intance = this;
        regId = PlayerPrefs.GetString("regId");
    }

    void Start()
    {
        loginInfo = PlayerPrefsX.GetStringArray(LoginMark);
        StartRequestHttp();
    }


    public static string HmacSHA256(string message, string secret)
    {
        secret = secret ?? "";
        var encoding = new System.Text.UTF8Encoding();
        byte[] keyByte = encoding.GetBytes(secret);
        byte[] messageBytes = encoding.GetBytes(message);
        using (var hmacsha256 = new HMACSHA256(keyByte))
        {
            byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
            return Convert.ToBase64String(hashmessage);
        }
    }

    /// <summary>
    /// 向数据库提交ERC20智能合约地址
    /// </summary>
    /// <param name="contraAddress"></param>
    /// <param name="symbol"></param>
    public void UpLoadContractAddress(string contraAddress, string symbol, string fullName, Action<Hashtable> Callback)
    {
        List<KeyValuePair<string, string>> ws = new List<KeyValuePair<string, string>>();
        ws.Add(new KeyValuePair<string, string>("op", "addtokencontract"));
        ws.Add(new KeyValuePair<string, string>("contractAddress", contraAddress));

        if (string.IsNullOrEmpty(symbol))
            symbol = " ";
        if (string.IsNullOrEmpty(fullName))
            fullName = " ";

        ws.Add(new KeyValuePair<string, string>("contractName", symbol));
		ws.Add(new KeyValuePair<string, string>("fullName", fullName));
        StartCoroutine(SendRequest(ws, Callback, null, false));
    }

    /// <summary>
    /// 请求网络数据
    /// </summary>
    public void StartRequestHttp()
    {
        CancelInvoke("UpdateHttpInfo");
        InvokeRepeating("UpdateHttpInfo", 0, 120);
    }

    /// <summary>
    /// 获取以太坊矿工费价格
    /// </summary>
    /// <returns></returns>
    private IEnumerator GetETHGasPrice()
    {
        Hashtable myJsonData = new Hashtable();
        myJsonData["id"] = 73; 
         myJsonData["jsonrpc"] = "2.0";
        myJsonData["method"] = "eth_gasPrice";
        ArrayList arrayList = new ArrayList();
        myJsonData["params"] = arrayList;
        string rpcRequestJson = Json.jsonEncode(myJsonData);
        UnityWebRequest unityRequest = QRPayTools.GetUnityWebRequest(rpcRequestJson);
        yield return unityRequest.SendWebRequest();
        if (unityRequest.error != null)
        {
            //PopupLine.Instance.Show("当前网络不可用，请检查网络配置");
            Debug.Log(unityRequest.error + " : " + rpcRequestJson);
        }
        else
        {
            byte[] results = unityRequest.downloadHandler.data;
            string responseJson = Encoding.UTF8.GetString(results).ToString();
            Hashtable table = Json.jsonDecode(responseJson) as Hashtable;
            if (table.Contains("error"))
            {
     //           string str1 = Json.jsonEncode(table["error"]);
            }
            else
            {
                string resultValue = table["result"].ToString();
                ethGasPrice_int64 = Convert.ToInt64(resultValue, 16);
            }
        }
    }
   

    private void UpdateHttpInfo()
    {
        StartCoroutine(GetSourcePrice());
    }

    private IEnumerator GetSourcePrice()
    {

        GetconstantValue();
        yield return new WaitForSeconds(0.5F);
        //GetBTCPrice();
        //GetETHPrice();
        //GetEOSPrice();
        StartCoroutine(GetETHGasPrice());
        yield return new WaitForSeconds(0.5F);
        GetBtcGasPrice();
    }

    private void GetconstantValue()
    {
        List<KeyValuePair<string, string>> ws = new List<KeyValuePair<string, string>>();
        ws.Add(new KeyValuePair<string, string>("op", "GetconstantValue"));
        StartCoroutine(HttpManager._Intance.SendRequest(ws, delegate (Hashtable hashtable)
        {
            ethMinFee = decimal.Parse(hashtable["ethMinFee"].ToString());
            ethMaxFee = decimal.Parse(hashtable["ethMaxFee"].ToString());
            ethTokenMinFee = decimal.Parse(hashtable["ethTokeMinFee"].ToString());
            ethTokenMaxFee = decimal.Parse(hashtable["ethTokeMaxFee"].ToString());
            current_usdt = decimal.Parse(hashtable["usdtormb"].ToString());

            kyberGas_Limit = decimal.Parse(hashtable["kyberGas"].ToString());
            ethGas_Limit = decimal.Parse(hashtable["ethGasLimit"].ToString());
            ethTokenGas_Limit = decimal.Parse(hashtable["ethTokenGasLimit"].ToString());

            ArrayList list = (ArrayList)hashtable["coinPrice"];
            for (int i = 0; i < list.Count; i++)
            {

                Hashtable temp = list[i] as Hashtable;
                decimal price = decimal.Parse(temp["coinPrice"].ToString());
                if (temp["coinName"].ToString().Equals("BTC"))
                {
                    btc_RMB = price;
                }
                else if (temp["coinName"].ToString().Equals("ETH"))
                {
                    eth_RMB = price;
                }
                else if (temp["coinName"].ToString().Equals("EOS"))
                {
                    eos_RMB = price;

                    if (eos_RMB == 0)
                        eos_RMB = 5;
                }
            }
        }));
    }


   /// <summary>
   /// 获取比特币矿工费价格
   /// </summary>
    private void GetBtcGasPrice()
    {
        QBitNinja4Unity.QBitNinjaClient.GetFee(NBitcoin.Network.Main, GetFee);
    }

    void GetFee(QBitNinja4Unity.Models.Fees result)
    {
        minBTCGasPrice = result.hourFee;
        int temp = result.fastestFee - minBTCGasPrice;
        if (temp == 0)
        {
            temp = result.hourFee / 2;
        }
        btcGaspercentage = temp;
    }

    public IEnumerator EOSSendRequest(string op, WWWForm ws, bool isPost = true, Action<Hashtable> SuccessCallback = null, Action Failure = null, bool isAtiveLoding = true)
    {
        if (isAtiveLoding)
            loadingPanel.gameObject.SetActive(true);

        string url = url_eos_nodeJs + op;
        UnityWebRequest www;
        if (isPost)
        {
            www = UnityWebRequest.Post(url, ws);
        }
        else
        {
            www = UnityWebRequest.Get(url);
        }
       
        www.chunkedTransfer = true;
        www.timeout = 10;
        yield return www.SendWebRequest();
        if (!www.isNetworkError)
        {
            yield return www;
            if (www.error != null)
            {
                Debug.LogError(www.error);
                PopupLine.Instance.Show("当前网络不可用，请检查网络配置");
                if (Failure != null)
                    Failure();
            }
            else
            {
                string jsonString = GetUTF8String(www.downloadHandler.data);
                Hashtable table = Json.jsonDecode(jsonString) as Hashtable;
                if (table != null && table.ContainsKey("error"))
                {
                    Debug.Log("Eos 网络错误：" + table["error"]);
                }
                if (SuccessCallback != null)
                {
                    SuccessCallback(table);
                }
            }
            loadingPanel.gameObject.SetActive(false);
        }
        else
        {
    //        PopupLine.Instance.Show("当前网络不可用，请检查网络配置");
            loadingPanel.gameObject.SetActive(false);
            if (Failure != null)
                Failure();
        }

        yield return 0;
    }

    #region
    public IEnumerator GetNodeJsRequest(string op, List<KeyValuePair<string, string>> parms, System.Action<Hashtable> callBack)
	{
        //http://47.96.131.169:8888/getgoodslist?sign=bb432b049e9fa65baad8ca6cb8ff6fac
        //http://47.96.131.169:8888/createorder?userid=26&address=1234567&hash=7893&goodid=1&sign=1dc86eb78e84ed92c9d24f53a2f16a4f;

        string url = url_nodeJs + op + "?";
        string singHx = "";

        for (int i = 0; i < parms.Count; i++)
        {
            url += parms[i].Key + "=" + parms[i].Value + "&";
            singHx += parms[i].Value;
        }

        string signInfo = EncryptWithMD5(singHx + m_node_tt);
        url += "sign=" + signInfo;

		UnityWebRequest www = UnityWebRequest.Get(url);
		www.timeout = 10;

		yield return www.SendWebRequest();

		if (!www.isNetworkError) {
			if (www.error != null)
			{
                if (callBack != null) {
					callBack(null);
				}
			}
			else
			{
				try
				{
					string jsonString = GetUTF8String(www.downloadHandler.data);
					Hashtable data = Json.jsonDecode (jsonString) as Hashtable;
                    string result = data["result"].ToString();

                    if (callBack != null)
                    {
                        if (result.StartsWith("Su"))
                        {
                            callBack(data);
                        }
                        else
                        {
                            callBack(null);
                        }      
                    }
				}
				catch {
					if (callBack != null) {
						callBack (null);
					}
				}
			}
		} else {
            if (callBack != null) {
				callBack (null);
			}
		}
	}
    #endregion
    private string EncryptWithMD5(string source)
    {
        byte[] sor = Encoding.UTF8.GetBytes(source);
        MD5 md5 = MD5.Create();
        byte[] result = md5.ComputeHash(sor);
        StringBuilder strbul = new StringBuilder(40);
        for (int i = 0; i < result.Length; i++)
        {
            strbul.Append(result[i].ToString("x2"));

        }
        return strbul.ToString().ToLower();
    }
    /// <summary>
    /// 请求服务器接口
    /// </summary>
    /// <param name="form"></param>
    /// <param name="Callback"></param>
    /// <param name="isAtiveLoding"></param>
    /// <returns></returns>
    public IEnumerator SendRequest(List<KeyValuePair<string, string>> form, Action<Hashtable> Callback = null, Action failCallBack = null, bool isAtiveLoding = true)
    {
        if (isAtiveLoding)
            loadingPanel.gameObject.SetActive(true);
        string url = url_walletServer;
        WWWForm ws = new WWWForm();
        string data = "";
        for (int i = 0; i < form.Count; i++)
        {
            data += form[i].Value;
            ws.AddField(form[i].Key, form[i].Value);
        }

        string sign = EncryptWithMD5(data + m_ttt_ustt);
        ws.AddField("sign", sign);
        UnityWebRequest www = UnityWebRequest.Post(url, ws);
        www.chunkedTransfer = true;
        www.timeout = 10;
        yield return www.SendWebRequest();
        if (!www.isNetworkError)
        {
            yield return www;
            if (www.error != null)
            {
                if (failCallBack != null)
                {
                    failCallBack();
                }
                Debug.LogError(www.error);
                PopupLine.Instance.Show("当前网络不可用，请检查网络配置");
            }
            else
            {
                //Debug.Log(www.downloadHandler.data.Length);
                //Debug.Log(form[0].Value);
                string jsonString = GetUTF8String(www.downloadHandler.data);
                if (Callback != null)
                {
                    Hashtable table = Json.jsonDecode(jsonString) as Hashtable;
                    Callback(table);
                }
            }
            loadingPanel.gameObject.SetActive(false);
        }
        else
        {
            if (failCallBack != null)
            {
                failCallBack();
            }
            PopupLine.Instance.Show("当前网络不可用，请检查网络配置");
            loadingPanel.gameObject.SetActive(false);
        }
        yield return 0;
    }

   
    public string UnicodeToString(string source)
    {
        string outStr = "";
        if (!string.IsNullOrEmpty(source))
        {
            string[] strlist = source.Replace("//", "").Split('u');
            try
            {
                for (int i = 1; i < strlist.Length; i++)
                {
                    //将unicode字符转为10进制整数，然后转为char中文字符  
                    outStr += (char)int.Parse(strlist[i], System.Globalization.NumberStyles.HexNumber);
                }
            }
            catch (FormatException ex)
            {
                outStr = ex.Message;
            }
        }
        return outStr;
    }

    private string GetUTF8String(byte[] buffer)
    {
        if (buffer == null)
            return null;

        if (buffer.Length <= 3)
        {
            return Encoding.UTF8.GetString(buffer);
        }

        byte[] bomBuffer = new byte[] { 0xef, 0xbb, 0xbf };

        if (buffer[0] == bomBuffer[0]
            && buffer[1] == bomBuffer[1]
            && buffer[2] == bomBuffer[2])
        {
            return new UTF8Encoding(false).GetString(buffer, 3, buffer.Length - 3);
        }

        return Encoding.UTF8.GetString(buffer);
    }

    public IEnumerator GetBTCUnspentBlance(string btcAddress, Action<ArrayList> callBack)
    {
        HttpManager._Intance.loadingPanel.SetActive(true);
        string url = "https://chain.api.btc.com/v3/address/" + btcAddress + "/unspent";
        
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.timeout = 10;
        yield return www.SendWebRequest();
        if (!www.isNetworkError)
        {
            if (www.error != null)
            {
                PopupLine.Instance.Show("网络错误,请检查网络配置");
                callBack(null);
            }
            else
            {
                try
                {
                    string jsonString = GetUTF8String(www.downloadHandler.data);
                    Debug.Log(jsonString);
                    Hashtable reslut = Json.jsonDecode(jsonString) as Hashtable;
                    Hashtable data = reslut["data"] as Hashtable;

                    ArrayList unspentlist = data["list"] as ArrayList;

                    if (callBack != null)
                    {
                        callBack(unspentlist);
                    }
                }
                catch
                {
                    PopupLine.Instance.Show("网络错误,请检查网络配置");
                    callBack(null);
                }
            }
        }
        else
        {
            PopupLine.Instance.Show("网络错误,请检查网络配置");
            callBack(null);
        }
        HttpManager._Intance.loadingPanel.SetActive(false);
    }
    public IEnumerator GetUSDTUnspentBlance(string btcAddress, Action<ArrayList> callBack)
    {
        HttpManager._Intance.loadingPanel.SetActive(true);
        string url = url_usdt_nodeJs+ "/select-balance?address=" + btcAddress;

        UnityWebRequest www = UnityWebRequest.Get(url);
        www.timeout = 10;
        yield return www.SendWebRequest();
        if (!www.isNetworkError)
        {
            if (www.error != null)
            {
                PopupLine.Instance.Show("网络错误,请检查网络配置");
                callBack(null);
            }
            else
            {
                try
                {
                    string jsonString = GetUTF8String(www.downloadHandler.data);
                    Debug.Log(jsonString);
                    Hashtable reslut = Json.jsonDecode(jsonString) as Hashtable;
                    Hashtable data = reslut["data"] as Hashtable;

                    ArrayList unspentlist = data["list"] as ArrayList;

                    if (callBack != null)
                    {
                        callBack(unspentlist);
                    }
                }
                catch
                {
                    PopupLine.Instance.Show("网络错误,请检查网络配置");
                    callBack(null);
                }
            }
        }
        else
        {
            PopupLine.Instance.Show("网络错误,请检查网络配置");
            callBack(null);
        }
        HttpManager._Intance.loadingPanel.SetActive(false);
    }
    ///// <summary>
    ///// 获取比特币价格
    ///// </summary>
    //private void GetBTCPrice()
    //{
    //    List<KeyValuePair<string, string>> ws = new List<KeyValuePair<string, string>>();
    //    ws.Add(new KeyValuePair<string, string>("op", "getServerPrice"));
    //    ws.Add(new KeyValuePair<string, string>("simpleName", "BTC"));
    //    StartCoroutine(HttpManager._Intance.SendRequest(ws, delegate (Hashtable table)
    //    {
    //        btc_RMB = decimal.Parse(table["price"].ToString());
    //        EventManager.Instance.SendEvent(EventID.UpdateBTCbalance);
    //    }));
    //}
    /// <summary>
    /// 获取EOS价格
    /// </summary>
    //private void GetEOSPrice()
    //{
    //    List<KeyValuePair<string, string>> ws = new List<KeyValuePair<string, string>>();
    //    ws.Add(new KeyValuePair<string, string>("op", "getServerPrice"));
    //    ws.Add(new KeyValuePair<string, string>("simpleName", "EOS"));
    //    StartCoroutine(HttpManager._Intance.SendRequest(ws, delegate (Hashtable table)
    //    {
    //        eos_RMB = decimal.Parse(table["price"].ToString());

    //        if (eos_RMB == 0)
    //            eos_RMB = 5;

    //        EventManager.Instance.SendEvent(EventID.UpdateEOSBalance);
    //    }));
    //}

    /// <summary>
    /// 获取以太坊价格
    /// </summary>
    //private void GetETHPrice()
    //{
    //    List<KeyValuePair<string, string>> ws = new List<KeyValuePair<string, string>>();
    //    ws.Add(new KeyValuePair<string, string>("op", "getServerPrice"));
    //    ws.Add(new KeyValuePair<string, string>("simpleName", "ETH"));
    //    StartCoroutine(HttpManager._Intance.SendRequest(ws, delegate (Hashtable table)
    //    {
    //        eth_RMB = decimal.Parse(table["price"].ToString());
    //        EventManager.Instance.SendEvent(EventID.UpdateETHbalance);
    //    }));
    //}
}
