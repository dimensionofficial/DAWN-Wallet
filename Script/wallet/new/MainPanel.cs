using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;



[System.Serializable]
public class CoinTypeObject
{
    public CoinInfoItemBase cloneItem;
    public GameObject titleObject;
    public Text totalCountText;
    public Text rmbCountText;
    public GameObject lastLine;
    public RectTransform backGround;
}

public class MainPanel : HotBasePanel
{
    public MultiJSData multiJSData;
    public GameObject noColdWalletShow;
    public GameObject noColdWalletObject;
    public GameObject importWalletBtn;

    public GameObject eyeOpen;
    public GameObject eyeShut;
    public GameObject redPoint;
    /// <summary>
    /// 单位类型
    /// </summary>
    public enum UnitType
    {
        RMB = 0,
        BTC = 1,
        USD = 2,
    }
    public UnitType currentUnitType = UnitType.RMB;
    public Image unitIcon;

    public GameObject noWalletMark;

//	public GameObject titleObject;
 //   public GameObject pullObject;
	public Text totalMony;

    public const float backgrounSize_Y0 = 230F;
    public const float backGrounSize_Y1 = 400F;
    public const float backGroundAddY = 150F;

    public CoinTypeObject usdtObjectType;
    public CoinTypeObject btcObjectType;
    public CoinTypeObject ethObjectType;
    public CoinTypeObject eosObjectType;
    public Transform pareten;

    public ScanAddress _scanPanl;

    private decimal m_btcCount;
    public decimal btcCount
    {
        get { return m_btcCount; }
        set
        {
            m_btcCount = value;
        }
    }
    private decimal m_usdtCount;
    public decimal usdtCount
    {
        get { return m_usdtCount; }
        set
        {
            m_usdtCount = value;
        }
    }
    private decimal m_ethCount;
    public decimal ethCount
    {
        get { return m_ethCount; }
        set
        {
            m_ethCount = value;
        }
    }

    private decimal m_eosCount;
    public decimal eosCount
    {
        get { return m_eosCount; }
        set
        {
            m_eosCount = value;
        }
    }

    public NewSettingPanel settingPanel;
    public WalletMarketPanel hangQingPanel;

    public WalletInfoPanel walletInfoPanel;
    public WalletInfoPanel multiWalletInfoPanel;
    //   public CoolPanel _coolPanel;
    public GameObject bottomBtn;
    public Button mainBtn;
    public GameObject mainNormal;
    public GameObject mainSelect;

    public Button settingBtn;
    public GameObject settingNormal;
    public GameObject settingSelect;

	public Button shanDuiBtn;
	public GameObject shanDuiNormal;
	public GameObject shanDuiSelect;

	public Button hangQingBtn;
	public GameObject hangQingNormal;
	public GameObject hangQingSelect;

    public Button creatBtn;

	public Button currentBtn;
    // key 为地址
    public Dictionary<string, BitcoinInfoItem> btcitemList = new Dictionary<string, BitcoinInfoItem>();
    public Dictionary<string, UsdtcoinInfoItem> usdtItemList = new Dictionary<string, UsdtcoinInfoItem>();
    public Dictionary<string, EthcoinInfoItem> ethItemList = new Dictionary<string, EthcoinInfoItem>();
    public Dictionary<string, EosItem> eosItemList = new Dictionary<string, EosItem>();
	private decimal btcTotalMoney;
    private decimal usdtTotalMoney;
    private decimal ethtotalMoney;
    private decimal eostotalMoney;

    
    public ScrollRect scrollRect;
    public void OnClickWalletInfoBtn(CoinInfoItemBase item)
    {
        walletInfoPanel.ShowMe(item);
        NewWalletManager._Intance.DOTweenCome(walletInfoPanel.transform, -1000, 0);
        BitcoinInfoItem btcItem = PanelManager._Instance._WalletInfoPanel.currentItem as BitcoinInfoItem;
    }

    private void SetBackGroundRectSzie(RectTransform backGround, int index)
    {
        backGround.sizeDelta = new Vector2(backGround.sizeDelta.x, 400 + index * 150F);
    }

    public void CleanDic()
    {
        foreach (var v in btcitemList)
        {
            Destroy(v.Value.gameObject);
        }
        btcitemList.Clear();
        foreach (var v in usdtItemList)
        {
            Destroy(v.Value.gameObject);
        }
        usdtItemList.Clear();
        foreach (var v in ethItemList)
        {
            Destroy(v.Value.gameObject);
        }
        ethItemList.Clear();
        foreach (var v in eosItemList)
        {
            Destroy(v.Value.gameObject);
        }
        eosItemList.Clear();
    }

    void Start()
    {
        if (IsHideAsset())
        {
            totalMony.text = "*****";
            eyeOpen.SetActive(false);
            eyeShut.SetActive(true);
        }
        else
        {
            eyeOpen.SetActive(true);
            eyeShut.SetActive(false);
        }
        EventManager.Instance.AddEventListener(EventID.UpdateBTCbalance, UpdateBTCCount);
        EventManager.Instance.AddEventListener(EventID.UpdateUSDTbalance, UpdateUSDTCount);
        EventManager.Instance.AddEventListener(EventID.UpdateETHbalance, UpdateETHCout);
        EventManager.Instance.AddEventListener(EventID.UpdateEOSBalance, UpdateEOSCount);
        EventManager.Instance.AddEventListener (EventID.UpdateTotalbalance, UpdateTotalMoney);
        RefreshMessageInfo();
    }

    void OnDestory()
    {
        EventManager.Instance.RemoveEventListener(EventID.UpdateBTCbalance, UpdateBTCCount);
        EventManager.Instance.RemoveEventListener(EventID.UpdateUSDTbalance, UpdateUSDTCount);
        EventManager.Instance.RemoveEventListener(EventID.UpdateETHbalance, UpdateETHCout);
        EventManager.Instance.RemoveEventListener(EventID.UpdateEOSBalance, UpdateEOSCount);
        EventManager.Instance.RemoveEventListener(EventID.UpdateTotalbalance, UpdateTotalMoney);
    }

    public bool JudegeItem()
    {
        if (btcitemList.Count > 0 || usdtItemList.Count > 0 || ethItemList.Count > 0 || eosItemList.Count > 0)
            return true;

        return false;
    }

	public void UpdateTotalMoney(params object[] obj)
	{
		if (btcitemList.Count == NewWalletManager._Intance.bitcoinAddresListInfo.Count)
		{
			foreach (var kvp in btcitemList)
			{
				if (kvp.Value.isGetBalance == false)
				{
					ShowTotalValue();
					return;
				}
			}
		} else
		{
			ShowTotalValue();
			return;
		}

		if (ethItemList.Count == NewWalletManager._Intance.ethcoinAddresListInfo.Count) {
			foreach (var kvp in ethItemList) {
				if (kvp.Value.isGetBalance == false)
				{
					ShowTotalValue();
					return;
				}
			}
		} else
		{
			ShowTotalValue();
			return;
		}

		ShowTotalValue();

 //      PanelManager._Instance.loadingPanel.SetActive(false);
	}

    public void OnClickBackUpBtn()
    {
        PanelManager._Instance._backUpPrivateKeyPanel.OpenMe();
    }

	private void ShowTotalValue()
	{
        //if (!string.IsNullOrEmpty(PlayerPrefs.GetString(NewWalletManager._Intance.userName + "WalletBTCTotalMoney")))
        //{
        //	btcTotalMoney = decimal.Parse(PlayerPrefs.GetString(NewWalletManager._Intance.userName + "WalletBTCTotalMoney"));
        //}
        //if (!string.IsNullOrEmpty(PlayerPrefs.GetString(NewWalletManager._Intance.userName + "WalletETCTotalMoney")))
        //{
        //	ethtotalMoney = decimal.Parse(PlayerPrefs.GetString(NewWalletManager._Intance.userName + "WalletETCTotalMoney"));
        //}

        decimal total = btcTotalMoney + ethtotalMoney + eostotalMoney+ usdtTotalMoney;

		PlayerPrefs.SetString(NewWalletManager._Intance.userName + "WalletaTotalMony", total.ToString());

		NewWalletManager._Intance.ShowProperty(totalMony, total);

       

		NewWalletManager._Intance.ShowProperty(btcObjectType.rmbCountText, btcTotalMoney);
        NewWalletManager._Intance.ShowProperty(usdtObjectType.rmbCountText, usdtTotalMoney);
        NewWalletManager._Intance.ShowProperty(ethObjectType.rmbCountText, ethtotalMoney);
		NewWalletManager._Intance.ShowProperty(eosObjectType.rmbCountText, eostotalMoney);

        if (IsHideAsset())
        {
            totalMony.text = "*****";
            btcObjectType.rmbCountText.text = "*****";
            usdtObjectType.rmbCountText.text = "*****";
            ethObjectType.rmbCountText.text = "*****";
            eosObjectType.rmbCountText.text = "*****";

            btcObjectType.totalCountText.text = "*****";
            usdtObjectType.totalCountText.text = "*****";
            ethObjectType.totalCountText.text = "*****";
            eosObjectType.totalCountText.text = "*****";


            foreach (var v in ethItemList)
            {
                v.Value.countText.text = "*****";
            }
            foreach (var v in btcitemList)
            {
                v.Value.countText.text = "*****";
            }
            foreach (var v in usdtItemList)
            {
                v.Value.countText.text = "*****";
            }
            foreach (var v in eosItemList)
            {
                v.Value.countText.text = "*****";
            }
        }
        else
        {
            if (m_btcCount == 0)
            {
                btcObjectType.totalCountText.text = "0 BTC";
            }
            else
            {
                NewWalletManager._Intance.ShowCount(btcObjectType.totalCountText, m_btcCount);
                btcObjectType.totalCountText.text = btcObjectType.totalCountText.text + " BTC";
            }

            if (m_usdtCount == 0)
            {
                usdtObjectType.totalCountText.text = "0 USDT";
            }
            else
            {
                NewWalletManager._Intance.ShowCount(usdtObjectType.totalCountText, m_usdtCount);
                usdtObjectType.totalCountText.text = usdtObjectType.totalCountText.text + " USDT";
            }
            
            if (ethCount == 0)
            {
                ethObjectType.totalCountText.text = "0 ETH";
            }
            else
            {
                NewWalletManager._Intance.ShowCount(ethObjectType.totalCountText, ethCount);
                ethObjectType.totalCountText.text = ethObjectType.totalCountText.text + " ETH";
            }

            eosObjectType.totalCountText.text = eosCount + " EOS";

            foreach (var v in btcitemList)
            {
                NewWalletManager._Intance.ShowCount(v.Value.countText, v.Value.coinInfo.money);
                v.Value.countText.text = v.Value.countText.text + " BTC";
            }

            foreach (var v in usdtItemList)
            {
                NewWalletManager._Intance.ShowCount(v.Value.countText, v.Value.coinInfo.money);
                v.Value.countText.text = v.Value.countText.text + " USDT";
            }
            foreach (var v in ethItemList)
            {
                v.Value.ShowBalancesCount();
                //NewWalletManager._Intance.ShowCount(v.Value.countText, v.Value.coinInfo.money);
                //v.Value.countText.text = v.Value.countText.text + " ETH";
            }
            
            foreach (var v in eosItemList)
            {
                EosItem item = v.Value as EosItem;
                v.Value.countText.text = item.eosWalletInfo.balance + " EOS";
            }
        }
    }

    public bool IsHideAsset()
    {
        int hide = PlayerPrefs.GetInt("AssetSwitch");
        if (hide >= 1)
            return true;

        return false;
    }

    public void OnClickAssetSwitchBtn()
    {
        if (IsHideAsset())
        {
            PlayerPrefs.SetInt("AssetSwitch", 0);
            eyeOpen.SetActive(true);
            eyeShut.SetActive(false);
        }
        else
        {
            PlayerPrefs.SetInt("AssetSwitch", 1);
            eyeOpen.SetActive(false);
            eyeShut.SetActive(true);
        }

        ShowTotalValue();
    }

    public void UpdateEOSCount(params object[] obj)
    {
        if (eosItemList.Count == NewWalletManager._Intance.eoscoinAddressListInfo.Count)
        {
            eosCount = 0;
            foreach (var v in eosItemList)
            {
                eosCount += v.Value.eosWalletInfo.balance;
            }
            eosObjectType.totalCountText.text = eosCount + " EOS";

            if (IsHideAsset())
            {
                eosObjectType.totalCountText.text = "*****";
            }

            eostotalMoney = eosCount * HttpManager._Intance.eos_RMB;
            
            NewWalletManager._Intance.ShowProperty(eosObjectType.rmbCountText, eostotalMoney);
            EventManager.Instance.SendEvent(EventID.UpdateTotalbalance);
        }
    }

    public void UpdateBTCCount(params object[] obj)
    {
        if (btcitemList.Count == NewWalletManager._Intance.bitcoinAddresListInfo.Count)
        {
            decimal tempCount = 0;
            foreach (var kvp in btcitemList)
            {
                if (kvp.Value.isGetBalance == false)
                    return;

                tempCount += kvp.Value.coinInfo.money;
            }
            btcCount = tempCount;


            if (m_btcCount == 0)
            {
                btcObjectType.totalCountText.text = "0 BTC";
            }
            else
            {
                NewWalletManager._Intance.ShowCount(btcObjectType.totalCountText, m_btcCount);
                btcObjectType.totalCountText.text = btcObjectType.totalCountText.text + " BTC";
            }

            if (IsHideAsset())
            {
                btcObjectType.totalCountText.text = "*****";
            }

            PlayerPrefs.SetString(NewWalletManager._Intance.userName + "WalletBTCTotalCount", m_btcCount.ToString());

            btcTotalMoney = decimal.Parse((HttpManager._Intance.btc_RMB * m_btcCount).ToString("f2"));

            PlayerPrefs.SetString(NewWalletManager._Intance.userName + "WalletBTCTotalMoney", btcTotalMoney.ToString());
            NewWalletManager._Intance.ShowProperty(btcObjectType.rmbCountText, btcTotalMoney);
            EventManager.Instance.SendEvent (EventID.UpdateTotalbalance);
        }
    }
    public void UpdateUSDTCount(params object[] obj)
    {
        //Debug.Log(usdtItemList.Count+"        "+ NewWalletManager._Intance.usdtAddresListInfo.Count);
        if (usdtItemList.Count == NewWalletManager._Intance.usdtAddresListInfo.Count)
        {
            decimal tempCount = 0;
            foreach (var kvp in usdtItemList)
            {
                if (kvp.Value.isGetBalance == false)
                    return;
                tempCount += kvp.Value.coinInfo.money;
            }
            usdtCount = tempCount;


            if (m_usdtCount == 0)
            {
                usdtObjectType.totalCountText.text = "0 USDT";
            }
            else
            {
                NewWalletManager._Intance.ShowCount(usdtObjectType.totalCountText, m_usdtCount);
                usdtObjectType.totalCountText.text = usdtObjectType.totalCountText.text + " USDT";
            }

            if (IsHideAsset())
            {
                usdtObjectType.totalCountText.text = "*****";
            }
            
            PlayerPrefs.SetString(NewWalletManager._Intance.userName + "WalletUSDTTotalCount", m_usdtCount.ToString());
            //------------------------------------------------------!?!
            usdtTotalMoney = decimal.Parse((HttpManager._Intance.current_usdt * m_usdtCount).ToString("f2"));
            PlayerPrefs.SetString(NewWalletManager._Intance.userName + "WalletUSDTTotalMoney", usdtTotalMoney.ToString());
            NewWalletManager._Intance.ShowProperty(usdtObjectType.rmbCountText, usdtTotalMoney);
            EventManager.Instance.SendEvent(EventID.UpdateTotalbalance);
        }
    }
    public void UpdateETHCout(params object[] obj)
    {
        if (ethItemList.Count == NewWalletManager._Intance.ethcoinAddresListInfo.Count)
        {
            decimal tempCount = 0;
            
            foreach (var kvp in ethItemList)
            {
                if (kvp.Value.isGetBalance == false)
                    return;

                tempCount += kvp.Value.coinInfo.money;
                tempCount += kvp.Value.coinInfo.ethTokenMoney;
            }
            ethCount = tempCount;
            if (ethCount == 0)
            {
                ethObjectType.totalCountText.text = "0 ETH";
            }
            else
            {
                NewWalletManager._Intance.ShowCount(ethObjectType.totalCountText, ethCount);
                ethObjectType.totalCountText.text = ethObjectType.totalCountText.text + " ETH";
            }

            if (IsHideAsset())
            {
                ethObjectType.totalCountText.text = "*****";
            }

            PlayerPrefs.SetString(NewWalletManager._Intance.userName + "WalletETHTotalCount", m_ethCount.ToString());
            ethtotalMoney = decimal.Parse((HttpManager._Intance.eth_RMB * ethCount).ToString("f2"));

            //string tempV = ethObjectType.rmbCountText.text;
            //if (ethtotalMoney == 0)
            //{
            //    if (!string.IsNullOrEmpty(tempV))
            //    {
            //        if (tempV.Length > 2)
            //        {
            //            tempV = tempV.Substring(2);
            //            ethtotalMoney = decimal.Parse(tempV);
            //        }

            //    }
            //}
            NewWalletManager._Intance.ShowProperty(ethObjectType.rmbCountText, ethtotalMoney);

			EventManager.Instance.SendEvent (EventID.UpdateTotalbalance);
        }
    }


   

    public void OnClickSwitchUnitBtn()
    {
        if (currentUnitType == UnitType.RMB)
        {
            currentUnitType = UnitType.USD;
        }
        else if (currentUnitType == UnitType.USD)
        {
            currentUnitType = UnitType.BTC;
        }
        else if (currentUnitType == UnitType.BTC)
        {
            currentUnitType = UnitType.RMB;
        }
        ShowUnitSprite();

        EventManager.Instance.SendEvent(EventID.UpdateTotalbalance);
        PlayerPrefs.SetInt(NewWalletManager._Intance.userName + NewWalletManager.COIN_UNIT_TYPE, (int)currentUnitType);
    }

    /// <summary>
    /// 钱包名字相同的添加后缀数量
    /// </summary>
    /// <param name="targetDic"></param>
    /// <returns></returns>
    public Dictionary<string, string> GetTheSameNameWallet(Dictionary<string, string> targetDic)
    {
        var duplicateValues = targetDic.GroupBy(x => x.Value).Where(x => x.Count() > 1);
        //loop dictionary duplicate values only   
        List<string> sanmNameList = new List<string>();   
        foreach (var item in duplicateValues)
        {
            sanmNameList.Add(item.Key);
        }

        Dictionary<string, string> tempDic = new Dictionary<string, string>();
        foreach (KeyValuePair<string, string> kvp in targetDic)
        {
            tempDic.Add(kvp.Key, kvp.Value);
        }


        Dictionary<string, string> tempDic1 = new Dictionary<string, string>();
        for (int i = 0; i < sanmNameList.Count; i++)
        {
            int nameIndex = 0;
            foreach (KeyValuePair<string, string> kvp in tempDic)
            {
                if (kvp.Value.Equals(sanmNameList[i]))
                {
                    if (nameIndex != 0)
                    {
                        string v = kvp.Value + "(" + nameIndex + ")";
                        tempDic1.Add(kvp.Key, v); 
                    }
                    nameIndex++;
                }
            }
        }

        foreach (KeyValuePair<string, string> kvp in tempDic1)
        {
            tempDic[kvp.Key] = kvp.Value;
        }


        return tempDic;
    }

    private Dictionary<string, EOSWalletInfo> GetTheSameNameWallet(Dictionary<string, EOSWalletInfo> targetDic)
    {
        var duplicateValues = targetDic.GroupBy(x => x.Value.walletName).Where(x => x.Count() > 1);
        //loop dictionary duplicate values only   
        List<string> sanmNameList = new List<string>();
        foreach (var item in duplicateValues)
        {
            sanmNameList.Add(item.Key);
        }

        Dictionary<string, EOSWalletInfo> tempDic = new Dictionary<string, EOSWalletInfo>();
        foreach (KeyValuePair<string, EOSWalletInfo> kvp in targetDic)
        {
            tempDic.Add(kvp.Key, kvp.Value);
        }


        Dictionary<string, EOSWalletInfo> tempDic1 = new Dictionary<string, EOSWalletInfo>();
        for (int i = 0; i < sanmNameList.Count; i++)
        {
            int nameIndex = 0;
            foreach (KeyValuePair<string, EOSWalletInfo> kvp in tempDic)
            {
                if (kvp.Value.walletName.Equals(sanmNameList[i]))
                {
                    if (nameIndex != 0)
                    {
                        string v = kvp.Value.walletName + "(" + nameIndex + ")";
                        kvp.Value.walletName = v;
                        tempDic1.Add(kvp.Key, kvp.Value);
                    }
                    nameIndex++;
                }
            }
        }

        foreach (KeyValuePair<string, EOSWalletInfo> kvp in tempDic1)
        {
            tempDic[kvp.Key] = kvp.Value;
        }


        return tempDic;
    }

    private string GetTheSameNameWallet(string targetKey, Dictionary<string, string> targetDic)
    {
        Dictionary<string, string> tempDic = GetTheSameNameWallet(targetDic);
        return tempDic[targetKey];
    }

    private EOSWalletInfo GetTheSameNameWallet(string targetKey, Dictionary<string, EOSWalletInfo> targetDic)
    {
        Dictionary<string, EOSWalletInfo> tempDic = GetTheSameNameWallet(targetDic);
        return tempDic[targetKey];
    }


    #region 生成一个币种钱包实例
    //v[0] 地址， v[1] 名字
    public void AddBitItem(string[] va)
    {
        Debug.Log("AddBitItem");

        if (btcitemList.ContainsKey(va[0]))
        {
            return;
        }
       
        string nameWallet = GetTheSameNameWallet(va[0], NewWalletManager._Intance.bitcoinAddresListInfo);
       
        va[1] = nameWallet;
        GameObject go;
        //判断是否是多签钱包
        go = GameObject.Instantiate(btcObjectType.cloneItem.gameObject);
        if (btcitemList.Count <= 0)
        {
            go.SetActive(true);
        }
        else if (btcitemList.Values.First().gameObject.activeInHierarchy)
        {
            go.SetActive(true);
        }
        else
        {
            go.SetActive(false);
        }
        BitcoinInfoItem item = go.GetComponent<BitcoinInfoItem>();
        item.Init(va);
        va[0] = va[0].ToLower();
        go.transform.SetParent(pareten);
        int a = btcObjectType.titleObject.transform.GetSiblingIndex() + 1 + btcitemList.Count;
        go.transform.SetSiblingIndex(a);
        go.GetComponent<RectTransform>().localScale = Vector3.one;
        btcitemList.Add(va[0], item);
        ShowTitle();
    }
    public void AddEthItem(string[] va)
    {
        if (ethItemList.ContainsKey(va[0].ToLower()))
        {
            return;
        }
        string nameWallet = GetTheSameNameWallet(va[0], NewWalletManager._Intance.ethcoinAddresListInfo);
        va[1] = nameWallet;
        int a = ethObjectType.titleObject.transform.GetSiblingIndex() + 1 + ethItemList.Count;
        GetEthItem(va, a);
        ShowTitle();
    }
    public void AddEosItem(string[] va)
    {
        if (eosItemList.ContainsKey(va[0].ToLower()))
        {
            return;
        }

        EOSWalletInfo nameWallet = GetTheSameNameWallet(va[0], NewWalletManager._Intance.eoscoinAddressListInfo);
        va[1] = nameWallet.walletName;

        GameObject go = GameObject.Instantiate(eosObjectType.cloneItem.gameObject);
        if (eosItemList.Count <= 0)
        {
            go.SetActive(true);
        }
        else if (eosItemList.Values.First().gameObject.activeInHierarchy)
        {
            go.SetActive(true);
        }
        else
        {
            go.SetActive(false);
        }
        EosItem item = go.GetComponent<EosItem>();
        item.InitEOS(nameWallet);
        go.transform.SetParent(pareten);
        int a = eosObjectType.titleObject.transform.GetSiblingIndex() + 1 + eosItemList.Count;
        go.transform.SetSiblingIndex(a);
        go.GetComponent<RectTransform>().localScale = Vector3.one;
        eosItemList.Add(va[0].ToLower(), item);
        ShowTitle();
    }
    public void AddUsdtItem(string[] va)
    {
        Debug.Log("addUsdt"+ va[0]);
        if (usdtItemList.ContainsKey(va[0]))
        {
            return;
        }
        //try
        //{
        //    for (int i = 0; i < multiJSData.multiWalletInfoList.Count; i++)
        //    {
        //        Debug.Log("mulit"+ multiJSData.multiWalletInfoList[i].Multi_btcAddress+" usdt:"+ va[0]);
        //        if (multiJSData.multiWalletInfoList[i].Multi_btcAddress == va[0])
        //        {
                    
        //            return;
        //        }
        //    }
        //}
        //catch (Exception)
        //{
        //}
        

        string nameWallet = GetTheSameNameWallet(va[0], NewWalletManager._Intance.usdtAddresListInfo);
        va[1] = nameWallet;
        GameObject go;
        go = GameObject.Instantiate(usdtObjectType.cloneItem.gameObject);
        if (usdtItemList.Count <= 0)
        {
            go.SetActive(true);
        }
        else if (usdtItemList.Values.First().gameObject.activeInHierarchy)
        {
            go.SetActive(true);
        }
        else
        {
            go.SetActive(false);
        }
       
        UsdtcoinInfoItem item = go.GetComponent<UsdtcoinInfoItem>();
        item.Init(va);
        go.transform.SetParent(pareten);
        int a = usdtObjectType.titleObject.transform.GetSiblingIndex() + 1 + usdtItemList.Count;
        go.transform.SetSiblingIndex(a);
        go.GetComponent<RectTransform>().localScale = Vector3.one;
        usdtItemList.Add(va[0], item);
        ShowTitle();
    }


    #endregion

    #region 登陆后实例化数据库返回的所有钱包
    private void InitBtcItem()
    {
        int i = 0;
        Dictionary<string, string> tempBtcAddresListDic = GetTheSameNameWallet(NewWalletManager._Intance.bitcoinAddresListInfo);
        foreach (KeyValuePair<string, string> kvp in tempBtcAddresListDic)
        {
            BitcoinInfoItem item = null;
            string[] valus = new string[2] { kvp.Key, kvp.Value };
            if (!btcitemList.ContainsKey(kvp.Key))
            {

                GameObject go;
                go = GameObject.Instantiate(btcObjectType.cloneItem.gameObject);
                go.name = i.ToString();
                go.SetActive(true);
                item = go.GetComponent<BitcoinInfoItem>();
                go.name = i.ToString();
                go.transform.SetParent(pareten);
                go.transform.SetSiblingIndex(i + 3);
                go.GetComponent<RectTransform>().localScale = Vector3.one;
                item.Init(valus);
                btcitemList.Add(kvp.Key.ToLower(), item);
            }
            else
            {
                item = btcitemList[kvp.Key];
                item.Init(valus);
            }
            i++;
        }
        ShowTitle();
    }
    private void InitEthItem()
    {
        int i = 0;
        Dictionary<string, string> tempEthAddresListDic = GetTheSameNameWallet(NewWalletManager._Intance.ethcoinAddresListInfo);
        foreach (KeyValuePair<string, string> kvp in tempEthAddresListDic)
        {
            string[] valus = new string[2] { kvp.Key, kvp.Value };
            if (!ethItemList.ContainsKey(kvp.Key.ToLower()))
            {
                int a = ethObjectType.titleObject.transform.GetSiblingIndex() + 1 + i;
                GetEthItem(valus, a);
            }
            else
            {
                ethItemList[kvp.Key.ToLower()].Init(valus);
            }

            i++;
        }
        ShowTitle();
    }
    private void InitEosItem()
    {
        int i = 0;
        Dictionary<string, EOSWalletInfo> tempEosAddresListDic = GetTheSameNameWallet(NewWalletManager._Intance.eoscoinAddressListInfo);

        foreach (KeyValuePair<string, EOSWalletInfo> kvp in tempEosAddresListDic)
        {
            string[] valus = new string[2] { kvp.Key, kvp.Value.walletName };
            if (!eosItemList.ContainsKey(kvp.Key.ToLower()))
            {
                GameObject go = GameObject.Instantiate(eosObjectType.cloneItem.gameObject);
                go.SetActive(true);
                go.name = i.ToString();
                EosItem item = go.GetComponent<EosItem>();
                item.InitEOS(kvp.Value);

                go.transform.SetParent(pareten);
                int a = eosObjectType.titleObject.transform.GetSiblingIndex();
                go.transform.SetSiblingIndex(a + 1 + i);
                go.GetComponent<RectTransform>().localScale = Vector3.one;
                eosItemList.Add(kvp.Key.ToLower(), item);
            }
            else
            {
                ethItemList[kvp.Key.ToLower()].Init(valus);
            }

            i++;
        }
        ShowTitle();
    }
    private void InitUsdtItem()
    {
        int i = 0;
        Dictionary<string, string> tempusdtAddresListDic = GetTheSameNameWallet(NewWalletManager._Intance.usdtAddresListInfo);
        foreach (KeyValuePair<string, string> kvp in tempusdtAddresListDic)
        {
            UsdtcoinInfoItem item = null;
            string[] valus = new string[2] { kvp.Key, kvp.Value };
            List<string> mulitAddress = new List<string>();
            bool addobj = true;
            //try
            //{
            //    mulitAddress.Clear();
            //    Debug.Log("mulit" + multiJSData.multiWalletInfoList[0].Multi_btcAddress + " usdt:" + kvp.Key);
            //    for (int j = 0; j < multiJSData.multiWalletInfoList.Count; j++)
            //    {
            //        mulitAddress.Add(multiJSData.multiWalletInfoList[j].Multi_btcAddress);
            //    }
            //}
            //catch (Exception)
            //{
                
            //}
            //if (mulitAddress.Contains(kvp.Key))
            //{
            //    continue;
            //}
            if (usdtItemList.ContainsKey(kvp.Key))
            {
                addobj = false;
            }

            if (addobj)
            {

                GameObject go;
                int a = usdtObjectType.titleObject.transform.GetSiblingIndex() + 1 + i;
                go = GameObject.Instantiate(usdtObjectType.cloneItem.gameObject);
                go.SetActive(true);
                item = go.GetComponent<UsdtcoinInfoItem>();
                item.Init(valus);
                go.transform.SetParent(pareten);
                go.transform.SetSiblingIndex(a);
                go.GetComponent<RectTransform>().localScale = Vector3.one;
                usdtItemList.Add(valus[0], item);
            }
            else
            {
                item = usdtItemList[kvp.Key];
                item.gameObject.SetActive(true);
                item.Init(valus);
            }
            i++;
        }
        ShowTitle();
    }
    #endregion



    private void GetEthItem(string[] valus, int index)
    {
        GameObject go = GameObject.Instantiate(ethObjectType.cloneItem.gameObject);
        if (ethItemList.Count <= 0)
        {
            go.SetActive(true);
        }
        else if (ethItemList.Values.First().gameObject.activeInHierarchy)
        {
            go.SetActive(true);
        }
        else
        {
            go.SetActive(false);
        }

        EthcoinInfoItem item = go.GetComponent<EthcoinInfoItem>();
        item.Init(valus);
        go.transform.SetParent(pareten);
        go.transform.SetSiblingIndex(index);
        go.GetComponent<RectTransform>().localScale = Vector3.one;
        ethItemList.Add(valus[0].ToLower(), item);
    }
  
    public bool isNoone;
    public void ShowTitle()
    {
//		titleObject.SetActive (false);
//      pullObject.SetActive(false);
        noWalletMark.SetActive(true);
        isNoone = true;
        ChangeObjectType(btcObjectType, btcitemList.Count, ref isNoone);
        ChangeObjectType(ethObjectType, ethItemList.Count, ref isNoone);
        ChangeObjectType(eosObjectType, eosItemList.Count, ref isNoone);
        ChangeObjectType(usdtObjectType, usdtItemList.Count, ref isNoone);
        /*
        if (btcitemList.Count > 0)
        {
           btcObjectType.titleObject.SetActive(true);
           btcObjectType.lastLine.SetActive(true);
           SetBackGroundSizeDate(btcObjectType, btcitemList.Count);
//		   titleObject.SetActive (true);
//         pullObject.SetActive(true);
           noWalletMark.SetActive(false);
           isNoone = false;
        }
        else
        {
            btcObjectType.titleObject.SetActive(false);
            btcObjectType.lastLine.SetActive(false);
        }

        if (ethItemList.Count > 0)
        {
            ethObjectType.titleObject.SetActive(true);
            ethObjectType.lastLine.SetActive(true);
            SetBackGroundSizeDate(ethObjectType, ethItemList.Count);
//			titleObject.SetActive (true);
//            pullObject.SetActive(true);
            noWalletMark.SetActive(false);
            isNoone = false;
        }
        else
        {
            ethObjectType.titleObject.SetActive(false);
            ethObjectType.lastLine.SetActive(false);
        }

        if (eosItemList.Count > 0)
        {
            eosObjectType.titleObject.SetActive(true);
            eosObjectType.lastLine.SetActive(true);
            SetBackGroundSizeDate(eosObjectType, eosItemList.Count);
//			titleObject.SetActive (true);
//          pullObject.SetActive(true);
            noWalletMark.SetActive(false);
            isNoone = false;
        }
        else
        {
            eosObjectType.titleObject.SetActive(false);
            eosObjectType.lastLine.SetActive(false);
        }

        if (usdtItemList.Count > 0)
        {
            eosObjectType.titleObject.SetActive(true);
            eosObjectType.lastLine.SetActive(true);
            SetBackGroundSizeDate(eosObjectType, eosItemList.Count);
            //			titleObject.SetActive (true);
            //          pullObject.SetActive(true);
            noWalletMark.SetActive(false);
            isNoone = false;
        }
        else
        {
            eosObjectType.titleObject.SetActive(false);
            eosObjectType.lastLine.SetActive(false);
        }
        */
        if (isNoone)
        {
            totalMony.text = "≈￥0";
        }
    }

    private void ChangeObjectType(CoinTypeObject ct, int count, ref bool isNoone)
    {
        if (count > 0)
        {
            ct.titleObject.SetActive(true);
            ct.lastLine.SetActive(true);
            SetBackGroundSizeDate(ct, count);
            //		   titleObject.SetActive (true);
            //         pullObject.SetActive(true);
            noWalletMark.SetActive(false);
            isNoone = false;
        }
        else
        {
            ct.titleObject.SetActive(false);
            ct.lastLine.SetActive(false);
        }
    }

    public void OnExitLogin()
    {
        btcTotalMoney = 0;
        usdtTotalMoney = 0;
        ethtotalMoney = 0;
        btcObjectType.totalCountText.text = "";
        usdtObjectType.totalCountText.text = "";
        ethObjectType.totalCountText.text = "";
        eosObjectType.totalCountText.text = "";

        foreach (var kvp in btcitemList)
        {
            Destroy(kvp.Value.gameObject);
        }
        btcitemList.Clear();
        foreach (var kvp in usdtItemList)
        {
            Destroy(kvp.Value.gameObject);
        }
        usdtItemList.Clear();
        foreach (var kvp in ethItemList)
        {
            Destroy(kvp.Value.gameObject);
        }
        ethItemList.Clear();

        foreach (var kvp in eosItemList)
        {
            Destroy(kvp.Value.gameObject);
        }
        eosItemList.Clear();

        Closed();
    }

    private bool isHideBtcItems = true;
    public void HideAllBTCItems(GameObject go)
    {
        isHideBtcItems = !isHideBtcItems;
        go.transform.Find("down").gameObject.SetActive(!isHideBtcItems);
        go.transform.Find("up").gameObject.SetActive(isHideBtcItems);
        if (isHideBtcItems == false)
        {
            SetBackGroundSizeDate(btcObjectType, 0);
        }
        else
        {
            SetBackGroundSizeDate(btcObjectType, btcitemList.Count);
        }
        
        foreach (var kvp in btcitemList)
        {
            kvp.Value.gameObject.SetActive(isHideBtcItems);
        }
    }
    private bool isHideUsdtItems = true;
    public void HideAllUsdtItems(GameObject go)
    {
        isHideUsdtItems = !isHideUsdtItems;
        go.transform.Find("down").gameObject.SetActive(!isHideUsdtItems);
        go.transform.Find("up").gameObject.SetActive(isHideUsdtItems);
        if (isHideUsdtItems == false)
        {
            SetBackGroundSizeDate(usdtObjectType, 0);
        }
        else
        {
            SetBackGroundSizeDate(usdtObjectType, usdtItemList.Count);
        }

        foreach (var kvp in usdtItemList)
        {
            kvp.Value.gameObject.SetActive(isHideUsdtItems);
        }
    }
    private bool isHideETHItems = true;
    public void HideAllETHItems(GameObject go)
    {
        
        isHideETHItems = !isHideETHItems;
        if (isHideETHItems == false)
        {
            SetBackGroundSizeDate(ethObjectType, 0);
        }
        else
        {
            SetBackGroundSizeDate(ethObjectType, ethItemList.Count);
        }
        go.transform.Find("down").gameObject.SetActive(!isHideETHItems);
        go.transform.Find("up").gameObject.SetActive(isHideETHItems);
        foreach (var kvp in ethItemList)
        {
            kvp.Value.gameObject.SetActive(isHideETHItems);
        }
    }
    private bool isHideEosItems = true;
    public void HideAllEOSItems(GameObject go)
    {

        isHideEosItems = !isHideEosItems;
        if (isHideEosItems == false)
        {
            SetBackGroundSizeDate(eosObjectType, 0);
        }
        else
        {
            SetBackGroundSizeDate(eosObjectType, eosItemList.Count);
        }
        go.transform.Find("down").gameObject.SetActive(!isHideEosItems);
        go.transform.Find("up").gameObject.SetActive(isHideEosItems);
        foreach (var kvp in eosItemList)
        {
            kvp.Value.gameObject.SetActive(isHideEosItems);
        }
    }

    //删除钱包
    public void OnDeletWallet(CoinInfoItemBase coinInfoItem)
    {
        switch (coinInfoItem.type)
        {
            case NewWalletManager.CoinType.BTC:
                btcitemList.Remove(coinInfoItem.coinInfo.address.ToLower());
                NewWalletManager._Intance.bitcoinAddresListInfo.Remove(coinInfoItem.coinInfo.address);
                EventManager.Instance.SendEvent(EventID.UpdateBTCbalance);
                break;
            case NewWalletManager.CoinType.ETH:
                ethItemList.Remove(coinInfoItem.coinInfo.address.ToLower());
                NewWalletManager._Intance.ethcoinAddresListInfo.Remove(coinInfoItem.coinInfo.address);
                EventManager.Instance.SendEvent(EventID.UpdateETHbalance);
                break;
            case NewWalletManager.CoinType.EOS:
                Debug.Log("coinInfoItem.coinInfo.address = " + coinInfoItem.coinInfo.address);
                eosItemList.Remove(coinInfoItem.coinInfo.address.ToLower());
                NewWalletManager._Intance.eoscoinAddressListInfo.Remove(coinInfoItem.coinInfo.address);
                break;
            case NewWalletManager.CoinType.USDT:
                usdtItemList.Remove(coinInfoItem.coinInfo.address);
                NewWalletManager._Intance.usdtAddresListInfo.Remove(coinInfoItem.coinInfo.address);
                break;
        }
        Destroy(coinInfoItem.gameObject);
        ShowTitle();
    }

    public override void Open()
    {
        currentUnitType = (UnitType)PlayerPrefs.GetInt(NewWalletManager._Intance.userName + NewWalletManager.COIN_UNIT_TYPE);
        PlayerPrefs.SetString(NewWalletManager._Intance.userName + "WalletETHTotalCount", "0");
        PlayerPrefs.SetString(NewWalletManager._Intance.userName + "WalletBTCTotalMoney", "0");
        PlayerPrefs.SetString(NewWalletManager._Intance.userName + "WalletBTCTotalCount", "0");
        PlayerPrefs.SetString(NewWalletManager._Intance.userName + "WalletETCTotalMoney", "0");

        SetBalanceFalse();
        gameObject.SetActive(true);
        InitBtcItem();
        InitEthItem();
        InitEosItem();
        InitUsdtItem();
        //NewWalletManager._Intance.DoTweenBack(settingPanel.transform, -1000);
        ClickMainBtn();
        ShowUnitSprite();

        if (!NewWalletManager._Intance.IsNeedColdWallet)
        {
            if (importWalletBtn != null)
                importWalletBtn.SetActive(true);
            //string str = SeedKeyManager.Instance.GetAddresses();

            //SeedKeyManager.Instance.SavaAddressToServer(str, null, null, null, false);
            StartCoroutine(ShowWallet());

            noColdWalletObject.SetActive(false);

            if (SeedKeyManager.Instance.IsBackUp())
            {
                noColdWalletShow.SetActive(false);
            }
            else
            {
                noColdWalletShow.SetActive(true);
            }

            ShowBottomBtnPos();
            PlayerPrefs.DeleteKey("ResetLoginAgain");
        }
        else
        {
            if(importWalletBtn != null)
                importWalletBtn.SetActive(false);

            noColdWalletObject.SetActive(true);
            noColdWalletShow.SetActive(false);
        }
    }

    IEnumerator ShowWallet()
    {
        string[] bipArr = SeedKeyManager.Instance.GetSeedBipList();
        for (int i = 0; i < bipArr.Length; i++)
        {
            string str = SeedKeyManager.Instance.GetAddresses(bipArr[i]);
            SeedKeyManager.Instance.AddBipToDic(bipArr[i]);
            SeedKeyManager.Instance.SavaAddressToServer(str, null, null, null, false);

            yield return new WaitForSeconds(0.5F);
        }

        yield return null;
    }

    private void ShowBottomBtnPos()
    {
        RectTransform rt1 = shanDuiBtn.gameObject.GetComponent<RectTransform>();
        float x1 = shanDuiBtn.gameObject.GetComponent<MainPanelBottomBtn>().pos2;

        RectTransform rt2 = hangQingBtn.gameObject.GetComponent<RectTransform>();
        float x2 = hangQingBtn.gameObject.GetComponent<MainPanelBottomBtn>().pos2;

        rt1.localPosition = new Vector3(x1, rt1.localPosition.y, rt1.localPosition.z);// 
        rt2.localPosition = new Vector3(x2, rt2.localPosition.y, rt2.localPosition.z);
    }

    void OnEnable()
    {
        //StartCoroutine(ShowLoading());
    }

    IEnumerator ShowLoading()
    {
        yield return new WaitForFixedUpdate();
        //if (btcitemList.Count == 0 && ethItemList.Count == 0)
        //{
        //    PanelManager._Instance.loadingPanel.SetActive(false);
        //}
        //else
        //{
        //    PanelManager._Instance.loadingPanel.SetActive(true);
        //}
    }

    private void ShowUnitSprite()
    {
        switch (currentUnitType)
        {
            case UnitType.BTC:
                unitIcon.overrideSprite = TextureUIAsset._Instance.btcUnit;
                break;
            case UnitType.RMB:
                unitIcon.overrideSprite = TextureUIAsset._Instance.rmbUnit;
                break;
            case UnitType.USD:
                unitIcon.overrideSprite = TextureUIAsset._Instance.usdUnit;
                break;

        }
    }

    private void SetBalanceFalse()
    {

        foreach (var kvp in ethItemList)
        {
            kvp.Value.isGetBalance = false;
        }

        foreach (var kvp in btcitemList)
        {
            kvp.Value.isGetBalance = false;
        }
        foreach (var kvp in usdtItemList)
        {
            kvp.Value.isGetBalance = false;
        }
        foreach (var kvp in eosItemList)
        {
            kvp.Value.isGetBalance = false;
        }
    }

    private void ClickMainBtn()
    {
        currentBtn = mainBtn;
		ShowBtnMarks ();
        settingPanel.gameObject.SetActive(false);
    }

    private void SetBackGroundSizeDate(CoinTypeObject currentObject, int count)
    {
        if (count > 0)
        {
            currentObject.backGround.sizeDelta = new Vector2(currentObject.backGround.sizeDelta.x, backGrounSize_Y1 + backGroundAddY * (count - 1));
        }
        else
        {
            currentObject.backGround.sizeDelta = new Vector2(currentObject.backGround.sizeDelta.x, backgrounSize_Y0);
        }
    }

    public void OnClickMainBtn()
    {
        if (currentBtn == mainBtn)
            return;
		
        ClosePanel();
        ClickMainBtn();

		ShowTitle ();
    }

    public void Refresh()
    {
        SetBalanceFalse();
        foreach (var kep in btcitemList)
        {
            kep.Value.RefreshBalance();
        }
        foreach (var kep in usdtItemList)
        {
            kep.Value.RefreshBalance();
        }
        foreach (var kep in ethItemList)
        {
            kep.Value.RefreshBalance();
        }
        foreach (var kep in eosItemList)
        {
            kep.Value.RefreshBalance();
        }
    }


    public void ClosePanel()
    {
        if (KyberTools.instance != null)
            KyberTools.instance.ShutDownKyberPage();

        switch (PanelManager._Instance.currentSubPage)
        {
            case PanelManager.SubPage.Mine:
                settingPanel.gameObject.SetActive(false);
                break;
            case PanelManager.SubPage.HangQing:
                hangQingPanel.gameObject.SetActive(false);
                break;
            case PanelManager.SubPage.Property:

                break;
            case PanelManager.SubPage.WalletInfo:
                PanelManager._Instance._WalletInfoPanel.OnClickBackBtn();
                break;
            case PanelManager.SubPage.WalletMatching:
                PanelManager._Instance._mainPanel._scanPanl.HideMe();
                break;
        }
    }

	public void OnClicShanDuiBtn()
	{
		if (currentBtn == shanDuiBtn)
			return;
		ClosePanel();
		currentBtn = shanDuiBtn;
		ShowBtnMarks ();
		PanelManager._Instance.OpenKyberPanel ();
	}

    public void OnClickSettingBtn()
    {
        if (currentBtn == settingBtn)
            return;
		noWalletMark.SetActive(false);
        ClosePanel();
        currentBtn = settingBtn;
		ShowBtnMarks ();
        settingPanel.Open();
        // NewWalletManager._Intance.DOTweenCome(settingPanel.transform, -1000, 0);
    }

    public void OnClickHangQing()
    {
        if (currentBtn == hangQingBtn)
            return;
        ClosePanel();
        currentBtn = hangQingBtn;
        ShowBtnMarks();
        hangQingPanel.Open();
    }

	private void ShowBtnMarks()
	{
		mainNormal.SetActive(true);
		settingNormal.SetActive(true);
		shanDuiNormal.SetActive (true);
		hangQingNormal.SetActive (true);

		mainSelect.SetActive(false);
		settingSelect.SetActive(false);
		shanDuiSelect.SetActive (false);
		hangQingSelect.SetActive (false);

		if (currentBtn == settingBtn) {
			settingSelect.SetActive (true);
			settingNormal.SetActive (false);
		} else if (currentBtn == mainBtn) {
			mainSelect.SetActive (true);
			mainNormal.SetActive (false);
		} else if (currentBtn == shanDuiBtn) {
			shanDuiNormal.SetActive (false);
			shanDuiSelect.SetActive (true);
		} else if (currentBtn == hangQingBtn) 
		{
            hangQingNormal.SetActive(false);
            hangQingSelect.SetActive(true);
		}
	}


    public void OnClickNewWalletBtn()
    {
        if (currentBtn == creatBtn)
        {
            Debug.Log("adfa");
            return;
        }

		ShowBtnMarks ();
        ClosePanel();
        currentBtn = creatBtn;
//        NewWalletManager._Intance.currentCoinType = NewWalletManager.CoinType.BTC;
        ShowCoolPanel();
    }

    private void ShowCoolPanel()
    {
        _scanPanl.Open();
       // _coolPanel.ShowMe();
       // NewWalletManager._Intance.DOTweenCome(_coolPanel.transform, -1000, 0);
    }
    public void OnClickNewETHBtn()
    {
//        NewWalletManager._Intance.currentCoinType = NewWalletManager.CoinType.ETH;
        ShowCoolPanel();
    }

    public void RefreshMessageInfo()
    {
        //int bage = MessageInfo.RefreshBage();
        //if(bage >0 )
        //{
        //    redPoint.SetActive(true);
        //}
        //else
        //{
        //    redPoint.SetActive(false);
        //}
    }

    public void SetRedPoint(int bage)
    {
        if (bage > 0)
        {
            redPoint.SetActive(true);
        }
        else
        {
            redPoint.SetActive(false);
        }
    }

}
