using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiSignatureManager : MonoBehaviour {
    public static MultiSignatureManager instance;
    public RectTransform menuMain;
    public RectTransform specificationPanel;
    public RectTransform createAccountPanel;
    public RectTransform restoreAccountPanel;
    public RectTransform authorizeQRPanel;

    RectTransform curPanel;
    RectTransform oldPanel;

    public RectTransform parent_authorizeList;
    public  string chooseWallet;
    public GameObject authorizeQRPrefab;
    private List<GameObject> authorizeList;
    public Image authorizeQRImg;
    private void Awake()
    {
        instance = this;
    }
    // Use this for initialization
    void Start () {
        authorizeList = new List<GameObject>();
        curPanel = menuMain;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// 打开授权二维码页面
    /// </summary>
    public void Btn_AuthorizeQR()
    {
        ShowBTCList();
    }
    public void Btn_specification()
    {
        PageDown(specificationPanel);
    }
   
    /// <summary>
    /// 展示BTC钱包列表
    /// </summary>
    private void ShowBTCList()
    {
        foreach (var v in PanelManager._Instance._mainPanel.btcitemList)
        {
            if (!MultiJSData.instance.IsMultiSigAddress(v.Value.coinInfo.address))
            {
                CreateAuthorizeList(v.Value.coinInfo.walletname);
            }
        }
        Btn_CreateQR();
    }
    private void CreateAuthorizeList(string sn)
    {
        GameObject gameObject=Instantiate(authorizeQRPrefab, parent_authorizeList);
        gameObject.transform.Find("Text").GetComponent<Text>().text = sn;
        gameObject.transform.Find("Toggle").GetComponent<Toggle>().group = parent_authorizeList.GetComponent<ToggleGroup>();
        authorizeList.Add(gameObject);
    }
    /// <summary>
    /// 生成授权二维码
    /// </summary>
    public void Btn_CreateQR()
    {
        foreach (string address in PanelManager._Instance._mainPanel.btcitemList.Keys)
        {
            if (PanelManager._Instance._mainPanel.btcitemList[address].coinInfo.walletname==chooseWallet)
            {
                try
                {
                    string key = NewWalletManager._Intance.GetBtcPukey(address);
                    CreateQR(key, authorizeQRImg);
                    //Debug.Log(chooseWallet + "address=" + address + "key:" + key);
                }
                catch (System.Exception)
                {
                    Debug.Log("未找到公钥");
                }
                
            }
        }
    }
    private QRCodeManager qrCodeManager = new QRCodeManager();
    /// <summary>
    /// 创建二维码
    /// </summary>
    /// <param name="info"></param>
    /// <param name="qrImage"></param>
    public void CreateQR(string info, Image qrImage)
    {
        string _info = "lisyng" + info;
        
        qrCodeManager.EncodeQRCode(qrImage, _info);
    }
    public void Btn_X()
    {
        for (int i = 0; i < authorizeList.Count; i++)
        {
            Destroy(authorizeList[i]);
        }
        authorizeList.Clear();
    }
    public void PageDown(RectTransform panel)
    {
        oldPanel = curPanel;
        curPanel = panel;
        panel.gameObject.SetActive(true);
    }
    public void PageUP()
    {
        curPanel.gameObject.SetActive(false);
        curPanel = oldPanel;
    }
}
