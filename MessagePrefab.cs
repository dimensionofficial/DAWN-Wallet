using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessagePrefab : MonoBehaviour {
    public GameObject redPoint;
    public Text titleText;
    public Text infoText;
    public Text timeText;
    public string str;
    public string hax;
    public int index;
    public string timeStr;
    public string dataStr;
    public MessageCenter.MessageType type;

    public void ShowDetailPage()
    {
        MessageCenter.instance.ShowMessageDetailPage(this);
        //if(redPoint.activeInHierarchy)
        //{
        //    string newStr = dataStr.Replace("|0|", "|1|");
        //    PlayerPrefs.SetString("message" + index, newStr);
        //    redPoint.gameObject.SetActive(false);
        //    int bage = MessageInfo.RefreshBage();
        //    JPushManage.instance.messageInfo.SetBage(bage);
        //    JPushManage.instance.mainPanel.SetRedPoint(bage);
        //}
    }

    public void Init(string title,string time,string fromOrTo,string content,string hash,int _index,string _type,string isReaded)
    {
        if (isReaded == "0")
        {
            redPoint.SetActive(true);
        }
        else
            redPoint.SetActive(false);

        titleText.text = title;
        timeText.text = MessageCenter.GetDateFromString(time);
        timeStr = time;
        infoText.text = fromOrTo;
        str = content;
        hax = hash;
        index = _index;
        if(_type.IndexOf("BTC") >= 0)
        {
            type = MessageCenter.MessageType.BTC;
        }
    }
}
