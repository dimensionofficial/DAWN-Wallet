using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EthHistory : MonoBehaviour {

    private static EthHistory instance;
    public static EthHistory Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject o = new GameObject();
                o.name = "EthHistory";
                instance = o.AddComponent<EthHistory>(); 
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
    public void GetEthHistory(string address, int startblock, System.Action<string> onFinished, System.Action<string> onFailed, int page = 1, int offset = 10)
    {
        string url = "https://api.etherscan.io/api?module=account&action=txlist&address="+
            address + "&startblock=" + startblock + "&endblock=99999999&page=" + page +"&offset="+ offset + "&sort=asc"; 
        StartCoroutine(Request(onFinished, onFailed, url));
    }

    public void GetEthErc20History(string address, int startblock, System.Action<string> onFinished, System.Action<string> onFailed, int page = 1, int offset = 10)
    {
        string url = "http://api.etherscan.io/api?module=account&action=tokentx&address=" +
            address + "&startblock=" + startblock +"&endblock=99999999&page=" + page + "&offset=" + offset + "&sort=asc";
        StartCoroutine(Request(onFinished, onFailed, url, true));
    }

    IEnumerator Request(System.Action<string> onFinished, System.Action<string> onFailed, string url, bool isDebug = false)
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.timeout = 10;
        yield return www.SendWebRequest();
        if (!www.isNetworkError)
        {
            if (string.IsNullOrEmpty(www.error))
            {
                //if (isDebug)
                //{
                //    Debug.Log(www.downloadHandler.text);
                //}
                //try
                //{
                //    Hashtable data = Json.jsonDecode(www.downloadHandler.text) as Hashtable;

                if (onFinished != null)
                    {
                        onFinished(www.downloadHandler.text);
                    }
                //}
                //catch
                //{
                //    PopupLine.Instance.Show("网络错误，请检查网络配置");
                //    if (onFailed != null)
                //    {
                //        onFailed("");
                //    }
                //}
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
}
