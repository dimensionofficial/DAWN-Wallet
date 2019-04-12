using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using System;
using System.Threading;
using HardwareWallet;
using NBitcoin.Crypto;
using SuperScrollView;


public class ScanAddress : MonoBehaviour
{
    public PageViewDemoScript guideObject;
    public Transform pageParent;
    private GameObject clonePageView;

    public ScanBar m_bar;
    public RawImage qrCameraImage;
    public WebCamTexture webCamTexture;
    bool isScanning;
    Result result = null;
    public GameObject obj1;
    public GameObject obj2;

    public GameObject resultObject;
    public Text resultText;

    private float duratime = 1;
    private float currentDuration = 0;
    private bool isStartScan = false;


    string btcAddress;
    string ethAddress;
    string eosAddress_owner;
    string eosAddress_admin;

    string walletName; //钱包名称
    string machineid; //机器id


    private void CreatPageViewDemoScript()
    {
        clonePageView = Instantiate(guideObject.gameObject);
        clonePageView.SetActive(true);
        clonePageView.transform.SetParent(pageParent, false);
    }

    public void Open()
    {
        PanelManager._Instance.currentSubPage = PanelManager.SubPage.WalletMatching;
        this.gameObject.SetActive(true);
        obj2.SetActive(false);
       
        resultObject.SetActive(false);
        qrCameraImage.gameObject.SetActive(false);

        if (PanelManager._Instance._mainPanel.isNoone)
        {
            isStartScan = true;
            CreatPageViewDemoScript();
            obj1.SetActive(false);
        }
        else
        {
            obj1.SetActive(true);
            guideObject.gameObject.SetActive(false);
            isStartScan = false;
            currentDuration = 0;
        }
    }


    private void ImmediatelyScan(bool _isCommon)
    {
        isCommon = _isCommon;

        obj1.SetActive(false);
        obj2.SetActive(true);

        isStartScan = true;
        qrCameraImage.gameObject.SetActive(true);
        webCamTexture = new WebCamTexture(Screen.width, Screen.height);
        OnScan();
    }
    public void OnClickGuideBtn()
    {
        OnClickBackPageGuide();
        ImmediatelyScan(false);
    }

    private void OnClickBackPageGuide()
    {
        if (clonePageView != null)
            Destroy(clonePageView);

        clonePageView = null;
    }
    public bool isTest = false;
    void Update()
    {
        currentDuration += Time.deltaTime;
        if (currentDuration >= duratime && isStartScan == false)
        {
            ImmediatelyScan(false);
        }

        if (isTest)
        {
            isTest = false;
            string str ="593F406FF5DEC41AF07E77A64EDFF5C34A0DDD31E41143BE09DB71D4A191220472C6330D24AF98D144FD4511C56579C7FE46F2671A9386CBA16B339430004A3BEAEE7092CE836C6829992CB87D370E615FFB1B4BD556941DC42C958E87B05874BABB12FEA86239F70015A038E1CD44837BDAFDBC5DCF9002E73A16B734B7EAAD6E44662526CCF32E53B5D9659F8558F8341DF77F3EB1E93AEA59FD8E9F5DA4D38FD981D73A30356D5437333880B50F3D1C82C013043BD3B246D8190AC645B27F";

            byte[] key = Hashes.SHA256(System.Text.UTF8Encoding.UTF8.GetBytes("fdsamcldi123sawqa"));
            byte[] tempAddress = QRPayTools.HexToByte(str);
            byte[] addressDecode = Encry.DecryData(tempAddress, key);
            if (addressDecode != null && addressDecode.Length > 0)
            {
                string address = System.Text.UTF8Encoding.UTF8.GetString(addressDecode);
                Debug.Log(address);
                ShowScanAddress(address);
            }
        }
    }

    private bool isCommon;
    public Action<string> ScanEndCallBack;

    public void SetScanEndCallBack(bool _isCommon, Action<string> callback)
    {
        ScanEndCallBack = callback;
        ImmediatelyScan(_isCommon);
    }



    public void HideMe()
    {
        PanelManager._Instance._mainPanel.settingPanel.gameObject.SetActive(false);
        PanelManager._Instance._WalletInfoPanel.currentItem = null;
        PanelManager._Instance._WalletInfoPanel.OnClickBackBtn();
        PanelManager._Instance._mainPanel.currentBtn = null;
        PanelManager._Instance.currentSubPage = PanelManager.SubPage.Property;
		PanelManager._Instance._mainPanel.OnClickMainBtn ();
		/*
        PanelManager._Instance._mainPanel.mainSelect.SetActive(true);
        PanelManager._Instance._mainPanel.mainNormal.SetActive(false);
        PanelManager._Instance._mainPanel.settingSelect.SetActive(false);
        PanelManager._Instance._mainPanel.settingNormal.SetActive(true);
        PanelManager._Instance._mainPanel.bottomBtn.SetActive(true);
       
        PanelManager._Instance._mainPanel.ShowTitle();
         */
        this.gameObject.SetActive(false);
        ClosedCamera();
        webCamTexture = null;
    }


    private void ClosedCamera()
    {
        if (webCamTexture != null)
        {
            if (webCamTexture.isPlaying)
                webCamTexture.Stop();
        }
    }

    public void OnClickBack()
    {
        OnClickBackPageGuide();
        HideMe();
    }

    void OnScan()
    {
        if (isScanning)
            StopCoroutine(Scanning());

        if (webCamTexture != null)
        {
            
            qrCameraImage.texture = webCamTexture;

            qrCameraImage.gameObject.SetActive(true);

            StartCoroutine(Scanning());
        }
    }

    IEnumerator Scanning()
    {
        m_bar.StartDo();
        webCamTexture.Play();
        isScanning = true;
        IBarcodeReader iBR = new BarcodeReader();
        while (webCamTexture.width == 16)
            yield return null;

        result = iBR.Decode(webCamTexture.GetPixels32(), webCamTexture.width, webCamTexture.height);
        int dot = 0;
        Vector2 scaleTo = new Vector2(webCamTexture.width, webCamTexture.height);
        
        float height = 1600F;
        float width = 900F;

        if (Camera.main.pixelHeight > height)
            height = Camera.main.pixelHeight;
        if (Camera.main.pixelWidth > width)
            width = Camera.main.pixelWidth;

        float scale = height * 1.0F / webCamTexture.width;
        scaleTo *= scale;
        if (scaleTo.y < width)
        {
            scale = width / scaleTo.y;
            scaleTo *= scale;
        }
        qrCameraImage.rectTransform.sizeDelta = scaleTo;
        qrCameraImage.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        qrCameraImage.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        qrCameraImage.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        qrCameraImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scaleTo.x);
        qrCameraImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scaleTo.y);
#if UNITY_ANDROID   
        qrCameraImage.rectTransform.localEulerAngles = new Vector3(0,0, -90);
#endif
#if UNITY_IPHONE
        qrCameraImage.rectTransform.localEulerAngles = new Vector3(0, 0, 90);
        qrCameraImage.rectTransform.localScale = new Vector3(-1, 1, 1);
#endif

        while (result == null)
        {
            if (dot % 2 == 0)
            {
                Color32[] colors = webCamTexture.GetPixels32();
                int w = webCamTexture.width;
                int h = webCamTexture.height;
                dot = 1;
                ThreadPool.QueueUserWorkItem((o) => {
                    result = iBR.Decode(colors, w, h);
                    dot = 0;
                }, null);
            }
            yield return new WaitForFixedUpdate();
        }

        isScanning = false;

        OnScanEnd(result.Text);
    }
  

    private void OnScanEnd(string str)
    {
        if (isCommon)
        {
            if (ScanEndCallBack != null)
                ScanEndCallBack(str);

            ScanEndCallBack = null;
            isCommon = false;
        }
        else
        {
            try
            {
                byte[] key = Hashes.SHA256(System.Text.UTF8Encoding.UTF8.GetBytes("fdsamcldi123sawqa"));
                byte[] tempAddress = QRPayTools.HexToByte(str);
                byte[] addressDecode = Encry.DecryData(tempAddress, key);
                if (addressDecode != null && addressDecode.Length > 0)
                {
                    string address = System.Text.UTF8Encoding.UTF8.GetString(addressDecode);
                    ShowScanAddress(address);
                }

            }
            catch (Exception ex)
            {
                ShowResult(false, "无法识别的二维码");
            }
        }     
    }

    

    private void ShowResult(bool isExitScan, string info)
    {
        resultObject.gameObject.SetActive(true);
		resultText.text = info;
        ClosedCamera();
        TimerManager.Instance.AddTimer(3, delegate 
        {
            if (isExitScan)
            {
                HideMe();
            }
            else
            {
                resultObject.SetActive(false);
                OnClickGoOnScan();
            }
        }); 
    }
    public void OnClickGoOnScan()
    {
        resultObject.SetActive(false);
        StartCoroutine(Scanning());
    }


    private void ShowScanAddress(string str)
    {
        string[] scanInfos = str.Split('&');
        if (scanInfos.Length != 4)
        {
            ShowResult(false, "无法识别的二维码");
            return;
        }

        string k = scanInfos[1];
        WalletTools tools = new WalletTools();

        btcAddress = tools.GetBTCPubAddress(k);
        ethAddress = tools.GetETHPubAddress(k);
        eosAddress_owner = tools.GetEOSPubAddress_owner(k);
        eosAddress_admin = tools.GetEOSPubAddress_admin(k);

        walletName = "SN " + scanInfos[0]; //钱包名称
        machineid = scanInfos[2]; //机器id



        ///相同地址
        if (NewWalletManager._Intance.IsSameAsAddress(btcAddress, NewWalletManager.CoinType.BTC) &&
            NewWalletManager._Intance.IsSameAsAddress(ethAddress, NewWalletManager.CoinType.ETH) &&
            NewWalletManager._Intance.IsSameAsAddress(eosAddress_admin, NewWalletManager.CoinType.EOS))
        {
            ShowResult(false, "已匹配过此钱包");
            return;
        }

        if (NewWalletManager._Intance.JudgeBTCAddress(btcAddress) != null &&
           NewWalletManager._Intance.JudgeETHAddress(ethAddress) &&
           NewWalletManager._Intance.JudgeEosAddress(eosAddress_admin))
        {
            SaveAddress();
            NewWalletManager._Intance.SavaBtcPubkey(btcAddress.ToLower(), k);
            NewWalletManager._Intance.SavaUsdtPubkey(btcAddress, k); 
        }
        else
        {
            ShowResult(false, "无法识别的二维码");
        }
    }

    


    /// <summary>
    /// 
    /// </summary>
    /// <param name="v"></param>
    /// <param name="1 比特币，2 以太坊，3 EOS"></param> 
    private void SaveAddress()
    {
        Hashtable h = new Hashtable();
        h["BTC"] = btcAddress;
        h["ETH"] = ethAddress;
        h["EOSowner"] = eosAddress_owner;
        h["EOSadmin"] = eosAddress_admin;
        h["walletName"] = walletName;
        h["machineid"] = machineid;
        string jsonstring = Json.jsonEncode(h);
        List<KeyValuePair<string, string>> ws = new List<KeyValuePair<string, string>>();
        ws.Add(new KeyValuePair<string, string>("op", "addAddress"));
        ws.Add(new KeyValuePair<string, string>("userName", NewWalletManager._Intance.userName));
        ws.Add(new KeyValuePair<string, string>("addressInfo", jsonstring));
     
        StartCoroutine(HttpManager._Intance.SendRequest(ws, delegate (Hashtable table)
        {
            NewWalletManager._Intance.SaveAddress(jsonstring);

            PanelManager._Instance._mainPanel.AddBitItem(new string[2] { btcAddress, walletName});
            PanelManager._Instance._mainPanel.AddUsdtItem(new string[2] { btcAddress, walletName });

            PanelManager._Instance._mainPanel.AddEthItem(new string[2] { ethAddress, walletName });

            PanelManager._Instance._mainPanel.AddEosItem(new string[2] { eosAddress_admin, walletName });

            ShowResult(true, "配对成功");
        }));
    }

}
