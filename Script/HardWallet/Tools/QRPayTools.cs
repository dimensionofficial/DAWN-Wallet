using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NBitcoin;
using HardwareWallet;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.JsonRpc.UnityClient;
using Nethereum.Hex.HexTypes;
using Nethereum.Signer;
using Nethereum.Hex.HexConvertors.Extensions;
using UnityEngine.Networking;
using System.Text;
using System;
using Nethereum.Signer.Crypto;
using NBitcoin.Crypto;
using System.IO;
using NBitcoin.DataEncoders;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Parameters;
using System.Linq;
using Org.BouncyCastle.Math.EC.Multiplier;

public class QRPayTools {
    private static string Bip44BTCPath = "m/44/0/0/0/0";
    private static string Bip44ETHPath = "m/44/60/0/0/0";
    private static string Bip44EOSPath = "m/44/194/0/0/0";
    
    #region BTC
    public static string CreateNoSignPayQRInfo(Money fee, List<decimal> toCount, List<BitcoinAddress> toAddress, string fromAddress, HashSet<Coin> coinsToSpend)
    {
        Hashtable ht = new Hashtable();
        ht["path"] = Bip44BTCPath;
        ht["sign"] = "BTC";
       

        string b = fee.Satoshi + "$";
        ArrayList tb = new ArrayList();
        for (int i = 0; i < toAddress.Count; i++)
        {
            string a = toCount[i].ToString() + "&" + toAddress[i].ToString();
            tb.Add(a);
        }
        string ttb = Json.jsonEncode(tb);
        b += ttb + "$";
        ArrayList t = new ArrayList();
        foreach (var v in coinsToSpend)
        {

            if (v.GetType() == typeof(ScriptCoin))
            {
                ScriptCoin temp = (ScriptCoin)v;
                string a = temp.Outpoint.ToString() + "&" + temp.Redeem.ToHex() + "&" + temp.TxOut.ScriptPubKey.ToHex() + "&" + temp.TxOut.Value.Satoshi;
                t.Add(a);
            }
            else
            {
                Coin temp = (Coin)v;
                string a = temp.Outpoint.ToString() + "&" + temp.TxOut.ScriptPubKey.ToHex() + "&" + temp.TxOut.Value.Satoshi;
                t.Add(a);
            }
        }
        string tp = Json.jsonEncode(t);
        b += tp;

        ht["data"] = Encry.ToBase64String(b);

        string js = Json.jsonEncode(ht);

        return js;
    }
    public static string MulitCreateNoSignPayQRInfo(List<string> needsign)
    {
        string js=null;
        for (int i = 0; i < needsign.Count; i++)
        {
            Hashtable ht = new Hashtable();
            ht["path"] = Bip44BTCPath;
            ht["sign"] = "OT2";
            ht["data"] = Encry.ToBase64String(needsign[i]);
            string j = Json.jsonEncode(ht);
            if (i==0)
            {
                js = j;
            }
            else
            {
                js = js + "^" + j;
            }
        }
        return js;
    }
    public static List<string> MulitSignInfo(string ders)
    {
        List<string> derList = new List<string>();
        string[] strs = ders.Split('^');
        if (strs.Length>0)
        {
            foreach (var item in strs)
            {
                derList.Add(item);
            }
            return derList;
        }
        else
        {
            return null;
        }
        
    }
    public static string GetHashtableStr(string str)
    {
        string[] strs = str.Split('$');
        if (strs.Length > 0)
        {
            try
            {
                Hashtable payData = new Hashtable();
                payData["fee"] = strs[0];

                ArrayList tempList = Json.jsonDecode(strs[1]) as ArrayList;
                ArrayList payItems = new ArrayList();
                for (int i = 0; i < tempList.Count; i++)
                {
                    string[] tempstrs = tempList[i].ToString().Split('&');
                    Hashtable item = new Hashtable();
                    item["count"] = tempstrs[0].ToString();
                    item["toAddress"] = tempstrs[1].ToString();
                    payItems.Add(item);
                }
                payData["payItems"] = payItems;

                ArrayList tempList2 = Json.jsonDecode(strs[2]) as ArrayList;
                ArrayList spendCoints = new ArrayList();
                for (int i = 0; i < tempList2.Count; i++)
                {
                    string[] tempstrs = tempList2[i].ToString().Split('&');
                    if (tempstrs.Length > 3)
                    {
                        Hashtable item = new Hashtable();
                        item["outPoint"] = tempstrs[0];
                        item["redeemScript"] = tempstrs[1];
                        item["scriptPubKey"] = tempstrs[2];
                        item["value"] = tempstrs[3];
                        spendCoints.Add(item);
                    }
                    else
                    {
                        Hashtable item = new Hashtable();
                        item["outPoint"] = tempstrs[0]; ;
                        item["scriptPubKey"] = tempstrs[1];
                        item["value"] = tempstrs[2];
                        spendCoints.Add(item);
                    }
                }
                payData["spendCoints"] = spendCoints;
                string result = Json.jsonEncode(payData);
                //Debug.Log(result.Length);
                return result;
            }
            catch
            {
                return null;
            }
        }

        return null;
    }

    public static bool GetPayInfo(string data, out List<string> sendToAddress, out string SendFromAddress, out double outAmount, out double outMinnerFee, BitcoinPubKeyAddress pubkey, NBitcoin.Network network)
    {
        SendFromAddress = "";
        outAmount = 0;
        outMinnerFee = 0;
        sendToAddress = new List<string>();
        try
        {
            Hashtable payDataInHardware = (Hashtable)Json.jsonDecode(data);
            Money feeInHardWare = new Money(long.Parse(payDataInHardware["fee"].ToString()));
            
            var changeScriptPubKey = pubkey;
            //input
            ArrayList spendCointsInHardware = (ArrayList)payDataInHardware["spendCoints"];
            Money txInAmount = Money.Zero;
            foreach (var v in spendCointsInHardware)
            {
                Hashtable item = (Hashtable)v;
                var scriptPubKey = new Script(NBitcoin.DataEncoders.Encoders.Hex.DecodeData(item["scriptPubKey"].ToString()));
                if (changeScriptPubKey.ScriptPubKey.ToHex() != scriptPubKey.ToHex())
                {
                    return false;
                }
            }
            var minerFee = new Money(long.Parse(payDataInHardware["fee"].ToString()));
            outMinnerFee = minerFee.Satoshi / (double)100000000;
            SendFromAddress = changeScriptPubKey.ToString();

            ArrayList payItemsInHardware = (ArrayList)payDataInHardware["payItems"];
            var hallOfTheMakersAmount = Money.Zero;
            foreach (var v in payItemsInHardware)
            {
                Hashtable item = (Hashtable)v;
                Money count = new Money(decimal.Parse(item["count"].ToString()), MoneyUnit.BTC);
                hallOfTheMakersAmount = hallOfTheMakersAmount + count;
            }
            outAmount = hallOfTheMakersAmount.Satoshi / (double)100000000;
            foreach (var v in payItemsInHardware)
            {
                Hashtable item = (Hashtable)v;
                var hallOfTheMakersAddress = BitcoinAddress.Create(item["toAddress"].ToString());
                sendToAddress.Add(hallOfTheMakersAddress.ToString());
            }
            if (sendToAddress.Count == 0 || outAmount == 0 || SendFromAddress == "")
            {
                return false;
            }
            return true;
        }
        catch(Exception e)
        {
            return false;
        }
    }

    public static string CreateSignedPayWithoutBuilder(string data, BitcoinSecret mainNetPrivateKey, NBitcoin.Network network)
    {
        try
        {
            data = QRPayTools.GetHashtableStr(data);
            Hashtable payDataInHardware = (Hashtable)Json.jsonDecode(data);
            Money feeInHardWare = new Money(long.Parse(payDataInHardware["fee"].ToString()));
            var changeScriptPubKey = mainNetPrivateKey.PubKey.GetAddress(network);
            var transaction = new NBitcoin.Transaction();
            //input
            ArrayList spendCointsInHardware = (ArrayList)payDataInHardware["spendCoints"];
            List<Coin> coinWillSpend = new List<Coin>();
            Money txInAmount = Money.Zero;
            foreach (var v in spendCointsInHardware)
            {
                Hashtable item = (Hashtable)v;
                if (item.ContainsKey("redeemScript"))
                {
                    OutPoint outPoint = OutPoint.Parse(item["outPoint"].ToString());
                    var scriptPubKey = new Script(NBitcoin.DataEncoders.Encoders.Hex.DecodeData(item["scriptPubKey"].ToString()));
                    TxOut txOut = new TxOut(new Money(long.Parse(item["value"].ToString())), scriptPubKey);
                    string redeemScriptStr = item["redeemScript"].ToString();
                    var redeemScript = redeemScriptStr.Length == 0 ? null : new Script(NBitcoin.DataEncoders.Encoders.Hex.DecodeData(redeemScriptStr));
                    Coin coin = new ScriptCoin(outPoint, txOut, redeemScript);
                    coinWillSpend.Add(coin);
                    txInAmount = txInAmount + coin.Amount;
                }
                else
                {
                    OutPoint outPoint = OutPoint.Parse(item["outPoint"].ToString());
                    var scriptPubKey = new Script(NBitcoin.DataEncoders.Encoders.Hex.DecodeData(item["scriptPubKey"].ToString()));
                    TxOut txOut = new TxOut(new Money(long.Parse(item["value"].ToString())), scriptPubKey);
                    Coin coin = new Coin(outPoint, txOut);
                    coinWillSpend.Add(coin);
                    txInAmount = txInAmount + coin.Amount;
                }
            }
            foreach (var v in coinWillSpend)
            {
                transaction.Inputs.Add(new TxIn()
                {
                    PrevOut = v.Outpoint
                });
            }
            //out
            var minerFee = new Money(long.Parse(payDataInHardware["fee"].ToString()));
            ArrayList payItemsInHardware = (ArrayList)payDataInHardware["payItems"];
            var hallOfTheMakersAmount = Money.Zero;
            foreach (var v in payItemsInHardware)
            {
                Hashtable item = (Hashtable)v;
                Money count = new Money(decimal.Parse(item["count"].ToString()), MoneyUnit.BTC);
                hallOfTheMakersAmount = hallOfTheMakersAmount + count;
            }
            Money changeBackAmount = txInAmount - hallOfTheMakersAmount - minerFee;

            TxOut changeBackTxOut = new TxOut()
            {
                Value = changeBackAmount,
                ScriptPubKey = mainNetPrivateKey.ScriptPubKey
            };

            List<TxOut> hallOfTheMakersTxOut = new List<TxOut>();

            foreach (var v in payItemsInHardware)
            {
                Hashtable item = (Hashtable)v;
                Money count = new Money(decimal.Parse(item["count"].ToString()), MoneyUnit.BTC);
                var hallOfTheMakersAddress = BitcoinAddress.Create(item["toAddress"].ToString()).ScriptPubKey;
                TxOut h = new TxOut()
                {
                    Value = count,
                    ScriptPubKey = hallOfTheMakersAddress
                };
                hallOfTheMakersTxOut.Add(h);
            }
            if (changeBackAmount.ToDecimal(MoneyUnit.BTC) > 0.000006M)
            {
                transaction.Outputs.Add(changeBackTxOut); 
            }
            foreach (var v in hallOfTheMakersTxOut)
            {
                transaction.AddOutput(v);
            }
            //sign
            foreach (var v in transaction.Inputs)
            {
                v.ScriptSig = mainNetPrivateKey.ScriptPubKey;
            }
            transaction.Sign(mainNetPrivateKey, false);
            return transaction.ToHex();
        }
        catch
        {
            return null;
        }
    }

    public static string RebuildTransaction(string data, BitcoinPubKeyAddress pubKey, string signData)
    {
        //try
        //{
        Debug.Log(data);
            Hashtable payDataInHardware = (Hashtable)Json.jsonDecode(data);
            Money feeInHardWare = new Money(long.Parse(payDataInHardware["fee"].ToString()));
            var changeScriptPubKey = pubKey;
            var transaction = new NBitcoin.Transaction();
            //input
            ArrayList spendCointsInHardware = (ArrayList)payDataInHardware["spendCoints"];
            List<Coin> coinWillSpend = new List<Coin>();
            Money txInAmount = Money.Zero;
            foreach (var v in spendCointsInHardware)
            {
                Hashtable item = (Hashtable)v;
                if (item.ContainsKey("redeemScript"))
                {
                    OutPoint outPoint = OutPoint.Parse(item["outPoint"].ToString());
                    var scriptPubKey = new Script(NBitcoin.DataEncoders.Encoders.Hex.DecodeData(item["scriptPubKey"].ToString()));
                    TxOut txOut = new TxOut(new Money(long.Parse(item["value"].ToString())), scriptPubKey);
                    string redeemScriptStr = item["redeemScript"].ToString();
                    var redeemScript = redeemScriptStr.Length == 0 ? null : new Script(NBitcoin.DataEncoders.Encoders.Hex.DecodeData(redeemScriptStr));
                    Coin coin = new ScriptCoin(outPoint, txOut, redeemScript);
                    coinWillSpend.Add(coin);
                    txInAmount = txInAmount + coin.Amount;
                }
                else
                {
                    OutPoint outPoint = OutPoint.Parse(item["outPoint"].ToString());
                    var scriptPubKey = new Script(NBitcoin.DataEncoders.Encoders.Hex.DecodeData(item["scriptPubKey"].ToString()));
                    TxOut txOut = new TxOut(new Money(long.Parse(item["value"].ToString())), scriptPubKey);
                    Coin coin = new Coin(outPoint, txOut);
                    coinWillSpend.Add(coin);
                    txInAmount = txInAmount + coin.Amount;
                }
            }
            foreach (var v in coinWillSpend)
            {
                transaction.Inputs.Add(new TxIn()
                {
                    PrevOut = v.Outpoint
                });
            }
            //out
            var minerFee = new Money(long.Parse(payDataInHardware["fee"].ToString()));
            ArrayList payItemsInHardware = (ArrayList)payDataInHardware["payItems"];
            var hallOfTheMakersAmount = Money.Zero;
            foreach (var v in payItemsInHardware)
            {
                Hashtable item = (Hashtable)v;
                Money count = new Money(decimal.Parse(item["count"].ToString()), MoneyUnit.BTC);
                hallOfTheMakersAmount = hallOfTheMakersAmount + count;
            }
            Money changeBackAmount = txInAmount - hallOfTheMakersAmount - minerFee;

            TxOut changeBackTxOut = new TxOut()
            {
                Value = changeBackAmount,
                ScriptPubKey = pubKey.ScriptPubKey
            };

            List<TxOut> hallOfTheMakersTxOut = new List<TxOut>();

            foreach (var v in payItemsInHardware)
            {
                Hashtable item = (Hashtable)v;
                Money count = new Money(decimal.Parse(item["count"].ToString()), MoneyUnit.BTC); 
                var hallOfTheMakersAddress = BitcoinAddress.Create(item["toAddress"].ToString()).ScriptPubKey;
                TxOut h = new TxOut()
                {
                    Value = count,
                    ScriptPubKey = hallOfTheMakersAddress
                };
                hallOfTheMakersTxOut.Add(h);
            }
            transaction.Outputs.Add(changeBackTxOut);
            foreach (var v in hallOfTheMakersTxOut)
            {
                transaction.AddOutput(v);
            }
            //sign
            string[] signList = signData.Split('*');
            int i = 0;
            foreach (var v in transaction.Inputs.AsIndexedInputs())
            {
                v.ScriptSig = new Script(signList[i]);
            }
            return transaction.ToHex();
        //}
        //catch
        //{
        //    return null;
        //}
    }
    #endregion

    #region ETH
    public static void CreateNoSignPayQRInfo_ETH(string addressFrom, string addressTo, System.Numerics.BigInteger transferAmount, System.Numerics.BigInteger gas, System.Numerics.BigInteger gasPrice, GameObject owner, string url, string tokenData, System.Action<string> callback)
    {
        owner.GetComponent<MonoBehaviour>().StartCoroutine(
            CreateNoSignPayQRInfo_ETH_IE(addressFrom, addressTo, transferAmount, gas, gasPrice, url, tokenData, callback));
    }
    public static string CreateSignedPayWithoutBuilder_ETH(string data, string privateKey)
    {
        try
        {
            Hashtable table = Json.jsonDecode(data) as Hashtable;
            string addressFrom = table["addressFrom"].ToString();
            string addressTo = table["addressTo"].ToString();
            System.Numerics.BigInteger transferAmount = System.Numerics.BigInteger.Parse(table["transferAmount"].ToString());
            System.Numerics.BigInteger gas = System.Numerics.BigInteger.Parse(table["gas"].ToString());
            System.Numerics.BigInteger gasPrice = System.Numerics.BigInteger.Parse(table["gasPrice"].ToString());
            System.Numerics.BigInteger nonce = System.Numerics.BigInteger.Parse(table["nonce"].ToString());
            if (table.ContainsKey("tokenData"))
            {
                string transData = table["tokenData"].ToString();
                var _transactionSigner = new TransactionSigner();
                var signedTransaction = _transactionSigner.SignTransaction(privateKey, addressTo, transferAmount, nonce, gasPrice, gas, transData);
                if (!_transactionSigner.VerifyTransaction(signedTransaction))
                {
                    throw new System.Exception("error code");
                }

                return signedTransaction;
            }
            //TransactionInput input = CreateTransferFundsTransactionInput(addressFrom, addressTo, privateKey,
            //transferAmount, new HexBigInteger(gas), new HexBigInteger(gasPrice));
            //gasPrice *= 1000000000;
            else
            {
                var _transactionSigner = new TransactionSigner();
                var signedTransaction = _transactionSigner.SignTransaction(privateKey, addressTo, transferAmount, nonce, gasPrice, gas);
                if (!_transactionSigner.VerifyTransaction(signedTransaction))
                {
                    throw new System.Exception("error code");
                }
                return signedTransaction;
            }
        }
        catch(System.Exception e)
        {
            Debug.Log(e.Message);
            return null;
        }
    }

    public static UnityWebRequest GetUnityWebRequest(string jsonString)
    {
        byte[] requestBytes = Encoding.UTF8.GetBytes(jsonString);
        UnityWebRequest unityRequest = new UnityWebRequest(ETHInfo._url, "POST");
        UploadHandlerRaw uploadHandler = new UploadHandlerRaw(requestBytes);
        unityRequest.SetRequestHeader("Content-Type", "application/json");
        uploadHandler.contentType = "application/json";
        unityRequest.uploadHandler = uploadHandler;
        unityRequest.downloadHandler = new DownloadHandlerBuffer();
        return unityRequest;
    }

    private static IEnumerator CreateNoSignPayQRInfo_ETH_IE(string addressFrom, string addressTo, System.Numerics.BigInteger transferAmount, System.Numerics.BigInteger gas, System.Numerics.BigInteger gasPrice, string url, string tokenData, System.Action<string> callback)
    {
        //{"jsonrpc":"2.0","method":"eth_getTransactionCount","params":["0x407d73d8a49eeb85d32cf465507dd71d507100c1","latest"],"id":1}
        //{"jsonrpc":"2.0","id":1,"result":"0x2"}
        //HexBigInteger nonce = null;
        Hashtable myJsonData = new Hashtable();
        myJsonData["id"] = 1;
        myJsonData["jsonrpc"] = "2.0";
        myJsonData["method"] = "eth_getTransactionCount";
        ArrayList arrayList = new ArrayList();
        arrayList.Add(addressFrom);
        arrayList.Add("pending");
        myJsonData["params"] = arrayList;
        string rpcRequestJson = Json.jsonEncode(myJsonData);
        UnityWebRequest unityRequest = GetUnityWebRequest(rpcRequestJson);
        yield return unityRequest.SendWebRequest();
        if (unityRequest.error != null)
        {
            Debug.Log(unityRequest.error);
        }
        else
        {
            byte[] results = unityRequest.downloadHandler.data;
            string responseJson = Encoding.UTF8.GetString(results).ToString();
            Hashtable tempTable = Json.jsonDecode(responseJson) as Hashtable;
            string Result = tempTable["result"].ToString();
            System.Int64 tempValue = Convert.ToInt64(Result, 16);
            //int tC = GetLocalHistroyWaitCount(addressFrom);
            //tempValue += tC;

            //Debug.Log("Nonce =   =======  " + tempValue);

            Hashtable ht = new Hashtable();
            ht["path"] = Bip44ETHPath;
            ht["sign"] = "ETH";

            Hashtable table = new Hashtable();
            table["addressFrom"] = addressFrom;
            table["addressTo"] = addressTo;
            table["transferAmount"] = transferAmount.ToString();
            table["gas"] = gas.ToString();
            table["gasPrice"] = gasPrice.ToString();
            table["nonce"] = tempValue.ToString();
            //string data = Json.jsonEncode(table);
            if(!string.IsNullOrEmpty(tokenData))
                table["tokenData"] = tokenData;

            string data = Json.jsonEncode(table);
            data = Encry.ToBase64String(data);
            ht["data"] = data;

            string result = Json.jsonEncode(ht);

            if (callback != null)
            {
                callback(result);
            }
        }
        //var _transactionCountRequest = new EthGetTransactionCountUnityRequest(url);
        //var _transactionSigner = new TransactionSigner();
        //yield return _transactionCountRequest.SendRequest(addressFrom, Nethereum.RPC.Eth.DTOs.BlockParameter.CreateLatest());
        //if (_transactionCountRequest.Exception == null)
        //{
        //    nonce = _transactionCountRequest.Result;
        //    Hashtable table = new Hashtable();
        //    table["addressFrom"] = addressFrom;
        //    table["addressTo"] = addressTo;
        //    table["transferAmount"] = transferAmount.ToString();
        //    table["gas"] = gas.ToString();
        //    table["gasPrice"] = gasPrice.ToString();
        //    table["nonce"] = nonce.Value.ToString();
        //    string result = Json.jsonEncode(table);
        //    if (callback != null)
        //    {
        //        callback(result);
        //    }
        //}
        //else
        //{
        //    if (callback != null)
        //    {
        //        callback(null);
        //    }
        //}
    }

    private static int GetLocalHistroyWaitCount(string ethAddress)
    {
        string[] local = PlayerPrefsX.GetStringArray(ethAddress + "LocalHistroy");
        string[] localKhyber = PlayerPrefsX.GetStringArray(ethAddress + "KyberHistroy");
        
        int noceCount = local.Length + localKhyber.Length;

        if (noceCount <= 0)
            return 0;

        string[] merge = local.Concat(localKhyber).ToArray();

        int overtimeCount = 0;

        for (int j = 0; j < merge.Length; j++)
        {
            Hashtable hs = Json.jsonDecode(merge[j]) as Hashtable;
            long time = long.Parse(hs["timeStamp"].ToString());
            TimeSpan cha = (DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)));
            long t = (long)cha.TotalSeconds;
            long tempTime = t - time;
            if (tempTime >= 3 * 60 * 60)
            {
                overtimeCount++;
            }
        }

        return noceCount - overtimeCount;
    }

    public static bool GetPayInfo_ETH(string data, out string sendToAddress, out string SendFromAddress, out double outAmount, out double outMinnerFee, string pubkey)
    {
        try
        {
            SendFromAddress = "";
            sendToAddress = "";
            outAmount = 0;
            outMinnerFee = 0;
            Hashtable table = Json.jsonDecode(data) as Hashtable;
            SendFromAddress = table["addressFrom"].ToString();
            if (pubkey != SendFromAddress)
            {
                return false;
            }
            sendToAddress = table["addressTo"].ToString();
            System.Numerics.BigInteger transferAmount = System.Numerics.BigInteger.Parse(table["transferAmount"].ToString());
            System.Numerics.BigInteger gas = System.Numerics.BigInteger.Parse(table["gas"].ToString());
            System.Numerics.BigInteger gasPrice = System.Numerics.BigInteger.Parse(table["gasPrice"].ToString());
            System.Numerics.BigInteger nonce = System.Numerics.BigInteger.Parse(table["nonce"].ToString());

            outAmount = (double)Nethereum.Util.UnitConversion.Convert.FromWei(transferAmount);
            outMinnerFee = (double)Nethereum.Util.UnitConversion.Convert.FromWei(gas * gasPrice);
            return true;
        }
        catch
        {
            SendFromAddress = "";
            sendToAddress = "";
            outAmount = 0;
            outMinnerFee = 0;
            return false;
        }
    }

    #endregion

    /// <summary>
    /// sign any data
    /// </summary>
    /// <param name="data"></param>
    /// <param name="privateKey"></param>
    /// <returns></returns>
    public static string SignAllWithNoneCoin(string data, byte[] privateKey)
    {
        try
        {
            EthECKey pvt = new EthECKey(privateKey, true);
            byte[] hash = data.HexToByteArray();
            EthECDSASignature signature = pvt.SignAndCalculateV(hash);
            string output = signature.R.ToHex() + "&" + signature.S.ToHex() + "&" + (int)signature.V;
            return output;
        }
        catch
        {
            return "";
        }
    }

    /// <summary>
    /// sign any data
    /// </summary>
    /// <param name="data"></param>
    /// <param name="privateKey"></param>
    /// <returns></returns>
    public static string SignAllWithNoneCoinA(string data, byte[] privateKey)
    {
        try
        {
            ECKey pvt = new ECKey(privateKey, true);
            byte[] hash = data.HexToByteArray();
            Nethereum.Signer.Crypto.ECDSASignature signature = pvt.Sign(hash);
            string output = signature.ToDER().ToHex();
            return output;
        }
        catch(Exception e)
        {
            return "";
        }
    }

    #region EOS
    /// <summary>
    /// 
    /// </summary>
    /// <param name="data">sha256 hex string</param>
    /// <param name="privateKey">pvt byte[]</param>
    /// <returns></returns>
    public static string SignHashEOS(string data, byte[] privateKey)
    {
        try
        {
            ECKey pvt = new ECKey(privateKey, true);
            byte[] hash = data.HexToByteArray();
            int nonce = 0;
            Nethereum.Signer.Crypto.ECDSASignature signature;
            var N_OVER_TWO = ECKey.Secp256k1.N.ShiftRight(1);
            byte[] der = null;
            var lenR = 0;
            var lenS = 0;
            while (true)
            {
                if (nonce == 0)
                {
                    signature = pvt.Sign(hash);
                    if (signature.S.CompareTo(N_OVER_TWO) > 0)
                    {
                        Org.BouncyCastle.Math.BigInteger s = ECKey.Secp256k1.N.Subtract(signature.S);
                        Org.BouncyCastle.Math.BigInteger r = signature.R;
                        signature = new Nethereum.Signer.Crypto.ECDSASignature(r, s);
                    }
                    nonce++;
                    der = signature.ToDER();
                    lenR = der[3];
                    lenS = der[5 + lenR];
                    if (lenR == 32 && lenS == 32)
                    {
                        break;
                    }
                    continue;
                }
                byte[] newHash = new byte[hash.Length + nonce];
                Array.Copy(hash, 0, newHash, 0, hash.Length);
                for (int i = hash.Length; i < newHash.Length; i++)
                {
                    newHash[i] = 0;
                }
                newHash = Hashes.SHA256(newHash);
                ECDsaSignerMine ecdmine = new ECDsaSignerMine();
                ecdmine.setPrivateKey(pvt.PrivateKey);
                signature = ecdmine.signHash(newHash, hash);

                if (signature.S.CompareTo(N_OVER_TWO) > 0)
                {
                    Org.BouncyCastle.Math.BigInteger s = ECKey.Secp256k1.N.Subtract(signature.S);
                    Org.BouncyCastle.Math.BigInteger r = signature.R;
                    signature = new Nethereum.Signer.Crypto.ECDSASignature(r, s);
                }
                //Debug.Log(newHash.ToHex());
                //Debug.Log("s-- " + signature.S.ToByteArrayUnsigned().ToHex());
                nonce++;
                der = signature.ToDER();
                lenR = der[3];
                lenS = der[5 + lenR];
                if (lenR == 32 && lenS == 32)
                {
                    break;
                }
            }
            
            
            //Debug.Log("nonce" + nonce);
            //Debug.Log(signature.R.ToByteArrayUnsigned().ToHex());
            //Debug.Log(signature.S.ToByteArrayUnsigned().ToHex());
            var recId = CalculateRecId(signature, hash, pvt);
            signature.V = (byte)(recId + 27);

            byte[] result = new byte[65];
            result[0] = (byte)(signature.V + 4);
            Array.Copy(signature.R.ToByteArrayUnsigned(), 0, result, 1, 32);
            Array.Copy(signature.S.ToByteArrayUnsigned(), 0, result, 33, 32);

            /*encode*/
            byte[] encode_type = System.Text.UTF8Encoding.UTF8.GetBytes("K1");
            byte[] check_encode = new byte[result.Length + encode_type.Length];
            Array.Copy(result, 0, check_encode, 0, result.Length);
            Array.Copy(encode_type, 0, check_encode, result.Length, encode_type.Length);
            var _checksum = Hashes.RIPEMD160(check_encode, check_encode.Length);

            byte[] base58sign = new byte[result.Length + 4];
            Array.Copy(result, 0, base58sign, 0, result.Length);
            Array.Copy(_checksum, 0, base58sign, result.Length, 4);
            Base58Encoder encoder = new Base58Encoder();
            string output = "SIG_K1_" + encoder.EncodeData(base58sign);
            return output;
        }
        catch
        {
            return null;
        }
    }

    static int CalculateRecId(Nethereum.Signer.Crypto.ECDSASignature signature, byte[] hash, ECKey _ecKey)
    {
        var recId = -1;
        var thisKey = _ecKey.GetPubKey(false); // compressed

        for (var i = 0; i < 4; i++)
        {
            var rec = ECKey.RecoverFromSignature(i, signature, hash, false);
            if (rec != null)
            {
                var k = rec.GetPubKey(false);
                if (k != null && k.SequenceEqual(thisKey))
                {
                    recId = i;
                    break;
                }
            }
        }
        if (recId == -1)
            throw new Exception("Could not construct a recoverable key. This should never happen.");
        return recId;
    }

    public static bool GetPayInfo_EOS(string data, out string sendToAddress, out string SendFromAddress, out double outAmount, out double outMinnerFee, string pubkey)
    {
        try
        {
            SendFromAddress = "";
            sendToAddress = "";
            outAmount = 0;
            outMinnerFee = 0;
            Hashtable table = Json.jsonDecode(data) as Hashtable;
            SendFromAddress = table["addressFrom"].ToString();
            if (pubkey != SendFromAddress)
            {
                return false;
            }
            sendToAddress = table["addressTo"].ToString();
            System.Numerics.BigInteger transferAmount = System.Numerics.BigInteger.Parse(table["transferAmount"].ToString());
            System.Numerics.BigInteger gas = System.Numerics.BigInteger.Parse(table["gas"].ToString());
            System.Numerics.BigInteger gasPrice = System.Numerics.BigInteger.Parse(table["gasPrice"].ToString());
            System.Numerics.BigInteger nonce = System.Numerics.BigInteger.Parse(table["nonce"].ToString());

            outAmount = (double)Nethereum.Util.UnitConversion.Convert.FromWei(transferAmount);
            outMinnerFee = (double)Nethereum.Util.UnitConversion.Convert.FromWei(gas * gasPrice);
            return true;
        }
        catch
        {
            SendFromAddress = "";
            sendToAddress = "";
            outAmount = 0;
            outMinnerFee = 0;
            return false;
        }
    }
    #endregion

    #region bip
    /// <summary>
    /// chinese bip to english
    /// </summary>
    /// <param name="bip"></param>
    /// <returns></returns>
    public static string ChangeBipLanguageToSpChinese(string bip)
    {
        try
        {
            var result = "";
            Mnemonic mnemo = new Mnemonic(bip, Wordlist.English);
            string[] chineseBip = Wordlist.ChineseSimplified.GetWords(mnemo.Indices);
            for (int i = 0; i < chineseBip.Length; i++)
            {
                result += chineseBip[i];
                if (i != chineseBip.Length - 1)
                {
                    result += " ";
                }
            }
            return result;
        }
        catch
        {
            return "";
        }
    }

    /// <summary>
    /// english bip to chinese
    /// </summary>
    /// <param name="bip"></param>
    /// <returns></returns>
    public static string ChangeBipLanguageToSpEnglish(string bip)
    {
        try
        {
            var result = "";
            Mnemonic mnemo = new Mnemonic(bip, Wordlist.ChineseSimplified);
            string[] chineseBip = Wordlist.English.GetWords(mnemo.Indices);
            for (int i = 0; i < chineseBip.Length; i++)
            {
                result += chineseBip[i];
                if (i != chineseBip.Length - 1)
                {
                    result += " ";
                }
            }
            return result;
        }
        catch
        {
            return ""; 
        }
    }

    public static string ChangeBipLanguageToSpNumber(string bip)//汉字转换为数字
    {
        try
        {
            var result = "";
            Mnemonic mnemo = new Mnemonic(bip, Wordlist.ChineseSimplified);
            for (int i = 0; i < mnemo.Indices.Length; i++)
            {
                result += mnemo.Indices[i].ToString("0000");
                if (i != mnemo.Indices.Length - 1)
                {
                    result += " ";
                }
            }
            return result;
        }
        catch
        {
            return "";
        }
    }


    public static string ChangeBipNumberToSpChinese(string bip)//数字转换成汉字
    {
        try
        {
            var result = "";
            string[] strArr = bip.Split(' ');
            int[] intArr = new int[strArr.Length];
            for (int i = 0; i < intArr.Length; i++)
            {
                intArr[i] = int.Parse(strArr[i]);
            }
            string[] chineseBip = Wordlist.ChineseSimplified.GetWords(intArr);
            for (int i = 0; i < chineseBip.Length; i++)
            {
                result += chineseBip[i];
                if (i != chineseBip.Length - 1)
                {
                    result += " ";
                }
            }
            return result;
        }
        catch
        {
            return "";
        }
    }

    public static ExtPubKey GetMastPubKey(string bip)
    {
        Mnemonic mnemo = new Mnemonic(bip, Wordlist.ChineseSimplified);
        var Seed = mnemo.DeriveSeed().ToHex();
        var masterKey = new ExtKey(Seed);
        return masterKey.Neuter();
    }

    /// <summary>
    /// 按照路径获取公钥
    /// </summary>
    /// <param name="masterKeyHex"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetPubKeyByPath(string masterKeyHex, string path)
    {
        try
        {
            var masterPubKey = new ExtPubKey(masterKeyHex.HexToByteArray());
            KeyPath keyPath = new KeyPath(path);
            return masterPubKey.Derive(keyPath).PubKey.ToHex();
        }
        catch
        {
            return null;
        }
    }

    public static byte[] GetPrivateKeyByPath(string bip, string path)
    {
        try
        {
            Mnemonic mnemo = new Mnemonic(bip, Wordlist.ChineseSimplified);
            var Seed = mnemo.DeriveSeed().ToHex();
            var masterKey = new ExtKey(Seed);
            KeyPath keyPath = new KeyPath(path);
            ExtKey key = masterKey.Derive(keyPath);
            return key.PrivateKey.ToBytes();
        }
        catch
        {
            return null;
        }
    }

    public static ExtKey GetMnemonic(string bip, WalletType type = WalletType.BTC)
    {
        Mnemonic mnemo = new Mnemonic(bip, Wordlist.ChineseSimplified);
        var Seed = mnemo.DeriveSeed().ToHex();
        var masterKey = new ExtKey(Seed);
        KeyPath keyPath;
        switch (type)
        {
            case WalletType.ETH:
                keyPath = new KeyPath(Bip44ETHPath);
                break;
            case WalletType.BTC:
                keyPath = new KeyPath(Bip44BTCPath);
                break;
            case WalletType.EOS:
                keyPath = new KeyPath(Bip44EOSPath);
                break;
            default:
                keyPath = new KeyPath(Bip44BTCPath);
                break;
        }
        ExtKey key = masterKey.Derive(keyPath);
        return key;
    }

    public static string CreateBipString()
    {
        Mnemonic mnemo = new Mnemonic(Wordlist.ChineseSimplified, WordCount.Twelve);
        string[] bip = mnemo.Words;
        string temp = "";
        for (int i = 0; i < bip.Length; i++)
        {
            temp += bip[i];
            if (i != bip.Length - 1)
            {
                temp += " ";
            }
        }
        if (VerifyBip(temp))
        {
            return temp;
        }
        else
        {
            return null;
        }
    }

    public static bool VerifyBip(string bip)
    {
        try
        {
            Mnemonic mnemo = new Mnemonic(bip, Wordlist.ChineseSimplified);
            return mnemo.IsValidChecksum;
        }
        catch
        {
            return false;
        }
    }

    #endregion
    public static string ToHexString(byte[] bytes)
    {
        string hexString = string.Empty;
        if (bytes != null)
        {
            StringBuilder strB = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                strB.Append(bytes[i].ToString("X2"));
            }
            hexString = strB.ToString();
        }
        return hexString;
    }

    public static byte[] HexToByte(string hex)
    {
        byte[] buffer = new byte[hex.Length / 2];
        for (int i = 0; i < hex.Length; i += 2)
        {
            buffer[i / 2] = (byte)Convert.ToByte(hex.Substring(i, 2), 16);
        }
        return buffer;
    }
}

public class ECDsaSignerMine : ECDsaSigner
{
    private readonly IDigest _digest;
    private byte[] _buffer = new byte[0];

    public ECDsaSignerMine()
            : base(new HMacDsaKCalculator(new Sha256Digest()))

        {
        _digest = new Sha256Digest();
    }

    public ECDsaSignerMine(Func<IDigest> digest)
            : base(new HMacDsaKCalculator(digest()))
        {
        _digest = digest();
    }


    public void setPrivateKey(ECPrivateKeyParameters ecKey)
    {
        Init(true, ecKey);
    }

    public Nethereum.Signer.Crypto.ECDSASignature signHash(byte[] hash, byte[] rootMessage)
    {
        return new Nethereum.Signer.Crypto.ECDSASignature(GenerateSignature2(hash, rootMessage));
    }

    public BigInteger[] GenerateSignature2(byte[] message, byte[] rootmessage)
    {

        ECDomainParameters parameters = key.Parameters;
        BigInteger n = parameters.N;
        BigInteger e = CalculateE(n, rootmessage);
        BigInteger d = ((ECPrivateKeyParameters)key).D;
        if (kCalculator.IsDeterministic)
        {
            kCalculator.Init(n, d, message);
        }
        else
        {
            kCalculator.Init(n, random);
        }
        ECMultiplier eCMultiplier = CreateBasePointMultiplier();
        BigInteger bigInteger3;
        BigInteger bigInteger4;
        while (true)
        {
            BigInteger bigInteger2 = kCalculator.NextK();
            ECPoint eCPoint = eCMultiplier.Multiply(parameters.G, bigInteger2).Normalize();
            bigInteger3 = eCPoint.AffineXCoord.ToBigInteger().Mod(n);
            if (bigInteger3.SignValue != 0)
            {
                bigInteger4 = bigInteger2.ModInverse(n).Multiply(e.Add(d.Multiply(bigInteger3))).Mod(n);
                if (bigInteger4.SignValue != 0)
                {
                    break;
                }
            }
        }
        return new BigInteger[2]
        {
        bigInteger3,
        bigInteger4
        };
    }

    public void update(byte[] buf)
    {
        _buffer = _buffer.Concat(buf).ToArray();
    }

    private BigInteger calculateE(
            BigInteger n,
            byte[] message)
    {
        int messageBitLength = message.Length * 8;
        BigInteger trunc = new BigInteger(1, message);
        if (n.BitLength < messageBitLength)
        {
            trunc = trunc.ShiftRight(messageBitLength - n.BitLength);
        }
        return trunc;
    }
}

public class BTCMulSig
{
    public static string CreateMulSigAddress(int minPro, List<PubKey> pubkeys)
    {
        if (minPro > pubkeys.Count)
        {
            return null;
        }
        try
        {
            Script mulSig = PayToMultiSigTemplate.Instance.GenerateScriptPubKey(minPro, pubkeys.ToArray());
            return mulSig.GetScriptAddress(NBitcoin.Network.Main).ToString();
        }
        catch(Exception e)
        {
            Debug.LogError(e.Message);
            return null;
        }
    }

    public class MulSigTransactionBuilder
    {
        NBitcoin.Transaction tx;
        TransactionBuilder builderCtx;
        Coin[] corpCoins;
        public MulSigTransactionBuilder(List<PubKey> pubkeys, int minPro, Coin[] corpCoins, string to, Money amount, Money fees, BitcoinAddress fromAddress)
        {
            Script mulSig = PayToMultiSigTemplate.Instance.GenerateScriptPubKey(minPro, pubkeys.ToArray());
            List<ScriptCoin> coins = new List<ScriptCoin>();
            for (int i = 0; i < corpCoins.Length; i++)
            {
                coins.Add(corpCoins[i].ToScriptCoin(mulSig));
            }
            var coinsAdd = coins.ToArray();
            var builder = new TransactionBuilder();
            builder.DustPrevention = false;
            builder.SetChange(fromAddress)
                .Send(BitcoinAddress.Create(to).ScriptPubKey, amount)
                .AddCoins(coinsAdd)
                .SendFees(fees);
            tx = builder.BuildTransaction(false);
            builderCtx = new TransactionBuilder();
            builderCtx.DustPrevention = false;
            builderCtx.SetChange(fromAddress)
                .Send(BitcoinAddress.Create(to).ScriptPubKey, amount)
                .AddCoins(coinsAdd)
                .SendFees(fees);
            this.corpCoins = coinsAdd;
        }

        public List<string> GetSignatureHash()
        {
            List<string> result = new List<string>();
            foreach (var v in tx.Inputs.AsIndexedInputs())
            {
                var hex = v.GetSignatureHash(corpCoins[v.Index]).ToBytes().ToHex();
                result.Add(hex);
            }
            return result;
        }
        
        public MulSigTransactionBuilder AddSig(PubKey pubKey, string data)
        {
            builderCtx.AddKnownSignature(pubKey,
                new TransactionSignature(NBitcoin.Crypto.ECDSASignature.FromDER(Encoders.Hex.DecodeData(data))));
            return this;
        }

        public NBitcoin.Transaction Finish()
        {
            tx = builderCtx.SignTransactionInPlace(tx);
            NBitcoin.Policy.TransactionPolicyError[] errors;
            if (builderCtx.Verify(tx, out errors))
            {
                Debug.Log(tx.Outputs.Count);
                return tx;
            }
            foreach (NBitcoin.Policy.TransactionPolicyError v in errors)
            {
                Debug.Log(v.ToString());
                PopupLine.Instance.Show(v.ToString());
            }
            return null;
        }
    }

    /// <summary>
    /// test rjj
    /// usdt get transaction
    /// modified for cold 
    /// </summary>
    public class USDTTransactionBuilder
    {
        NBitcoin.Transaction tx;
        TransactionBuilder builderCtx;
        Dictionary<Coin, HistoryManager.InputVo> dic;
        public USDTTransactionBuilder(List<HistoryManager.InputVo> inputVos)
        {
            Debug.Log("USDTTransactionBuilder");
            dic = new Dictionary<Coin, HistoryManager.InputVo>();
            Coin[] corpCoins = new Coin[inputVos.Count];
            GetCoins(inputVos);
        }

        public List<HistoryManager.InputVo> GetSignatureHash()
        {
            Debug.Log("GetSignatureHash");
            List<HistoryManager.InputVo> result = new List<HistoryManager.InputVo>();
            IndexedTxIn txIn = new IndexedTxIn();
            List<Coin> corpCoins = new List<Coin>();
            foreach (var v in dic)
            {
               corpCoins.Add(v.Key);
            }
            //var coinsAdd = corpCoins.ToArray();
            //var builder = new TransactionBuilder();
            //builder.DustPrevention = false;
            //builder.AddCoins(coinsAdd);
            //tx = builder.BuildTransaction(false);
            //builderCtx = new TransactionBuilder();
            //builderCtx.DustPrevention = false;
            //builderCtx.AddCoins(coinsAdd);
            var transaction = new NBitcoin.Transaction();
            //input
            //List<Coin> coinWillSpend = new List<Coin>();
            //foreach (var v in spendCointsInHardware)
            //{
            //    Hashtable item = (Hashtable)v;
            //    OutPoint outPoint = OutPoint.Parse(item["outPoint"].ToString());
            //    var scriptPubKey = new Script(NBitcoin.DataEncoders.Encoders.Hex.DecodeData(item["scriptPubKey"].ToString()));
            //    TxOut txOut = new TxOut(new Money(long.Parse(item["value"].ToString())), scriptPubKey);
            //    Coin coin = new Coin(outPoint, txOut);
            //    coinWillSpend.Add(coin);
            //}
            foreach (var v in corpCoins)
            {
                transaction.Inputs.Add(new TxIn()
                {
                    PrevOut = v.Outpoint
                });
            }
            List<string> resultstr = new List<string>();
            foreach (var v in transaction.Inputs.AsIndexedInputs())
            {
                
                var hex = v.GetSignatureHash(corpCoins[(int)v.Index]).ToBytes().ToHex();
                Debug.Log("hex:"+ hex);
                resultstr.Add(hex);
            }
            for (int i = 0; i < dic.Count; i++)
            {
                HistoryManager.InputVo newOne = dic[corpCoins[i]];
                newOne.signature = resultstr[i];
                Debug.Log("newOne.signature:" + newOne.signature);
                result.Add(newOne);
            }
            return result;
        }
        private void GetCoins(List<HistoryManager.InputVo> inputVos)
        {
            Debug.Log("GetCoins");
            for (int i = 0; i < inputVos.Count; i++)
            {
                QBitNinja4Unity.Models.CoinJson cj = new QBitNinja4Unity.Models.CoinJson();
                object tempTransactionId =inputVos[i].txid;
                if (tempTransactionId != null)
                    cj.transactionId = (string)tempTransactionId;
                cj.index = (uint)inputVos[i].index;
                cj.value = long.Parse(inputVos[i].value.ToString());
                object tempScripPk = inputVos[i].scriptPubKey;
                if (tempScripPk != null)
                    cj.scriptPubKey = (string)tempScripPk;
                cj.redeemScript = "";
                this.dic.Add(Create(cj) as Coin, inputVos[i]);
            }
           
        }

        private ICoin Create(QBitNinja4Unity.Models.CoinJson coin)
        {
            var transactionId = uint256.Parse(coin.transactionId);
            var scriptPubKey = new Script(NBitcoin.DataEncoders.Encoders.Hex.DecodeData(coin.scriptPubKey));
            var redeemScript = coin.redeemScript.Length == 0 ? null : new Script(NBitcoin.DataEncoders.Encoders.Hex.DecodeData(coin.redeemScript));
            var iCoin = redeemScript == null ?
                                    new Coin(new OutPoint(transactionId, coin.index), new TxOut(coin.value, scriptPubKey)) :
                                    new ScriptCoin(new OutPoint(transactionId, coin.index), new TxOut(coin.value, scriptPubKey), redeemScript);

            return iCoin;
        }
    }
}
