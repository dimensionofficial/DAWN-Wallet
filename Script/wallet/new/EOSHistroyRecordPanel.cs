using System.Collections;
using System.Collections.Generic;
using Nethereum.Util;
using UnityEngine;
using UnityEngine.UI;

public class EOSHistroyRecordPanel : MonoBehaviour
{

    public HistroyRecord.TabType currentTab = HistroyRecord.TabType.SelectALL;

    public string currentAddress;
    [System.NonSerialized]
    public string tokenName;


    public RectTransform viewport;
    public Transform bottom;

    public Color receiveColor;
    public Color myselfReceiveColor;
    public Color sendColor;

    public RectTransform recordParent;
    public EosRecordPrefab recordPrefab;
    public EOSTransactionPanel transactionPanel;

    private bool isNoMore = false;
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

    /// <summary>
    /// 当前item的sibling
    /// </summary>
    private int index = 0;

    private int m_decimal = 18;

    public List<RecordTabBtn> tabBtnList = new List<RecordTabBtn>();
    private RecordTabBtn currentTabBtn;

    public List<EOSHistoryRcord> currentShowRecord = new List<EOSHistoryRcord>();

    public List<EosRecordPrefab> cloneRecordItemList = new List<EosRecordPrefab>();

    public void RefreshItem(List<EOSHistoryRcord> targetList, string unit)
    {
        if (cloneRecordItemList.Count > 0)
            return;

        //if (type == CurrentHistroyType.None)
        //    return;

        InitRcordItem(targetList);
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

        FirstShowPageItem();
    }

    private void InitTabBtns()
    {
        currentTab = HistroyRecord.TabType.SelectALL;
        currentTabBtn = tabBtnList[0];
        currentTabBtn.selectMark.SetActive(true);
        for (int i = 1; i < tabBtnList.Count; i++)
        {
            tabBtnList[i].selectMark.SetActive(false);
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

    public void InitRcordItem(List<EOSHistoryRcord> targetList)
    { 
        currentShowRecord = targetList;
        InitTabBtns();

        FirstShowPageItem();
    }
    private void FirstShowPageItem()
    {
        loadText.text = "点击加载更多";

        isNoMore = false;

        InitScrollView();

        ActiveRecord(true);
        index = 0;

        ShowOnePageItem();

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



    public void ShowOnePageItem()
    {
        if (isNoMore)
        {
            return;
        }
        currentPageCount = 0;
        int startIndex = index;
        InitRecordItem(startIndex);
        cliceLoadMore.transform.SetAsLastSibling();
        bottom.SetAsLastSibling();
    }

    private void InitRecordItem(int startIndex)
    {
        EosItem it = PanelManager._Instance._eosWalletInfoPanel.currentItem as EosItem;

        if (it.eosWalletInfo != null && !string.IsNullOrEmpty(it.eosWalletInfo.account))
        {
            string account = it.eosWalletInfo.account;

            for (int i = currentShowRecord.Count - 1 - startIndex; i >= 0; i--)
            {
                EOSHistoryRcord historyRcord = currentShowRecord[i];
                AddRecordItem(historyRcord, account);
                index++;
                currentPageCount++;

                if (i <= 0)
                {
                    loadText.text = "没有更多信息";
                    isNoMore = true;
                }

                if (currentPageCount >= pageCount)
                {
                    break;
                }

            }
        }
    }

    private void AddRecordItem(EOSHistoryRcord historyRcord, string myAccount)
    {
        string myAddress = currentAddress;
        string fromAddress = historyRcord.transferdata.from;
        string toAddress = historyRcord.transferdata.to;
        switch (historyRcord.actType)
        {
            case EOSHistoryRcord.ActType.transfer:
            case EOSHistoryRcord.ActType.buyrambytes:
            case EOSHistoryRcord.ActType.sellram:
            case EOSHistoryRcord.ActType.buyram:
                fromAddress = historyRcord.transferdata.from;
                 toAddress = historyRcord.transferdata.to;
                break;
            case EOSHistoryRcord.ActType.delegatebw:
                fromAddress = historyRcord.delegatebwdata.from;
                toAddress = historyRcord.delegatebwdata.receiver;
                break;
            case EOSHistoryRcord.ActType.undelegatebw:
                fromAddress = historyRcord.undelegatebwdata.from;
                toAddress = historyRcord.undelegatebwdata.receiver;
                break;
        }
       

        if (currentTab != HistroyRecord.TabType.SelectALL)
        {
            if (currentTab == HistroyRecord.TabType.SendType && historyRcord.actType == EOSHistoryRcord.ActType.delegatebw)
            {
                
            }
            else
            {
                if (fromAddress == myAddress && toAddress == myAddress)
                {
                    if (historyRcord.actType == EOSHistoryRcord.ActType.delegatebw)
                        return;

                    if (currentTab != HistroyRecord.TabType.RecordType)
                    {
                        if (currentTab != HistroyRecord.TabType.SelfType)
                        {
                            return;
                        }
                    }
                }
                else if (fromAddress == myAddress)
                {
                    if (currentTab != HistroyRecord.TabType.SendType)
                    {
                        return;
                    }
                }
                else if (toAddress == myAddress)
                {
                    if (currentTab != HistroyRecord.TabType.RecordType)
                    {
                        return;
                    }
                }
            }
        }

        EosRecordPrefab prefab = Instantiate(recordPrefab);
        prefab.transform.SetParent(recordParent, false);
        prefab.Init(historyRcord, myAccount);

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
 //           type = CurrentHistroyType.None;
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
