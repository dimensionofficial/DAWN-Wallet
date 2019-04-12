using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NBitcoin;

public class mulsigtest : MonoBehaviour {

	// Use this for initialization
	void Start () {
        test_createmulsig();
    }

    void test_createmulsig()
    {
        string bip1 = QRPayTools.CreateBipString();
        string bip2 = QRPayTools.CreateBipString();
        string bip3 = QRPayTools.CreateBipString();

        var pri1 = new NBitcoin.BitcoinSecret(QRPayTools.GetMnemonic(bip1).PrivateKey, NBitcoin.Network.Main);
        var privatekey1 = QRPayTools.GetMnemonic(bip1).PrivateKey;
        var add1 = pri1.PubKey.GetAddress(NBitcoin.Network.Main).ToWif();
        var pub1 = pri1.PubKey;
        Debug.Log("pub1 " + pub1.ToString());
        //Debug.Log("address a:" + add1);

        var pri2 = new NBitcoin.BitcoinSecret(QRPayTools.GetMnemonic(bip2).PrivateKey, NBitcoin.Network.Main);
        var add2 = pri2.PubKey.GetAddress(NBitcoin.Network.Main).ToWif();
        var privatekey2 = QRPayTools.GetMnemonic(bip2).PrivateKey;
        var pub2 = pri2.PubKey;
        //Debug.Log("address b:" + add2);

        var pri3 = new NBitcoin.BitcoinSecret(QRPayTools.GetMnemonic(bip3).PrivateKey, NBitcoin.Network.Main);
        var add3 = pri3.PubKey.GetAddress(NBitcoin.Network.Main).ToWif();
        var pub3 = pri3.PubKey;
        //Debug.Log("address c:" + add3);

        List<PubKey> pubkeys = new List<PubKey>();
        pubkeys.Add(pub1);
        pubkeys.Add(pub2);
        pubkeys.Add(pub3);

        var mulsigAddress = BTCMulSig.CreateMulSigAddress(3, pubkeys);
        Debug.Log(mulsigAddress);
        Debug.Log("Create script address success!!!");

        ///test transfer
        Script mulSig = PayToMultiSigTemplate.Instance.GenerateScriptPubKey(2, pubkeys.ToArray());
        Transaction corpFunding = new Transaction()
        {
            Outputs =
            {
                 new TxOut(new Money(10, MoneyUnit.BTC), mulSig.Hash),
                 new TxOut(new Money(20, MoneyUnit.BTC), mulSig.Hash),
                 new TxOut(new Money(30, MoneyUnit.BTC), mulSig.Hash)
            }
        };
        List<Coin> coins = new List<Coin>();
        for (int i = 0; i < corpFunding.Outputs.Count; i++)
        {
            Coin c = new Coin(new OutPoint(corpFunding.GetHash(), i), corpFunding.Outputs[i]);
            coins.Add(c);
        }
        Coin[] coinArray = coins.ToArray();
        BitcoinAddress temp = BitcoinAddress.Create("1QJrJUgAtFJYKLPAj8b8TnCUMy8XPn5DKr", NBitcoin.Network.Main);
        BTCMulSig.MulSigTransactionBuilder builder =
            new BTCMulSig.MulSigTransactionBuilder(pubkeys, 2, coinArray, "1QJrJUgAtFJYKLPAj8b8TnCUMy8XPn5DKr", new Money(55, MoneyUnit.BTC), new Money(0.01m, MoneyUnit.BTC), temp);
        List<string> needsign = builder.GetSignatureHash();

        //sign with pub1冷钱包1签名
        foreach (var v in needsign)
        {
            string der = QRPayTools.SignAllWithNoneCoinA(v, privatekey1.ToBytes());
            builder.AddSig(pub1, der);
        }
        //sign with pub2冷钱包2签名
        foreach (var v in needsign)
        {
            string der = QRPayTools.SignAllWithNoneCoinA(v, privatekey2.ToBytes());
            builder.AddSig(pub2, der);
        }

        //创建transaction
        var tx = builder.Finish();
        Debug.Log(tx.Check());
    }
}
