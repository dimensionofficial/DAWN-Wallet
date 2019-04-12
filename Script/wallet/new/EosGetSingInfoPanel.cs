using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using HardwareWallet;

public class EosGetSingInfoPanel : MonoBehaviour
{
    public static EosGetSingInfoPanel Instance;
    public float eos_register_ram;
    public float eos_register_cpu;
    public float eos_register_network;

    public enum EosSingType
    {
        None,
        /// <summary>
        /// 发送eos
        /// </summary>
        Transfer,
        /// <summary>
        /// 投票
        /// </summary>
        Voteproducer,
        /// <summary>
        /// 资源操作
        /// </summary>
        OperantResources
    }
    private EosSingType curSingType = EosSingType.None;

    void Awake()
    {
        Instance = this;
    }

    #region

    #endregion


    /// 发送EOS
    public void Gettransfer(string eosOwerAddress, EOSSendScanSing.LastPanel currenPanl, EOSWalletInfo eosWalletInfo, string currentToAddress, string quantity, string meno)
    {
        WWWForm ws = new WWWForm();
        ws.AddField("sender", eosWalletInfo.account);
        ws.AddField("receiver", currentToAddress);
        ws.AddField("quantity", quantity);
        ws.AddField("memo", meno);
        StartCoroutine(HttpManager._Intance.EOSSendRequest("transfer", ws, true, delegate(Hashtable h)
        {
            if (h != null && h.ContainsKey("error"))
            {
                PopUpBox.Instance.Show(null, null, "确定", "", "错误提示", h["error"].ToString());
                return;
            }

            Hashtable tb = h["transaction"] as Hashtable;
            PanelManager._Instance._eosScanSing.SetSendInfo(currenPanl, tb, quantity, currentToAddress, eosWalletInfo.account, meno, "EOS");
            GetsignHex(h, EosSingType.Transfer, eosOwerAddress);

        }, null, true));
    }



    //获取账户信息
    public void GetAccount(string account, Action<Hashtable> successCallback, Action failureCallback, bool isShowLoadingPanel)
    {
        WWWForm ws = new WWWForm();
        //ws.AddField("name", "eosuser1tg42");
        ws.AddField("name", account);
        StartCoroutine(HttpManager._Intance.EOSSendRequest("getaccount", ws, true, successCallback, failureCallback, isShowLoadingPanel));
    }

    ///该eos地址是否注册过账户
    public void GetKeyAccounts(string ownerAddress, Action<Hashtable> successCallback, Action failureCallback, bool isShowLoadingPanel)
    {
        WWWForm ws = new WWWForm();
        ws.AddField("pubKey", ownerAddress);
        StartCoroutine(HttpManager._Intance.EOSSendRequest("getKeyAccounts", ws, true, successCallback, failureCallback, isShowLoadingPanel));
    }

    //历史记录
    public void GetHistory(EOSWalletInfo eosWalletInfo, Action<Hashtable> successCallback, Action failureCallback, bool isShowLoadingPanel)
    {
        WWWForm ws = new WWWForm();
        ws.AddField("name", eosWalletInfo.account); //iloveandylau
        //ws.AddField("name", "iloveandylau");
        StartCoroutine(HttpManager._Intance.EOSSendRequest("history", ws, true, successCallback, failureCallback, isShowLoadingPanel));
    }

    //发送到区块上
    public void Pushtransaction(Hashtable htable, string singInfo, Action<Hashtable> successCallback, Action failureCallback, bool isShowLoadingPanel)
    {

        ArrayList tp = new ArrayList();
        tp.Add(singInfo);

        Hashtable target = new Hashtable();
        target["compression"] = htable["compression"];
        target["transaction"] = htable["transaction"];
        target["signatures"] = tp;

        string transactionStr = Json.jsonEncode(target);
  //      Debug.Log("transaction:" + transactionStr);
        ////http://192.168.50.136:3000/pushtransaction?transaction=发送到区块连上
        WWWForm ws = new WWWForm();
        ws.AddField("transaction", transactionStr);
        StartCoroutine(HttpManager._Intance.EOSSendRequest("pushtransaction", ws, true, successCallback, failureCallback, isShowLoadingPanel));
    }

    //http://192.168.50.136:3000/voteproducer?name=iloveandylau&voteList={"voteList":["eos42freedom","eosfishrocks"]}
    public void Voteproducer(string eosAdminAddress, EOSSendScanSing.LastPanel currenPanl, string account, List<string> list, Action endCallBack = null)
    {
        Hashtable voteList = new Hashtable();
        voteList["voteList"] = list.ToArray();
        string josnStr = Json.jsonEncode(voteList);
 //       Debug.Log(josnStr);
        WWWForm ws = new WWWForm();
        ws.AddField("name", account);
        ws.AddField("voteList", josnStr);
        PanelManager._Instance.loadingPanel.SetActive(true);
        StartCoroutine(HttpManager._Intance.EOSSendRequest("voteproducer", ws, true, delegate (Hashtable t)
        {
            PanelManager._Instance.loadingPanel.SetActive(false);
            if (t != null && t.ContainsKey("error"))
            {
                PopUpBox.Instance.Show(null, null, "确定", "", "错误提示", t["error"].ToString());
                return;
            }

            Hashtable tb = t["transaction"] as Hashtable;
            PanelManager._Instance._eosScanSing.SetSendInfo(currenPanl, tb, "", "", "", "", "", endCallBack);
            GetsignHex(t, EosSingType.Voteproducer, eosAdminAddress);
        }, delegate() 
        {
            PanelManager._Instance.loadingPanel.SetActive(false);
        }, true));
    }
    //http://192.168.50.136:3000/sellram?seller=iloveandylau&bytes=128.9  
    public void Sellram(string eosAdminAddress,EOSSendScanSing.LastPanel currenPanl, string account,  string cbyte)
    {
        PanelManager._Instance._eosResourcesPanel.loadingPanel.SetActive(true);
        WWWForm ws = new WWWForm();
        ws.AddField("seller", account);
        ws.AddField("bytes", cbyte);
        StartCoroutine(HttpManager._Intance.EOSSendRequest("sellram", ws, true, delegate (Hashtable t)
        {
           
            PanelManager._Instance._eosResourcesPanel.loadingPanel.SetActive(false);
            if (t != null && t.ContainsKey("error"))
            {
                PopUpBox.Instance.Show(null, null, "确定", "", "错误提示", t["error"].ToString());
                return;
            }


            Hashtable tb = t["transaction"] as Hashtable;
            PanelManager._Instance._eosScanSing.SetSendInfo(currenPanl, tb, cbyte, account, "", "出售内存", "bytes");
            GetsignHex(t, EosSingType.OperantResources, eosAdminAddress);
        }, delegate()
        {
            PanelManager._Instance._eosResourcesPanel.loadingPanel.SetActive(false);
        }, false));
    }
    //http://192.168.50.136:3000/buyrameos?payer=iloveandylau&receiver=iloveandylau&quant=1
    public void BuyramEos(string eosAdminAddress, EOSSendScanSing.LastPanel currenPanl, string payer, string reciver, string quant)
    {
        PanelManager._Instance._eosResourcesPanel.loadingPanel.SetActive(true);
        WWWForm ws = new WWWForm();
        ws.AddField("sender", payer);
        ws.AddField("receiver", reciver);
        ws.AddField("amt", quant);
        StartCoroutine(HttpManager._Intance.EOSSendRequest("buyrameos", ws, true, delegate (Hashtable t)
        {
            PanelManager._Instance._eosResourcesPanel.loadingPanel.SetActive(false);
            if (t != null && t.ContainsKey("error"))
            {
                PopUpBox.Instance.Show(null, null, "确定", "", "错误提示", t["error"].ToString());
                return;
            }

            
            Hashtable tb = t["transaction"] as Hashtable;
            PanelManager._Instance._eosScanSing.SetSendInfo(currenPanl, tb, quant, payer, reciver, "购买内存", "EOS");
            GetsignHex(t, EosSingType.OperantResources, eosAdminAddress);
        },
        delegate()
        {
            PanelManager._Instance._eosResourcesPanel.loadingPanel.SetActive(false);
        }, false));
    }

    ////http://192.168.50.136:3000/undelegatebw?name=iloveandylau&net=0.01&cpu=0.01
    public void Undelegatebw(string eosAdminAddress,EOSSendScanSing.LastPanel currenPanl, string sender, string receiver, string net, string cpu)
    {
        PanelManager._Instance._eosResourcesPanel.loadingPanel.SetActive(true);

        float netC = float.Parse(net);
        string mean = "";
        string toCount;
        if (netC > 0)
        {
            toCount = net;
            mean = "赎回网络抵押";
        }
        else
        {
            toCount = cpu;
            mean = "赎回CPU抵押";
        }

        WWWForm ws = new WWWForm();
        ws.AddField("sender", sender);
        ws.AddField("receiver", receiver);
        ws.AddField("net", net);
        ws.AddField("cpu", cpu);
        
        StartCoroutine(HttpManager._Intance.EOSSendRequest("undelegatebw", ws, true, delegate (Hashtable t)
        {
            
            PanelManager._Instance._eosResourcesPanel.loadingPanel.SetActive(false);
            if (t != null && t.ContainsKey("error"))
            {
                PopUpBox.Instance.Show(null, null, "确定", "", "错误提示", t["error"].ToString());
                return;
            }


            Hashtable tb = t["transaction"] as Hashtable;
            PanelManager._Instance._eosScanSing.SetSendInfo(currenPanl, tb, toCount, sender, receiver, "赎回抵押", "EOS");
            GetsignHex(t, EosSingType.OperantResources, eosAdminAddress);
        }, delegate()
        {
            PanelManager._Instance._eosResourcesPanel.loadingPanel.SetActive(false);
        }, false));
    }

    ////http://192.168.50.136:3000/delegatebw?name=iloveandylau&net=0.01&cpu=0.01
    public void Delegatebw(string eosAdminAddress,EOSSendScanSing.LastPanel currenPanl, string sender, string receiver, string net, string cpu)
    {
        PanelManager._Instance._eosResourcesPanel.loadingPanel.SetActive(true);

        float netC = float.Parse(net);
        string mena = "";

        string toCount;
        if (netC > 0)
        {
            toCount = net;
            mena = "抵押网络";
        }
        else
        {
            toCount = cpu;
            mena = "抵押CPU";
        }

        WWWForm ws = new WWWForm();
        ws.AddField("sender", sender);
        ws.AddField("receiver", receiver);
        ws.AddField("net", net);
        ws.AddField("cpu", cpu);

        StartCoroutine(HttpManager._Intance.EOSSendRequest("delegatebw", ws, true, delegate (Hashtable t)
        {
            PanelManager._Instance._eosResourcesPanel.loadingPanel.SetActive(false);
            if (t != null && t.ContainsKey("error"))
            {
                PopUpBox.Instance.Show(null, null, "确定", "", "错误提示", t["error"].ToString());
                return;
            }

            Hashtable tb = t["transaction"] as Hashtable;
            PanelManager._Instance._eosScanSing.SetSendInfo(currenPanl, tb, toCount, sender, receiver, mena, "EOS");
            GetsignHex(t, EosSingType.OperantResources, eosAdminAddress);
        }, delegate()
        {
            PanelManager._Instance._eosResourcesPanel.loadingPanel.SetActive(false);
        }, false));
    }

    private void GetsignHex(Hashtable t, EosSingType singType, string eosAdminAddress)
    {
//        Debug.Log("EOS未签名数据 ： " + Json.jsonEncode(t));

        string strHex = t["signHex"].ToString();

        Hashtable ht = new Hashtable();
        ht["path"] = WalletTools.eospath_admin;
        ht["sign"] = "EOS";
        strHex = Encry.ToBase64String(strHex);
        ht["data"] = strHex;

        string signStr = Json.jsonEncode(ht);

        if (NewWalletManager._Intance.IsNeedColdWallet)
        {
            PanelManager._Instance._eosScanSing.ShowQR(signStr);
        }
        else
        {
            if (singType == EosSingType.Transfer)
            {
                PanelManager._Instance._eosSendCoinPanel.ActiveNativeEditBox(false);
                PanelManager._Instance._pingCodeInputBox.OpenMe(signStr, PingCodeInputBox.SingType.EOS, eosAdminAddress, delegate () 
                {
                    PanelManager._Instance._eosSendCoinPanel.ActiveNativeEditBox(true);
                });
            }
            else
            {
                PanelManager._Instance._pingCodeInputBox.OpenMe(signStr, PingCodeInputBox.SingType.EOS, eosAdminAddress, null);
            }
            
            PanelManager._Instance._pingCodeInputBox.SetEosSingInfo(WalletTools.eospath_admin, singType);
        }
       
    }
}
