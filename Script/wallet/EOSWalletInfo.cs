using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EosResource
{
    private decimal m_used;
    /// <summary>
    /// 用的
    /// </summary>
    public decimal used
    {
        get
        {
            return m_used / 1024;
        }
        set
        {
            m_used = value;
        }
    }

    public decimal m_available;
    /// <summary>
    /// 可用的
    /// </summary>
    public decimal available
    {
        get
        {
            return m_available / 1024;
        }
        set
        {
            m_available = value;
        }
    }

    public decimal m_max;
    /// <summary>
    /// 最大
    /// </summary>
    public decimal max
    {
        get
        {
            return m_max / 1024;
        }
        set
        {
            m_max = value;
        }
    }
}

[System.Serializable]
public class Total_resources
{
    public string owner;
    public string net_weigth;
    public string cpu_weight;
    public string ram_bytes;
}
[System.Serializable]
public class Self_delegated_bandwidth
{
    public string from;
    public string to;
    public string net_weight = "0 EOS";
    public string cpu_weight = "0 EOS";
}

[System.Serializable]
public class Refund_request
{
    public string owner;
    public long request_time;
    public decimal net_amount;
    public decimal cpu_amount;
}

[System.Serializable]
public class EOSWalletInfo
{
    public enum EOSAccountState
    {
        None,
        /// <summary>
        /// 未支付
        /// </summary>
        Unpaid,
        /// <summary>
        /// 支付中
        /// </summary>
        Payment,
        /// <summary>
        /// 支付成功
        /// </summary>
        PaySuccess,
        /// <summary>
        /// 有账户
        /// </summary>
        Activate,
        /// <summary>
        /// 
        /// </summary>
        WaiterFor,
    }
    public EOSAccountState accountState = EOSAccountState.None;

    public List<string> accountList = new List<string>();

    public string account;
    /// <summary>
    /// EOS拥有地址
    /// </summary>
    public string ownerAddress;
    /// <summary>
    /// EOS管理地址
    /// </summary>
    public string adminAddress;
    public EosResource eosRAM = new EosResource();
    public EosResource eosCPU = new EosResource();
    public EosResource eosNetwork = new EosResource();
    public Total_resources total_resources = new Total_resources();
    public Self_delegated_bandwidth self_delegated_bandwidth = new Self_delegated_bandwidth();
    public Refund_request refund_request = new Refund_request();

    /// <summary>
    /// 投票节点名称
    /// </summary>
    public List<string> votedList = new List<string>();
    /// <summary>
    /// 余额
    /// </summary>
    public decimal balance;
    /// <summary>
    /// 抵押的EOS数量
    /// </summary>
    public decimal mortgageBalance;
    /// <summary>
    /// 钱包名称
    /// </summary>
    public string walletName;
}
