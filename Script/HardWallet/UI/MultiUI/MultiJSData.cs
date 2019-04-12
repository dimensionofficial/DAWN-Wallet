using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.Text;
using System.IO;
using System;

[Serializable]
public class MultiJSData : MonoBehaviour {
    public static MultiJSData instance;
    //路径赋值
    private string filepath;
    public List<MultiWalletInfo> multiWalletInfoList;
    private MultiJSData() { }
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        multiWalletInfoList = new List<MultiWalletInfo>();
        filepath = Application.dataPath + @"/Resources/MultiWalletInfo.json";
        GetJsonData();
    }
    public void SaveMultiWalletInfo(MultiWalletInfo multiWalletInfo, Action<bool> CallBack)
    {
        if (multiWalletInfoList.Contains(multiWalletInfo))
        {
            multiWalletInfoList.Remove(multiWalletInfo);
            multiWalletInfoList.Add(multiWalletInfo);
        }
        else
        {
            multiWalletInfoList.Add(multiWalletInfo);
        }
        SaveJsonData(multiWalletInfoList,CallBack);
    }
    public void SaveMultiWalletInfo(Action<bool> CallBack = null)
    {
        SaveJsonData(multiWalletInfoList, CallBack);
    }
    public string SaveJsonData(List<MultiWalletInfo>  multiWalletInfo, Action<bool> CallBack=null)
    {
        StringBuilder sb = new StringBuilder();
        JsonWriter writer = new JsonWriter(sb);
        writer.WriteObjectStart();

        writer.WritePropertyName("MultiWalletInfo");
        writer.WriteArrayStart();
       
        for (int n = 0; n < multiWalletInfo.Count; n++)
        {
            writer.WriteObjectStart();
            writer.WritePropertyName("Multi_walletName");
            writer.Write(multiWalletInfo[n].Multi_walletName);

            writer.WritePropertyName("Multi_btcAddress");
            writer.Write(multiWalletInfo[n].Multi_btcAddress);

            writer.WritePropertyName("MultiSig_M");
            writer.Write(multiWalletInfo[n].MultiSig_M);

            writer.WritePropertyName("MultiSig_N");
            writer.Write(multiWalletInfo[n].MultiSig_N);

            writer.WritePropertyName("walletName");
            writer.WriteArrayStart();
            writer.WriteObjectStart();

            for (int i = 0; i < multiWalletInfo[n].walletName.Count; i++)
            {
                writer.WritePropertyName(i.ToString());
                writer.Write(multiWalletInfo[n].walletName[i].ToString());
            }
            writer.WriteObjectEnd();
            writer.WriteArrayEnd();

            writer.WritePropertyName("btcAddress");
            writer.WriteArrayStart();
            writer.WriteObjectStart();

            for (int i = 0; i < multiWalletInfo[n].btcAddress.Count; i++)
            {
                writer.WritePropertyName(i.ToString());
                writer.Write(multiWalletInfo[n].btcAddress[i].ToString());
            }
            writer.WriteObjectEnd();
            writer.WriteArrayEnd();

            writer.WritePropertyName("pubstr");
            writer.WriteArrayStart();
            writer.WriteObjectStart();

            for (int i = 0; i < multiWalletInfo[n].pubstr.Count; i++)
            {
                writer.WritePropertyName(i.ToString());
                writer.Write(multiWalletInfo[n].pubstr[i].ToString());
            }
            writer.WriteObjectEnd();
            writer.WriteArrayEnd();

            writer.WriteObjectEnd();
        }
        
        writer.WriteArrayEnd();
        writer.WriteObjectEnd();

        //将jsonData对象转成json格式字符串
        string jsonStr = sb.ToString();

        SaveJson(jsonStr, CallBack);
        return jsonStr;
    }
    public void GetJsonData()
    {
        //流读取器
        //StreamReader sr = new FileInfo(filepath).OpenText();
        //读取文本内容，直到结束
        //string json = sr.ReadToEnd();
        string json = GetJsonStringFromLocal();
        JsonData jd = JsonMapper.ToObject(json);
        JsonData jdWalletInfo = jd["MultiWalletInfo"];
        multiWalletInfoList.Clear();
        for (int i = 0; i < jdWalletInfo.Count; i++)
        {
            MultiWalletInfo multiWalletInfo = new MultiWalletInfo();
            multiWalletInfo.btcAddress = new List<string>();
            multiWalletInfo.walletName = new List<string>();
            multiWalletInfo.pubstr = new List<string>();
            multiWalletInfo.Multi_walletName= jdWalletInfo[i]["Multi_walletName"].ToString();
            multiWalletInfo.Multi_btcAddress = jdWalletInfo[i]["Multi_btcAddress"].ToString();
            multiWalletInfo.MultiSig_M =(int)jdWalletInfo[i]["MultiSig_M"];
            multiWalletInfo.MultiSig_N =(int)jdWalletInfo[i]["MultiSig_N"];
            JsonData JdWalletName = jdWalletInfo[i]["walletName"];
            for (int j = 0; j < JdWalletName[0].Count; j++)
            {
                multiWalletInfo.walletName.Add(JdWalletName[0][j].ToString());
            }
            JsonData JdBtcAddress = jdWalletInfo[i]["btcAddress"];
            for (int j = 0; j < JdWalletName[0].Count; j++)
            {
                multiWalletInfo.btcAddress.Add(JdBtcAddress[0][j].ToString());
                
            }
            JsonData JdPubstr = jdWalletInfo[i]["pubstr"];
            for (int j = 0; j < JdWalletName[0].Count; j++)
            {
                multiWalletInfo.pubstr.Add(JdPubstr[0][j].ToString());
            }
            multiWalletInfoList.Add(multiWalletInfo);
        }


        for (int j = 0; j < multiWalletInfoList.Count; j++)
        {

            if (NewWalletManager._Intance.usdtAddresListInfo.ContainsKey(multiWalletInfoList[j].Multi_btcAddress))
            {
                NewWalletManager._Intance.usdtAddresListInfo.Remove(multiWalletInfoList[j].Multi_btcAddress);
            }
        }
    }
    private void Print()
    {
        foreach (var item in multiWalletInfoList)
        {
            Debug.Log("Multi_walletName=" + item.Multi_walletName+ "  Multi_btcAddress= "+ item.Multi_btcAddress+ " MultiSig_M=" + item.MultiSig_M+ " MultiSig_N=" + item.MultiSig_N);
            for (int i = 0; i < item.pubstr.Count; i++)
            {
                Debug.Log("SN="+ item.walletName[i]+"  Address="+item.btcAddress[i]+"  puk="+item.pubstr[i]);
            }
        }
    }
    public MultiWalletInfo GetMultiWalletInfo(string btcAddress)
    {
        try
        {
            foreach (var multiWalletInfo in multiWalletInfoList)
            {
                if (multiWalletInfo.Multi_btcAddress == btcAddress)
                {
                    return multiWalletInfo;
                }
            }
        }
        catch (System.Exception)
        {
            
        }
        
        return null;
    }

    public void SaveJson(string jsonstr,System.Action<bool> CallBack)
    {
        PlayerPrefs.SetString("MultiWalletInfo", jsonstr);
        if (CallBack!=null)
        {
            CallBack(true);
        }
    }
    public string GetJsonStringFromLocal()
    {
        if (!PlayerPrefs.HasKey("MultiWalletInfo"))
        {
            PlayerPrefs.SetString("MultiWalletInfo", SaveJsonData(multiWalletInfoList));

            string js = SaveJsonData(multiWalletInfoList);
            Debug.Log("没有MultiWalletInfo");
        }
        return PlayerPrefs.GetString("MultiWalletInfo");
    }
    void SaveJson(string text,string filepath)
    {
        //流写入器
        StreamWriter sw;
        //文件信息操作类
        FileInfo info = new FileInfo(filepath);
        //判断路径是否存在
        if (!info.Exists)
        {
            sw = info.CreateText();
        }
        else
        {
            //先删除再创建
            info.Delete();
            sw = info.CreateText();
        }

        sw.WriteLine(text);
        sw.Close();
        sw.Dispose();
    }
    public bool RemoveAddress(string addr)
    {
        bool isRemove = false;
        for (int i = 0; i < multiWalletInfoList.Count; i++)
        {
            if (multiWalletInfoList[i].Multi_btcAddress == addr)
            {
                multiWalletInfoList.Remove(multiWalletInfoList[i]);
                isRemove = true;
            }
        }
        return isRemove;
    }
    public bool IsMultiSigAddress(string addr)
    {
        if (multiWalletInfoList.Count>0)
        {
            foreach (var multiWalletInfo in multiWalletInfoList)
            {
                if (multiWalletInfo.Multi_btcAddress == addr)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
