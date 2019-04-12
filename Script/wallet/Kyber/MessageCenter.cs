using LitJson;
using Nethereum.JsonRpc.UnityClient;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MessageCenter : MonoBehaviour {
    public static MessageCenter instance;

    public GameObject messageDetailPage;
    public Text title;
    public Text timeText;
    public Text content;
    public Button showMoreDetailButton;
    public List<MessageData> dataList = new List<MessageData>();
    public RectTransform messageParent;
    public MessagePrefab messagePrefab;

    private System.DateTime startTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
    void Awake()
    {
        instance = this;
        Debug.Log("||".Split('|').Length);
    }
	// Use this for initialization
	void Start () {
        //RefreshMessageData();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Show()
    {
        gameObject.SetActive(true);
        HideMessageDetailPage();
        StartCoroutine(JPushManage.instance.RefreshNotifyInfoIE(RefreshNotifyInfo));
    }
    public void Hide()
    {
        InputFieldManage.instance.OpenInputFields();
        gameObject.SetActive(false);
    }

    public void SetAllReaded()
    {
        List<int> _list = new List<int>();
        for (int i = 0; i < messageParent.childCount; i++)
        {
            MessagePrefab prefab = messageParent.GetChild(i).GetComponent<MessagePrefab>();
            prefab.redPoint.SetActive(false);
            _list.Add(prefab.index);
        }
        string tempStr = "";
        for (int i = 0; i < _list.Count; i++)
        {
            if (i == 0)
                tempStr += _list[i];
            else
                tempStr += "|" + _list[i];
        }
        PlayerPrefs.SetString("ReadedMessageID", tempStr);
        JPushManage.instance.messageInfo.SetBage(0);
    }

    public enum MessageType
    {
        Notify,
        BTC,
        ETH,
        Kyber,
        ERC20,
        EOS,
        EOSToken
    }

    public void ShowMessageDetailPage(MessagePrefab prefab)
    {
        messageDetailPage.SetActive(true);
        title.text = prefab.titleText.text;
        timeText.text = prefab.timeStr + "    " + prefab.infoText.text;
        content.text = prefab.str;
        if(prefab.redPoint.activeInHierarchy)
        {
            prefab.redPoint.SetActive(false);
            AddToReadedList(prefab.index);
            JPushManage.instance.messageInfo.MulBage();
        }
    }
    public void HideMessageDetailPage()
    {
        messageDetailPage.SetActive(false);
    }

    public void ShowMoreDetail()
    {

    }

    public void RefreshMessageData() //弃用
    {
        Debug.Log("refresh");
        int startIndex = dataList.Count;
        int InstaniateIndex = dataList.Count;
        while (true)
        {
            startIndex++;
            string str = PlayerPrefs.GetString("message" + startIndex);
            if(str == null || str == "")
            {
                Debug.Log("没有更多消息了" + (startIndex--).ToString());
                break;
            }
            else
            {
                MessageData data = new MessageData();
                data.dataStr = str;
                string[] strArr = str.Split('|');
                data.content = strArr[0];
                data.type = strArr[1];
                data.hash = strArr[2];
                data.isReaded = strArr[3];
                data.time = strArr[4];
                data.index = startIndex;
                dataList.Add(data);
            }
        }

        int bage = 0;
        for (int i = InstaniateIndex; i < dataList.Count; i++)
        {
            MessageData message = dataList[i];
            MessagePrefab prefab = Instantiate(messagePrefab);
            prefab.transform.SetParent(messageParent);
            prefab.transform.localScale = new Vector3(1, 1, 1);
            prefab.dataStr = message.dataStr;

            //string title,string time,string fromOrTo,string content,string hash,int index,string type,string isReaded
            if (message.type.IndexOf("BTC") >= 0 || message.type.IndexOf("ETH") >= 0)//BTC ETH
            {
                string address = message.content.Split('址')[1].Substring(0, 23);
                string title = "";
                string fromOrTo = "";
                if (message.content.IndexOf("一笔转入资金") >= 0)
                {
                    title = "您收到一笔转入资金";
                    fromOrTo = "来自：" + address;
                }
                else if (message.content.IndexOf("转出一笔资金") >= 0)
                {
                    title = "您成功转出一笔资金";
                    fromOrTo = "转账至：" + address;
                }
                string time = message.time;
                string content = message.content;
                string hash = message.hash;
                int index = message.index;
                string type = message.type;
                string isReaded = message.isReaded;
                prefab.Init(title, time, fromOrTo, content, hash, index, type, isReaded);
            }
            else if (message.type.IndexOf("Kyber") >= 0)//Kyber
            {

            }
            else if (message.type.IndexOf("ERC20") >= 0)//ERC20
            {

            }
            else if (message.type.IndexOf("EOS") >= 0)//EOS
            {

            }
            else if (message.type.IndexOf("EOSToken") >= 0)//EOSToken
            {

            }
            else if (message.type.IndexOf("Notify") >= 0)//Notify
            {

            }
            if (dataList[i].isReaded == "0")
                bage++;
        }
        JPushManage.instance.messageInfo.SetBage(bage);
    }

    bool IsContainMessage(MessageData message)
    {
        for (int i = 0; i < messageParent.childCount; i++)
        {
            MessagePrefab prefab = messageParent.GetChild(i).GetComponent<MessagePrefab>();
            if (prefab.index == message.index)
                return true;
        }
        return false;
    }

    public static string GetDateFromString(string _time)
    {
        return _time.Substring(0, 10);
    }

    public void RefreshNotifyInfo(string _str)
    {
        string notifyStr = JPushManage.instance.testNotify;
        notifyStr = _str;
        JsonData _jd = JsonMapper.ToObject(notifyStr);
        List<MessageData> tempList = new List<MessageData>();
        List<int> readedList = JPushManage.instance.messageInfo.GetReadedList();
        List<int> idList = new List<int>();
        float prefabHeight = messagePrefab.GetComponent<RectTransform>().rect.height;
        for (int i = 0; i < _jd.Count; i++)
        {
            JsonData temp = _jd[i];
            MessageData message = new MessageData();
            message.index = int.Parse(temp["id"].ToString()); idList.Add(message.index);
            message.content = temp["content"].ToString();
            message.title = temp["title"].ToString();
            message.time = temp["dutyTime"].ToString().Replace("T"," ");
            message.from = temp["author"].ToString();
            tempList.Add(message);
            //创建prefab
            if(!IsContainMessage(message))
            {
                MessagePrefab prefab = Instantiate(messagePrefab);
                prefab.index = message.index;
                if (readedList.Contains(prefab.index))
                {
                    prefab.redPoint.SetActive(false);
                }
                else
                {
                    prefab.redPoint.SetActive(true);
                }
                prefab.transform.SetParent(messageParent);
                prefab.transform.localScale = new Vector3(1, 1, 1);
                messageParent.sizeDelta = new Vector2(0, messageParent.childCount * prefabHeight);
                prefab.str = message.content;
                prefab.titleText.text = message.title;
                prefab.infoText.text = "来自: " + message.from;
                prefab.timeStr = message.time;
                prefab.timeText.text = message.time.Substring(0,10);
            }
        }
        int _count = 0;
        for (int i = 0; i < idList.Count; i++)
        {
            if (!readedList.Contains(idList[i]))
                _count++;
        }
        JPushManage.instance.messageInfo.SetBage(_count);
    }

    public void AddToReadedList(int _id)
    {
        List<int> readedList = JPushManage.instance.messageInfo.GetReadedList();
        if(!readedList.Contains(_id))
        {
            readedList.Add(_id);
        }
        string tempStr = "";
        for (int i = 0; i < readedList.Count; i++)
        {
            if (i == 0)
                tempStr += readedList[i];
            else
                tempStr += "|" + readedList[i];
        }
        PlayerPrefs.SetString("ReadedMessageID", tempStr);
    }

    
}

public class MessageData
{
    public string content;
    public string type;
    public string hash;
    public string isReaded;
    public int index;
    public string time;
    public string dataStr;
    public string title;
    public string from;
}
