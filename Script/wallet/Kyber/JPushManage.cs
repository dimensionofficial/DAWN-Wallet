using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JPush;
using UnityEngine.UI;
using System;
using LitJson;
using UnityEngine.Networking;
using NBitcoin;
using System.Text;

public class JPushManage : MonoBehaviour {
    public Text idText;
    public Text noficationText;
    public static JPushManage instance;
    public MessageInfo messageInfo;
    public string testStr;
    public MainPanel mainPanel;
    public MessageDetail messageDetail;
    public TransactionPanel transactionPanel;
    public string testNotify;

    TimeSpan cha;
    long t;
    private System.DateTime startTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
    private void Awake()
    {
        instance = this;
        //PlayerPrefs.SetString("ReadedMessageID", "");
    }
    // Use this for initialization
    void Start () {
        StartCoroutine(RefreshNotifyInfoIE(RefreshNotifyMessage));
#if UNITY_EDITOR
        return;
#endif
        JPushBinding.Init(name);
        //string regId = JPushBinding.GetRegistrationId();
        //if(regId == "" || regId == null)
        //{
        //    idText.text = "错误";
        //}
        //else
        //{
        //    idText.text = regId;
        //}
        #region 清除推送消息红点
        StartCoroutine(CleanNotification());
        #endregion
    }

    IEnumerator CleanNotification()
    {
#if UNITY_IPHONE
		UnityEngine.iOS.LocalNotification l = new UnityEngine.iOS.LocalNotification();
		l.applicationIconBadgeNumber = -1;
		l.hasAction = false;
		UnityEngine.iOS.NotificationServices.PresentLocalNotificationNow(l);
		yield return new WaitForSeconds(0.2f);
		UnityEngine.iOS.NotificationServices.CancelAllLocalNotifications();
		UnityEngine.iOS.NotificationServices.ClearLocalNotifications();
#endif
        yield return null;
    }

    // Update is called once per frame
    void Update () {
		if(Input.GetKeyDown(KeyCode.Space))
        {
            //OnReceiveNotification(testStr);
            OnOpenNotification(testStr);
        }
	}

    public string GetRegID()
    {
        string regId = JPushBinding.GetRegistrationId();
        return regId;
    }

    public void OnReceiveNotification(string notification)
    {
        if(noficationText != null)
        {
            noficationText.text = notification;
        }

        Hashtable _jd = Json.jsonDecode(notification) as Hashtable;
#if UNITY_ANDROID
        string _content = _jd["content"].ToString();
        Hashtable extras = _jd["extras"] as Hashtable;
#elif  UNITY_IOS
        Hashtable aps = _jd["aps"] as Hashtable;
        string _content = aps["alert"].ToString();
        Hashtable extras = _jd;
#endif
        string _type = "";
        if (extras["type"] != null)
        {
            _type = extras["type"].ToString();
        }

        if (_type == "Notify")
        {
            mainPanel.SetRedPoint(1);
        }

        if(messageInfo.messageCenter.gameObject.activeInHierarchy)
        {
            StartCoroutine(RefreshNotifyInfoIE(messageInfo.messageCenter.RefreshNotifyInfo));
        }
    }
    public IEnumerator GetRegIDIE()
    {
#if UNITY_EDITOR
        yield break;
#endif
        if (idText != null)
        {
            idText.text = "获取中";
        }
        string regId = "";
        while (regId == "" || regId == null)
        {
            regId = GetRegID();
            yield return new WaitForSeconds(2);
        }
        if(idText!= null)
        {
            idText.text = regId;
        }
        HttpManager._Intance.regId = regId;
        PlayerPrefs.SetString("regId", regId);
        //保存到服务器
        List<KeyValuePair<string, string>> ws = new List<KeyValuePair<string, string>>();
        ws.Add(new KeyValuePair<string, string>("op", "saveRegId"));
        ws.Add(new KeyValuePair<string, string>("userName", HttpManager._Intance.loginInfo[0]));Debug.Log("我的用户名：" + HttpManager._Intance.loginInfo[0]);
        ws.Add(new KeyValuePair<string, string>("regId", regId));
        StartCoroutine(HttpManager._Intance.SendRequest(ws, OnCallback));
    }
    private void OnCallback(Hashtable jsonData)
    {
        int re = System.Convert.ToInt32(jsonData["error"]);
        if (re == -1)
        {
            Debug.Log("没有该账号！");
        }
        else if (re == 0)
        {
            Debug.Log("保存成功！");
        }
        else
        {
            Debug.Log("网络异常！"); 
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if(!pause)
        {
#if UNITY_IOS
            JPushBinding.ResetBadge();
#endif
        }
    }
    public void OnOpenNotification(string notification)
    {
        Debug.Log(notification);
        Hashtable _jd = Json.jsonDecode(notification) as Hashtable;
#if UNITY_ANDROID
        string _content = _jd["content"].ToString();
        Hashtable extras = _jd["extras"] as Hashtable;
#elif  UNITY_IOS
        Hashtable aps = _jd["aps"] as Hashtable;
        string _content = aps["alert"].ToString();
        Hashtable extras = _jd;
#endif
        string _type = "";
        if (extras["type"] != null)
        {
            _type = extras["type"].ToString();
        }
        if(_type == "Notify")
        {
            messageInfo.Show();
            return;
        }
        string _hash = "";
        if (extras["hash"] != null)
        {
            _hash = extras["hash"].ToString();
        }
        string _from = "";
        if (extras["from"] != null)
        {
            _from = extras["from"].ToString();
        }
        string _to = "";
        if (extras["to"] != null)
        {
            _to = extras["to"].ToString();
        }
        string _value = "";
        if (extras["value"] != null)
        {
            _value = extras["value"].ToString();
        }
        string _gas = "";
        if (extras["gas"] != null)
        {
            _gas = extras["gas"].ToString();
        }
        string _block = "";
        if (extras["block"] != null)
        {
            _block = extras["block"].ToString();
        }
        string _time = "";
        if (extras["time"] != null)
        {
            if(_type != "EOS")
            {
                long timeTemp = long.Parse(extras["time"].ToString());
                DateTime dt = startTime.AddMilliseconds(timeTemp).ToLocalTime();
                _time = dt.ToString("yyyy-MM-dd HH:mm:ss");
            }
            else
            {
                _time = extras["time"].ToString().Replace("T", " ");
            }
        }
        string _memo = "";
        if (extras["memo"] != null)
        {
            _memo = extras["memo"].ToString();
        }
        string _coinName = "";
        if (extras["coinName"] != null)
        {
            _coinName = extras["coinName"].ToString();
        }
        transactionPanel.Open(_type,_from, _to,_value, _coinName,_memo, _hash,_block,_time,_gas);
        InputFieldManage.instance.CloseInputFields();
    }

    public void GetETHTranscation(string hash)
    {
        //gettransaction
        List<KeyValuePair<string, string>> ws = new List<KeyValuePair<string, string>>();
        ws.Add(new KeyValuePair<string, string>("transaction", "0x4191fa4083fdece5f19dea960be97e7f25bae81e0875436561d85845802c3043"));
        StartCoroutine(HttpManager._Intance.GetNodeJsRequest("gettransaction", ws, (Hashtable data) =>
        {

            if (data != null)
            {
                string result = data["result"].ToString();
                Debug.Log(result);
            }
            else
            {
                PopupLine.Instance.Show("订单异常，请联系客服");
            }
        }));
    }
    public void RefreshNotifyMessage(string _str)
    {
        string notifyStr = _str;
        JsonData _jd = JsonMapper.ToObject(notifyStr);
        //List<MessageData> tempList = new List<MessageData>();
        //for (int i = 0; i < _jd.Count; i++)
        //{
        //    Hashtable temp = _jd[i] as Hashtable;
        //    MessageData message = new MessageData();
        //    message.index = int.Parse(temp["id"].ToString());
        //    message.content = temp["str"].ToString();
        //    message.title = temp["title"].ToString();
        //    long timeStamp = long.Parse(temp["time"].ToString());
        //    DateTime dt = startTime.AddMilliseconds(timeStamp).ToLocalTime();
        //    message.time = dt.ToString("yyyy-MM-dd HH:mm:ss");
        //    message.from = temp["from"].ToString();
        //}
        List<int> tempList = new List<int>();
        for (int i = 0; i < _jd.Count; i++)
        {
            JsonData temp = _jd[i];
            tempList.Add(int.Parse(temp["id"].ToString()));
        }
        messageInfo.SetBage(tempList);
    }

    public IEnumerator RefreshNotifyInfoIE(Action<string> Callback)
    {
        string url = "http://47.96.145.254:9001/http/all-message.action";
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.timeout = 10;
            HttpManager._Intance.loadingPanel.gameObject.SetActive(true);
            yield return www.SendWebRequest();
            if (www.error != null)
            {
                Debug.Log(www.error);
                HttpManager._Intance.loadingPanel.gameObject.SetActive(false);
            }
            else
            {
                if (www.responseCode == 200 && Callback != null)//200表示接受成功
                {
                    Callback(www.downloadHandler.text);
                    HttpManager._Intance.loadingPanel.gameObject.SetActive(false);
                }
            }
        }
    }
}
