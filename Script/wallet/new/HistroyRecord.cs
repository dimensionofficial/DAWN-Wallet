using System.Collections;
using System.Collections.Generic;
using Nethereum.Util;
using UnityEngine;
using UnityEngine.UI;

public class HistroyRecord : MonoBehaviour
{


    public enum TabType
    {
        SelectALL = 0,
        SendType,
        RecordType,
        SelfType,
    }
    public TabType currentTab = TabType.SelectALL;

    public enum CurrentHistroyType
    {
        None,
        BTC,
        ETH,
        TOKen,
        USDT,
    }
    public CurrentHistroyType type = CurrentHistroyType.None;
    [System.NonSerialized]
    public string currentAddress;
//    [System.NonSerialized]
    public string tokenName;

    public RectTransform viewport;
    public Transform bottom;

    public RectTransform recordParent;
    public RecordPrefab recordPrefab;
    public TransactionPanel transactionPanel;

    public bool isNoMore = false;
    public GameObject cliceLoadMore;
    public Text loadText;

    [System.NonSerialized]
    public string currentUnit;

    /// <summary>
    /// 一页多少个item
    /// </summary>
    public const int pageCount = 20;
    /// <summary>
    /// 当前页显示到第几个
    /// </summary>
    private int currentPageCount = 0;

    private TokenService tokenService;

    /// <summary>
    /// 当前item的sibling
    /// </summary>
    private int index = 0;

    private int m_decimal = 18;

    public List<RecordTabBtn> tabBtnList = new List<RecordTabBtn>();
    private RecordTabBtn currentTabBtn;

    public List<ETHHistoryRcord> currentShowRecord = new List<ETHHistoryRcord>();

    public List<RecordPrefab> cloneRecordItemList = new List<RecordPrefab>();


    public bool isERC20 = false;

    public void RefreshItem(List<ETHHistoryRcord> targetList, string unit)
    {
        if (cloneRecordItemList.Count > 0)
            return;

        if (type == CurrentHistroyType.None)
            return;

        InitRcordItem(targetList, unit);
    }


    //public void RefreshTokenItem(EthTokenItem tokenItem)
    //{
    //    if (gameObject.activeInHierarchy == false || cloneRecordItemList.Count > 0)
    //        return;

    //    if (type == CurrentHistroyType.None)
    //        return;

    //    ShowRecord(tokenItem);
    //}

    public void OnClickTabBtns(RecordTabBtn targetBtn)
    {
        if (targetBtn.myType == currentTab)
            return;

        targetBtn.selectMark.SetActive(true);
        if (currentTabBtn != null)
        {
            currentTabBtn.selectMark.SetActive(false);
        }
        currentTabBtn = targetBtn;

        currentTab = targetBtn.myType;

        SqliteDicItem.HistoryType historyType = SqliteDicItem.HistoryType.All;
        switch (currentTab)
        {
            case TabType.SelectALL:
                historyType = SqliteDicItem.HistoryType.All;
                break;
            case TabType.SelfType:
                historyType = SqliteDicItem.HistoryType.Self;
                break;
            case TabType.SendType:
                historyType = SqliteDicItem.HistoryType.Send;
                break;
            case TabType.RecordType:
                historyType = SqliteDicItem.HistoryType.Received;
                break;
        }
        string tokenContractAddress = "";
        bool isToken = false;
        if (type == CurrentHistroyType.TOKen)
        {
            tokenContractAddress = tokenService.tokenContractAddress;
            isToken = true;
        }
        if (type == CurrentHistroyType.USDT)
        {
            BTCGetHistory.Instance.GetUSDTHistory(currentAddress, (o) =>
            {
                Debug.Log("USDT 记录 " + o.Count);
                List<ETHHistoryRcord> rcordList = PanelManager._Instance._WalletInfoPanel.currentItem.AddLocalData(o, false, "USDT");
                InitRcordItem(rcordList, "USDT", false);
            }, (err) =>
            {

            }, 1, 20);
        }
        else
        {
            HistoryManagerNew.Instance.GetHistory(1, pageCount, currentAddress, (o) =>
            {
                List<ETHHistoryRcord> rcordList = PanelManager._Instance._WalletInfoPanel.currentItem.AddLocalData(o, isToken, tokenName);
                InitRcordItem(rcordList, currentUnit, false);
            }, tokenContractAddress, false, isToken, historyType);
        }
    
    }

    private void InitTabBtns()
    {
        currentTab = TabType.SelectALL;
        currentTabBtn = tabBtnList[0];
        currentTabBtn.selectMark.SetActive(true);
        for (int i = 1; i < tabBtnList.Count; i++)
        {
            tabBtnList[i].selectMark.SetActive(false);
        }
    }

    public void ShowRecord(EthTokenItem tokenItem)
    {

        EthcoinInfoItem currentItem = PanelManager._Instance._WalletInfoPanel.currentItem as EthcoinInfoItem;

        if (currentItem.type != NewWalletManager.CoinType.ETH)
        {
            isERC20 = false;
            return;
        }
        tokenName = "";
        isERC20 = true;
        currentAddress = currentItem.coinInfo.address.ToLower();
        //显示记录
        //string myAddress = currentItem.coinInfo.address.ToLower();
        if (tokenItem.tokenNameText.text == "ETH")//以太坊记录
        {
  //        isERC20 = false;
            m_decimal = 18;
            type = CurrentHistroyType.ETH;
            HistoryManagerNew.Instance.GetHistory(1, pageCount, currentAddress, (o) =>
            {
                List<ETHHistoryRcord> rcordList = currentItem.AddLocalData(o, false, tokenName);
                InitRcordItem(rcordList, "ETH");
            });
        }
        else
        {
            HistoryManagerNew.Instance.GetHistory(1, HistroyRecord.pageCount, currentAddress, (o) =>
            {
                if (!EthTokenManager._Intance.tokenServiceDic.ContainsKey(tokenItem.containsAddress.ToLower()))
                {
                    tokenService = EthTokenManager._Intance.GetTokenService(tokenItem.containsAddress, tokenItem.tokenNameText.text, true);
                }
                else
                {
                    tokenService = EthTokenManager._Intance.tokenServiceDic[tokenItem.containsAddress.ToLower()];
                }
                m_decimal = tokenService.tokenDecimal;

                type = CurrentHistroyType.TOKen;
                tokenName = tokenItem.tokenNameText.text;

                List<ETHHistoryRcord> tokenHistorys = currentItem.AddLocalData(o, true, tokenName);

                InitRcordItem(tokenHistorys, tokenItem.tokenNameText.text);

            }, tokenItem.containsAddress, false, true);
        }
    }

    private bool IsFullRect()
    {
        int c = cloneRecordItemList.Count;
        int temp = c * 140 + (c - 1) * 2;
        if (temp > viewport.rect.height)
            return true;

        return false;
    }

    private void InitScrollView()
    {
        RectTransform rt = recordParent.GetComponent<RectTransform>();
        rt.localPosition = new Vector3(rt.localPosition.x, 0, rt.localPosition.z);
    }

    public void InitRcordItem(List<ETHHistoryRcord> targetList, string unit, bool isInitBabBtns = true)
    {
        currentPage = 2;
        if (unit.Equals("BTC") || unit.Equals("USDT"))
        {
            isERC20 = false;
        }
        currentUnit = unit;
        currentShowRecord = targetList;

        if(isInitBabBtns)
            InitTabBtns();

        FirstShowPageItem(targetList);
    }

    private void FirstShowPageItem(List<ETHHistoryRcord> listRcord)
    {
        loadText.text = "点击加载更多";
        isNoMore = false;
        
        InitScrollView();

        ActiveRecord(true);
        index = 0;

        ShowOnePageItem(listRcord);

        if (IsFullRect())
        {
            cliceLoadMore.gameObject.SetActive(true);
            bottom.gameObject.SetActive(true);
        }
        else
        {
            cliceLoadMore.gameObject.SetActive(false);
            bottom.gameObject.SetActive(false);
        }
    }

    private int currentPage = 2;

    public void OnClickMorePage()
    {

        if (isNoMore || type == CurrentHistroyType.None)
        {
            return;
        }

        SqliteDicItem.HistoryType historyType = SqliteDicItem.HistoryType.All;
        switch (currentTab)
        {
            case TabType.SelectALL:
                historyType = SqliteDicItem.HistoryType.All;
                break;
            case TabType.SelfType:
                historyType = SqliteDicItem.HistoryType.Self;
                break;
            case TabType.SendType:
                historyType = SqliteDicItem.HistoryType.Send;
                break;
            case TabType.RecordType:
                historyType = SqliteDicItem.HistoryType.Received;
                break;
        }
        string tokenContractAddress = "";
        bool isToken = false;

        if (type == CurrentHistroyType.TOKen)
        {
            tokenContractAddress = tokenService.tokenContractAddress;
            isToken = true;
        }

        HistoryManagerNew.Instance.GetHistory(currentPage, HistroyRecord.pageCount, currentAddress, (o) =>
        {
            ShowOnePageItem(o);
            currentPage++;
           
        }, tokenContractAddress, false, isToken, historyType);

    }

    public void ShowOnePageItem(List<ETHHistoryRcord> listRcord)
    {
        if (isNoMore)
        {
            return;
        }
        currentPageCount = 0;
        InitRecordItem(listRcord);
        cliceLoadMore.transform.SetAsLastSibling();
        bottom.SetAsLastSibling();
        if (listRcord.Count < HistroyRecord.pageCount)
        {
            loadText.text = "没有更多信息";
            isNoMore = true;
        }
    }

    private void InitRecordItem(List<ETHHistoryRcord> listRcord)
    {
        for (int i = 0; i < listRcord.Count; i++)
        {
            ETHHistoryRcord historyRcord = listRcord[i];

            if (type == CurrentHistroyType.ETH)
            {
                if (historyRcord.input.Equals("0x"))
                {
                    AddRecordItem(historyRcord, index);
                    index++;
                    currentPageCount++;
                }
            }
            else
            {
                AddRecordItem(historyRcord, index);
                index++;
                currentPageCount++;
            }


            //if (i <= 0)
            //{
            //    loadText.text = "没有更多信息";
            //    isNoMore = true;
            //}

            if (currentPageCount >= pageCount)
            {
                break;
            }

        }
    }

    private void AddRecordItem(ETHHistoryRcord historyRcord, int siblingIndex)
    {
        
        CoinInfoItemBase currentItem = PanelManager._Instance._WalletInfoPanel.currentItem;
        if (currentItem==null)
        {
            return;
        }
        string myAddress = currentItem.coinInfo.address.ToLower();
        string fromAddress = historyRcord.from.ToLower();
        string toAddress = historyRcord.to.ToLower();

        if (currentTab != TabType.SelectALL)
        {
            if (fromAddress == myAddress && toAddress == myAddress)
            {
                if (currentTab != TabType.SelfType)
                {
                    return;
                }
            }
            else if (fromAddress == myAddress)
            {
                if (currentTab != TabType.SendType)
                {
                    return;
                }
            }
            else if (toAddress == myAddress)
            {
                if (currentTab != TabType.RecordType)
                {
                    return;
                }
            }
        }

        RecordPrefab prefab = Instantiate(recordPrefab);
        prefab.transform.SetParent(recordParent, false);

        prefab.InitShow(historyRcord, isERC20, m_decimal, currentUnit);
        prefab.Open.onClick.AddListener(delegate ()
        {
            transactionPanel.Open(prefab);
        });

        cloneRecordItemList.Add(prefab);
    }

    public void OnClickBackBtn()
    {
        ActiveRecord(false);
    }

    public void ActiveRecord(bool isActivePage = false)
    {
        if (!isActivePage)
        {
            currentAddress = "";
            type = CurrentHistroyType.None;
        }
        gameObject.SetActive(isActivePage);
        //清除现有记录prefab
        List<GameObject> tempList = new List<GameObject>();
        for (int i = 0; i < cloneRecordItemList.Count; i++)
        {
            tempList.Add(cloneRecordItemList[i].gameObject);
        }
        for (int i = 0; i < tempList.Count; i++)
        {
            Destroy(tempList[i]);
        }

        cloneRecordItemList.Clear();
    }


}
