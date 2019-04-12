using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Threading;
using NBitcoin;
using Nethereum.JsonRpc.UnityClient;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using Nethereum.Hex.HexConvertors.Extensions;
using System.Text;
using Newtonsoft.Json.Linq;

public class EOSSendScanSing : MonoBehaviour
{
    public enum LastPanel
    {
        none,
        /// <summary>
        /// 交易EOS
        /// </summary>
        transfer,
        /// <summary>
        /// 抵押
        /// </summary>
        delegatebw,
        /// <summary>
        /// 买内存（直接买多少内存）
        /// </summary>
        buyrambytes,
        /// <summary>
        /// 买内存（用EOS买）
        /// </summary>
        buyram,
        /// <summary>
        /// 取消内存
        /// </summary>
        undelegatebw,
        /// <summary>
        /// 出售内存
        /// </summary>
        sellram,
        /// <summary>
        /// 投票
        /// </summary>
        voteproducer,
    }

    public LastPanel lastPanel = LastPanel.none;

    public Text toAddressText;
    
    public Text toCountText;
    public Text toCountRMBtext;

 // public NativeEditBox commentNativeBox;
    public Text commentText;

    public const int lastCount = 7;

	public ShowManyQRImage showManyQRImage;

    public GameObject goOnObject;
    public Text goOnText;

    public GameObject reScanObject;

    public decimal costCount;
    public EOSSendCoinPanel sendPanel;
    public Text titleText;
    public Text noticeText;

    public GameObject qRObject;

    public SendOverPanel sendOverObject;
    public Text sendOverText;

    public GameObject sendObject;
    public Text singInfoText;
    public GameObject sendOverBtn;
    Result result = null;
    public ScanBar m_bar;
    public GameObject scanObject;
    public GameObject inputInfoObject;

    public RawImage qrCameraImage;
    private WebCamTexture webCamTexture;
    bool isScanning;
    public string toAddress;
    private string toCount;
    public string unit;
    string[] mulitPic = null;
    private bool isSameUnit;
    private string toCountUnit;
    private string commentInfo;
    public string bigNumber;

    private Hashtable htable;

    public Text confirmTitle;
    public Text confirmBtnText;
    public GameObject transferObjct;
    public GameObject voteproducerObjct;
    public GameObject voteproducerChangeObject;

    public Action sendCallBack;

    /// <summary>
    /// 发送界面
    /// </summary>
    /// <param name="currenPanl"></param>
    /// <param name="hash"></param>
    /// <param name="_toCount"></param>
    /// <param name="toAccount"></param>
    /// <param name="fromAccount"></param>
    /// <param name="comment"></param>
    public void SetSendInfo(LastPanel currenPanl, Hashtable hash, string _toCount, string toAccount, string fromAccount, string comment, string _unit, Action _sendCallBack = null)
    {
        this.gameObject.SetActive(true);
        lastPanel = currenPanl;
        htable = hash;
        toCount = _toCount;
        toAddress = toAccount;
        commentInfo = comment;
        sendCallBack = _sendCallBack;
        unit = _unit;
    }

  

    private Action<string> ScanEndBack;

    public void OnBackShowQR()
    {
        showManyQRImage.ClosedMe();
        gameObject.SetActive(false);
        sendPanel.ActiveNativeEditBox(true);
    }

    public void NextBtn()
    {
		showManyQRImage.ClosedMe ();
        titleText.text = "扫描签名数据";
        noticeText.text = "扫描冷钱包签名数据二维码";
        qRObject.SetActive(false);
        scanObject.SetActive(true);
        inputInfoObject.SetActive(false);
        sendObject.SetActive(false);
        strinfo = "";
        ScanEndBack = OnScanEnd;
        OnScan(false);
    }

    public void ShowQR(string str)
    {
        ScanEndBack = null;
        this.gameObject.SetActive(true);
        sendPanel.ActiveNativeEditBox(false);

        showManyQRImage.ShowMangQr(str);
        scanObject.SetActive(false);
        inputInfoObject.SetActive(true);
        qRObject.SetActive(true);
        sendObject.SetActive(false);

    }

    public void ClosedCamera()
    {
        if (webCamTexture != null)
        {
            if (webCamTexture.isPlaying)
                webCamTexture.Stop();
            webCamTexture = null;
        }  
    }

    public void OnClickScanSendAddress(Action<string> callBack)
    {
        titleText.text = "扫描账户";
        noticeText.text = "扫描需要发送的EOS账户";
        ScanEndBack = null;
        this.gameObject.SetActive(true);
        sendPanel.ActiveNativeEditBox(false);

        ScanEndBack = callBack;
        scanObject.SetActive(true);
        inputInfoObject.SetActive(false);
        qRObject.SetActive(false);
        sendObject.SetActive(false);
        OnScan(true);
    }

    public void OnClickBack()
    {
        strinfo = "";
        ClosedCamera();
        this.gameObject.SetActive(false);
        sendPanel.ActiveNativeEditBox(true);
        inputInfoObject.SetActive(true);
    }

    void OnScan(bool isScanAddress)
    {
        
        mulitPic = null;

        if (webCamTexture == null)
        {
            webCamTexture = new WebCamTexture(Screen.width, Screen.height);
        }
            

        if (isScanning)
            StopCoroutine(Scanning(isScanAddress));

        if (webCamTexture != null)
        {
            webCamTexture.Play();
            qrCameraImage.texture = webCamTexture;

            qrCameraImage.gameObject.SetActive(true);

            StartCoroutine(Scanning(isScanAddress));
        }
    }

    IEnumerator Scanning(bool isScanAddress)
    {
        m_bar.StartDo();
        isScanning = true;
        IBarcodeReader iBR = new BarcodeReader();

        while (webCamTexture.width == 16)
            yield return null;

        result = iBR.Decode(webCamTexture.GetPixels32(), webCamTexture.width, webCamTexture.height);
        int dot = 0;

        float height = 1600F;
        float width = 900F;

        if (Camera.main.pixelHeight > height)
            height = Camera.main.pixelHeight;
        if (Camera.main.pixelWidth > width)
            width = Camera.main.pixelWidth;

        Vector2 scaleTo = new Vector2(webCamTexture.width, webCamTexture.height);
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

        if (isScanAddress)
        {
            isScanning = false;
            ClosedCamera();
            if (ScanEndBack != null)
            {
                ScanEndBack(result.Text);
                ScanEndBack = null;
            }
        }
        else
        {

            try
            {
                string count = result.Text.Substring(result.Text.Length - lastCount, lastCount);
                string[] num = count.Split('/');
                int cur = int.Parse(num[0]);
                int tol = int.Parse(num[1]);
                if (mulitPic == null)
                {
                    mulitPic = new string[tol];
                }
                if (mulitPic.Length != tol)
                {
                    throw new System.Exception("error qr");
                }
                else
                {
                    bool isReadScan = false;
                    if (!string.IsNullOrEmpty(mulitPic[cur - 1]))
                    {
                        isReadScan = true;
                    }
                    mulitPic[cur - 1] = result.Text.Substring(0, result.Text.Length - lastCount);
                    if (cur == tol)
                    {
                        string resultData = "";
                        foreach (var v in mulitPic)
                        {
                            resultData += v;
                        }

                        isScanning = false;
                        ClosedCamera();

                        if (ScanEndBack != null)
                        {
                            ScanEndBack(resultData);
                            ScanEndBack = null;
                        }
                    }
                    else
                    {
                        goOnObject.gameObject.SetActive(true);
                        if (isReadScan)
                        {
                            goOnText.text = cur + " / " + tol + "扫描完成\r\n此二维码已扫过，请继续下一张";
                        }
                        else
                        {
                            goOnText.text = cur + " / " + tol + "扫描完成\r\n请扫描下一张二维码";
                        }   
                    }
                }
            }
            catch (System.Exception e)
            {
                reScanObject.SetActive(true);
            }
        }
    }

    public void OnClickReScanBtn()
    {
        reScanObject.SetActive(false);
        StartCoroutine(Scanning(false));
    }
    public void OnClickCancel()
    {
        reScanObject.SetActive(false);
        isScanning = false;
        ClosedCamera();
        scanObject.SetActive(false);
    }

    public void OnClickGoOnScan()
    {
        goOnObject.gameObject.SetActive(false);
        StartCoroutine(Scanning(false));
    }

    public string strinfo;

    public void OnScanEnd(string str)
    {
        ClosedCamera();
        scanObject.SetActive(false);
        inputInfoObject.SetActive(true);
        sendObject.SetActive(true);
      
        voteproducerObjct.SetActive(false);
        voteproducerChangeObject.SetActive(false);
        if (lastPanel == LastPanel.voteproducer)
        {
            transferObjct.SetActive(false);
            EosItem item = PanelManager._Instance._eosWalletInfoPanel.currentItem as EosItem;
            if (item.eosWalletInfo.votedList.Count > 0)
            {
                voteproducerChangeObject.SetActive(true);
            }
            else
            {
                voteproducerObjct.SetActive(true);
            }
            confirmBtnText.text = "确认投票";
        }
        else
        {
            transferObjct.SetActive(true);
            confirmBtnText.text = "发送交易";
        }


        if (string.IsNullOrEmpty(str))
        {
            PopupLine.Instance.Show("扫描错误,请重新扫描");
            toAddressText.text = "";
            toCountText.text = "";
        }
        else
        {
            strinfo = str;
            toAddressText.text = toAddress;
            toCountText.text = toCount.ToString() + " " + unit;
            commentText.text = commentInfo;
            toCountRMBtext.text = sendPanel.moneyText.text;
        }
    }

    public void OnClickOverBtn()
    {
        sendOverObject.gameObject.SetActive(false);
        this.gameObject.SetActive(false);
        sendPanel.ActiveNativeEditBox(true);
    }



    public void OnClickSendOKBtn()
    {
        //SendSuccess();
        PanelManager._Instance._eosResourcesPanel.loadingPanel.SetActive(true);
        EosGetSingInfoPanel.Instance.Pushtransaction(htable, strinfo, delegate(Hashtable t)
        {
            PanelManager._Instance._eosResourcesPanel.loadingPanel.SetActive(false);
            if (t != null && t.ContainsKey("error"))
            {
                PopUpBox.Instance.Show(null, null, "确定", "", "错误提示", t["error"].ToString());
                return;
            }
            
            SendSuccess();
        }, delegate()
        {
            PanelManager._Instance._eosResourcesPanel.loadingPanel.SetActive(false);
        }, false);
       
    }


    private void SendSuccess()
    {
       
        sendObject.SetActive(false);
        this.gameObject.SetActive(false);

        if (sendCallBack != null)
            sendCallBack();

        CoinInfoItemBase ite = PanelManager._Instance._eosWalletInfoPanel.currentItem;
        EosItem eosIt = null;
        if (ite != null)
        {
            eosIt = ite as EosItem;
        }
        else
        {
            eosIt = PanelManager._Instance._eosRegisterPanel._item;
        }
      
        if (eosIt != null)
        {
            SendOverLater(eosIt);
        }

        

        switch (lastPanel)
        {
            case LastPanel.transfer:
                PanelManager._Instance._eosSendCoinPanel.gameObject.SetActive(false);
                sendPanel.ActiveNativeEditBox(true);
                sendOverObject.OpenEos(toAddress, toCount, commentInfo);
                break;

            case LastPanel.buyram:
            case LastPanel.buyrambytes:
            case LastPanel.delegatebw:
            case LastPanel.sellram:
            case LastPanel.undelegatebw:
                PanelManager._Instance._eosResourcesPanel.resourcesOverPanel.Show(lastPanel, toCountText.text, toAddress, commentInfo);
                PanelManager._Instance._eosResourcesPanel.ReSet();
                break;
        }
   
    }

    private void SendOverLater(EosItem item)
    {
        item.InitEOS(item.eosWalletInfo);
    }

    public void AddLocalContain(string hash, string erc20Name, string contractAddress, string tokenFullName)
    {
        Hashtable hs = new Hashtable();
        TimeSpan cha = (DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)));
        long t = (long)cha.TotalSeconds;
        hs["timeStamp"] = t.ToString();
        hs["hash"] = hash;
        hs["from"] = PanelManager._Instance._WalletInfoPanel.currentItem.coinInfo.address;
        hs["to"] = toAddress.ToString();
        hs["value"] = bigNumber.ToString();
 //     hs["gas"] = free.ToString();
        hs["tokenSymbol"] = erc20Name;
        hs["contractAddress"] = contractAddress;
        hs["confixmations"] = 0 + "";
        hs["fullName"] = tokenFullName;
        string jsonValue = Json.jsonEncode(hs);
        Debug.Log(hash + " : " + erc20Name);
        string[] tempInfo = PlayerPrefsX.GetStringArray(PanelManager._Instance._WalletInfoPanel.currentItem.coinInfo.address + "LocalHistroy");
        List<string> tempList;

        if (tempInfo.Length > 0)
        {
            tempList = new List<string>(tempInfo);
        }
        else
        {
           tempList = new List<string>();
        }
        tempList.Add(jsonValue);

        Debug.Log("代币发送本地保存 = " +  jsonValue);

        PlayerPrefsX.SetStringArray(PanelManager._Instance._WalletInfoPanel.currentItem.coinInfo.address + "LocalHistroy", tempList.ToArray());
    }
}
