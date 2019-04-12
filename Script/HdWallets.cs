using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NBitcoin;
public class HdWallets
{

    public string[] GetBitcoin(string wordList, int index)
    {
        return GetInfoList(GetKeyByEnlishWord(wordList), index);
    }

    private Mnemonic GetKeyByEnlishWord(string wordList)
    {
        Mnemonic mnemo = new Mnemonic(wordList, Wordlist.English);
        return mnemo;
    }

    public Mnemonic GetEnlishWordList()
    {
        Mnemonic mnemo = new Mnemonic(Wordlist.English, WordCount.Twelve);
        return mnemo;
    }


    public string[] GetInfoList(Mnemonic mnemo, int index)
    {

        BitcoinSecret _bitcoinseret = new BitcoinSecret(QRPayTools.GetMnemonic(mnemo.ToString()).PrivateKey, NBitcoin.Network.Main);
       
        //ExtKey masterKey = mnemo.DeriveExtKey("my password");
        //ExtPubKey masterPubKey = masterKey.Neuter();

        //ExtKey key1 = masterKey.Derive((uint)index);
        //ExtPubKey pubkey1 = masterPubKey.Derive((uint)index);

        Key privateKey = _bitcoinseret.PrivateKey;
        string str0 = privateKey.GetBitcoinSecret(NBitcoin.Network.Main).ToString();

        PubKey publicKey = _bitcoinseret.PubKey;
        string str1 = publicKey.GetAddress(NBitcoin.Network.Main).ToString();

        KeyId publicKeyHash = publicKey.Hash;
        string str2 = publicKeyHash.GetAddress(NBitcoin.Network.Main).ToString();

        string[] strs = new string[3];
        strs[0] = str0;
        strs[1] = str1;
        strs[2] = str2;
        return strs;
    }


}
