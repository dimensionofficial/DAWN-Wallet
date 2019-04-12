using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class HotWalletLoginPanel : HotBasePanel
{
    public GameObject loginSelectMark;
    public LoginInfoPanel loginObject;

    public GameObject registerSelectMark;
    public RegisterInfoPanel registerObject;
    public int versionCode = 1;

    public GameObject xieyiPage;
    public GameObject createOrResume;
    public Toggle isAgreeToggle;
    public Button RegisButton;
    public Text getPasswordText;
    public bool getPasswordFlag;

    public Image logoImage;

    void Start()
    {
        DetectionVersion();
    }

    public void CreatWallet()
    {

    }

    public void OnClickAgreeBtn()
    {
        if (NewWalletManager._Intance.IsNeedColdWallet)
        {
            InitApp();
        }
        else
        {
            ShowCreateResume();
        }
    }

    private void ShowCreateResume()
    {
        PanelManager._Instance._createOrResume.OpenMe();
        xieyiPage.SetActive(false);
    }

    private void DetectionVersion()
    {
        if (NewWalletManager._Intance.IsNeedColdWallet)
        {
            logoImage.sprite = TextureUIAsset._Instance.cLogo;
        }
        else
        {
            logoImage.sprite = TextureUIAsset._Instance.hLogo;
        }

        List<KeyValuePair<string, string>> ws = new List<KeyValuePair<string, string>>();
        ws.Add(new KeyValuePair<string, string>("op", "getversion"));
        if (!NewWalletManager._Intance.IsNeedColdWallet)
        {
            ws.Add(new KeyValuePair<string, string>("hot", "1"));
        }
        StartCoroutine(HttpManager._Intance.SendRequest(ws, (o) =>
        {
            int targetVersion = int.Parse(o["versioncode"].ToString());
            bool maintenance = o["maintenance"].ToString() == "0" ? true : false;
            if (targetVersion > versionCode)
            {
                loginObject.userName.gameObject.SetActive(false);
                bool needUpdate = o["needupdate"].ToString() == "0" ? true : false;
                string downloadLink = o["downloadlink"].ToString();
                string versionDes = o["description"].ToString();
                if (needUpdate)
                {
                    PopUpBox.Instance.Show(() =>
                    {
                        Application.OpenURL(downloadLink);
                        Application.Quit();
                    }, null, "马上更新", "", "发现新版本", versionDes + "旧版本将无法支持此次更新的重要功能，请您及时下载最新版本。");
                }
                else
                {
                    PopUpBox.Instance.Show(() =>
                    {
                        Application.OpenURL(downloadLink);
                    }, () =>
                    {
                        StartApp();
                        //loginObject.userName.gameObject.SetActive(true);
                    }, "马上更新", "以后再说", "发现新版本", versionDes);
                }
            }
            else if (maintenance)
            {
                loginObject.userName.gameObject.SetActive(false);
                string maintenance_description = o["maintenance_description"].ToString();
                PopUpBox.Instance.Show(() =>
                {
                    Application.Quit();
                }, null, "我知道了", "", "停机维护", maintenance_description);
            }
            else
            {
                StartApp();
            }
        }));
    }

    private void StartApp()
    {
        if (PlayerPrefs.HasKey("firstopenshowsplash"))
        {
            InitApp();
        }
        else
        {
            if (PlayerPrefs.HasKey("ResetLoginAgain"))
            {
                ShowCreateResume();
            }
            else
            {
                xieyiPage.SetActive(true);
            }
               
        }
    }


    public void InitApp()
    {
        PlayerPrefs.SetInt("firstopenshowsplash", 1);
        Open();
    }

    public override void Open()
    {
    //    OnLoginPanel(); 
        base.Open();
        ///第一个是账户名，第二个是密码//008615923342964
        //if (NewWalletManager._Intance.isDelPlayerPrefs && NewWalletManager._Intance.IsNeedColdWallet)
        //{
        //    HttpManager._Intance.loginInfo = new string[2] { "008615923342964", "12345678q" };
        //}
        if (HttpManager._Intance.loginInfo.Length != 2)
        {
            string userName = System.Guid.NewGuid().ToString();
            string passworld = "";
            HttpManager._Intance.loginInfo = new string[2] { userName, passworld };
        }
        else
        {
            if (string.IsNullOrEmpty(HttpManager._Intance.loginInfo[0]))
            {
                string userName = System.Guid.NewGuid().ToString();
                string passworld = "";
                HttpManager._Intance.loginInfo = new string[2] { userName, passworld };
            }
        }
        AutoLogin(HttpManager._Intance.loginInfo);
        //Debug.Log(tableHash);
        //OnLoginSuccsee("008615923342964", "12345678q", Json.jsonDecode(tableHash) as Hashtable);

    }

    private void AutoLogin(string[] infos)
    {
        List<KeyValuePair<string, string>> ws = new List<KeyValuePair<string, string>>();
        ws.Add(new KeyValuePair<string, string>("op", "login"));
        ws.Add(new KeyValuePair<string, string>("userName", infos[0]));
        HttpManager._Intance.StartCoroutine(HttpManager._Intance.SendRequest(ws, AutoLoginCallback));
    }

    private void AutoLoginCallback(Hashtable jsonData)
    {
        int re = System.Convert.ToInt32(jsonData["error"]);
        if (re == 0)
        {
            string[] infos = new string[2] { HttpManager._Intance.loginInfo[0], HttpManager._Intance.loginInfo[1] };
            HttpManager._Intance.loginInfo = infos;
            PlayerPrefsX.SetStringArray(HttpManager.LoginMark, infos);
            OnLoginSuccsee(HttpManager._Intance.loginInfo[0], HttpManager._Intance.loginInfo[1], jsonData);
        }
    }

    public void OnLoginPanel()
    {
        PanelManager._Instance.OpenPanel(loginObject, registerObject);
    }
    public void OnRegisterPanel()
    {
        HideXieyiPage();
        PanelManager._Instance.OpenPanel(registerObject, loginObject);
        getPasswordText.text = "注 册";
        getPasswordFlag = false;
    }

    public void OpenGetPassWordPage()
    {
        PanelManager._Instance.OpenPanel(registerObject, loginObject);
        getPasswordText.text = "取回密码";
        getPasswordFlag = true;
    }

    public void OnVerifyPanel()
    {
        HideXieyiPage();
        PanelManager._Instance.OpenPanel(registerObject, loginObject);
    }

    public void OnLoginSuccsee(string userName, string passWorld, Hashtable table)
    {
        NewWalletManager._Intance.Init(userName, passWorld, table);

        PanelManager._Instance.OpenPanel(PanelManager._Instance._mainPanel, this);
    }

    public void OnReadProtol()
    {
        xieyiPage.SetActive(false);
        createOrResume.SetActive(true);
    }

    public void ShowXieyiPage()
    {
        xieyiPage.SetActive(true);
        isAgreeToggle.isOn = false;
        loginObject.gameObject.SetActive(false);
    }
    public void BackXieyi()
    {
        HideXieyiPage();
        loginObject.gameObject.SetActive(true);
    }
    public void HideXieyiPage()
    {
        xieyiPage.SetActive(false);
    }
    public void OnToggleChage()
    {
        if (isAgreeToggle.isOn)
        {
            RegisButton.interactable = true;
        }else
        {
            RegisButton.interactable = false;
        }
    }
}

