using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using Excel;
using LitJson;
using System.Xml;

public class LanguageData
{
    private string filePath =
#if UNITY_ANDROID && !UNITY_EDITOR
                  "jar:file:///" + Application.dataPath + "!/assets/";
#elif UNITY_IPHONE && !UNITY_EDITOR
                   "file:///" + Application.dataPath +"/Raw/";
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR
                  "file://" + Application.dataPath + "/StreamingAssets/";
    #else
                   string.Empty;  
    #endif

    public const string ExcelPathName = "/LanguageData.xlsx";

    public Dictionary<int, List<string>> languageDic = new Dictionary<int, List<string>>();



    public void FillSoundData()
    {
        Test(1, "多语言", "multi_language");
        Test(2, "货币单位", "monetary unit");
        Test(3, "修改密码", "change password");
        Test(4, "备份种子", "multi_language");
        Test(5, "验证种子", "multi_language");
        Test(6, "指纹登录", "multi_language");
        Test(7, "帮助", "multi_language");
        Test(8, "关于我们", "multi_language");
        Test(9, "设置", "multi_language");
        Test(10, "钱包", "multi_language");
        Test(11, "资讯", "multi_language");
        Test(12, "收款地址不合法", "multi_language");
        Test(13, "没有足够的币", "multi_language");
        Test(14, "发送数量最低不能低于0.01", "multi_language");
        Test(15, "请输入发送数量", "multi_language");
        Test(16, "没有足够的资金", "multi_language");
        Test(17, "网络异常", "multi_language");
        Test(18, "发送成功", "multi_language");
        Test(19, "余额", "multi_language");
        Test(20, "您有部分余额尚未被区块确认，请稍后再试", "multi_language");
        Test(21, "ETH余额不足");
        Test(22, "发送数量有误", "multi_language");
        // XmlDocument doc;
        //// try
        //// {
        //     doc = new XmlDocument();
        //     string path = filePath + "info.xml";
        //     doc.LoadXml(path);
        //     XmlNode root = doc.SelectSingleNode("Config");
        //     XmlNodeList items = root.SelectNodes("Node");
        //     foreach (XmlNode v in items)
        //     {
        //         int key = int.Parse(v.Attributes.GetNamedItem("Title").Value);
        //         List<string> tempData = new List<string>();
        //         tempData.Add(v.Attributes.GetNamedItem("Info0").Value);
        //         tempData.Add(v.Attributes.GetNamedItem("Info1").Value);
        //         languageDic.Add(key, tempData);
        //     }
        //   }
        //   catch (Exception e)
        //   {
        //       Debug.LogError(e.Message);
        //   }
    }

    private void Test(int key, params string[] str)
    {
        List<string> tempData = new List<string>(str);
        languageDic.Add(key, tempData);
    }
}
