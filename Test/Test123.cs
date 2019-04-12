using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NBitcoin;
using System.Linq;
using LitJson;
using System;
using QBitNinja4Unity.Models;
using NBitcoin.BouncyCastle.Math;
using Nethereum.RPC.Eth.DTOs;
using System.Numerics;
using Nethereum.JsonRpc.UnityClient;
using Nethereum.Hex.HexTypes;
using Nethereum.Signer;
using UnityEngine.Networking;
using System.Text;
using QBitNinja4Unity;

public class Test123 : MonoBehaviour {

    public enum FeesType
    {
        fastestFee = 30,
        halfHourFee = 30,
        hourFee = 10,
    }
    public FeesType feesType = FeesType.fastestFee;

    public MoneyUnit moneyType = MoneyUnit.Satoshi;


    public bool j = false;


    public string transactionStr;

    void Start()
    {
        var ethEck = new EthECKey(QRPayTools.GetMnemonic(SeedKeyManager.Instance.firstSeedBip, HardwareWallet.WalletType.ETH).PrivateKey.ToBytes(), true);
        string privateKey = ethEck.GetPrivateKey();
        Debug.Log(privateKey);
        //TestAddAdress();
       // StartCoroutine(TestSend());
        //Debug.Log(Convert.ToInt64("0x2", 16));         
        //    Test();
        // long number = Convert.ToInt64("0xf86b0285012a05f2008261a8946eb88024554eb84afe19b16aa3d46465ac0d812b87038d7ea4c68000801ba06c9ea757cb8613551eddda6398b30191f6e7dc83d12b8512c117f61801513ccaa0718507d077e80700d0a39467f6ac1fabe6b59cb634ae706bb8359db7580f96f7", 16);
        // Debug.Log(number);//Convert.ToString(number, 16));
        // return;
        //StartCoroutine(GetEthGasPrice());
        //      StartCoroutine(HttpManager._Intance.GetEthTransactionsByAddress("0x6eB88024554Eb84AFe19b16aA3d46465Ac0d812B"));

        //Debug.Log("width = " + (Camera.main.pixelWidth));
        //Debug.Log("height = " + (Camera.main.pixelHeight));
        //TestSendBTC();

        //Money fee = new Money(decimal.Parse("0.01452309"), MoneyUnit.BTC);
        //Debug.Log(fee.ToString());
        //decimal feeCount = decimal.Parse(fee.ToString());
        //feeCount += decimal.Parse("0.00000001");
        //Debug.Log(feeCount.ToString());
        //fee = new Money(feeCount,MoneyUnit.BTC);
        //Debug.Log(fee.ToString());
    }

    private void ReGetFee(ref Money _fee)
    {
        string tempFee = _fee.ToString();
        int tempIndex = 0;
        string newStr = "";
        bool isFind = false;
        for (int i = tempFee.Length - 1; i >= 0; i--)
        {

            if (!isFind)
            {
                int lastNumber = (int)tempFee[i];
                if (lastNumber > 0)
                {
                    isFind = true;
                    lastNumber -= 1;
                    tempIndex = i;
                }
                newStr += lastNumber.ToString();
            }

            newStr += tempFee[i];
        }

        string newStr1 = "";
        bool isNo = false;
        for (int i = newStr.Length - 1; i >= 0; i--)
        {
            newStr1 += newStr[i];

            if (newStr[i] != '.' && (int)newStr[i] > 0)
            {
                isNo = true;
            }
        }

        if (!isNo)
        {
            _fee = new Money(decimal.Parse(newStr1), MoneyUnit.BTC);
        }
    }

    public string tempStr;
    private void TestSendBTC()
    {
        
        //BitcoinPubKeyAddress fromAddress = new BitcoinPubKeyAddress("mpTJhEqhuR4U4qwYNDBRihTNN9NWdqpeuv", NewWalletManager._Intance.btcNetwork);//BitcoinPubKeyAddress(string base58, Network expectedNetwork = null);
        //string str = QRPayTools.GetHashtableStr(tempStr);
        //Hashtable temp = Json.jsonDecode(str) as Hashtable;
        //Hashtable data = temp["data"] as Hashtable;
        //string sss = Json.jsonEncode(data);
        //string singInfo = QRPayTools.RebuildTransaction(sss, fromAddress, "");
        var tx = new NBitcoin.Transaction("01000000094740109145066e9fabb0450a7c99f4a802ad6acd78aa58756193db18b0299fa5010000006b483045022100ab1b459b57202d5573228088272b9ac798385fafe230851fb6d33a7661b87035022026c54ce39e774014669f27750232b0edc45b3b25030b76bfb4417154f9897be60121025fbca15362204a561f69636486760a80c2349e1c50fee6d9def5f3f23002a53effffffffd28d77a60611314ea3c99bb1a5db0c454a28c88f64e1b0016aecdc7d6acc131f010000006a473044022056763335c995e45b3f887b57a1263c918b66e55f0bfa011d1d9cc0f2bb2fe67502201bda1a32f46130e9ea8d405c3f54c33d121aaaa87d665abdabe6af46e46f7ec50121025fbca15362204a561f69636486760a80c2349e1c50fee6d9def5f3f23002a53effffffff53a01466e170f7ae03a38b9dc9a47926dea8357a1245c3d18df6e3dc1bf5afa6010000006a47304402204beaf7541233424a583f9d5cff1a0dd271a50e04c496d7e04daadba154eda6ae02203aec9033bb9b0947032828ea94f308fbee458bdf78a3ddc843da0139730a23c30121025fbca15362204a561f69636486760a80c2349e1c50fee6d9def5f3f23002a53effffffff053bab146a7b3e4498259bab1298e974270cd761cf2dfc56b86748f7f8baa422010000006a47304402205a0c2f4e04494344c238927b2ac334ead8a512620e5c107d114a8aa10724f41b02200d9eb9e344cc21f88427ee399fe025531a913cd5da5fc35b841e56a0459219210121025fbca15362204a561f69636486760a80c2349e1c50fee6d9def5f3f23002a53effffffff0d68ac6d12e4fd75a1a4aa5b9992b813fddf2a2be31292031d0bd6fcc4506f57010000006a47304402204b569aa11eb00eb4899ef22521b5577f089e8cb8df3f75c8786a0ed96552aff802207518fd4da2a8ced29285e9d559d8e02d588a943b2be3c6d877d752ac18e37da60121025fbca15362204a561f69636486760a80c2349e1c50fee6d9def5f3f23002a53effffffffdbe18bf145793019423d5a9c22f5344feb9a2417a1d48f4fd75fd2be7c1b8123010000006b483045022100e0c718bdd22016fc9a471311b86280e6c6679a9e8c96ae8010d1914ba9a2700902202c6770bfdae40b95f424b2ceaaa1fd8a822858648097c10bdb6d2ef17116c3db0121025fbca15362204a561f69636486760a80c2349e1c50fee6d9def5f3f23002a53effffffff8c2b6070487152ba1094d883d888dce6ab55b9b53d4f8a92a4190e6ad7edef16010000006b483045022100f9f839500c823030da9fc26d5e35811cc59e3dbbd196a266dbf958d75fe572a5022071be2453a56bcc25a0f04eee5b3e889b5107efe83298e21ef3834d2cf09a5b7f0121025fbca15362204a561f69636486760a80c2349e1c50fee6d9def5f3f23002a53effffffff8f240d46e5bd3bcb55374db95ec3a42aa568106de8b2c8823f441e5e5b54dd36010000006b483045022100b75d10e1269434d807f9979eb5575e830dadc9b253f035204b1d90507196379f022036d37661223032253aaf1ee62731c674aba018a2059c8dd94f128e64cb40659d0121025fbca15362204a561f69636486760a80c2349e1c50fee6d9def5f3f23002a53effffffff026ca28d506fc6f227e7298fddd1edccf3064db62b2762b8e4c935ed35d24417010000006b483045022100bdf37e78f867faf403413c4b93859daf8dbefb862f7162e242b462849da1783202206603607f129c1df990727888ce490561bcee435fdecf88770ad75c6316944dc40121025fbca15362204a561f69636486760a80c2349e1c50fee6d9def5f3f23002a53effffffff0264000000000000001976a9141839429c532f4b0d5083ace76bb4f59c39cd296d88ac70170000000000001976a9146ad6c0de8b550438c6bcc8cbedca28b6a05841f888ac00000000");
        QBitNinja4Unity.QBitNinjaClient.Broadcast(tx, NewWalletManager._Intance.btcNetwork, Send3);
    }

    //获得交易结果
    void Send3(QBitNinja.Client.Models.BroadcastResponse broadcastResponse, NBitcoin.Network network)
    {

        if (broadcastResponse.Error == null)
        {
            //      ShowSendOverObject(true);
            UnityEngine.Debug.Log("Success! You can check out the hash of the transaciton in any block explorer:");
            //UnityEngine.Debug.Log(transaction.GetHash());
        }
        else
        {
            UnityEngine.Debug.Log(string.Format("ErrorCode: {0}", broadcastResponse.Error.ErrorCode));
            UnityEngine.Debug.Log("Error message: " + broadcastResponse.Error.Reason);
            //    ShowSendOverObject(false);
        }
     
    }

    private void TestAddAdress()
    {
        WWWForm ws = new WWWForm();
        ws.AddField("op", "addAddress");
        ws.AddField("userName", "caolin");
        ws.AddField("addressType", 2);
        ws.AddField("address", "0x6eB88024554Eb84AFe19b16aA3d46465Ac0d812B");
        ws.AddField("walletName", "个人钱包");
  //      StartCoroutine(HttpManager._Intance.SendRequest(ws, null));
    }

    void Update()
    {
        if (j)
        {
            j = false;
            Money m_fee = new Money(10 * 255, moneyType);
            
            Debug.Log(m_fee);
        }
    }

    private IEnumerator GetETHGas()
    {
        Hashtable myJsonData = new Hashtable();
        myJsonData["id"] = 73;
        myJsonData["jsonrpc"] = "2.0";
        myJsonData["method"] = "eth_gasPrice";
        ArrayList arrayList = new ArrayList();
        myJsonData["params"] = arrayList;
        string rpcRequestJson = Json.jsonEncode(myJsonData);
        UnityWebRequest unityRequest = QRPayTools.GetUnityWebRequest(rpcRequestJson);
        yield return unityRequest.SendWebRequest();
        if (unityRequest.error != null)
        {
            Debug.Log(unityRequest.error + " : " + rpcRequestJson);
        }
        else
        {
            byte[] results = unityRequest.downloadHandler.data;
            string responseJson = Encoding.UTF8.GetString(results).ToString();
            Hashtable table = Json.jsonDecode(responseJson) as Hashtable;
            if (table.Contains("error"))
            {
                string str1 = Json.jsonEncode(table["error"]);
            }
            else
            {
                string resultValue = table["result"].ToString();
                Debug.Log(resultValue);
                Int64 gas = Convert.ToInt64(resultValue, 16);
                Debug.Log(gas);
            }
        }
    }


    private IEnumerator GetAppInfo()
    {

        UnityWebRequest unityRequest = new UnityWebRequest("https://www.okcoin.cn/api/v1/exchange_rate.do");//("http://data.gateio.io/api2/1/ticker/");
        //   UploadHandlerRaw uploadHandler = new UploadHandlerRaw(requestBytes);
        unityRequest.SetRequestHeader("Content-Type", "application/json");
        //    uploadHandler.contentType = "application/json";
        //    unityRequest.uploadHandler = uploadHandler;
        unityRequest.downloadHandler = new DownloadHandlerBuffer();
        yield return unityRequest.SendWebRequest();
        if (unityRequest.error != null)
        {
            Debug.Log(unityRequest.error);
        }
        else
        {
            Debug.Log(unityRequest.downloadHandler.text);
            byte[] results = unityRequest.downloadHandler.data;
            string responseJson = Encoding.UTF8.GetString(results).ToString();
            Debug.Log("responseJson = " + responseJson);
        }
    }

    private IEnumerator TestTran()
    {
        //{"jsonrpc":"2.0","method":"eth_getTransactionCount","params":["0x407d73d8a49eeb85d32cf465507dd71d507100c1","latest"],"id":1}
        Hashtable myJsonData = new Hashtable();
        myJsonData["id"] = 1;
        myJsonData["jsonrpc"] = "2.0";
        myJsonData["method"] = "eth_getTransactionCount";
        ArrayList arrayList = new ArrayList();
        arrayList.Add("address");
        arrayList.Add("latest");
        myJsonData["params"] = arrayList;
        string rpcRequestJson = Json.jsonEncode(myJsonData);

        var _transactionCountRequest = new EthGetTransactionCountUnityRequest(ETHInfo._url);
        var _transactionSigner = new TransactionSigner();
        yield return _transactionCountRequest.SendRequest("0x6eB88024554Eb84AFe19b16aA3d46465Ac0d812B", Nethereum.RPC.Eth.DTOs.BlockParameter.CreateLatest());
    }

    IEnumerator GetEthGasPrice()
    {
        //{"jsonrpc":"2.0","method":"eth_gasPrice","params":[],"id":73}
        Hashtable myJsonData = new Hashtable();
        myJsonData["id"] = 73;
        myJsonData["jsonrpc"] = "2.0";
        myJsonData["method"] = "eth_gasPrice";
        ArrayList arrayList = new ArrayList();
        myJsonData["params"] = arrayList;
        string rpcRequestJson = Json.jsonEncode(myJsonData);
        UnityWebRequest unityRequest = QRPayTools.GetUnityWebRequest(rpcRequestJson);
        yield return unityRequest.SendWebRequest();
        ///     Result
        ///     {
        ///     "id":73,
        ///     "jsonrpc": "2.0",
        ///     "result": "0x09184e72a000" // 10000000000000
        ///     }
        if (unityRequest.error != null)
        {
            Debug.Log(unityRequest.error + " : " + rpcRequestJson);
        }
        else
        {
            byte[] results = unityRequest.downloadHandler.data;
            string responseJson = Encoding.UTF8.GetString(results).ToString();
            Debug.Log(responseJson);
            Hashtable table = Json.jsonDecode(responseJson) as Hashtable;
            if (table.Contains("error"))
            {
                string str1 = Json.jsonEncode(table["error"]);
                Hashtable table1 = Json.jsonDecode(str1) as Hashtable;
            }
            else
            {
                
                string resultValue = table["result"].ToString();
                Int64 price = Convert.ToInt64(resultValue, 16);
                decimal f = Nethereum.Util.UnitConversion.Convert.FromWei(price, 18);
                Debug.Log(resultValue + " ----------- " + price  + " --------- " + f.ToString());
            }
            //Hashtable table = Json.jsonDecode(responseJson) as Hashtable;
            //string Result = table["result"].ToString();
            //Int64 ethBalance = Convert.ToInt64(Result, 16);
            //coinInfo.money = Nethereum.Util.UnitConversion.Convert.FromWei(ethBalance, 18);
            //countText.text = coinInfo.money.ToString();
        }
    }

    IEnumerator TestSend()
    {
        //{"jsonrpc":"2.0","method":"eth_sendRawTransaction","params":[{see above}],"id":1}
        Hashtable myJsonData = new Hashtable();
        myJsonData["id"] = 1;
        myJsonData["jsonrpc"] = "2.0";
        myJsonData["method"] = "eth_sendRawTransaction";
        ArrayList arrayList = new ArrayList();
       
        arrayList.Add("0xf8a92484bf54b680830226c8944d415ebf33ddb206d858fea170ad55375e8848fd80b844a9059cbb000000000000000000000000ea372370b6e25b7281cce0a74f38e95a4a64a6400000000000000000000000000000000000000000000000008ac7230489e800001ba0852434a0d9ae5aba009077d9cb4401fb863a771629aca53af3d3a850061c8413a03ee3548126f151f0ea31ad4b5b69bb88a4aae4c897ad16e8b0cc8fcae447ecb8");
        //arrayList.Add("0xf86d808609184e72a0008276c0946d04bdb3e8738fe0c59a69811b07af20e459573c888ac7230489e80000801ba063053265c045b77f4beb9592e1918d1956c42858a603e6b9d58df97778e866f4a03c0190465c85cb9da02370576dd3cd376efff669898c8cd1f690644bf1f43532");
        //arrayList.Add("0xf86d738609184e72a0008276c0947854c8a4dbc0ab7e639626db2db11a4bd7d59d9e888ac7230489e80000801ca0410fbd32e016d9b17ec15c753b64a16de917ddb1d067aeec1cccf7a269d4cb44a022f8f90ba0699da349ede6c7512387cac004a167da3f36bdc98735e04a214fb9"); 
         myJsonData["params"] = arrayList;
        string rpcRequestJson = Json.jsonEncode(myJsonData);
        Debug.Log(rpcRequestJson);
        UnityWebRequest unityRequest = QRPayTools.GetUnityWebRequest(rpcRequestJson);
        yield return unityRequest.SendWebRequest();

        if (unityRequest.error != null)
        {
            Debug.Log(unityRequest.error + " : " + rpcRequestJson);
        }
        else
        {
            byte[] results = unityRequest.downloadHandler.data;
            string responseJson = Encoding.UTF8.GetString(results).ToString();
            Debug.Log(responseJson);
            Hashtable table = Json.jsonDecode(responseJson) as Hashtable;
            Debug.Log(table.Contains("result"));
            Debug.Log(table.Contains("error"));
            if (table.Contains("error"))
            {
                string str1 = Json.jsonEncode(table["error"]);
                Debug.Log(str1);
                Hashtable table1 = Json.jsonDecode(str1) as Hashtable;
                Debug.Log(table1["message"].ToString());
            }
            else
            {
                Debug.Log("发送成功");
            }
            //Hashtable table = Json.jsonDecode(responseJson) as Hashtable;
            //string Result = table["result"].ToString();
            //Int64 ethBalance = Convert.ToInt64(Result, 16);
            //coinInfo.money = Nethereum.Util.UnitConversion.Convert.FromWei(ethBalance, 18);
            //countText.text = coinInfo.money.ToString();
        }
//        Debug.Log("AAAAAAAAAA");
 //       Debug.Log(strinfo);

        //var _ethSendTransactionRequest = new EthSendRawTransactionUnityRequest(ETHInfo._url);
        //yield return _ethSendTransactionRequest.SendRequest("f86b0285012a05f2008261a8946eb88024554eb84afe19b16aa3d46465ac0d812b87038d7ea4c68000801ba06c9ea757cb8613551eddda6398b30191f6e7dc83d12b8512c117f61801513ccaa0718507d077e80700d0a39467f6ac1fabe6b59cb634ae706bb8359db7580f96f7");
        //Debug.Log("BBBBBBBBBB");
        //if (_ethSendTransactionRequest.Exception == null)
        //{
        //    Debug.Log(_ethSendTransactionRequest.Result);
        //    //walletInfoPanel.currentItem.RefreshBalance();
        //}
        //else
        //{
        //    Debug.Log(_ethSendTransactionRequest.Exception);
        //}
    }

    public void Test()
    {
        HdWallets hdWallets = new HdWallets();
        Mnemonic mnemo = hdWallets.GetEnlishWordList();
        string worldList = mnemo.ToString();
        Debug.Log("worldList = " + worldList);

        ExtKey masterKey = mnemo.DeriveExtKey();
        Debug.Log("BTC 1 私钥: " + masterKey.PrivateKey.GetBitcoinSecret(NBitcoin.Network.Main).ToString());
        Debug.Log("BTC 1 = " + masterKey.ScriptPubKey.GetDestinationAddress(NBitcoin.Network.Main));

        byte[] speed = mnemo.DeriveSeed();

        Nethereum.Signer.EthECKey ecKey = new Nethereum.Signer.EthECKey(speed, true);
        string m_address = ecKey.GetPublicAddress();
        Debug.Log("ETH1 私钥：" + ecKey.GetPrivateKey());
        Debug.Log( "ETH 1 = " + m_address);

        mnemo = new Mnemonic(worldList, Wordlist.English);
        ExtKey masterKey1 = mnemo.DeriveExtKey();
        Debug.Log("BTC 2 私钥: " + masterKey1.PrivateKey.GetBitcoinSecret(NBitcoin.Network.Main).ToString());
        Debug.Log("BTC 2 =" + masterKey1.ScriptPubKey.GetDestinationAddress(NBitcoin.Network.Main));

        byte[] speed1 = mnemo.DeriveSeed();
        Nethereum.Signer.EthECKey ecKey1 = new Nethereum.Signer.EthECKey(speed1, true);
        string m_address1 = ecKey1.GetPublicAddress();
        Debug.Log("ETH2 私钥：" + ecKey1.GetPrivateKey());
        Debug.Log( "ETH 2 = " + m_address1);
    }

    // Update is called once per frame
 //   void Update ()
 //   {
 //       if (j)
 //       {
 //           j = false;
 //           //GetMoneyFee();
 //           Test();
 //       }	
	//}

    /// <summary>
    /// 获取旷工费
    /// </summary>
    public void GetMoneyFee()
    {
        QBitNinja4Unity.QBitNinjaClient.GetFee(NBitcoin.Network.Main, GetFee);
    }

    Money fee;
    void GetFee(QBitNinja4Unity.Models.Fees result)
    {
        var txSizeInBytes = 250;
        var fastestSatoshiPerByteFee = (int)feesType;
        fee = new Money(fastestSatoshiPerByteFee * txSizeInBytes, moneyType);
        Debug.Log(fee);
    }
}
