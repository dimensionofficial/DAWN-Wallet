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

public class SendScanSing : MonoBehaviour
{
    public Text toAddressText;
    
    public Text toCountText;
    public Text toCountRMBtext;

    public Text gasText;
    public Text gasRMBtext;

    public NativeEditBox commentNativeBox;
    public Text commentText;

    public const int lastCount = 7;

	public ShowManyQRImage showManyQRImage;

    public GameObject goOnObject;
    public Text goOnText;

    public GameObject reScanObject;

    public decimal costCount;
    public SendCoinPanel sendPanel;
    public WalletInfoPanel walletInfoPanel;
    public Text titleText;
    public Text noticeText;

    public GameObject qRObject;
    public Image qRImage;
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
    private string fromAddress;
    private string toAddress;
    private decimal free;
    private decimal toCount;
    string[] mulitPic = null;
    private bool isSameUnit;
    private string freeUnit;
    private string toCountUnit;
    private bool isETHerc20;
    private string commentInfo;
    public string contractAddress;

    public string tokenFullName;
    public string bigNumber;
    //多签判断
    public bool isMultiAddres = false;
    public Image titleColor;
    public Image title2Color;
    public Image title3Color;
    public Color normlColor2;
    public Color multiColor2;
    public Image titleImage;
    public Image title2Image;
    public Sprite normlSprite;
    public Sprite multiSprite;
    public void SetColor()
    {
        isMultiAddres = MultiJSData.instance.IsMultiSigAddress(walletInfoPanel.currentItem.coinInfo.address);
        if (isMultiAddres)
        {
            titleColor.color = multiColor2;
            title2Color.color = multiColor2;
            title3Color.color = multiColor2;
            titleImage.sprite = multiSprite;
            title2Image.sprite = multiSprite;
        }
        else
        {
            titleColor.color = normlColor2;
            title2Color.color = normlColor2;
            title3Color.color = normlColor2;
            titleImage.sprite = normlSprite;
            title2Image.sprite = normlSprite;
        }
    }
    /// <summary>
    /// 是否是支付
    /// </summary>
    private bool isPay = false;
    private string payAddress;
    private int goodsid;

    public void SetPayInfo(string address, int _goodsid)
    {
        isPay = true;
        goodsid = _goodsid;
        payAddress = address;
    }

    public void SetSendInfo(decimal _free, string toCountbigInt, decimal _toCount, string _fromAddress,  string _toaddress, string _freeUnit, string _toCountUnit, bool isSameUnit, string _commentInfo, string _tokenFullName)
    {
        SetColor();
        sendPanel.ActiveNativeEditBox(false);
        gameObject.SetActive(true);

        bigNumber = toCountbigInt;
        if (_freeUnit.Equals("ETH"))
        {
            if (!_toCountUnit.Equals("ETH"))
            {
                isETHerc20 = true;
            }
            else
            {
                isETHerc20 = false;
            }
        }
        else if (_freeUnit.Equals("BTC"))
        {
            isETHerc20 = false;
        }
        tokenFullName = _tokenFullName;
        fromAddress = _fromAddress;
        toAddress = _toaddress;
        free = _free;
        toCount = _toCount;
        freeUnit = _freeUnit;
        toCountUnit = _toCountUnit;
        if (isSameUnit)
        {
            costCount = free + toCount;
        }
        commentInfo = _commentInfo;
    }

  

    private Action<string> ScanEndBack;

    public void OnBackShowQR()
    {
        if (NewWalletManager._Intance.currentCoinType == NewWalletManager.CoinType.BTC)
        {
            sendPanel.feeSlider.gameObject.SetActive(false);
            sendPanel.sline.gameObject.SetActive(false);
        }
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


    public void ShowQR(string str, bool _isPay = false)
    {
        isPay = _isPay;
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
        SetColor();
        titleText.text = "扫描地址";
        noticeText.text = "扫描需要发送的二维码地址";
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
        if (NewWalletManager._Intance.currentCoinType == NewWalletManager.CoinType.BTC)
        {
            sendPanel.feeSlider.gameObject.SetActive(false);
            sendPanel.sline.gameObject.SetActive(false);
        }
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
        isScanning = false;
        ClosedCamera();
        inputInfoObject.gameObject.SetActive(true);
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
        if (MultiJSData.instance.IsMultiSigAddress(walletInfoPanel.currentItem.coinInfo.address))
        {
            if (sendPanel.surScanCount > 1)
            {
                sendPanel.surScanCount--;
                ClosedCamera();
                scanObject.SetActive(false);
                inputInfoObject.SetActive(true);
                if (string.IsNullOrEmpty(str))
                {
                    PopupLine.Instance.Show("扫描错误,请重新扫描");
                    toAddressText.text = "";
                    toCountText.text = "";
                    gasText.text = "";
                }
                else
                {
                    List<string> derList = QRPayTools.MulitSignInfo(str);
                    foreach (var item in derList)
                    {
                        Debug.Log("sig=" + item);
                        sendPanel.BtcMulsigBuilder.AddSig(sendPanel.curWalletPuk, item);
                    }
                    sendPanel.oldwallets.Add(sendPanel.curWalletPuk);
                    sendPanel.oldwalletsAddress.Add(sendPanel.curWalletAddress);
                    sendPanel.RefreshWallerList();
                }
            }
            else
            {
                OnScanEndOne(str);
            }
        }
        else
        {
            ClosedCamera();
            scanObject.SetActive(false);
            inputInfoObject.SetActive(true);
            sendObject.SetActive(true);
            if (string.IsNullOrEmpty(str))
            {
                PopupLine.Instance.Show("扫描错误,请重新扫描");
                toAddressText.text = "";
                toCountText.text = "";
                gasText.text = "";
            }
            else
            {
                strinfo = str;
                string one = toAddress.Substring(0, 10);
                string two = toAddress.Substring(toAddress.Length - 10, 10);
                toAddressText.text = one + "..." + two;
                toCountText.text = toCount.ToString() + " " + toCountUnit;
                gasText.text = free.ToString() + " " + freeUnit; ;
                commentText.text = commentInfo;
                gasRMBtext.text = sendPanel.feeMonyText.text;
                toCountRMBtext.text = sendPanel.moneyText.text;
            }
        }
    }
    public void OnScanEndOne(string str)
    {
        ClosedCamera();
        scanObject.SetActive(false);
        inputInfoObject.SetActive(true);
        sendObject.SetActive(true);
        if (string.IsNullOrEmpty(str))
        {
            PopupLine.Instance.Show("扫描错误,请重新扫描");

            toAddressText.text = "";
            toCountText.text = "";
            gasText.text = "";
        }
        else
        {
            List<string> derList = QRPayTools.MulitSignInfo(str);
            foreach (var item in derList)
            {
                Debug.Log("sig="+item);
                sendPanel.BtcMulsigBuilder.AddSig(sendPanel.curWalletPuk, item);
            }
            sendPanel.oldwallets.Add(sendPanel.curWalletPuk);
            sendPanel.oldwalletsAddress.Add(sendPanel.curWalletAddress);
            sendPanel.RefreshWallerList();
            string one = toAddress.Substring(0, 10);
            string two = toAddress.Substring(toAddress.Length - 10, 10);
            toAddressText.text = one + "..." + two;
            toCountText.text = toCount.ToString() + " " + toCountUnit;
            gasText.text = free.ToString() + " " + freeUnit; ;
            commentText.text = commentInfo;
            gasRMBtext.text = sendPanel.feeMonyText.text;
            toCountRMBtext.text = sendPanel.moneyText.text;
        }
    }

    public void OnClickOverBtn()
    {
        sendOverObject.gameObject.SetActive(false);
        this.gameObject.SetActive(false);
        sendPanel.ActiveNativeEditBox(true);
    }

    private void EosPay(string hash)
    {
        //http://47.96.131.169:8888/createorder?userid=26&address=1234567&hash=7893&goodid=1&sign=1dc86eb78e84ed92c9d24f53a2f16a4f
        List<KeyValuePair<string, string>> ws = new List<KeyValuePair<string, string>>();
        ws.Add(new KeyValuePair<string, string>("userid", NewWalletManager._Intance.userId));
        ws.Add(new KeyValuePair<string, string>("address", payAddress));
        ws.Add(new KeyValuePair<string, string>("hash", hash));
        ws.Add(new KeyValuePair<string, string>("goodid", goodsid.ToString()));
        HttpManager._Intance.StartCoroutine(HttpManager._Intance.GetNodeJsRequest("createorder", ws, (Hashtable data) =>
        {
            if (data != null)
            {
                EosItem eosIt = PanelManager._Instance._eosRegisterPanel._item;
                if(eosIt != null)
                    eosIt.InitEOS(eosIt.eosWalletInfo);
            }
        }));
    }

    private void ShowSendOverObject(bool isSuccess, string hashStr, string comment)
    {
        HttpManager._Intance.loadingPanel.SetActive(false);

        if (isSuccess)
        {
            if (isPay)
            {
                EosPay(hashStr);
            }
            else
            {
                if (walletInfoPanel.currentItem.type == NewWalletManager.CoinType.ETH)
                {
                    if (sendPanel.currentTokenItem != null && sendPanel.currentTokenItem.isToken)
                    {
                        decimal tempNuber = sendPanel.currentTokenItem.erc_Token.tokenNumber;
                        tempNuber -= toCount;
                        sendPanel.currentTokenItem.erc_Token.tokenNumber = tempNuber;
                        sendPanel.currentTokenItem.ShowETHToken(sendPanel.currentTokenItem.erc_Token);
                        sendPanel.ShowCount();
                    }
                    else
                    {
                        walletInfoPanel.currentItem.coinInfo.money -= costCount;
                        walletInfoPanel.UpdateMoney();
                        sendPanel.ShowCount();
                    }
                }
                else
                {
                    walletInfoPanel.currentItem.coinInfo.money -= costCount;
                    walletInfoPanel.UpdateMoney();
                    sendPanel.ShowCount();
                }

                walletInfoPanel.currentItem.GetHistory();
            }

            sendOverObject.Open(toAddress, toCount, free, isSuccess, hashStr, commentInfo, freeUnit, toCountUnit, isSameUnit);
            if (isPay)
            {
                sendOverObject.SetPayInfo(goodsid);
            }
            sendObject.SetActive(false);
            this.gameObject.SetActive(false);
            sendPanel.ActiveNativeEditBox(true);
        }
        else
        {
            sendPanel.ActiveNativeEditBox(false);
            PopUpBox.Instance.Show(() => {
                OnClickBack();
                sendPanel.walletPanel.SetActive(false);
                sendPanel.ActiveNativeEditBox(true);
            }, null, "确定", "", "转账失败", "可能因以下原因造成：\r\n1.矿工费用设置过低。\r\n2.签名数据过期          \r\n请重试                      ");
            //sendOverObject.Open(toAddress, toCount, free, isSuccess, hashStr, commentInfo, freeUnit, toCountUnit, isSameUnit);
        }
    }

    Transaction tx;
    public void OnClickSendOKBtn()
    {
        HttpManager._Intance.loadingPanel.SetActive(true);

        if (isPay)
        {
            StartCoroutine(SendETH());
        }
        else
        {
            if (NewWalletManager._Intance.currentCoinType == NewWalletManager.CoinType.BTC)
            {
                //判断是否是多签钱包
                if (MultiJSData.instance.IsMultiSigAddress(walletInfoPanel.currentItem.coinInfo.address))
                {
                    try
                    {
                        tx = sendPanel.BtcMulsigBuilder.Finish();
                        QBitNinja4Unity.QBitNinjaClient.Broadcast(tx, NewWalletManager._Intance.btcNetwork, Send3);
                    }
                    catch (Exception ex)
                    {
                        ShowSendOverObject(false, tx.GetHash().ToString(), commentInfo);
                    }
                }
                else
                {
                    try
                    {
                        tx = new NBitcoin.Transaction(strinfo);
                        QBitNinja4Unity.QBitNinjaClient.Broadcast(tx, NewWalletManager._Intance.btcNetwork, Send3);
                    }
                    catch (Exception ex)
                    {
                        ShowSendOverObject(false, tx.GetHash().ToString(), commentInfo);
                    }
                }
            }
            else if (NewWalletManager._Intance.currentCoinType == NewWalletManager.CoinType.USDT)
            {
                try
                {
                    List<string> derList = QRPayTools.MulitSignInfo(strinfo);
                    for (int i = 0; i < derList.Count; i++)
                    {
                        Debug.Log("sig=" + derList[i]);
                        sendPanel.inputVos[i].signature= derList[i];
                    }
                    sendPanel.PostInputSig(SendUsdt);
                }
                catch (Exception ex)
                {
                    ShowSendOverObject(false, tx.GetHash().ToString(), commentInfo);
                }
            }
            else if (NewWalletManager._Intance.currentCoinType == NewWalletManager.CoinType.ETH)
            {
                StartCoroutine(SendETH());
            }
        }
    }

    IEnumerator SendETH()
    {
        if (!NewWalletManager._Intance.isTestSend)
        {
            Hashtable myJsonData = new Hashtable();
            myJsonData["id"] = 1;
            myJsonData["jsonrpc"] = "2.0";
            myJsonData["method"] = "eth_sendRawTransaction";

            ArrayList arrayList = new ArrayList();
            string oxStr = "0x" + strinfo;
            arrayList.Add(oxStr);
            myJsonData["params"] = arrayList;
            string rpcRequestJson = Json.jsonEncode(myJsonData);
            UnityWebRequest unityRequest = QRPayTools.GetUnityWebRequest(rpcRequestJson);
            yield return unityRequest.SendWebRequest();
            if (unityRequest.error != null)
            {
                sendOverBtn.SetActive(true);
            }
            else
            {
                string r = unityRequest.downloadHandler.text;
                byte[] results = unityRequest.downloadHandler.data;
                string responseJson = Encoding.UTF8.GetString(results).ToString();
                Hashtable table = Json.jsonDecode(responseJson) as Hashtable;
                if (table.Contains("error"))
                {
                    string str1 = Json.jsonEncode(table["error"]);
                    Debug.Log(str1);
                    Hashtable table1 = Json.jsonDecode(str1) as Hashtable;
                    Debug.Log(table1["message"].ToString());

                    string hashStr = "";
                    if (table.ContainsKey("result"))
                    {
                        hashStr = table["result"].ToString();
                    }

                    ShowSendOverObject(false, hashStr, commentInfo);
                }
                else
                {
                    AddLocalContain(table["result"].ToString(), toCountUnit, contractAddress, tokenFullName);
                    ShowSendOverObject(true, table["result"].ToString(), commentInfo);
                }
            }
        }
        else
        {
            AddLocalContain("fdasfasdfdsafasfadhlafjdka", toCountUnit, contractAddress, tokenFullName);
            ShowSendOverObject(true, "fdasfasdfdsafasfadhlafjdka", commentInfo);
        }

    }

    public void AddLocalContain(string hash, string erc20Name, string contractAddress, string tokenFullName)
    {
        Hashtable hs = new Hashtable();
        TimeSpan cha = (DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)));
        long t = (long)cha.TotalSeconds;
        hs["timeStamp"] = t.ToString();
        hs["hash"] = hash;
        hs["from"] = fromAddress;
        hs["to"] = toAddress.ToString();
        hs["value"] = bigNumber.ToString();
        hs["gas"] = free.ToString();
        hs["tokenSymbol"] = erc20Name;
        hs["contractAddress"] = contractAddress;
        hs["confixmations"] = 0 + "";
        hs["fullName"] = tokenFullName;
        if (toCountUnit.Equals("ETH"))
        {
            hs["input"] = "0x";
        }
        
        string jsonValue = Json.jsonEncode(hs);
        string[] tempInfo;
        bool isUsdt = false;
        if (string.IsNullOrEmpty(tokenFullName) && tokenFullName.Equals("USDT"))
        {
            isUsdt = true;
            tempInfo = PlayerPrefsX.GetStringArray(fromAddress + "USDT" + "LocalHistroy");
        }
        else
        {
            tempInfo = PlayerPrefsX.GetStringArray(fromAddress + "LocalHistroy");
        }
        
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

        if (isUsdt)
        {
            PlayerPrefsX.SetStringArray(fromAddress + "USDT" + "LocalHistroy", tempList.ToArray());
        }
        else
        {
            PlayerPrefsX.SetStringArray(fromAddress + "LocalHistroy", tempList.ToArray());
        }
       
    }

    //获得交易结果
    void Send3(QBitNinja.Client.Models.BroadcastResponse broadcastResponse, NBitcoin.Network network)
    {

        if (broadcastResponse.Error == null)
        {
            AddLocalContain(tx.GetHash().ToString(), "", "", "");
            ShowSendOverObject(true, tx.GetHash().ToString(), commentInfo);
            UnityEngine.Debug.Log("Success! You can check out the hash of the transaciton in any block explorer:");
            return;
        }
        else
        {
            UnityEngine.Debug.Log(string.Format("ErrorCode: {0}", broadcastResponse.Error.ErrorCode));
            UnityEngine.Debug.Log("Error message: " + broadcastResponse.Error.Reason);
            ShowSendOverObject(false, tx.GetHash().ToString(), commentInfo);
        }
    }

    void SendUsdt(string result,bool isOk)
    {
        Debug.Log("result===========:"+result);
        if (isOk)
        {
            AddLocalContain(result, "", "", "USDT");
        }
        ShowSendOverObject(isOk, result, commentInfo);
    }



}
