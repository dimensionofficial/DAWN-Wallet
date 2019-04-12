using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LitJson;
using NBitcoin;
using QBitNinja4Unity.Models;
using UnityEngine;
using UnityEngine.UI;

public class BitcoinInfoItem : CoinInfoItemBase
{
    public bool isMultiAddres = false;
    public Image iconImage;
    public Sprite normalMark;
    public Sprite muiltMark;
    //v[0] 地址， v[1] 名字
    public override void Init(string[] v)
    {
        base.Init(v);
        HistoryManagerNew.Instance.Refresh(coinInfo.address, null, RefreshType.BTC);
        try
        {
            isMultiAddres = MultiJSData.instance.IsMultiSigAddress(coinInfo.address);
        }
        catch (System.Exception)
        {
            isMultiAddres = false;
        }
       
        if (isMultiAddres)
        {
            iconImage.sprite = muiltMark;
        }
        else
        {
            iconImage.sprite = normalMark;
        }
    }

    public override void GetHistory()
    {
        HistoryManagerNew.Instance.Refresh(coinInfo.address, null, RefreshType.BTC);

        //if (isFirst)
        //{
        //    //HistoryManager.Instance.GetBTCHistory(coinInfo.address, GetHistroyRecordInfoNet, GetHistroyRecordInfo);  
        //}
        //else
        //{
        //    //HistoryManager.Instance.GetBTCHistory(coinInfo.address, GetHistroyRecordInfoNet, GetHistroyRecordInfo);
        //}
    }

    public override void RefreshBalance()
    {

        string tempMoney = PlayerPrefs.GetString(coinInfo.address + "Balance");
        if (!string.IsNullOrEmpty(tempMoney))
        {
            isGetBalance = true;
            coinInfo.money = decimal.Parse(tempMoney);
            NewWalletManager._Intance.ShowCount(countText, coinInfo.money);
            countText.text = countText.text + " BTC";
            EventManager.Instance.SendEvent(EventID.UpdateBTCbalance);
        }
       
        HistoryManager.Instance.GetBTCBalance(coinInfo.address, GetBalance);
    }



    private void RefreshBalace(params object[] objs)
    {
        RefreshBalance();
    }

    void Start()
    {
        EventManager.Instance.AddEventListener(EventID.UpdateBTCCount, RefreshBalace);
    }

    void OnDestroy()
    {
        EventManager.Instance.RemoveEventListener(EventID.UpdateBTCCount, RefreshBalace);
    }

    void GetBalance(decimal v)
    {
        countText.gameObject.SetActive(true);
        coinInfo.money = (decimal)v;
        NewWalletManager._Intance.ShowCount(countText, coinInfo.money);
       
        countText.text = countText.text + " BTC";
        isGetBalance = true;

        PlayerPrefs.SetString(coinInfo.address + "Balance", coinInfo.money + "");
        EventManager.Instance.SendEvent(EventID.UpdateBTCbalance);
    }

    //private QBitNinja4Unity.Models.BalanceSummaryDetails GetBalaSum(JsonData data)
    //{
    //    QBitNinja4Unity.Models.BalanceSummaryDetails bs = new QBitNinja4Unity.Models.BalanceSummaryDetails();

    //    bs.transactionCount = (int)data["transactionCount"];
    //    object o = data["amount"];
    //    if (o.ToString() != "0")
    //        bs.amount = long.Parse(o.ToString());
    //    else
    //        bs.amount = 0;

    //    object b = data["received"];
    //    if (b.ToString() != "0")
    //        bs.received = long.Parse(b.ToString());
    //    else
    //        bs.received = 0;


    //    ArrayList strs1 = new ArrayList(data["assets"]);
    //    QBitNinja4Unity.Models.AssetBalanceSummaryDetails[] assets = new QBitNinja4Unity.Models.AssetBalanceSummaryDetails[strs1.Count];

    //    for (int j = 0; j < strs1.Count; j++)
    //    {
    //        JsonData tj = (JsonData)strs1[j];
    //        QBitNinja4Unity.Models.AssetBalanceSummaryDetails temp = new QBitNinja4Unity.Models.AssetBalanceSummaryDetails();
    //        temp.asset = (string)tj["asset"];
    //        temp.quantity = (long)tj["quantity"];
    //        temp.received = (long)tj["received"];
    //        assets[0] = temp;
    //    }

    //    bs.assets = assets;

    //    return bs;
    //}


    //void GetBalanceSummaryResponse(QBitNinja.Client.Models.BalanceSummary result, NBitcoin.Network network)
    //{
    //    countText.gameObject.SetActive(true);
    //    coinInfo.money = decimal.Parse(result.Spendable.Amount.ToString());
    //    coinInfo.money = decimal.Parse(coinInfo.money.ToString());
    //    NewWalletManager._Intance.ShowCount(countText, coinInfo.money);
    //    countText.text = countText.text + " BTC";
    //    //PanelManager._Instance._mainPanel.btcCount += coinInfo.money;
    //    isGetBalance = true;
    //    EventManager.Instance.SendEvent(EventID.UpdateBTCbalance);
    //}
}
