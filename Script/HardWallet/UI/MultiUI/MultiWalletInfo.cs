using NBitcoin;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MultiWalletInfo  {
    public List<string> pubstr;
    public List<string> btcAddress; //地址
    public List<string> walletName; //钱包名称
    public int MultiSig_M;
    public int MultiSig_N;
    public string Multi_walletName;
    public string Multi_btcAddress;
    // Use this for initialization
    void Start () {
        pubstr = new List<string>();
        btcAddress = new List<string>();
        walletName = new List<string>();
    }
    public List<PubKey> GetPukList()
    {
        List<PubKey> pubList = new List<PubKey>();
        for (int i = 0; i < pubstr.Count; i++)
        {
            PubKey pubKey = new PubKey(pubstr[i]);
            pubList.Add(pubKey);
        }
        return pubList;
    }
    public PubKey GetPuk(string addr)
    {
        for (int i = 0; i < btcAddress.Count; i++)
        {
            if (addr== btcAddress[i])
            {
                PubKey pubKey = new PubKey(pubstr[i]);
                return pubKey;
            }
        }
        return null;
    }
    public string GetAddress(PubKey puk)
    {
        List<PubKey> pubList = GetPukList();

        for (int i = 0; i < pubList.Count; i++)
        {
            if (puk == pubList[i])
            {
                return btcAddress[i];
            }
        }
        return null;
    }
}
