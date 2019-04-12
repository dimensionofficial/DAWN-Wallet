using System;
using System.Collections;
using System.Collections.Generic;
using NBitcoin;
using Nethereum.JsonRpc.UnityClient;
using UnityEngine;

public class ETHInfo
{
    public string cachedPassword;

    public string address;
    public string privateKey;
    public static string _url = "https://mainnet.infura.io";
    public static string _testUrl = "https://kovan.infura.io";
    public static string keyberUrl = "https://kovan.etherscan.io/";

    public ETHInfo(Nethereum.Signer.EthECKey ecKey)
    {
        //ecKey = Nethereum.Signer.EthECKey.GenerateKey();
        var m_address = ecKey.GetPublicAddress();
        address = m_address.ToString();
        Debug.Log("ETH 地址：" + address);

        privateKey = ecKey.GetPrivateKey();
        Debug.Log("ETH 私钥：" + privateKey.ToString());

        // Then we define a new KeyStore service
        // var keystoreservice = new Nethereum.KeyStore.KeyStoreService();
        //var m_encryptedJson = keystoreservice.EncryptAndGenerateDefaultKeyStoreAsJson("", privateKeyBytes, address);
        //encryptedJson = m_encryptedJson.ToString();
    }

    public void importAccountFromPrivateKey(string accountPrivateKey)
    {
        // Here we try to get the public address from the secretKey we defined
        try
        {
            var m_address = Nethereum.Signer.EthECKey.GetPublicAddress(accountPrivateKey);
            Debug.Log("ETH 地址 = " + m_address);
            // Then we define the accountAdress private variable with the public key
            //address = m_address;
        }
        catch (Exception e)
        {
            // If we catch some error when getting the public address we just display the exception in the console
            Debug.Log("Error importing account from PrivateKey: " + e);
        }
    }

    public IEnumerator getAccountBalance(System.Action<decimal> callback)
    {
        var getBalanceRequest = new EthGetBalanceUnityRequest(_url);
        yield return getBalanceRequest.SendRequest(address, Nethereum.RPC.Eth.DTOs.BlockParameter.CreateLatest());
        if (getBalanceRequest.Exception == null)
        {
            var balance = getBalanceRequest.Result.Value;
            callback(Nethereum.Util.UnitConversion.Convert.FromWei(balance, 18));
        }
        else
        {
            throw new System.InvalidOperationException("Get balance request failed");
        }

    }
}
