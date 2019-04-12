using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LitJson;
using NBitcoin;
using QBitNinja4Unity.Models;
using UnityEngine;
using UnityEngine.UI;

public class UsdtcoinInfoItem : CoinInfoItemBase
{
    public bool isMultiAddres = false;
    public Image iconImage;
    public Sprite normalMark;
    public Sprite muiltMark;
    //v[0] 地址， v[1] 名字
    public override void Init(string[] v)
    {
        base.Init(v);
        HistoryManagerNew.Instance.Refresh(coinInfo.address, null, RefreshType.USDT);
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
        HistoryManagerNew.Instance.Refresh(coinInfo.address, null, RefreshType.USDT);

        //if (isFirst)
        //{
        //    HistoryManager.Instance.GetBTCHistory(coinInfo.address, GetHistroyRecordInfoNet, GetHistroyRecordInfo);  
        //}
        //else
        //{
        //    HistoryManager.Instance.GetBTCHistory(coinInfo.address, GetHistroyRecordInfoNet, GetHistroyRecordInfo);
        //}
    }

    public override void RefreshBalance()
    {
        string tempMoney = PlayerPrefs.GetString("USDT"+coinInfo.address + "Balance");
        if (!string.IsNullOrEmpty(tempMoney))
        {
            isGetBalance = true;
            coinInfo.money = decimal.Parse(tempMoney);
            NewWalletManager._Intance.ShowCount(countText, coinInfo.money);
            countText.text = countText.text + " USDT";
            EventManager.Instance.SendEvent(EventID.UpdateUSDTbalance);
        }

        HistoryManager.Instance.GetUSDTBalance(coinInfo.address, GetBalance);
    }



    private void RefreshBalace(params object[] objs)
    {
        RefreshBalance();
    }

    void Start()
    {
        EventManager.Instance.AddEventListener(EventID.UpdateUSDTCount, RefreshBalace);
    }

    void OnDestroy()
    {
        EventManager.Instance.RemoveEventListener(EventID.UpdateUSDTCount, RefreshBalace);
    }

    void GetBalance(decimal v)
    {
        if (v<0)
        {
            v =decimal.Parse(PlayerPrefs.GetString("USDT" + coinInfo.address + "Balance"));
        }
        countText.gameObject.SetActive(true);
        coinInfo.money = (decimal)v;
        NewWalletManager._Intance.ShowCount(countText, coinInfo.money);

        countText.text = countText.text + " USDT";
        isGetBalance = true;

        PlayerPrefs.SetString("USDT" + coinInfo.address + "Balance", coinInfo.money + "");
        EventManager.Instance.SendEvent(EventID.UpdateUSDTbalance);
    }

}

