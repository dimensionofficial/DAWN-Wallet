using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageInfo : MonoBehaviour {
    public GameObject msgObj;
    public Text msgText;
    public MessageCenter messageCenter;
    public static MessageInfo instance;
    private void Awake()
    {
        instance = this;
    }
    // Use this for initialization
    void Start () {
            
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Show()
    {
        gameObject.SetActive(true);
        ShowMsgCenter();
    }

    public void ShowMsgCenter()
    {
        messageCenter.Show();
    }
    public void SetBage(int bage)
    {
        if(bage <= 0)
        {
            msgObj.gameObject.SetActive(false);
            msgText.text = "0";
            JPushManage.instance.mainPanel.SetRedPoint(0);
        }
        else
        {
            msgObj.gameObject.SetActive(true);
            msgText.text = bage.ToString();
            JPushManage.instance.mainPanel.SetRedPoint(bage);
        }

    }

    public void SetBage(List<int> idList)
    {
        List<int> readedList = GetReadedList();
        int _count = 0;
        for (int i = 0; i < idList.Count; i++)
        {
            if (!readedList.Contains(idList[i]))
                _count++;
        }
        SetBage(_count);
    }

    public List<int> GetReadedList()
    {
        string idStr = PlayerPrefs.GetString("ReadedMessageID");
        List<int> readedList = new List<int>();
        if (idStr.Length > 0)
        {
            string[] idArr = idStr.Split('|');
            for (int i = 0; i < idArr.Length; i++)
            {
                readedList.Add(int.Parse(idArr[i]));
            }
        }
        return readedList;
    }
    public void MulBage()
    {
        int _bage = int.Parse(msgText.text);
        SetBage(_bage - 1);
    }
    public static int  RefreshBage()
    {
        Debug.Log("refresh");
        int startIndex = 0;
        int bage = 0;
        while (true)
        {
            startIndex++;
            string str = PlayerPrefs.GetString("message" + startIndex);
            if (str == null || str == "")
            {
                Debug.Log("没有更多消息了" + (startIndex--).ToString());
                break;
            }
            else
            {
                string[] strArr = str.Split('|');
                if (strArr[3] == "0")
                    bage++;
            }
        }
        return bage;
    }
}
