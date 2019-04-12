using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NBitcoin;
using System;
using UnityEngine.Networking;

public class BTCGetHistory : MonoBehaviour {

    private static BTCGetHistory instance;
    public static BTCGetHistory Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject o = new GameObject();
                o.name = "EthHistory";
                instance = o.AddComponent<BTCGetHistory>();
            }
            return instance;
        }
    }

    /// <summary>
    /// 获取ETH交易记录
    /// </summary>
    /// <param name="地址"></param>
    /// <param name="获取成功回调"></param>
    /// <param name="失败回调"></param>
    /// <param name="page"></param>
    /// <param name="offset"></param>
    public void GetBTCHistory(string address, System.Action<List<ETHHistoryRcord>> onFinished, System.Action<string> onFailed, int page = 1, int offset = 50)
    {
        string url = "https://chain.api.btc.com/v3/address/"+ address + "/tx?" + "page=" + page + "&pagesize=" + offset;
        StartCoroutine(Request(onFinished, onFailed, url, address));
    }
    
    public void GetUSDTHistory(string address, System.Action<List<ETHHistoryRcord>> onFinished, System.Action<string> onFailed, int page = 0, int offset = 50)
    {
        Debug.Log("GetUSDTHistory");
        string url = HttpManager.url_usdt_nodeJs+"select-history?address=" + address + "&page="+ page;
        Debug.Log("GetUSDTHistoryurl:" + url);
        StartCoroutine(USDTRequest(onFinished, onFailed, url, address));
    }

    IEnumerator Request(System.Action<List<ETHHistoryRcord>> onFinished, System.Action<string> onFailed, string url, string address)
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.timeout = 10;
        yield return www.SendWebRequest();
        if (!www.isNetworkError)
        {
            if (string.IsNullOrEmpty(www.error))
            {
                //try
                //{
                string dataText = www.downloadHandler.text;
                bool rr = LoomRefresher.RunAsync(() =>
                {
                    Hashtable data = Json.jsonDecode(dataText) as Hashtable;
                    if (data == null)
                    {
                        //Debug.Log(www.downloadHandler.text);
                        //PopupLine.Instance.Show("网络错误，请检查网络配置");
                        if (onFailed != null)
                        {
                            onFailed("");
                        }
                    }
                    else if (data.ContainsKey("err_no") && "0" != data["err_no"].ToString())
                    {
                        if (data["data"] == null)
                        {
                            LoomRefresher.QueueOnMainThread(() =>
                            {
                                if (onFinished != null)
                                {
                                    onFinished(new List<ETHHistoryRcord>());
                                }
                            });
                        }
                        else
                        {
                            if (onFailed != null)
                            {
                                onFailed("");
                            }
                        }
                    }
                    else
                    {
                        data = data["data"] as Hashtable;
                        ArrayList list = data["list"] as ArrayList;
                        List<ETHHistoryRcord> result = new List<ETHHistoryRcord>();
                        foreach (var v in list)
                        {
                            Hashtable item = v as Hashtable;
                            string hsah = item["hash"].ToString();
                            Money fee = new Money(long.Parse(item["fee"].ToString()));
                            string gas = (fee.ToDecimal(MoneyUnit.BTC)).ToString("#0.########");
                            var startTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                            long time = long.Parse(item["created_at"].ToString());
                            DateTime dt = startTime.AddSeconds(time).ToLocalTime();
                            string timeStamp = dt.ToString("yyyy-MM-dd HH:mm:ss");
                            string confirmations = item["confirmations"].ToString();
                            if (confirmations == "0")
                            {
                                continue;
                            }
                            string blockNumber = item["block_height"].ToString();

                            Dictionary<string, long> otherInputs = new Dictionary<string, long>();
                            Dictionary<string, long> otherOutputs = new Dictionary<string, long>();
                            long mineInputs = 0;
                            long mineOutputs = 0;
                            ArrayList inputs = item["inputs"] as ArrayList;
                            foreach (var i in inputs)
                            {
                                Hashtable inp = i as Hashtable;
                                ArrayList addr = inp["prev_addresses"] as ArrayList;
                                if (addr.Count <= 0)
                                {
                                    continue;
                                }
                                string add = addr[0].ToString();
                                long value = long.Parse(inp["prev_value"].ToString());
                                if (add == address)
                                {
                                    mineInputs += value;
                                }
                                else
                                {
                                    if (otherInputs.ContainsKey(add))
                                    {
                                        otherInputs[add] += value;
                                    }
                                    else
                                    {
                                        otherInputs.Add(add, value);
                                    }
                                }
                            }
                            ArrayList outputs = item["outputs"] as ArrayList;
                            foreach (var i in outputs)
                            {
                                Hashtable inp = i as Hashtable;
                                ArrayList addr = inp["addresses"] as ArrayList;
                                if (addr.Count <= 0)
                                {
                                    continue;
                                }
                                string add = addr[0].ToString();
                                long value = long.Parse(inp["value"].ToString());
                                if (add == address)
                                {
                                    mineOutputs += value;
                                }
                                else
                                {
                                    if (otherOutputs.ContainsKey(add))
                                    {
                                        otherOutputs[add] += value;
                                    }
                                    else
                                    {
                                        otherOutputs.Add(add, value);
                                    }
                                }
                            }
                            string from = "";
                            string to = "";
                            long mineValueLong = 0;
                            if (mineInputs > mineOutputs)
                            {
                                from = address;
                                mineValueLong = mineInputs - mineOutputs;
                                if (otherOutputs.Count > 0)
                                {
                                    foreach (var ss in otherOutputs.Keys)
                                    {
                                        to = ss;
                                    }
                                }
                                else
                                {
                                    to = address;
                                }
                            }
                            else
                            {
                                to = address;
                                mineValueLong = mineOutputs - mineInputs;
                                if (otherInputs.Count > 0)
                                {
                                    foreach (var ss in otherInputs.Keys)
                                    {
                                        from = ss;
                                    }
                                }
                                else
                                {
                                    from = address;
                                }
                            }
                            string mineValue = (new Money(mineValueLong)).ToDecimal(MoneyUnit.BTC).ToString("#0.########");
                            ETHHistoryRcord re = new ETHHistoryRcord();
                            re.gas = gas;
                            re.from = from;
                            re.to = to;
                            re.hash = hsah;
                            re.timeStamp = timeStamp;
                            re.value = mineValue;
                            re.confixmations = confirmations;
                            re.blockNumber = blockNumber;
                            result.Add(re);
                        }
                        LoomRefresher.QueueOnMainThread(() =>
                        {
                            if (onFinished != null)
                            {
                                onFinished(result);
                            }
                        });
                    }
                });
                if (rr == false)
                {
                    if (onFailed != null)
                    {
                        onFailed("");
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
        }
        else
        {
            //PopupLine.Instance.Show("网络错误，请检查网络配置");
            if (onFailed != null)
            {
                onFailed("");
            }
        }
    }
    IEnumerator USDTRequest(System.Action<List<ETHHistoryRcord>> onFinished, System.Action<string> onFailed, string url, string address)
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.timeout = 10;
        yield return www.SendWebRequest();
        if (!www.isNetworkError)
        {
            if (string.IsNullOrEmpty(www.error))
            {
                string dataText = www.downloadHandler.text;
                Debug.Log("111222:USDT:" + dataText);
                
                    Hashtable data = Json.jsonDecode(dataText) as Hashtable;
                Debug.Log("111222:result:" + data["result"].ToString());
                bool bl =bool.Parse(data["result"].ToString());
                if (data == null)
                    {
                        //Debug.Log(www.downloadHandler.text);
                        //PopupLine.Instance.Show("网络错误，请检查网络配置");
                        if (onFailed != null)
                        {
                            onFailed("");
                        }
                    }
                    else if (!bl)
                    {
                        LoomRefresher.QueueOnMainThread(() =>
                        {
                            if (onFinished != null)
                            {
                                onFinished(new List<ETHHistoryRcord>());
                            }
                        });
                    }
                    else
                    {
                        ArrayList transactions = data["transactions"] as ArrayList;
                        List<ETHHistoryRcord> result = new List<ETHHistoryRcord>();
                        foreach (var v in transactions)
                        {
                            Hashtable transaction = v as Hashtable;
                            string amount = transaction["amount"].ToString();
                        string confirmations = transaction["confirmations"].ToString();
                        if (confirmations == "0")
                        {
                            continue;
                        }
                        string block = null;
                        string blockhash =null;

                        if (transaction.ContainsKey("block"))
                        {
                            block = transaction["block"].ToString();
                        }
                        if (transaction.ContainsKey("blockhash"))
                        {
                            blockhash = transaction["blockhash"].ToString();
                        }
                        
                            string blocktime =transaction["blocktime"].ToString();
                            string fee = transaction["fee"].ToString();
                            string referenceaddress = transaction["referenceaddress"].ToString();
                            string sendingaddress = transaction["sendingaddress"].ToString();
                            string txid = transaction["txid"].ToString();
                            var startTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                            long time = long.Parse(blocktime);
                            DateTime dt = startTime.AddSeconds(time).ToLocalTime();
                            string timeStamp = dt.ToString("yyyy-MM-dd HH:mm:ss");

                            ETHHistoryRcord re = new ETHHistoryRcord();
                            re.gas = fee;
                            re.from = sendingaddress;
                            re.to = referenceaddress;
                            re.hash = txid;
                            re.timeStamp = timeStamp;
                            re.value = amount;
                            re.confixmations = confirmations;
                            re.blockNumber = block;
                            result.Add(re);
                        }
                        LoomRefresher.QueueOnMainThread(() =>
                        {
                            if (onFinished != null)
                            {
                                onFinished(result);
                            }
                        });
                    }
            }
            else
            {
                Debug.Log("网络错误，请检查网络配置");
                if (onFailed != null)
                {
                    onFailed("");
                }
            }
        }
        else
        {
            Debug.Log("网络错误，请检查网络配置");
            if (onFailed != null)
            {
                onFailed("");
            }
        }
    }
}
