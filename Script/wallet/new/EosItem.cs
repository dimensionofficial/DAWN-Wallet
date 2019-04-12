using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EosItem : CoinInfoItemBase
{
    public EOSWalletInfo eosWalletInfo;

    public Text noteInfo;
    public GameObject registerObjct;
    public GameObject noRegisterObject;

    public List<EOSHistoryRcord> m_eosHistoryRcordlist = new List<EOSHistoryRcord>();
    public List<EOSHistoryRcord> eosHistoryRcordlist = new List<EOSHistoryRcord>();

    public Dictionary<string, EOSHistoryRcord> tempRecordDic = new Dictionary<string, EOSHistoryRcord>();

    private string hash;
    private string goodsid;
    /// <summary>
    /// 
    /// </summary>
    public bool havaEosInfo = false;

    public void InitEOS(EOSWalletInfo _eosWalletInfo)
    {
        hash = "";
        goodsid = "";
        eosWalletInfo = _eosWalletInfo;
        string[] v = new string[2] { _eosWalletInfo.adminAddress, _eosWalletInfo.walletName };
        Init(v);

        RequestEosAccount();
    }

    private void RequestEosAccount()
    {
        if (eosWalletInfo == null || string.IsNullOrEmpty(eosWalletInfo.adminAddress))
        {
            return;
        }

        StartCoroutine(GetEosAccount());

//      InitEosLater();
    }

    private IEnumerator GetEosAccount()
    {
        if (string.IsNullOrEmpty(eosWalletInfo.account))
        {
            EosGetSingInfoPanel.Instance.GetKeyAccounts(eosWalletInfo.adminAddress, delegate (Hashtable t)
            {

                ArrayList arry = t["account_names"] as ArrayList;

                if (arry != null && arry.Count > 0)
                {
                    eosWalletInfo.accountState = EOSWalletInfo.EOSAccountState.Activate;
                    eosWalletInfo.account = arry[0].ToString();
                    for (int i = 0; i < arry.Count; i++)
                    {
                        eosWalletInfo.accountList.Add(arry[i].ToString());
                    }
                }

                if (!string.IsNullOrEmpty(eosWalletInfo.account))
                {
                    List<KeyValuePair<string, string>> ws = new List<KeyValuePair<string, string>>();
                    ws.Add(new KeyValuePair<string, string>("op", "setEosuserName"));
                    ws.Add(new KeyValuePair<string, string>("eosAddress", eosWalletInfo.adminAddress));
                    ws.Add(new KeyValuePair<string, string>("eosuserName", eosWalletInfo.account));
                    StartCoroutine(HttpManager._Intance.SendRequest(ws, delegate(Hashtable h) 
                    {
                        Debug.Log("setEosuserName 返回值：" +  Json.jsonEncode(h));
                    }));
                }

                InitEosLater();
            }, null, false);

        }
        else
        {
            eosWalletInfo.accountState = EOSWalletInfo.EOSAccountState.Activate;
            InitEosLater();
        }

       
        yield return null;
    }

    public void OnCreatAccountLater()
    {
        noteInfo.text = "正在获取账户信息....";
        eosWalletInfo.accountState = EOSWalletInfo.EOSAccountState.WaiterFor;
    }

    public void OnPayMentLater()
    {
        noteInfo.text = "支付进行中";
        eosWalletInfo.accountState = EOSWalletInfo.EOSAccountState.WaiterFor;
    }

    public override void Init(string[] v)
    {
        coinname = v[1];
        nameText.text = coinname;

        iconName.text = coinname.Substring(0, 1);
        coinInfo = new CoinInfo();
        coinInfo.walletname = coinname;
        coinInfo.address = v[0];
    }

    public IEnumerator GetEosAccountInfo()
    {
        if (havaEosInfo == false)
        {
            countText.text = "正在获取账户信息....";
            havaEosInfo = true;
        }

        EosGetSingInfoPanel.Instance.GetAccount(eosWalletInfo.account, delegate (Hashtable t)
        {
            if (t != null && t.ContainsKey("error"))
            {
                return;
            }

            if (t.ContainsKey("core_liquid_balance"))
            {
                string ba = t["core_liquid_balance"].ToString();
                ba = ba.Substring(0, ba.Length - 4);
                eosWalletInfo.balance = decimal.Parse(ba);
            }
            else
            {
                eosWalletInfo.balance = 0;
            }

            eosWalletInfo.eosRAM.max = long.Parse(t["ram_quota"].ToString());
            eosWalletInfo.eosRAM.used = long.Parse(t["ram_usage"].ToString());

            Hashtable net = t["net_limit"] as Hashtable;
            eosWalletInfo.eosNetwork.max = long.Parse(net["max"].ToString());
            eosWalletInfo.eosNetwork.used = long.Parse(net["used"].ToString());
            eosWalletInfo.eosNetwork.available = long.Parse(net["available"].ToString());

            Hashtable cpu = t["cpu_limit"] as Hashtable;
            eosWalletInfo.eosCPU.used = long.Parse(cpu["used"].ToString());
            eosWalletInfo.eosCPU.max = long.Parse(cpu["max"].ToString());
            eosWalletInfo.eosCPU.available = long.Parse(cpu["available"].ToString());

            countText.text = eosWalletInfo.balance + " EOS";
            if (PanelManager._Instance._mainPanel.IsHideAsset())
            {
                countText.text = "*****";
            }

            Hashtable toResources = t["total_resources"] as Hashtable;
            if (toResources != null)
            {
                eosWalletInfo.total_resources.owner = toResources["owner"].ToString();
                eosWalletInfo.total_resources.net_weigth = toResources["net_weight"].ToString();
                eosWalletInfo.total_resources.cpu_weight = toResources["net_weight"].ToString();
                eosWalletInfo.total_resources.ram_bytes = toResources["ram_bytes"].ToString();
            }

            Hashtable self_delegated = t["self_delegated_bandwidth"] as Hashtable;
            if (self_delegated != null)
            {
                eosWalletInfo.self_delegated_bandwidth.from = self_delegated["from"].ToString();
                eosWalletInfo.self_delegated_bandwidth.to = self_delegated["to"].ToString();
                eosWalletInfo.self_delegated_bandwidth.net_weight = self_delegated["net_weight"].ToString();
                eosWalletInfo.self_delegated_bandwidth.cpu_weight = self_delegated["cpu_weight"].ToString();
            }

            Hashtable refund_requestHash = t["refund_request"] as Hashtable;
            if (refund_requestHash != null)
            {
                eosWalletInfo.refund_request.owner = refund_requestHash["owner"].ToString();
                DateTime tt = Convert.ToDateTime(refund_requestHash["request_time"].ToString());

                long overTime = tt.Ticks + (3 * 24 * 60 * 60 * 10000000L) + (8 * 60 * 60 * 10000000L);
                eosWalletInfo.refund_request.request_time = overTime;
                string netStr = refund_requestHash["net_amount"].ToString();
                eosWalletInfo.refund_request.net_amount = decimal.Parse(netStr.Substring(0,netStr.Length - 4));
                string cpuStr = refund_requestHash["cpu_amount"].ToString();
                eosWalletInfo.refund_request.cpu_amount = decimal.Parse(cpuStr.Substring(0,cpuStr.Length - 4));
            }

            Hashtable tempInfo = t["voter_info"] as Hashtable;
            if (tempInfo != null)
            {
                ArrayList tempArr = tempInfo["producers"] as ArrayList;
                if (tempArr.Count > 0)
                {
                    for (int i = 0; i < tempArr.Count; i++)
                    {
                        string temoStr = tempArr[i].ToString();
                        if (!eosWalletInfo.votedList.Contains(temoStr))
                            eosWalletInfo.votedList.Add(temoStr);
                    }
                }
            }

            EventManager.Instance.SendEvent(EventID.UpdateEOSBalance);


        }, null, false);

        yield return null;
    }

    private void InitEosLater()
    {
        if (eosWalletInfo.accountState == EOSWalletInfo.EOSAccountState.Activate)
        {
            registerObjct.SetActive(true);
            noRegisterObject.SetActive(false);
            StartCoroutine(GetEosAccountInfo());
        }
        else
        {

            registerObjct.SetActive(false);
            noRegisterObject.SetActive(true);

            StartCoroutine(CheckState());
        }
    }


    private IEnumerator CheckState()
    {
        //eosWalletInfo.accountState = EOSWalletInfo.EOSAccountState.PaySuccess;
        //noteInfo.text = "待创建EOS钱包";
        //return;

        List<KeyValuePair<string, string>> ws = new List<KeyValuePair<string, string>>();
        ws.Add(new KeyValuePair<string, string>("userid", NewWalletManager._Intance.userId));
        ws.Add(new KeyValuePair<string, string>("address", eosWalletInfo.adminAddress));
        StartCoroutine(HttpManager._Intance.GetNodeJsRequest("getuseableorder", ws, (Hashtable data) =>
        {
            if (data != null)
            {
 //               Debug.Log(eosWalletInfo.ownerAddress + " | getuseableorder" + " : " + data["result"] + " : " + Json.jsonEncode(data));

                string result = data["result"].ToString();
               
                if (result.StartsWith("Su"))
                {
                    ArrayList arrList = data["data"] as ArrayList;
                    for (int j = 0; j < arrList.Count; j++)
                    {
                        Hashtable hs = arrList[j] as Hashtable;
                        string tempHash = hs["hash"].ToString();
                        string tempGoodsid = hs["goodsid"].ToString();

                        for (int i = 0; i < NewWalletManager._Intance.goodsInfoList.Count; i++)
                        {
                            GoodsInfo goodsInfo = NewWalletManager._Intance.goodsInfoList[i];
                            if (tempGoodsid.Equals(goodsInfo.id.ToString()))
                            {
                                if (goodsInfo.goodsType == GoodsInfo.GoodsType.EOS_CVT)
                                {
                                    hash = tempHash;
                                    goodsid = tempGoodsid;
                                    eosWalletInfo.accountState = EOSWalletInfo.EOSAccountState.PaySuccess;
                                    noteInfo.text = "支付成功,请输入账户名称完成注册";
                                    return;
                                }
                            }
                        }
                    }
                }
            }

            StartCoroutine(CheckPaying());
        }));

        yield return null;
    }

    private IEnumerator CheckPaying()
    {
        List<KeyValuePair<string, string>> ws = new List<KeyValuePair<string, string>>();
        ws.Add(new KeyValuePair<string, string>("userid", NewWalletManager._Intance.userId));
        ws.Add(new KeyValuePair<string, string>("address", eosWalletInfo.adminAddress));
        StartCoroutine(HttpManager._Intance.GetNodeJsRequest("getorder", ws, (Hashtable data) =>
        {
            if (data != null)
            {
 //               Debug.Log("getorder" + " : " + data["result"] + " : " + Json.jsonEncode(data));
                string result = data["result"].ToString();

                if (result.StartsWith("Su"))
                {
                    ArrayList arrList = data["data"] as ArrayList;
                    for (int j = 0; j < arrList.Count; j++)
                    {
                        Hashtable hs = arrList[j] as Hashtable;
                        int goodsid = int.Parse(hs["goodsid"].ToString());

                        for (int i = 0; i < NewWalletManager._Intance.goodsInfoList.Count; i++)
                        {
                            GoodsInfo goodsInfo = NewWalletManager._Intance.goodsInfoList[i];
                            if (goodsInfo.id == goodsid)
                            {
                                if (goodsInfo.goodsType == GoodsInfo.GoodsType.EOS_CVT)
                                {
                                    eosWalletInfo.accountState = EOSWalletInfo.EOSAccountState.Payment;
                                    noteInfo.text = "支付进行中";
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            eosWalletInfo.accountState = EOSWalletInfo.EOSAccountState.Unpaid;
            noteInfo.text = "待创建EOS钱包";
        }));

        yield return null;
    }


    public override void RefreshBalance()
    {
        if (havaEosInfo)
        {
            StartCoroutine(GetEosAccountInfo());
        }
        else
        {
            RequestEosAccount();
        }

    }

    public override void GetHistory()
    {
        //if (string.IsNullOrEmpty(eosWalletInfo.account))
        //    return;


        //EosGetSingInfoPanel.Instance.GetHistory(eosWalletInfo, delegate (Hashtable t)
        //{
        //    if (t != null && t.ContainsKey("actions"))
        //    {
        //        eosHistoryRcordlist = new List<EOSHistoryRcord>();
        //        ArrayList list = t["actions"] as ArrayList;
        //        for (int i = 0; i < list.Count; i++)
        //        {
        //            AddEosHistroy(list[i] as Hashtable);
        //        }
        //    }
           
        //}, null, false);
    }

    public string GetDateTime(string dtime)
    {
        DateTime tt = Convert.ToDateTime(dtime);
        long tts = tt.Ticks;
        tts = tts + (8 * 60 * 60 * 10000000L);
        DateTime dttd = new DateTime(tts);
        return dttd.ToString("yyyy-MM-dd HH:mm:ss");
    }

    public void AddEosHistroy(Hashtable t)
    {
        string eosBlockNumber = t["block_num"].ToString();
        if (tempRecordDic.ContainsKey(eosBlockNumber))
            return;

        Hashtable action = t["action_trace"] as Hashtable;
        Hashtable act = action["act"] as Hashtable;
        string name = act["name"].ToString();
        if (name.Equals("voteproducer"))
        {
            return;
        }

        string global_action_seq = t["global_action_seq"].ToString();
        string trxId = action["trx_id"].ToString();

        EOSHistoryRcord eosHistory = new EOSHistoryRcord();

        eosHistory.actName = name;
        eosHistory.block_num = eosBlockNumber;

        

        eosHistory.block_time = GetDateTime(t["block_time"].ToString());
        eosHistory.global_action_seq = global_action_seq;
        eosHistory.trx_id = trxId;

        Hashtable data = act["data"] as Hashtable;
        ArrayList inline_traces = action["inline_traces"] as ArrayList;
        Hashtable concreteness = null;
        if (inline_traces.Count > 0)
        {
            Hashtable ct = inline_traces[0] as Hashtable;
            Hashtable at = ct["act"] as Hashtable;
            string na = at["name"].ToString();
            if (na.Equals("transfer"))
            {
                concreteness = at["data"] as Hashtable;
            }
        }


        if (name.Equals("refund"))
        {
            eosHistory.actType = EOSHistoryRcord.ActType.refund;
            SetTransfer(eosHistory, concreteness);
        }
        else if (name.Equals("transfer"))
        {
            eosHistory.actType = EOSHistoryRcord.ActType.transfer;
            SetTransfer(eosHistory, data);
        }
        else if (name.Equals("delegatebw"))
        {
            eosHistory.actType = EOSHistoryRcord.ActType.delegatebw;
            eosHistory.delegatebwdata = new DelegatebwData();
            eosHistory.delegatebwdata.from = data["from"].ToString();
            eosHistory.delegatebwdata.receiver = data["receiver"].ToString();
            eosHistory.delegatebwdata.stake_net_quantity = data["stake_net_quantity"].ToString();
            eosHistory.delegatebwdata.stake_cpu_quantity = data["stake_cpu_quantity"].ToString();
            eosHistory.delegatebwdata.transfer = data["transfer"].ToString();

            SetTransfer(eosHistory, concreteness);

        }
        else if (name.Equals("buyrambytes"))
        {
            eosHistory.actType = EOSHistoryRcord.ActType.buyrambytes;
            eosHistory.buyrambytesdata = new BuyrambytesData();
            eosHistory.buyrambytesdata.payer = data["payer"].ToString();
            eosHistory.buyrambytesdata.receiver = data["receiver"].ToString();
            eosHistory.buyrambytesdata.bytes = data["bytes"].ToString();

            SetTransfer(eosHistory, concreteness);
        }
        else if (name.Equals("undelegatebw"))
        {
            eosHistory.actType = EOSHistoryRcord.ActType.undelegatebw;
            eosHistory.undelegatebwdata = new UndelegatebwData();
            eosHistory.undelegatebwdata.from = data["from"].ToString();
            eosHistory.undelegatebwdata.receiver = data["receiver"].ToString();
            eosHistory.undelegatebwdata.unstake_cpu_quantity = data["unstake_cpu_quantity"].ToString();
            eosHistory.undelegatebwdata.unstake_net_quantity = data["unstake_net_quantity"].ToString();
        }
        else if (name.Equals("sellram"))
        {
            eosHistory.actType = EOSHistoryRcord.ActType.sellram;
            eosHistory.sellramdata = new SellramData();
            eosHistory.sellramdata.account = data["account"].ToString();
            eosHistory.sellramdata.bytes = data["bytes"].ToString();

            SetTransfer(eosHistory, concreteness);
        }
        else if (name.Equals("buyram"))
        {
            eosHistory.actType = EOSHistoryRcord.ActType.buyram;
            eosHistory.buramData = new BuyRamData();

            eosHistory.buramData.payer = data["payer"].ToString();
            eosHistory.buramData.receiver = data["receiver"].ToString();
            eosHistory.buramData.quant = data["quant"].ToString();

            SetTransfer(eosHistory, concreteness);
        }
        else
        {
            return;
        }
        tempRecordDic.Add(eosBlockNumber, eosHistory);
       // m_eosHistoryRcordlist.Add(eosHistory);
        eosHistoryRcordlist.Add(eosHistory);
    }

    private void SetTransfer(EOSHistoryRcord eosHistory, Hashtable data)
    {
        if (data == null)
            return;

        eosHistory.transferdata = new TransferData();
        eosHistory.transferdata.from = data["from"].ToString();
        eosHistory.transferdata.quantity = data["quantity"].ToString();
        eosHistory.transferdata.to = data["to"].ToString();
        eosHistory.transferdata.memo = data["memo"].ToString();
    }


    private bool ContainHistory(string trxId)
    {
        for (int i = 0; i < eosHistoryRcordlist.Count; i++)
        {
            if (eosHistoryRcordlist[i].trx_id.Equals(trxId))
                return true;
        }
        return false;
    }

    public void OnClickMe()
    {
        switch (eosWalletInfo.accountState)
        {
            case EOSWalletInfo.EOSAccountState.Activate:
                PanelManager._Instance._eosWalletInfoPanel.ShowMe(this);
                break;
            case EOSWalletInfo.EOSAccountState.Unpaid:
                PanelManager._Instance._eosRegisterPanel.Show(this);
                break;
            case EOSWalletInfo.EOSAccountState.Payment:
                PanelManager._Instance._eosRegisteringPanel.Show(eosWalletInfo);
                break;
            case EOSWalletInfo.EOSAccountState.PaySuccess:
                PanelManager._Instance._eosCreatAccountPanel.Show(this, hash, goodsid);
                break;
            default:
                
                break;
        }

        PanelManager._Instance._eosResourcesPanel.GetEOSGlobal();
    }

}
