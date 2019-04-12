using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NBitcoin;

public class Bitcoin
{

    private string m_privateKey;
    private string m_pubKeyAddress;
    private string m_bitcoinAddress;
    /// <summary>
    /// 私有密匙
    /// </summary>
    public string privateKey
    {
        get
        {
            return m_privateKey;
        }
    }
    /// <summary>
    /// 公钥地址
    /// </summary>
    public string pubKeyAddress
    {
        get
        {
            return m_pubKeyAddress;
        }
    }
    /// <summary>
    /// 比特币地址
    /// </summary>
    public string bitcoinAddress
    {
        get
        {
            return m_bitcoinAddress;
        }
    }
    ///// <summary>
    ///// 私有密匙
    ///// </summary>
    //private BitcoinSecret m_mainNetPrivateKey;
    ///// <summary>
    ///// 获取私有密匙 get our private key for the mainnet
    ///// </summary>
    //public BitcoinSecret GetMainNetPrivateKey
    //{
    //    get
    //    {
    //        return m_mainNetPrivateKey;
    //    }
    //}
    ///// <summary>
    ///// 公钥地址
    ///// </summary>
    //private BitcoinPubKeyAddress m_bitcoinPubKeyAddress;
    ///// <summary>
    ///// 获取公钥地址
    ///// </summary>
    //public BitcoinPubKeyAddress GetBitcoinPubKeyAddress
    //{
    //    get { return m_bitcoinPubKeyAddress; }
    //}
    ///// <summary>
    ///// 比特币地址
    ///// </summary>
    //private BitcoinAddress m_mainNetAddress;
    ///// <summary>
    ///// 获取比特币地址
    ///// </summary>
    //public BitcoinAddress GetMainNetAddress
    //{
    //    get { return m_mainNetAddress; }
    //}

    

    public Bitcoin()
    {
        Key privateKey = new Key();
        m_privateKey = privateKey.GetBitcoinSecret(NBitcoin.Network.Main).ToString();

        PubKey publicKey = privateKey.PubKey;
        m_pubKeyAddress = publicKey.GetAddress(NBitcoin.Network.Main).ToString();

        KeyId publicKeyHash = publicKey.Hash;
        m_bitcoinAddress = publicKeyHash.GetAddress(NBitcoin.Network.Main).ToString();
    }



    public Bitcoin(string[] strs)
    {
        m_privateKey = strs[0];
        m_pubKeyAddress = strs[1];
        m_bitcoinAddress = strs[2];
        Debug.Log("BTC 地址：" + m_bitcoinAddress);
    }

}
