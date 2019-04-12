using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Nethereum.Util;
using NBitcoin;
using UnityEngine.Networking;


public class HistoryManager : MonoBehaviour {
    private static HistoryManager instance;
    public static HistoryManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject o = new GameObject();
                o.name = "HistoryManager";
                instance = o.AddComponent<HistoryManager>();
                temporaryCachePath = Application.temporaryCachePath;
            }
            return instance;
        }
    }
    static string temporaryCachePath;
    public void GetBTCBalance(string address, System.Action<decimal> onFinished)
    {
        string url = "https://chain.api.btc.com/v3/address/" + address;
        StartCoroutine(CommRequest((s) => {
            Hashtable source = Json.jsonDecode(s) as Hashtable;
            if (source == null || !source.ContainsKey("data"))
            {
                //PopupLine.Instance.Show("网络错误，请检查网络配置");
                return;
            }

            Hashtable data = source["data"] as Hashtable;
            long l;
            
            if (data != null)
            {
                l = long.Parse(data["balance"].ToString());
            }
            else
            {
                l = 0;
            }
            Money m = new Money(l);
            decimal result = m.ToDecimal(MoneyUnit.BTC);
            if (onFinished != null)
            {
                onFinished(result);
            }
        }, null, url));
    }
    public void GetUSDTBalance(string address, System.Action<decimal> onFinished)
    {
        string url = HttpManager.url_usdt_nodeJs + "select-balance?address=" + address;
        StartCoroutine(CommRequest((s) => {
            Hashtable source = Json.jsonDecode(s) as Hashtable;
            if (source == null)
            {
                //PopupLine.Instance.Show("网络错误，请检查网络配置");
                return;
            }
            Debug.Log(source.ToString());
            bool bl = bool.Parse(source["result"].ToString());
            if (!bl)
            {
                Debug.Log(source["result"].ToString());
                return;
            }
            long l;
            Debug.Log(source["result"].ToString());
            if (source["balance"] != null)
            {
                l = long.Parse(source["balance"].ToString());
            }
            else
            {
                l = -1;
            }
            Money m = new Money(l);
            decimal result = m.ToDecimal(MoneyUnit.BTC);
            Debug.Log("result:" + result);
            if (onFinished != null)
            {
                Debug.Log("result:"+result);
                onFinished(result);
            }
        }, null, url));
    }


    IEnumerator CommRequest(System.Action<string> onFinished, System.Action<string> onFailed, string url)
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.timeout = 10;
        yield return www.SendWebRequest();
        if (!www.isNetworkError)
        {
            if (string.IsNullOrEmpty(www.error))
            {
                if (onFinished != null)
                {
                    onFinished(www.downloadHandler.text);
                }
            }
        }
        else
        {
            //PopupLine.Instance.Show("网络错误，请检查网络配置");
            if (onFailed != null)
            {
                onFailed("");
            }
        }
        yield return new WaitForSeconds(0.5f);
    }
    public void GetUSDTFee(string fromAddress, string toAddress, string amount, System.Action<List<InputVo>,bool,string> onFinished)
    {
        string url = HttpManager.url_usdt_nodeJs + "get-input?fromAddress=" + fromAddress + "&amount=" + amount + "&toAddress=" + toAddress;
        Debug.Log(url);
        StartCoroutine(CommRequest((s) =>
        {
            Debug.Log("1");
            InputResult result = JsonUtility.FromJson<InputResult>(s);
            if (result==null)
            {
                Debug.Log("网络错误，请检查网络配置");
                //PopupLine.Instance.Show("网络错误，请检查网络配置");
                return;
            }
            bool bl = bool.Parse(result.result);
            if (!bl)
            {
                Debug.Log("false");
                onFinished(null, false, result.message);
                return;
            }
            if (onFinished != null)
            {
                Debug.Log("onFinished != null");
                onFinished(result.inputs, true, null);
            }
        }, null, url));
    }
    /// <summary>
    /// test rjj
    /// get usdt input
    /// </summary>
    /// <param name="fromAddress"></param>
    /// <param name="amount"></param>
    /// <param name="onFinished"></param>
    //public void GetUSDTInput(string fromAddress, string toAddress, string amount, string fee, System.Action<List<InputVo>> onFinished,string pubkey)
    //{
    //    string url = JavaUrl + "make-input?fromAddress=" + fromAddress + "&amount=" + amount+"&pubKey="+ pubkey+"&toAddress="+ toAddress+"&fee="+fee;
    //    Debug.Log(url);
    //    StartCoroutine(CommRequest((s) => 
    //    {
    //        Debug.Log("1");
    //        InputResult result = JsonUtility.FromJson<InputResult>(s);
    //        if (null == result || null == result.result || null == result.inputs)
    //        {
    //            Debug.Log("2");
    //            //PopupLine.Instance.Show("网络错误，请检查网络配置");
    //            return;
    //        }
    //        if (result.result.Equals("false"))
    //        {
    //            Debug.Log("false");
    //            //onFinished1(result.message);
    //            return;
    //        }
    //        if (onFinished != null)
    //        {
    //            Debug.Log("onFinished != null");
    //            onFinished(result.inputs);
    //        }
    //    }, null, url));
    //}

    /// <summary>
    /// test rjj
    /// transfer usdt transaction
    /// </summary>
    /// <param name="fromAddress"></param>
    /// <param name="toAddress"></param>
    /// <param name="amount"></param>
    /// <param name="onFinished"></param>
    /// <param name="updatedInputs"></param>
    public void PostUSDTTx(string fromAddress, string toAddress, string amount, List<InputVo> updatedInputs, System.Action<string,bool> onFinished,string fee)
    {
        Debug.Log("PostUSDTTx");
        string url = HttpManager.url_usdt_nodeJs + "make-tx?fromAddress=" + fromAddress + "&toAddress=" + toAddress + "&amount=" + amount+"&fee="+fee;
        Debug.Log(url);
        StartCoroutine(PutRequest((s) => {
            TxResult result = JsonUtility.FromJson<TxResult>(s);
            Debug.Log(result+ "===result.result="+ result.result);
            if (null == result || null == result.result)
            {
                //PopupLine.Instance.Show("网络错误，请检查网络配置");
                onFinished("网络错误，请检查网络配置", false);
                return;
            }
            bool bl = bool.Parse(result.result);
            if (bl)
            {
                onFinished(result.txid, true);
            }
            else
            {
                onFinished(result.message, false);
            }
        }, url, updatedInputs));
    }
    public void PostUSDTInput(string fromAddress, string toAddress, string amount, List<InputVo> updatedInputs, System.Action<List<InputVo>,bool,string> onFinished, string fee)
    {
        Debug.Log("PostUSDTInput");
        string url = HttpManager.url_usdt_nodeJs + "make-input?fromAddress=" + fromAddress + "&toAddress=" + toAddress + "&amount=" + amount + "&fee=" + fee;
        Debug.Log(url);
        StartCoroutine(PutRequest1((s) => {
            InputResult result = JsonUtility.FromJson<InputResult>(s);
            //Debug.Log(result + "===result.result=" + result.result);
            if (null == result || null == result.result)
            {
                //PopupLine.Instance.Show("网络错误，请检查网络配置");
                return;
            }
            bool bl = bool.Parse(result.result);
            if (bl)
            {
                onFinished(result.inputs, true, null);
            }
            else
            {
                onFinished(null, false, result.message);
            }
        }, url, updatedInputs));
    }

    IEnumerator PutRequest(System.Action<string> onFinished, string url, List<InputVo> updatedInputs)
    {
        ArrayList list = new ArrayList();
        foreach (var s in updatedInputs)
        {
            Hashtable item = new Hashtable();
            item["txid"] = s.txid;
            item["value"] = s.value;
            item["index"] = s.index;
            item["scriptPubKey"] = s.scriptPubKey;
            item["signature"] = s.signature;
            item["pubKey"] = s.pubKey;
            list.Add(item);
        }
        string data = Json.jsonEncode(list);
        /*
        Debug.Log("PostRequest");
        Debug.Log("json:  ->>>" + Json.jsonEncode(updatedInputs));
        string data = "{[";
        updatedInputs.ForEach(
            (s) =>
            {
                data += "{\"txid:\"" + "\"s.txid + ",value:" + s.value + ",index:" + s.index + ",scriptPubKey:" + s.scriptPubKey + ",signature:" + s.signature
                    + "},";
            });
        data = data.Substring(0, data.Length - 1);
        data += "]}";
        */
        
        Debug.Log(data);
        UnityWebRequest www = UnityWebRequest.Put(url, System.Text.Encoding.Default.GetBytes(data));
        www.SetRequestHeader("Content-Type", "application/json;charset=UTF-8");
        
        www.timeout = 10;
        yield return www.SendWebRequest();
        Debug.Log("PostRequest2");
        if (!www.isNetworkError)
        {
            Debug.Log("!www.isNetworkError");
            if (string.IsNullOrEmpty(www.error))
            {
                Debug.Log("string.IsNullOrEmpty(www.error)");
                if (onFinished != null)
                {
                    Debug.Log("onFinished != null");
                    onFinished(www.downloadHandler.text);
                }
            }
        }
        else
        {
            //PopupLine.Instance.Show("网络错误，请检查网络配置");
            onFinished(null);
        }
        yield return new WaitForSeconds(0.5f);
    }
    IEnumerator PutRequest1(System.Action<string> onFinished, string url, List<InputVo> updatedInputs)
    {
        ArrayList list = new ArrayList();
        foreach (var s in updatedInputs)
        {
            Hashtable item = new Hashtable();
            item["txid"] = s.txid;
            item["index"] = s.index;
            item["pubKey"] = s.pubKey;
            list.Add(item);
        }
        string data = Json.jsonEncode(list);
        /*
        Debug.Log("PostRequest");
        Debug.Log("json:  ->>>" + Json.jsonEncode(updatedInputs));
        string data = "{[";
        updatedInputs.ForEach(
            (s) =>
            {
                data += "{\"txid:\"" + "\"s.txid + ",value:" + s.value + ",index:" + s.index + ",scriptPubKey:" + s.scriptPubKey + ",signature:" + s.signature
                    + "},";
            });
        data = data.Substring(0, data.Length - 1);
        data += "]}";
        */

        Debug.Log(data);
        UnityWebRequest www = UnityWebRequest.Put(url, System.Text.Encoding.Default.GetBytes(data));
        www.SetRequestHeader("Content-Type", "application/json;charset=UTF-8");

        www.timeout = 10;
        yield return www.SendWebRequest();
        Debug.Log("PostRequest2");
        if (!www.isNetworkError)
        {
            Debug.Log("!www.isNetworkError");
            if (string.IsNullOrEmpty(www.error))
            {
                Debug.Log("string.IsNullOrEmpty(www.error)");
                if (onFinished != null)
                {
                    Debug.Log("onFinished != null");
                    Debug.Log(www.downloadHandler.text);
                    onFinished(www.downloadHandler.text);
                }
            }
        }
        else
        {
            //PopupLine.Instance.Show("网络错误，请检查网络配置");
            onFinished(null);
        }
        yield return new WaitForSeconds(0.5f);
    }

    [System.Serializable]
    public class InputVo
    {
        public string txid;

        public int index;

        public long value;

        public string scriptPubKey;

        public string redeemScript;

        public string signature;

        public string pubKey;
    }

    [System.Serializable]
    public class InputResult
    {
        public string result;

        public List<InputVo> inputs;

        public string message;
    }

    [System.Serializable]
    public class TxResult
    {
        public string result;

        public string message;

        public string txid;
    }
}
