using NBitcoin;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class NewMultiSigWallet : MonoBehaviour {
    public GameObject newMultiSigWallet;
    public GameObject mulitmainPanle;
    public GameObject setPanle;
    public GameObject choosePanle;
    public ScanPuk scanPuk;
    public static NewMultiSigWallet instance;
    public impowerManager impowerManager;
    public CreateMnlitSuccess createMnlitSuccess;
    private void Awake()
    {
        instance = this;
    }
    private NewMultiSigWallet() { }
    public List<string> pubstr;
    public List<PubKey> pubKeys;
    public List<string> btcAddress; //地址
    public List<string> walletName; //钱包名称
    public int MultiSig_M;
    public int MultiSig_N;
    public string Multi_walletName;
    public string Multi_btcAddress;

    public InputField input_M;
    public InputField input_N;
    public NativeEditBox input_Nbox;

    public InputField input_Name;
    public InputField input_Note;

    public Button btn_CreateSet;
    public Button btn_CreateImpower;
    public Button btn_CreateSuccess;
    bool impowerAll=false;
    bool notesAll=true;
    // Use this for initialization
    void Start () {
        pubstr = new List<string>();
        pubKeys = new List<PubKey>();
        btcAddress = new List<string>();
        walletName = new List<string>();
    }

    private string GetInputValue(InputField t)
    {
        string v = t.text;
        if (string.IsNullOrEmpty(v))
        {
            NativeEditBox eb = t.gameObject.GetComponent<NativeEditBox>();
            if (eb != null)
                v = eb.text;
        }

        return v;
    }

    private void SetInputValue(InputField f, string v)
    {
        f.text = v;
        NativeEditBox eb = f.gameObject.GetComponent<NativeEditBox>();
        if (eb != null)
            eb.text = v;
    }
    /// <summary>
    /// setM_N按钮
    /// </summary>
    /// <returns></returns>
    public void InputM_N()
    {   bool inputok=false;
        MultiSig_N = StrToInt(GetInputValue(input_N));
        MultiSig_M = StrToInt(GetInputValue(input_M));
        if (MultiSig_N>=2&&MultiSig_N<=4)
        {
            if (MultiSig_M>=2&& MultiSig_M<= MultiSig_N)
            {
                inputok=true;
            }
            else
            {
                inputok = false;
            }
            
        }
        btn_CreateSet.interactable = inputok;
    }
    /// <summary>
    /// 授权完成按钮
    /// </summary>
    /// <returns></returns>
    public void Impower()
    {
        if (pubstr.Count == MultiSig_N)
        {
            impowerAll = true;
        }
        else
        {
            impowerAll = false;
            btn_CreateImpower.interactable = false;
        }
        if (impowerAll&&notesAll)
        {
            btn_CreateImpower.interactable = true;
        }
    }
    /// <summary>
    /// 创建成功
    /// </summary>
    /// <returns></returns>
    public void Success()
    {
        bool _success = false;
        if (GetInputValue(input_Name).Length>0&& GetInputValue(input_Name).Length<9)
        {
            _success = true;
            for (int i = 0; i < GetInputValue(input_Name).Length; i++)
            {
                if (!Regex.IsMatch(GetInputValue(input_Name).ToString(), @"^[A-Za-z0-9]+$"))
                {
                    _success = false;
                }
            }
            SaveWalletSN();
        }
        createMnlitSuccess.nameWaring.gameObject.SetActive(!_success);
        btn_CreateSuccess.interactable = _success;
    }

    public void SaveWalletSN()
    {
        if (input_Name.text != null)
        {
            Multi_walletName = GetInputValue(input_Name);// input_Name.text;
        }
    }
    /// <summary>
    /// 字符转整形
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public  int StrToInt(string str)
    {
        int num;
        if (int.TryParse(str, out num))
        {
            return num;
        }
        else
        {
            //警告
            return -1;
        }
    }
    /// <summary>
    /// 创建账户地址
    /// </summary>
    /// <returns></returns>
    public  string CreateBTCAddress()
    {
        foreach (var k in pubstr)
        {
            PubKey pub = new PubKey(k);
            pubKeys.Add(pub);
        }
        Multi_btcAddress = BTCMulSig.CreateMulSigAddress(MultiSig_M,pubKeys);
        return Multi_btcAddress;
    }
    public void SetNotes()
    {
        for (int i = 0; i < MultiSig_N; i++)
        {
            if (impowerManager.Inputstr[i].text==""|| impowerManager.Inputstr[i].text == null)
            {
                
            }
            else
            {
                if (!NotesChange(impowerManager.Inputstr[i].text))
                {
                    Debug.Log(impowerManager.Inputstr[i].text);
                    btn_CreateImpower.interactable = false;
                    notesAll = false;
                }
            }
        }
        if (impowerAll && notesAll)
        {
            btn_CreateImpower.interactable = true;
        }
        else if (!notesAll)
        {
            PopupLine.Instance.Show("备注请使用最多8位英文字母和数字组成");
        }
    }
    public bool NotesChange(string str)
    {
        if (str.Length > 8)
        {
            Debug.Log("str.Length>8");
            return false;
        }
        string str2 = str.Replace(" ","");
        if (!Regex.IsMatch(str2, @"^[A-Za-z0-9]+$"))
        {
            Debug.Log("!Regex.IsMatch :"+ str2);
            return false;
        }
        
        return true;
    }
    public void AddNotes()
    {
        walletName.Clear();
        for (int i = 0; i < MultiSig_N; i++)
        {
            walletName.Add(impowerManager.Inputstr[i].text);
        }
    }
    public void ReMoveWallte(string address)
    {
        for (int i = 0; i < btcAddress.Count; i++)
        {
            if (btcAddress[i]== address)
            {
                pubstr.Remove(pubstr[i]);
                btcAddress.Remove(btcAddress[i]);
            }
        }
    }
    public void SaveAndRest()
    {
        HttpManager._Intance.loadingPanel.SetActive(true);
        SaveWalletInfo(IsOver);
    }
    public void ResetSuccessPlanle()
    {
        SetInputValue(input_Name, null); //input_Name.text = null;
        input_Note.text = null;
        Multi_walletName = null;
        Multi_btcAddress = null;
        btn_CreateSuccess.interactable = false;
    }
    public  void RestInfo()
    {
        HttpManager._Intance.loadingPanel.SetActive(false);
        createMnlitSuccess.Btn_Success();
        SetInputValue(input_M, null);// input_M.text = null;
        SetInputValue(input_N, null); //input_N.text = null;
        SetInputValue(input_Name, null); //input_Name.text = null;
        input_Note.text = null;
        try
        {
            MultiSig_M = -1;
            MultiSig_N = -1;
            Multi_walletName = null;
            Multi_btcAddress = null;
            pubstr.Clear();
            pubKeys.Clear();
            btcAddress.Clear();
            walletName.Clear();
        }
        catch (System.Exception)
        {
            
        }
        btn_CreateSet.interactable = false;
        btn_CreateImpower.interactable = false;
        btn_CreateSuccess.interactable = false;
        impowerManager.transform.gameObject.SetActive(false);
        createMnlitSuccess.transform.gameObject.SetActive(false);
        mulitmainPanle.SetActive(false);
        newMultiSigWallet.SetActive(false);
    }
    public void SetRestInfo()
    {
        HttpManager._Intance.loadingPanel.SetActive(false);
        createMnlitSuccess.Btn_Success();
        SetInputValue(input_M, null);// input_M.text = null;
        SetInputValue(input_N, null); //input_N.text = null;
        SetInputValue(input_Name, null); //input_Name.text = null;
        input_Note.text = null;
        try
        {
            MultiSig_M = -1;
            MultiSig_N = -1;
            Multi_walletName = null;
            Multi_btcAddress = null;
            pubstr.Clear();
            pubKeys.Clear();
            btcAddress.Clear();
            walletName.Clear();
        }
        catch (System.Exception)
        {

        }
        btn_CreateSet.interactable = false;
        btn_CreateImpower.interactable = false;
        btn_CreateSuccess.interactable = false;
        impowerManager.transform.gameObject.SetActive(false);
        createMnlitSuccess.transform.gameObject.SetActive(false);
        newMultiSigWallet.SetActive(false);
    }
    public void  IsOver(bool isok)
    {
        if (isok)
        {
            SeedKeyManager.Instance.SavaAddressToServer(Multi_walletName, Multi_btcAddress, NewWalletManager.CoinType.BTC, RestInfo, null,true);

        }
    }
    private void SaveWalletInfo(Action<bool> CallBack)
    {
        MultiWalletInfo multiWalletInfo = new MultiWalletInfo();
        multiWalletInfo.btcAddress = new List<string>();
        multiWalletInfo.walletName = new List<string>();
        multiWalletInfo.pubstr = new List<string>();
        multiWalletInfo.Multi_btcAddress = Multi_btcAddress;
        multiWalletInfo.Multi_walletName = Multi_walletName;
        multiWalletInfo.MultiSig_N = MultiSig_N;
        multiWalletInfo.MultiSig_M = MultiSig_M;
        for (int i = 0; i < walletName.Count; i++)
        {
            multiWalletInfo.walletName.Add(walletName[i]);
            multiWalletInfo.btcAddress.Add(btcAddress[i]);
            multiWalletInfo.pubstr.Add(pubstr[i]);
        }
        for (int i = 0; i < multiWalletInfo.walletName.Count; i++)
        {
            //Debug.Log("第"+i+":walletName=" + multiWalletInfo.walletName[i]+ " ;btcAddress="+ multiWalletInfo.btcAddress [i]+ " ;pubstr=" + multiWalletInfo.pubstr[i]);
        }
        MultiJSData.instance.SaveMultiWalletInfo(multiWalletInfo,CallBack);
    }
}
