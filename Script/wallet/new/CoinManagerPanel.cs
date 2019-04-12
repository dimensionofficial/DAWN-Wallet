using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveSwitchTokenInfo
{
	//string conAddress, string symbol, string iconPath, string fullName
	/// <summary>
	/// 合约地址
	/// </summary>
	public string conAddress;
	/// <summary>
	/// 代币简称
	/// </summary>
	public string symbol;
	/// <summary>
	/// 代币icon
	/// </summary>
	public string iconPath;
	/// <summary>
	/// 全名
	/// </summary>
	public string fullName;
}

public class CoinManagerPanel : MonoBehaviour
{
    public Button delBtn;

    public GameObject ethItem;

    public WalletInfoPanel walletInfoPanel;

    public RectTransform conect;

    public InputField inputText;

    public Transform parent;

    public CoinSwitchItem cloneItem;

    public List<CoinSwitchItem> itemList = new List<CoinSwitchItem>();

    public GameObject controlPanel;
    
    [System.NonSerialized]
    public EthcoinInfoItem currentethItem;


    public GameObject noSearch;
    public GameObject putToken;

    public void OnChangeInput()
    {
        string v = inputText.text;
        v = v.Replace(" ", "");

        Search(v);
        InitItem();

        if (string.IsNullOrEmpty(v))
        {
            delBtn.gameObject.SetActive(false);
            ShowMe(currentethItem);
        }
        else
        {
            Search(v);
            delBtn.gameObject.SetActive(true);
        }
    }

    public void OnClickRegulateBtn()
    {
        controlPanel.SetActive(true);
    }

    public void OnClickDelBtn()
    {
        inputText.text = "";
        inputText.GetComponent<NativeEditBox>().text = "";
    }

    Coroutine SearchCor = null;

    void Search(string name)
    {
        if (SearchCor != null)
        {
            StopCoroutine(SearchCor);
            SearchCor = null;
            HttpManager._Intance.loadingPanel.gameObject.SetActive(false);
        }
        if (name.Length < 2)
        {
            return;
        }
        List<KeyValuePair<string, string>> ws = new List<KeyValuePair<string, string>>();
        ws.Add(new KeyValuePair<string, string>("op", "searchtoken"));
        ws.Add(new KeyValuePair<string, string>("keywords", name));
        SearchCor = StartCoroutine(HttpManager._Intance.SendRequest(ws, delegate (Hashtable table)
        {
            ArrayList data = table["tokenList"] as ArrayList;
            EthTokenManager._Intance.InitTokenList(data);
            SearchA(name);
        }));
    }

    void SearchA(string name)
    {
        bool isOne = true;
        foreach (KeyValuePair<string, TokenService> kvp in EthTokenManager._Intance.tokenServiceDic)
        {
            bool isSearch = false;
            if (name.Length >= 2)
            {
                if (kvp.Value.TokenInfo.symbol.ToLower().Contains(name.ToLower()))
                {
                    isSearch = true;
                }
            }
            else
            {
                if (kvp.Value.TokenInfo.symbol.ToLower().Equals(name.ToLower()))
                {
                    isSearch = true;
                }
            }
            if (isSearch)
            {
                isOne = false;

                CoinSwitchItem switchItem = GetItem();
                List<string> addressList = currentethItem.GetTokenAddressKeyList();
                bool isOn = false;
                for (int i = 0; i < addressList.Count; i++)
                {
                    ERCContractInofo c = currentethItem.GetERContractInfo(addressList[i]);

                    if (c.contractAddress.Equals(kvp.Key))
                    {
                        isOn = true;
                        break;
                    }
                }
                switchItem.Show(currentethItem, kvp.Value.TokenInfo.iconPath, kvp.Key, kvp.Value.TokenInfo.symbol, kvp.Value.TokenInfo.fullName, isOn);
            }
        }

        if (isOne)
        {
            noSearch.SetActive(true);
        }
        else
        {
            noSearch.SetActive(false);
        }

        ethItem.SetActive(false);
       // yield return 0;
    }

    /// <summary>
    /// 按字母排序
    /// </summary>
    public void OnClickSortByLetter()
    {
        itemList.Sort(
               delegate (CoinSwitchItem p1, CoinSwitchItem p2)
               {
                   return p1.coinName.text.CompareTo(p2.coinName.text);//升序
                });

			

        for (int i = 0; i < itemList.Count; i++)
        {
            itemList[i].transform.SetSiblingIndex(i + 2);
        }

        controlPanel.SetActive(false);
    }
	/// <summary>
	/// 按余额排
	/// </summary>
    public void OnClickSortByBalance()
    {
		itemList.Sort(
			delegate (CoinSwitchItem p1, CoinSwitchItem p2)
			{
				return p2.blance.CompareTo(p1.blance);//降序
			});
				
		for (int i = 0; i < itemList.Count; i++)
		{
			itemList[i].transform.SetSiblingIndex(i + 2);
		}

		controlPanel.SetActive(false);
    }

	public void OnClickHideNoBalace()
	{
		InitItem ();

		List<string> tempAdderss = new List<string> ();

		List<SaveSwitchTokenInfo> savaTokenList = new List<SaveSwitchTokenInfo> ();

		foreach (var kv in currentethItem.tokenDicBalance) 
		{
			if (kv.Value > 0)
			{
				tempAdderss.Add(kv.Key);
				SaveSwitchTokenInfo tempSavaInfo = new SaveSwitchTokenInfo ();
				if (EthTokenManager._Intance.tokenServiceDic.ContainsKey (kv.Key))
				{
					CoinSwitchItem switchItem = GetItem ();
					TokenService ts = EthTokenManager._Intance.tokenServiceDic [kv.Key];
					Debug.Log (ts.TokenInfo.symbol + " || 全名：" + ts.TokenInfo.fullName + " || Icon：" + ts.TokenInfo.iconPath);
					switchItem.Show (currentethItem, ts.TokenInfo.iconPath, kv.Key, ts.TokenInfo.symbol, ts.TokenInfo.fullName, true);
					tempSavaInfo.conAddress = ts.tokenContractAddress;
					tempSavaInfo.symbol = ts.TokenInfo.symbol;
					tempSavaInfo.iconPath = ts.TokenInfo.iconPath;
					tempSavaInfo.fullName = ts.TokenInfo.fullName;
				}else
				{
					
				}

				savaTokenList.Add (tempSavaInfo);
			}
		}

		currentethItem.SetTokenAddressList(savaTokenList);
	}


    public void ShowMe(EthcoinInfoItem ethItem)
    {
        PanelManager._Instance._mainPanel.bottomBtn.SetActive(false);
        gameObject.SetActive(true);
        currentethItem = ethItem;
        //StopCoroutine(ShowItemList(currentethItem.GetTokenAddressKeyList()));
        //ShowItemList(currentethItem.GetTokenAddressKeyList());
        InitItem();
        //StartCoroutine(ShowItemList(currentethItem.GetTokenAddressKeyList()));
        ShowItemList(currentethItem.GetTokenAddressKeyList());
        noSearch.SetActive(false);
        putToken.SetActive(false);
    }

    void ShowItemList(List<string> tokenAddressList)
    {
   //     yield return 0;

        for (int i = 0; i < tokenAddressList.Count; i++)
        {
            ERCContractInofo cn = currentethItem.GetERContractInfo(tokenAddressList[i]);
  //          yield return 0;
            CoinSwitchItem temp = GetItem();
            TokenInfo tokenInfo = EthTokenManager._Intance.tokenServiceDic[cn.contractAddress].TokenInfo;
            string icon = tokenInfo.iconPath;
            string tempAddress = EthTokenManager._Intance.tokenServiceDic[cn.contractAddress].tokenContractAddress;
            string tokenName = tokenInfo.symbol;
            string fullName = tokenInfo.fullName;
            temp.Show(currentethItem, icon, tempAddress, tokenName, fullName, true);
        }

        foreach (KeyValuePair<string, TokenService> kvp in EthTokenManager._Intance.tokenServiceDic)
        {
   //         yield return 0;
            if (kvp.Value.TokenInfo.isShow > 0 && !isEqualsItem(tokenAddressList, kvp.Value.tokenContractAddress))
            {
                CoinSwitchItem switchItem = GetItem();
                
                switchItem.Show(currentethItem, kvp.Value.TokenInfo.iconPath, kvp.Key, kvp.Value.TokenInfo.symbol, kvp.Value.TokenInfo.fullName, false);
            }
        }
        ethItem.SetActive(true);
        ethItem.transform.SetAsFirstSibling();
    }


    private bool isEqualsItem(List<string> nameList, string name)
    {
        for (int i = 0; i < nameList.Count; i++)
        {
            ERCContractInofo cn = currentethItem.GetERContractInfo(nameList[i]);

            if (cn.contractAddress.Equals(name))
                return true;
        }
        return false;
    }

    public void OnClickBackBtn()
    {
      // PanelManager._Instance._mainPanel.bottomBtn.SetActive(true);
        gameObject.SetActive(false);
        conect.localPosition = Vector3.zero;
        //string temp = "";
        //for (int i = 0; i < itemList.Count; i++)
        //{
        //    if (itemList[i].switchToggle.isOn)
        //    {
        //        temp += itemList[i].tokenName + ";";
        //    }
        //}

        //if (temp.Length > 0)
        //{
        //    temp = temp.Substring(0, temp.Length - 1);
        //}
        //else
        //{
        //    temp = "";
        //}
        //PlayerPrefs.SetString(currentethItem.coinInfo.address, temp);

        walletInfoPanel.ShowMe(currentethItem);
 //       currentethItem.RefreshBalance();
    }

    private void InitItem()
    {
        if (itemList.Count > 0)
        {
            for (int i = 0; i < itemList.Count; i++)
            {
                itemList[i].gameObject.SetActive(false);
                itemList[i].isUsed = false;
            }
        }
    }

    public void OnClickPutTokenBtn()
    {
        putToken.SetActive(true);
        inputText.gameObject.SetActive(false);
    }

    public void OnClickPutTokenBackBtn()
    {
        putToken.SetActive(false);
        inputText.gameObject.SetActive(true);
    }

    public CoinSwitchItem GetItem()
    {
        for (int i = 0; i < itemList.Count; i++)
        {
            if (itemList[i].isUsed == false)
            {
                itemList[i].gameObject.SetActive(true);
                itemList[i].isUsed = true;
                return itemList[i];
            }
        }

        GameObject tokenItem = GameObject.Instantiate(cloneItem.gameObject);
        tokenItem.gameObject.SetActive(true);
        tokenItem.transform.SetParent(parent, false);
        CoinSwitchItem switchItem = tokenItem.GetComponent<CoinSwitchItem>();
        switchItem.isUsed = true;
        itemList.Add(switchItem);
        return switchItem;
    }
}
