using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
using UnityEngine.UI;
using System.Security.Cryptography;
using System.Text;

public class LoginInfoPanel : HotBasePanel {
    public MainPanel mainPanel;
    public InputField userName;
    public InputField passWorld;
    public GameObject eyeOn;
    public GameObject eyeOff;
    public Text infoText;
    public Text currentRegionText;
    public SelectRegionPanel region;
    string currentRegion = "";

    private bool passWorldIsON = false;

    public override void Open()
    {
        if (HttpManager._Intance.loginInfo.Length == 1)
        {
            string region = PlayerPrefs.GetString("region", "");
            if (string.IsNullOrEmpty(region))
            {
                userName.text = HttpManager._Intance.loginInfo[0];
                currentRegion = "0086";
                currentRegionText.text = "+86";
            }
            else
            {
                string na = HttpManager._Intance.loginInfo[0].Substring(region.Length);
                userName.text = na;
                currentRegion = region;
                currentRegionText.text = "+" + region.Substring(2);
            }
        }
        else
        {
            currentRegion = "0086";
            currentRegionText.text = "+86";
        }
        infoText.text = "";
        passWorld.text = "";
        ShowPassWorld();
        base.Open();
    }

    public void OnClickEyeBtn()
    {
        passWorldIsON = !passWorldIsON;
        ShowPassWorld();
    }

    private void ShowPassWorld()
    {
        if (passWorldIsON)
        {
            eyeOn.SetActive(true);
            eyeOff.SetActive(false);
            passWorld.contentType = InputField.ContentType.Standard;
            passWorld.gameObject.SetActive(false);
            passWorld.gameObject.SetActive(true);
        }
        else
        {
            eyeOn.SetActive(false);
            eyeOff.SetActive(true);
            passWorld.contentType = InputField.ContentType.Password;
            passWorld.gameObject.SetActive(false);
            passWorld.gameObject.SetActive(true);
        }
    }

    public void OnClickLoginBtn()
    {
        infoText.text = "";
        if (userName.text != "" && passWorld.text != "")
        {
            List<KeyValuePair<string, string>> ws = new List<KeyValuePair<string, string>>();
            ws.Add(new KeyValuePair<string, string>("op", "login"));
            ws.Add(new KeyValuePair<string, string>("userName", currentRegion + userName.text));
            ws.Add(new KeyValuePair<string, string>("passWorld", EncryptWithMD5(passWorld.text)));
            StartCoroutine(HttpManager._Intance.SendRequest(ws, OnCallback));
        }
        else
        {
            infoText.text = "账号或密码不能为空！";
        }
    }

    
   

    private string EncryptWithMD5(string source)
    {
        byte[] sor = Encoding.UTF8.GetBytes(source);
        MD5 md5 = MD5.Create();
        byte[] result = md5.ComputeHash(sor);
        StringBuilder strbul = new StringBuilder(40);
        for (int i = 0; i < result.Length; i++)
        {
            strbul.Append(result[i].ToString("x2"));

        }
        return strbul.ToString().ToLower();
    }

    

    private void OnCallback(Hashtable jsonData)
    {
        int re = System.Convert.ToInt32(jsonData["error"]);
        if (re == -1)
        {
            infoText.text = "密码不正确！";
        }
        else if (re == -2)
        {
            infoText.text = "没有该账号！";
        }
        else if (re == 0)
        {
            string pw = EncryptWithMD5(passWorld.text);
            string[] infos = new string[2] { currentRegion + userName.text, pw };
            HttpManager._Intance.loginInfo = infos;
            PlayerPrefsX.SetStringArray(HttpManager.LoginMark, infos);
            PlayerPrefs.SetString("region", currentRegion);
            PanelManager._Instance._loginPanel.OnLoginSuccsee(currentRegion + userName.text, pw,  jsonData);
        }
        else
        {
            infoText.text = "网络异常";
        }
    }

    public void ShowRegion()
    {
        region.Show((s) => {
            currentRegion = s;
            currentRegionText.text = "+" + currentRegion.Substring(2);
        }, currentRegionText.text);
    }
}
