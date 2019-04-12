using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class WalletChangePassword : HotBasePanel
{
    public InputField oldPingText;
    public GameObject eyeOpen1;
    public GameObject eyeColse1;

    public InputField newPing1Text;
    public GameObject eyeOpen2;
    public GameObject eyeColse2;

    public InputField newPing2Text;
    public GameObject eyeOpen3;
    public GameObject eyeColse3;


    public Text noteText;

    public override void Open()
    {
        noteText.text = "";
        oldPingText.text = "";
        eyeOpen1.SetActive(false);
        eyeColse1.SetActive(true);
        oldPingText.contentType = InputField.ContentType.Password;

        newPing1Text.text = "";
        eyeOpen2.SetActive(false);
        eyeColse2.SetActive(true);
        newPing1Text.contentType = InputField.ContentType.Password;

        newPing2Text.text = "";
        eyeOpen3.SetActive(false);
        eyeColse3.SetActive(true);
        newPing2Text.contentType = InputField.ContentType.Password;

        base.Open();
    }

    public void OnClickShowPassWordBtn(InputField inputText)
    {
        if (inputText.contentType == InputField.ContentType.Password)
        {
            inputText.contentType = InputField.ContentType.Standard;
            HidEyeMark(false, inputText);
        }
        else
        {
            inputText.contentType = InputField.ContentType.Password;
            HidEyeMark(true, inputText);
        }
        inputText.gameObject.SetActive(false);
        inputText.gameObject.SetActive(true);
    }
    private void HidEyeMark(bool isHide, InputField inputText)
    {
        if (inputText == newPing2Text)
        {
            eyeColse3.SetActive(isHide);
            eyeOpen3.SetActive(!isHide);
        }
        else if (inputText == newPing1Text)
        {
            eyeColse2.SetActive(isHide);
            eyeOpen2.SetActive(!isHide);
        }
        else if (inputText == oldPingText)
        {
            eyeColse1.SetActive(isHide);
            eyeOpen1.SetActive(!isHide);
        }
    }

    public void OnClickOKBtn()
    {
        string oldpw = oldPingText.text;
        if (!oldpw.Equals(SeedKeyManager.Instance.pingCode))
        {
            ShowNoticeText("旧密码不正确");
            return;
        }

        if (newPing1Text.text == "")
        {
            ShowNoticeText("新密码不能为空");
            return;
        }

        if (!newPing1Text.text.Equals(newPing2Text.text))
        {
            ShowNoticeText("新密码两次输入不相同");
            return;
        }

        if (newPing1Text.text.Equals(oldPingText.text))
        {
            ShowNoticeText("新密码与旧密码重复");
            return;
        }

        if (newPing1Text.text.Length < 8 || newPing1Text.text.Length > 12)
        {
            ShowNoticeText("密码长度必须在8-12位之间");
            return;
        }

        if (!IsContainLetter(newPing1Text.text) || !IsContainNumber(newPing1Text.text))
        {
            ShowNoticeText("密码必须同时包含数字与字母");
            return;
        }

        PopupLine.Instance.Show("密码修改成功");
        Closed();
        SeedKeyManager.Instance.pingCode = newPing1Text.text;

        //http://47.96.131.169:8090/test1.php?op=ChangeLoginPassWorld&userName=caolin&newPassWorld=1234
        //List<KeyValuePair<string, string>> ws = new List<KeyValuePair<string, string>>();
        //ws.Add(new KeyValuePair<string, string>("op", "changeLoginPassWorld"));
        //ws.Add(new KeyValuePair<string, string>("userName", NewWalletManager._Intance.userName));
        //ws.Add(new KeyValuePair<string, string>("oldpassworld", EncryptWithMD5(oldPingText.text)));
        //ws.Add(new KeyValuePair<string, string>("newPassWorld", EncryptWithMD5(newPing1Text.text)));
        //StartCoroutine(HttpManager._Intance.SendRequest(ws, delegate (Hashtable table)
        //{

        //    int re = System.Convert.ToInt32(table["error"]);
        //    if (re == -1)
        //    {
        //        noteText.text = "网络异常,修改失败";
        //    }
        //    else if (re == 0)
        //    {
        //        Closed();
        //        noteText.text = "";
        //        string pwMd = EncryptWithMD5(newPing1Text.text);
        //        NewWalletManager._Intance.passWorld = pwMd;
        //        string[] infos = new string[2] { NewWalletManager._Intance.userName, NewWalletManager._Intance.passWorld};
        //        HttpManager._Intance.loginInfo = infos;
        //        PlayerPrefsX.SetStringArray(HttpManager.LoginMark, infos);
        //    }
        //    else
        //    {
        //        noteText.text = "网络异常，修改失败";
        //    }

        //}));
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

    private void ShowNoticeText(string notex)
    {
        CancelInvoke();
        noteText.gameObject.SetActive(true);
        noteText.text = notex;
        Invoke("HideNoticeText", 2);
    }

    private void HideNoticeText()
    {
        noteText.gameObject.SetActive(false);
    }
}
