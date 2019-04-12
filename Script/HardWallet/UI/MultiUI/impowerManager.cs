using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class impowerManager : MonoBehaviour {
    public NewMultiSigWallet newMultiSigWallet;
    public static impowerManager instance;
    private impowerManager() { }
    public ScanPuk scanPuk;

    public GameObject[] createimpowerwallet;
    public List<Text> title;
    public List<Text> addrs;
    List<string> addressList;
    public List<Text> SNstr;
    public List<InputField> Inputstr;
    public List<Button> Qr;
    public List<Button> SN;

    List<GameObject> snListGB;
    public  GameObject chooseSN;
    public  GameObject scanQR;
    public GameObject impowerPrefab;
    public RectTransform parent_impowerSNList;
    public string chooseWallet;
    public  int curSN;
    private void Awake()
    {
        instance = this;
    }
    // Use this for initialization
    void Start () {
        chooseWallet = "";
        title = new List<Text>();
        addrs = new List<Text>();
        addressList = new List<string>();
        SNstr = new List<Text>();
        Inputstr = new List<InputField>();
        Qr = new List<Button>();
        SN = new List<Button>();
        snListGB = new List<GameObject>();
        for (int i = 0; i < createimpowerwallet.Length; i++)
        {
            title.Add(createimpowerwallet[i].GetComponent<ChooseImpowerWallet>().title);
            addrs.Add(createimpowerwallet[i].GetComponent<ChooseImpowerWallet>().addrs);
            addressList.Add(createimpowerwallet[i].GetComponent<ChooseImpowerWallet>().addrs.text);
            Qr.Add(createimpowerwallet[i].GetComponent<ChooseImpowerWallet>().Qr);
            SN.Add(createimpowerwallet[i].GetComponent<ChooseImpowerWallet>().SN);
            SNstr.Add(createimpowerwallet[i].GetComponent<ChooseImpowerWallet>().SNstr);
            Inputstr.Add(createimpowerwallet[i].GetComponent<ChooseImpowerWallet>().Inputstr);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void ToQR()
    {
        
    }
    public void ToChooseSN()
    {
        ShowBTCList();
    }
    public void ShowWallet()
    {
        newMultiSigWallet.btn_CreateImpower.interactable = false;
        for (int i = 0; i < createimpowerwallet.Length; i++)
        {
            createimpowerwallet[i].SetActive(false);
        }
        for (int j = 0; j < newMultiSigWallet.btcAddress.Count; j++)
        {
            Debug.Log("show");
            newMultiSigWallet.walletName.Remove(newMultiSigWallet.walletName[j]);
            newMultiSigWallet.pubstr.Remove(newMultiSigWallet.pubstr[j]);
            newMultiSigWallet.btcAddress.Remove(newMultiSigWallet.btcAddress[j]);
        }
        for (int i = 0; i < newMultiSigWallet.MultiSig_N; i++)
        {
            createimpowerwallet[i].SetActive(true);
        }
        try
        {
            for (int i = 0; i < newMultiSigWallet.MultiSig_N; i++)
            {
                addrs[i].text = "";
                addressList[i] = null;
                SNstr[i].text = "选择钱包";
                Inputstr[i].text = "";
            }
        }
        catch (System.Exception)
        {
            
        }
        
    }
    Dictionary<int, string> ischooseName = new Dictionary<int, string>();
    Dictionary<int, string> notchooseName = new Dictionary<int, string>();
    /// <summary>
    /// 展示BTC钱包列表
    /// </summary>
    private void ShowBTCList()
    {
        foreach (var v in PanelManager._Instance._mainPanel.btcitemList)
        {
            if (!newMultiSigWallet.btcAddress.Contains(v.Value.coinInfo.address))
            {
                if (!MultiJSData.instance.IsMultiSigAddress(v.Value.coinInfo.address))
                {
                    CreateAuthorizeList(v.Value.coinInfo.walletname);
                }
            }
        }
    }
    /// <summary>
    /// 选择SN完成
    /// </summary>
    public void Btn_CreateSN()
    {
        if (chooseWallet == "")
        {
            if (addrs[curSN].text != "")
            {
                Debug.Log(addrs[curSN].text);
                foreach (string address in PanelManager._Instance._mainPanel.btcitemList.Keys)
                {
                    
                    if (address == addressList[curSN].ToLower())
                    {
                        newMultiSigWallet.ReMoveWallte(addressList[curSN]);
                    }
                }
                addrs[curSN].text = "";
                addressList[curSN] = null;
                SNstr[curSN].text = "选择钱包";
                Inputstr[curSN].text = "";
            }
            for (int i = 0; i < snListGB.Count; i++)
            {
                Destroy(snListGB[i]);
            }
            snListGB.Clear();
            chooseWallet = "";
            curSN = -1;
            newMultiSigWallet.Impower();
            newMultiSigWallet.choosePanle.SetActive(false);
            return;
        }
        if (chooseWallet=="无")
        {
            if (addrs[curSN].text != "")
            {
                foreach (string address in PanelManager._Instance._mainPanel.btcitemList.Keys)
                {
                    
                    if (address == addressList[curSN].ToLower())
                    {
                        newMultiSigWallet.ReMoveWallte(addressList[curSN]);
                    }
                }
                addrs[curSN].text = "";
                addressList[curSN] = null;
                SNstr[curSN].text = "选择钱包";
                Inputstr[curSN].text = "";
            }
            for (int i = 0; i < snListGB.Count; i++)
            {
                Destroy(snListGB[i]);
            }
            snListGB.Clear();
            chooseWallet = "";
            curSN = -1;
            newMultiSigWallet.Impower();
            newMultiSigWallet.choosePanle.SetActive(false);
            return;
        }
        foreach (string address in PanelManager._Instance._mainPanel.btcitemList.Keys)
        {
            if (PanelManager._Instance._mainPanel.btcitemList[address].coinInfo.walletname == chooseWallet)
            {
                try
                {
                    WalletTools tools = new WalletTools();
                    string key = NewWalletManager._Intance.GetBtcPukey(address);
                    newMultiSigWallet.pubstr.Add(key);
                    newMultiSigWallet.btcAddress.Add(tools.GetBTCAddressByPuk(key));
                    addressList[curSN] = tools.GetBTCAddressByPuk(key);
                    string qianAdr = addressList[curSN].Substring(0, 10);
                    string houAdr = addressList[curSN].Substring(addressList[curSN].Length - 10, 10);
                    addrs[curSN].text = qianAdr + "..." + houAdr;
                    SNstr[curSN].text = PanelManager._Instance._mainPanel.btcitemList[address].coinInfo.walletname;
                    Inputstr[curSN].text = PanelManager._Instance._mainPanel.btcitemList[address].coinInfo.walletname;
                    chooseWallet = "";
                    curSN = -1;
                    //if (newMultiSigWallet.pubstr.Count == newMultiSigWallet.MultiSig_N)
                    //{
                    //    scanPuk.ShowScanAddress();
                    //}
                }
                catch (System.Exception e)
                {
                    Debug.Log("未找到公钥"+e);
                }

            }
        }
        for (int i = 0; i < snListGB.Count; i++)
        {
            Destroy(snListGB[i]);
        }
        snListGB.Clear();
        newMultiSigWallet.Impower();
        newMultiSigWallet.choosePanle.SetActive(false);
    }
    public void ScanShowAddress(string btcAddress)
    {
        addressList[curSN] = btcAddress;
        string qianAdr = addressList[curSN].Substring(0, 10);
        string houAdr = addressList[curSN].Substring(addressList[curSN].Length - 10, 10);
        addrs[curSN].text = qianAdr + "..." + houAdr;
        SNstr[curSN].text = "";
        Inputstr[curSN].text = "";
        chooseWallet = "";
        curSN = -1;
    }
    public void Btn_X()
    {
        for (int i = 0; i < snListGB.Count; i++)
        {
            Destroy(snListGB[i]);
        }
        snListGB.Clear();
        chooseWallet = "";
    }
    public void List_Rest()
    {
        StartCoroutine(List_RestTor());
    }
    IEnumerator List_RestTor()
    {
        for (int i = 0; i < newMultiSigWallet.MultiSig_N; i++)
        {
            addrs[i].text = "";
            addressList[i] = null;
            SNstr[i].text = "选择钱包";
            Inputstr[i].text = "";
            createimpowerwallet[i].SetActive(false);
        }
        newMultiSigWallet.btcAddress.Clear();
        newMultiSigWallet.pubstr.Clear();
        yield return new WaitForSeconds(0.1f);
        newMultiSigWallet.setPanle.SetActive(true);
        transform.gameObject.SetActive(false);
    }
    private void CreateAuthorizeList(string sn)
    {
        GameObject gameObject = Instantiate(impowerPrefab, parent_impowerSNList);
        gameObject.transform.Find("Text").GetComponent<Text>().text = sn;
        gameObject.transform.Find("Toggle").GetComponent<Toggle>().group = parent_impowerSNList.GetComponent<ToggleGroup>();
        gameObject.transform.Find("Toggle").GetComponent<Toggle>().isOn = false;
        chooseSN.transform.Find("Toggle").GetComponent<Toggle>().isOn = false;
        snListGB.Add(gameObject);
    }
}
