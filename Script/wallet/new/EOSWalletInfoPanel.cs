using Nethereum.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EOSWalletInfoPanel : MonoBehaviour
{
    private bool isFirstShow = true;

    public Text addressText;

    public GameObject remindObject;

//  [System.NonSerialized]
    public CoinInfoItemBase currentItem;


    public Text walletName;
    public EOSSendCoinPanel sendCoinPanel;
    public ReceiveCoinPanel receiveCoinPanel;
    public Image m_Image;
    public Text countText;
    public Text rmbText;
    public Text typeText1;

    public GameObject delBtnObject;

    public EOSHistroyRecordPanel recordPage;

    public EOSWalletInfo eoswalletInfo;
    public EosVotePanel eosVotePanel;

    public GameObject eosHistoryLoading;

    void Start()
    {
        EventManager.Instance.AddEventListener(EventID.UpdateTotalbalance, UpdateMoney);
    }

    void OnDestory()
    {
        EventManager.Instance.RemoveEventListener(EventID.UpdateTotalbalance, UpdateMoney);
    }

    private void UpdateMoney(params object[] obj)
    {
        UpdateMoney();
    }

    public void UpdateMoney()
    {
        if (currentItem == null)
            return;

        EosItem item = currentItem as EosItem;

        decimal rmb = 0;
        NewWalletManager._Intance.ShowCount(countText, item.eosWalletInfo.balance);

        decimal cup = 0;
        decimal net = 0;
        string c = item.eosWalletInfo.self_delegated_bandwidth.cpu_weight;
        if (!string.IsNullOrEmpty(c))
        {
            c = c.Substring(0, c.Length - 4);
            cup = decimal.Parse(c);
        }
        string n = item.eosWalletInfo.self_delegated_bandwidth.net_weight;
        if (!string.IsNullOrEmpty(n))
        {
            n = n.Substring(0, n.Length - 4);
            net = decimal.Parse(n);
        }
        decimal tot = cup + net;

        m_Image.overrideSprite = TextureUIAsset._Instance.eosIcon;
        countText.text = countText.text + " EOS<size=27>   （抵押 "+ tot + " EOS）</size>";
        typeText1.text = "EOS";
        rmb = item.eosWalletInfo.balance * HttpManager._Intance.eos_RMB;

        NewWalletManager._Intance.ShowProperty(rmbText, rmb);

    }

    public void ShowMe(CoinInfoItemBase currentItem)
    {
        EosItem eos = currentItem as EosItem;
        eoswalletInfo = eos.eosWalletInfo;

        gameObject.SetActive(true);
        delBtnObject.SetActive(true);

        PanelManager._Instance.currentSubPage = PanelManager.SubPage.WalletInfo;
        //rmbText.text ="￥" currentItem.coinInfo.money * HttpManager._
        string adr = currentItem.coinInfo.address;

        string qianAdr = adr.Substring(0, 10); //adr.Substring(result.Text.Length - lastCount, lastCount);
        string houAdr = adr.Substring(adr.Length - 10, 10);

        addressText.text = qianAdr + "..." + houAdr;

        this.currentItem = currentItem;
        walletName.text = currentItem.coinname;

        NewWalletManager._Intance.currentCoinType = currentItem.type;

        UpdateMoney();
        if (isFirstShow)
        {
            isFirstShow = false;
            eosHistoryLoading.SetActive(true);
        }
        EosGetSingInfoPanel.Instance.GetHistory(eoswalletInfo, delegate (Hashtable t)
        {
            if (t != null && t.ContainsKey("actions"))
            {
                eos.eosHistoryRcordlist = new List<EOSHistoryRcord>();
                eos.tempRecordDic.Clear();
                ArrayList list = t["actions"] as ArrayList;
                for (int i = 0; i < list.Count; i++)
                {
                    eos.AddEosHistroy(list[i] as Hashtable);
                }
                eos.tempRecordDic.Clear();
            }
            recordPage.currentAddress = eoswalletInfo.account;
            recordPage.InitRcordItem(eos.eosHistoryRcordlist);
            eosHistoryLoading.SetActive(false);
        }, delegate() 
        {
            eosHistoryLoading.SetActive(false);
        }, false);

    }

    public void OnClickBackBtn()
    {
        gameObject.SetActive(false);
        PanelManager._Instance._mainPanel.bottomBtn.SetActive(true);
        currentItem = null;
//      recordPage.ActiveRecord(false);
    }

    public void OnClickSendBtn()
    {
        //sendCoinPanel.SetETHTokenItem(currentTokenItem);
        sendCoinPanel.Open(eoswalletInfo);
        //NewWalletManager._Intance.DOTweenCome(sendCoinPanel.transform, -1000, 0);
    }

    public void OnClickReceiveBtn()
    {
        receiveCoinPanel.Open(null, true);
        NewWalletManager._Intance.DOTweenCome(receiveCoinPanel.transform, -1000, 0);
    }

    public void OnClickResourcesBtn()
    {
        PanelManager._Instance._eosResourcesPanel.Open(eoswalletInfo);
    }

    public void OnClickDelBtn()
    {
        if (NewWalletManager._Intance.IsNeedColdWallet)
        {
            remindObject.SetActive(true);
        }
        else
        {
            if (SeedKeyManager.Instance.IsBackUp() || !SeedKeyManager.Instance.isFirstSeedBip(currentItem.coinInfo.address))
            {
                remindObject.SetActive(true);
            }
            else
            {
                PopUpBox.Instance.Show(delegate ()
                {
                    PanelManager._Instance._backUpPrivateKeyPanel.OpenMe();
                }, delegate ()
                {
                    OnClickDelWallet();
                }, "马上备份", "不需要", "重要提示", "您尚未备份钱包,如无妥善备份,删除钱包后将无法找回钱包，请慎重处理该操作");
            }
        }
    }

    public void OnClickDelWallet()
    {
        List<KeyValuePair<string, string>> ws = new List<KeyValuePair<string, string>>();
        ws.Add(new KeyValuePair<string, string>("op", "deletAddress"));
        ws.Add(new KeyValuePair<string, string>("userName", NewWalletManager._Intance.userName));
        ws.Add(new KeyValuePair<string, string>("address", currentItem.coinInfo.address));
        ws.Add(new KeyValuePair<string, string>("type", ((int)(currentItem.type)).ToString()));
        StartCoroutine(HttpManager._Intance.SendRequest(ws, CallBack));
    }

    private void CallBack(Hashtable h)
    {
        int re = System.Convert.ToInt32(h["error"]);
        if (re == 0)
        {
            this.gameObject.SetActive(false);
            PanelManager._Instance.currentSubPage = PanelManager.SubPage.Property;
            PanelManager._Instance._mainPanel.bottomBtn.SetActive(true);
            PanelManager._Instance._mainPanel.OnDeletWallet(currentItem);
            EventManager.Instance.SendEvent(EventID.UpdateTotalbalance);
            remindObject.SetActive(false);
        }
        else
        {
            remindObject.SetActive(false);
            PopupLine.Instance.Show("删除失败请重试");
        }
    }

    private void GetFailer(string str)
    {
        Debug.Log(str);
    }
    public void OnClickVoteButton()
    {
        eosVotePanel.ShowVotePage();
    }
   
}


