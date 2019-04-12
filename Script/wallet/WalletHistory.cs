using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nethereum.Util;

[System.Serializable]
public class TransferData
{
    public string from;
    public string to;
    public string quantity;
    public string memo;
}
[System.Serializable]
public class DelegatebwData
{
    public string from;
    public string receiver;
    public string stake_net_quantity;
    public string stake_cpu_quantity;
    public string transfer;
}
[System.Serializable]
public class BuyrambytesData
{
    public string payer;
    public string receiver;
    public string bytes;
}
[System.Serializable]
public class BuyRamData
{
    public string payer;
    public string receiver;
    public string quant;
}
[System.Serializable]
public class UndelegatebwData
{
    public string from;
    public string receiver;
    public string unstake_net_quantity;
    public string unstake_cpu_quantity;
}
[System.Serializable]
public class SellramData
{
    public string account;
    public string bytes;
}

[System.Serializable]
public class EOSHistoryRcord
{
    public string global_action_seq;
    public string actName;
    public enum ActType
    {
        none,
        /// <summary>
        /// 发送EOS
        /// </summary>
        transfer,
        /// <summary>
        /// 抵押
        /// </summary>
        delegatebw,
        /// <summary>
        /// 买内存（直接买多少内存）
        /// </summary>
        buyrambytes,
        /// <summary>
        /// 买内存（用EOS买）
        /// </summary>
        buyram,
        /// <summary>
        /// 取消内存
        /// </summary>
        undelegatebw,
        /// <summary>
        /// 出售内存
        /// </summary>
        sellram,
        /// <summary>
        /// 投票
        /// </summary>
        voteproducer,
        /// <summary>
        /// 
        /// </summary>
        refund,
    }
    public ActType actType = ActType.none;
    public string block_num;
    public string block_time;
    public string trx_id;
    public TransferData transferdata = new TransferData();
    public DelegatebwData delegatebwdata;
    public BuyrambytesData buyrambytesdata;
    public UndelegatebwData undelegatebwdata;
    public SellramData sellramdata;
    public BuyRamData buramData;
}

[System.Serializable]
public class ETHHistoryRcord:ICloneable
{
    public enum RecordType
    {
        None,
        BTC,
        ETH,
        EthToken,
    }
    public RecordType recordType = RecordType.None;

    public int tokendecimals;
    public string blockNumber;
    public string timeStamp;
    public long timeTick;
    public string hash;
    public string from;
    public string to;
    public string value;
    public string gas;
    public string tokenSymbol; //erc20代币名称
	public string fullName;
    public string contractAddress; //erc20智能合约地址
    public string confixmations;
    public bool isPending; // 是否在发送中
    public bool isOverTime;
    public string txReceiptStatus;
    public string input = "";
    /// <summary>
    /// 备注
    /// </summary>
    public string comment;

    public object Clone()
    {
        ETHHistoryRcord result = new ETHHistoryRcord();
        result.recordType = recordType;
        result.tokendecimals = tokendecimals;
        result.timeStamp = timeStamp;
        result.hash = hash;
        result.from = from;
        result.to = to;
        result.value = value;
        result.gas = gas;
        result.tokenSymbol = tokenSymbol;
        result.fullName = fullName;
        result.contractAddress = contractAddress;
        result.confixmations = confixmations;
        result.isPending = isPending;
        result.isOverTime = isOverTime;
        result.txReceiptStatus = txReceiptStatus;
        result.input = input;
        result.comment = comment;
        return result;
    }
}

public class WalletHistory : MonoBehaviour {

    private System.DateTime startTime;

    /// <summary>
    /// 比特币/以太坊历史记录
    /// </summary>
    public List<ETHHistoryRcord> ethlist = new List<ETHHistoryRcord>();

    /// <summary>
    /// 代币历史记录(key 代币名称， 代币记录)
    /// </summary>
    public Dictionary<string, List<ETHHistoryRcord>> etherc20Dic = new Dictionary<string, List<ETHHistoryRcord>>();

    public bool isTest = true;

    // Use this for initialization
    void Start ()
    {
        Debug.Log(Application.temporaryCachePath);
        //startTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        //if (isTest)
        //{
        //    EthHistory.Instance.GetEthHistory("0x8DA3dCdcAc16EAF1abA359e831B0152d49fB1b36", GetEthRecordInfo, GetFailer, 1, 20);
        //    EthHistory.Instance.GetEthErc20History("0x8DA3dCdcAc16EAF1abA359e831B0152d49fB1b36", GetEthErc20Record, GetFailer, 1, 20);
        //}
    
    }



    private void GetEthRecordInfo(string str)
    {
        GetRecord(str, false);
    }

    private void GetEthErc20Record(string str)
    {
        GetRecord(str, true);
    }

    private void GetRecord(string str, bool isErc20)
    {
        Debug.Log(str);
        Hashtable d = Json.jsonDecode(str) as Hashtable;
        ArrayList ar = d["result"] as ArrayList;
        for (int i = 0; i < ar.Count; i++)
        {
            Hashtable h = ar[i] as Hashtable;
            ETHHistoryRcord ethRcord = new ETHHistoryRcord();
            long time = long.Parse(h["timeStamp"].ToString());
            DateTime dt = startTime.AddSeconds(time).ToLocalTime();
            ethRcord.timeStamp = dt.ToString("yyyy-MM-dd HH:mm:ss");
            ethRcord.hash = h["hash"].ToString();
            ethRcord.from = h["from"].ToString();
            ethRcord.to = h["to"].ToString();
            System.Numerics.BigInteger tempValue = System.Numerics.BigInteger.Parse(h["value"].ToString());
            ethRcord.value = UnitConversion.Convert.FromWei(tempValue, 18).ToString();

            System.Numerics.BigInteger gasUsed = System.Numerics.BigInteger.Parse(h["gasUsed"].ToString());
            System.Numerics.BigInteger gasPrice = System.Numerics.BigInteger.Parse(h["gasPrice"].ToString());
            ethRcord.gas = UnitConversion.Convert.FromWei(gasUsed * gasPrice, 18).ToString();

            ethRcord.confixmations = h["confirmations"].ToString();
            if (isErc20)
            {
                ethRcord.tokenSymbol = h["tokenSymbol"].ToString();
                if (etherc20Dic.ContainsKey(ethRcord.tokenSymbol))
                {
                    etherc20Dic[ethRcord.tokenSymbol].Add(ethRcord);
                }
                else
                {
                    List<ETHHistoryRcord> tempList = new List<ETHHistoryRcord>();
                    tempList.Add(ethRcord);

                    etherc20Dic.Add(ethRcord.tokenSymbol, tempList);
                }
            }
            else
            {
                if (h["input"].ToString().Equals("0x"))
                    ethlist.Add(ethRcord);
            }
        }
        foreach (var item in etherc20Dic)
        {
            Debug.Log(item.Key);
        }
    }

    private void GetFailer(string str)
    {
        Debug.Log(str);
    }
}
