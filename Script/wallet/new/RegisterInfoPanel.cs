using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Security.Cryptography;

public class RegisterInfoPanel : HotBasePanel
{
    public MainPanel mainPanel;
    public Text userName;
    public InputField passWorld;
    public InputField againPassWorld;
    public Text infoText;

    public Image eyeIamge1;
    public Image eyeImage2;

    public Sprite eyeOn;
    public Sprite eyeOff;

    private bool isEyeOn1 = false;
    private bool isEyeOn2 = false;

    public GameObject panel1;
    public GameObject panel2;

    public InputField phoneNumber;
    public InputField verifyCode;
    public Text infoText2;
    public Text sendInfo;
    public Button sendSMSBtn;
    public Button registBtn;
    public SelectRegionPanel region;
    private System.DateTime timer;
    bool isWait = false;
    public Text currentRegionText1;
    public Text currentRegionText2;
    string currentRegion;
    public Text getPasswordText;
    public Text getPasswordText2;
    public Text warnText;
    public Text getPasswordText3;

    public override void Open()
    {
        timer = System.DateTime.Now.AddDays(-10);
        infoText.text = "";
        ShowEye();
        isWait = false;
        userName.text = "";
        passWorld.text = "";
        currentRegionText1.text = "+86";
        currentRegionText2.text = "+86";
        againPassWorld.text = "";
        userName.text = "";
        phoneNumber.text = "";
        verifyCode.text = "";
        sendSMSBtn.interactable = true;
        infoText2.text = "";
        sendInfo.text = "发送验证码";
        sendInfo.color = Color.white;
        currentRegion = "0086";
        infoText2.color = Color.red;
        //passWorld.GetComponent<NativeEditBox>().text = "";
        //againPassWorld.GetComponent<NativeEditBox>().text = "";
        panel1.gameObject.SetActive(true);
        panel2.gameObject.SetActive(false);
        base.Open();
    }

    public void OnClicEyeBtn1()
    {
        isEyeOn1 = !isEyeOn1;
        ShowEye();
    }
    public void OnClickEyeBtn2()
    {
        isEyeOn2 = !isEyeOn2;
        ShowEye();
    }

    private void ShowEye()
    {
        if (isEyeOn1)
        {
            eyeIamge1.overrideSprite = eyeOn;
            passWorld.contentType = InputField.ContentType.Standard;
            passWorld.gameObject.SetActive(false);
            passWorld.gameObject.SetActive(true);
        }
        else
        {
            eyeIamge1.overrideSprite = eyeOff;
            passWorld.contentType = InputField.ContentType.Password;
            passWorld.gameObject.SetActive(false);
            passWorld.gameObject.SetActive(true);
        }

        if (isEyeOn2)
        {
            eyeImage2.overrideSprite = eyeOn;
            againPassWorld.contentType = InputField.ContentType.Standard;
            againPassWorld.gameObject.SetActive(false);
            againPassWorld.gameObject.SetActive(true);
        }
        else
        {
            eyeImage2.overrideSprite = eyeOff;
            againPassWorld.contentType = InputField.ContentType.Password;
            againPassWorld.gameObject.SetActive(false);
            againPassWorld.gameObject.SetActive(true);
        }
    }


    public void OnClickRegisterBtn()
    {
        return;

        infoText.text = "";
        if (userName.text == "")
        {
            infoText.text = "用户名不能为空";
        }
        else if (passWorld.text == "")
        {
            infoText.text = "密码不能为空";
        }
        if (passWorld.text.Length < 8 || passWorld.text.Length > 12)
        {
            infoText.text = "密码长度必须在8-12位之间";
            return;
        }

        if (!IsContainLetter(passWorld.text) || !IsContainNumber(passWorld.text))
        {
            infoText.text = "密码必须同时包含数字与字母";
            return;
        }
        else if (!passWorld.text.Equals(againPassWorld.text))
        {
            infoText.text = "两次输入的密码不相同";
        }
        else
        {
            List<KeyValuePair<string, string>> ws = new List<KeyValuePair<string, string>>();
            if (!PanelManager._Instance._loginPanel.getPasswordFlag)
                ws.Add(new KeyValuePair<string, string>("op", "register"));
            else
                ws.Add(new KeyValuePair<string, string>("op", "getPassword"));
            ws.Add(new KeyValuePair<string, string>("userName", currentRegion + userName.text));
            ws.Add(new KeyValuePair<string, string>("passWorld", EncryptWithMD5(passWorld.text)));
            StartCoroutine(HttpManager._Intance.SendRequest(ws, OnCallback));
        }
    }

    bool IsContainLetter(string str)
    {
        for (int i = 0; i < str.Length; i++)
        {
            if ((str[i] >= 65 && str[i] <= 90) || (str[i] >= 97 && str[i] <= 122))
            {
                return true;
            }
        }
        return false;
    }
    bool IsContainNumber(string str)
    {
        for (int i = 0; i < str.Length; i++)
        {
            if (str[i] >= 48 && str[i] <= 57)
            {
                return true;
            }
        }
        return false;
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

    private void OnCallback(Hashtable hashtable)
    {
        Debug.Log(Json.jsonEncode(hashtable));
        int re = System.Convert.ToInt32(hashtable["error"]);
        if (re == -1)
        {
            infoText.text = "该账号已被注册！";
        }
        else if (re == 0)
        {
            if (PanelManager._Instance._loginPanel.getPasswordFlag)
                PanelManager._Instance._loginPanel.OnLoginPanel();
            else
            {
                string pw = EncryptWithMD5(passWorld.text);
                string[] infos = new string[2] { currentRegion + userName.text, pw };
                HttpManager._Intance.loginInfo = infos;
                PlayerPrefsX.SetStringArray(HttpManager.LoginMark, infos);
                PlayerPrefs.SetString("region", currentRegion);
                PanelManager._Instance._loginPanel.OnLoginSuccsee(currentRegion + userName.text, pw, hashtable);
            }
        }
        else
        {
            infoText.text = "网络异常";
        }
    }

    public void OnNextStep()
    {
        infoText2.text = "";
        infoText2.color = Color.red;
        if (verifyCode.text == "")
        {
            infoText2.text = "验证码不能为空!";
            return;
        }
        if (!IsHandset(phoneNumber.text))
        {
            infoText2.text = "手机号码格式错误!";
            return;
        }
        List<KeyValuePair<string, string>> ws = new List<KeyValuePair<string, string>>();
        if (!PanelManager._Instance._loginPanel.getPasswordFlag)
            ws.Add(new KeyValuePair<string, string>("op", "verifycode"));
        else
            ws.Add(new KeyValuePair<string, string>("op", "modifycode"));
        ws.Add(new KeyValuePair<string, string>("phone", currentRegion + phoneNumber.text));
        ws.Add(new KeyValuePair<string, string>("code", verifyCode.text));
        StartCoroutine(HttpManager._Intance.SendRequest(ws, (s) =>
        {
            if (s["error"].ToString() == "0")
            {
                userName.text = phoneNumber.text;
                panel1.gameObject.SetActive(false);
                panel2.gameObject.SetActive(true);
                if (!PanelManager._Instance._loginPanel.getPasswordFlag)
                {
                    getPasswordText.text = "注 册";
                    getPasswordText2.text = "注 册";
                    getPasswordText3.text = "密码";
                    warnText.gameObject.SetActive(true);
                }
                else
                {
                    getPasswordText.text = "重设密码";
                    getPasswordText2.text = "确 定";
                    getPasswordText3.text = "新密码";
                    warnText.gameObject.SetActive(false);
                }
            }
            else
            {
                infoText2.text = "验证码不正确";
            }
        }));
    }

    public void OnSend()
    {
        infoText2.text = "";
        infoText2.color = Color.red;
        if (!IsHandset(phoneNumber.text))
        {
            infoText2.text = "手机号码格式错误!";
            return;
        }
        List<KeyValuePair<string, string>> ws = new List<KeyValuePair<string, string>>();
        if (!PanelManager._Instance._loginPanel.getPasswordFlag)
            ws.Add(new KeyValuePair<string, string>("op", "sendSms"));
        else
            ws.Add(new KeyValuePair<string, string>("op", "sendModifySms"));
        ws.Add(new KeyValuePair<string, string>("phone", currentRegion + phoneNumber.text));
        StartCoroutine(HttpManager._Intance.SendRequest(ws, (s) =>
        {
            if (s["error"].ToString() == "0")
            {
                infoText2.color = Color.gray;
                infoText2.text = "验证码已发送至" + "+" + currentRegion.Substring(2) + phoneNumber.text;
                timer = System.DateTime.Now;
            }
            else
            {
                infoText2.text = "手机号已经被注册!";
                if (PanelManager._Instance._loginPanel.getPasswordFlag)
                    infoText2.text = "该手机号尚未注册!";
            }
        }));
    }

    private bool IsHandset(string str_handset)
    {
        return str_handset.Length > 5;
    }

    void Update()
    {
        if ((System.DateTime.Now - timer).TotalSeconds < 60)
        {
            sendSMSBtn.interactable = false;
            sendInfo.text = "重新发送(" + (int)(60 - (System.DateTime.Now - timer).TotalSeconds) + ")";
            sendInfo.color = Color.gray;
        }
        else
        {
            sendSMSBtn.interactable = true;
            sendInfo.text = "发送验证码";
            sendInfo.color = Color.white;
        }
        if (passWorld.text.Length < 8 || passWorld.text.Length > 12 || againPassWorld.text.Length < 8 || againPassWorld.text.Length > 12)
        {
            registBtn.interactable = false;
        }
        else
        {
            registBtn.interactable = true;
        }
    }

    public void ShowRegion()
    {
        region.Show((s) =>
        {
            currentRegion = s;
            currentRegionText1.text = "+" + s.Substring(2);
            currentRegionText2.text = "+" + s.Substring(2);
        }, "+" + currentRegion.Substring(2));
    }
}
