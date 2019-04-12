using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Excel;
//using System.Data;
using LitJson;
using System.Xml;

public class LanguageSetting : MonoBehaviour
{
    
    private LanguageData data;

    public static LanguageSetting Intance;  

    public enum LanguageType
    {
        Chinese = 0,
        English,
    }
    public LanguageType currentLanguage = LanguageType.Chinese;

    void Awake()
    {
        Intance = this;
        data = new LanguageData();
        data.FillSoundData();
    }

    public void SetDataToXML()
    {

    }

    public void SetText(Text text, int id)
    {
        if (text != null)
        {
            text.text = data.languageDic[id][(int)currentLanguage];
        }
        else
        {
            Debug.Log("UGUI Text Is Null");
        }
    }

    public const string ExcelPathName = "/LanguageData.xlsx";
//    [ContextMenu("TOXML")]
//    public void GameReadExcel()
//    {
//        FileStream stream = File.Open(Application.dataPath + ExcelPathName, FileMode.Open, FileAccess.Read);
//        IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

//        DataSet result = excelReader.AsDataSet();

//        int columns = result.Tables[0].Columns.Count;//获取列数  
//        int rows = result.Tables[0].Rows.Count;//获取行数  

//        XmlDocument xml = CreateXML();

//        //从第二行开始读  
//        for (int i = 1; i < rows; i++)
//        {
//            string node = "";
//            string[] infos = new string[columns - 1];
//            for (int j = 0; j < columns; j++)
//            {
//                if (j == 0)
//                {
//                    node = result.Tables[0].Rows[i][j].ToString();
//                }
//                else
//                {
//                    infos[j - 1] = result.Tables[0].Rows[i][j].ToString();
//                }
//            }
//            AddNodeToXML(xml, node, infos);
//        }
//        UpdateNodeToXML();
//        SaveXML(xml);
//    }
//    XmlDocument CreateXML()
//    {
//        //新建xml对象
//        XmlDocument xml = new XmlDocument();
//        //加入声明
//        xml.AppendChild(xml.CreateXmlDeclaration("1.0", "UTF-8", null));
//        //加入根元素
//        xml.AppendChild(xml.CreateElement("Config"));
//        return xml;
//    }

//    void AddNodeToXML(XmlDocument xml, string titleValue, params string[] infoValue)
//    {
//        //获取根节点
//        XmlNode root = xml.SelectSingleNode("Config");
//        //添加元素
//        XmlElement element = xml.CreateElement("Node");
//        element.SetAttribute("Type", "string");
//        //在Node节点下添加子节点
//        XmlElement titleElelment = xml.CreateElement("Title");
//        //titleElelment.SetAttribute("Title", TitleValue);
//        titleElelment.InnerText = titleValue;
//        element.AppendChild(titleElelment);

//        for (int i = 0; i < infoValue.Length; i++)
//        {
//            XmlElement infoElement = xml.CreateElement("Info" + i);
//            infoElement.InnerText = infoValue[i];
//            element.AppendChild(infoElement);
//        }



//        root.AppendChild(element);
//    }

//    void UpdateNodeToXML()
//    {
//        string filepath = Application.dataPath + @"/info.xml";
//        if (File.Exists(filepath))
//        {
//            XmlDocument xmldoc = new XmlDocument();
//            xmldoc.Load(filepath);  //根据指定路径加载xml
//            XmlNodeList nodeList = xmldoc.SelectSingleNode("Config").ChildNodes; //Node节点
//            //遍历所有子节点
//            foreach (XmlElement xe in nodeList)
//            {
//                //拿到节点中属性Type=“string”的节点
//                if (xe.GetAttribute("Type") == "string")
//                {
//                    //更新节点属性
//                    xe.SetAttribute("type", "text");
//                    //继续遍历
//                    foreach (XmlElement xelement in xe.ChildNodes)
//                    {
//                        if (xelement.Name == "TitleNode")
//                        {
//                            //修改节点名称对应的数值，而上面的拿到节点连带的属性
//                            //xelement.SetAttribute("Title", "企业简介");
//                            xelement.InnerText = "企业简介";
//                        }
//                    }
//                    break;
//                }
//            }
//            xmldoc.Save(filepath);
//            Debug.Log("Update XML OK!");
//        }
//    }

//    void SaveXML(XmlDocument xml)
//    {
//        //存储xml文件
//#if UNITY_EDITOR || UNITY_STANDALONE
//        xml.Save(Application.dataPath + "/StreamingAssets/info.xml");
//#elif UNITY_ANDROID
//        xml.Save(Application.persistentDataPath + "/info.xml");
//#endif
//    }
}
