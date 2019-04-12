using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class ExchangeRate : MonoBehaviour
{


    private float current_usdt;
    private float btc_usdt;
    private float eth_usdt;

    private float m_btcRMB;
    public float btc_RMB
    {
        get { return m_btcRMB; }
        set
        {
            m_btcRMB = value;
        }
    }

    private float m_ethRMB;
    public float eth_RMB
    {
        get { return m_ethRMB; }
        set
        {
            m_ethRMB = value;
        }
    }

    void Start()
    {
        GetRate();
    }

    public void GetRate()
    {
        StartCoroutine(Get_Usdt());
        StartCoroutine(Get_Ticker());
    }

    private IEnumerator Get_Usdt()
    {
        UnityWebRequest unityRequest = new UnityWebRequest("http://data.gateio.io/api2/1/ticker/usdt_cny");
        unityRequest.SetRequestHeader("Content-Type", "application/json");
        unityRequest.downloadHandler = new DownloadHandlerBuffer();
        yield return unityRequest.SendWebRequest();
        if (unityRequest.error != null)
        {
            Debug.Log(unityRequest.error);
        }
        else
        {
            byte[] results = unityRequest.downloadHandler.data;
            string responseJson = Encoding.UTF8.GetString(results).ToString();
            Hashtable table = Json.jsonDecode(responseJson) as Hashtable;
            string str = table["last"].ToString();
            current_usdt = float.Parse(str);
            eth_RMB = eth_usdt * current_usdt;
            btc_RMB = btc_usdt * current_usdt;
        }
    }

    private IEnumerator Get_Ticker()
    {
        UnityWebRequest unityRequest = new UnityWebRequest("http://data.gateio.io/api2/1/tickers");
        unityRequest.SetRequestHeader("Content-Type", "application/json");
        unityRequest.downloadHandler = new DownloadHandlerBuffer();
        yield return unityRequest.SendWebRequest();
        if (unityRequest.error != null)
        {
            Debug.Log(unityRequest.error);
        }
        else
        {
            byte[] results = unityRequest.downloadHandler.data;
            string responseJson = Encoding.UTF8.GetString(results).ToString();
            Hashtable table = Json.jsonDecode(responseJson) as Hashtable;

            string ethJsonStr = Json.jsonEncode(table["eth_usdt"]);
            string btcJsonStr = Json.jsonEncode(table["btc_usdt"]);
            Hashtable ethTable = Json.jsonDecode(ethJsonStr) as Hashtable;
            Hashtable btcTable = Json.jsonDecode(btcJsonStr) as Hashtable;

            eth_usdt = float.Parse(ethTable["last"].ToString());
            btc_usdt = float.Parse(btcTable["last"].ToString());
            eth_RMB = eth_usdt * current_usdt;
            btc_RMB = btc_usdt * current_usdt;
        }
    }
}
