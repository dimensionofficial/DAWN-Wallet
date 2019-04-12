using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NBitcoin;
using NBitcoin.Crypto;
using System;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Signer;
using Nethereum.Util;
using NBitcoin.DataEncoders;
using Nethereum.JsonRpc.UnityClient;
using Nethereum.Signer.Crypto;

namespace HardwareWallet
{
    public interface IWalletDataBase
    {
        string CreateSignedPayString(string data, string pincode);
        string CreateAddressString(string pincode);
        bool GetPayInfo(string data, string pubKey, out List<string> sendToAddress,
            out string SendFromAddress,
            out double outAmount,
            out double outMinnerFee);
        string GetBipWords(string pincode);
        string keyPath { get; set; }
        string signMethod { get; set; }
        string data { get; set; }
    }

    public class BTCWalletData : IWalletDataBase
    {
        public string keyPath
        {
            get;set;
        }

        public string signMethod
        {
            get;set;
        }

        public string data
        {
            get;set;
        }

        public string GetBipWords(string pincode)
        {
            string bip = SaveAndLoad.LoadString(FlowManager.BIPKEYWORDS, System.Text.UTF8Encoding.UTF8.GetBytes(pincode));
            if (bip == null || !QRPayTools.VerifyBip(bip))
            {
                PopBox.Instance.ShowMsg("cant read bip from local");
                return null;
            }
            return bip;
        }

        public string CreateSignedPayString(string data, string pincode)
        {
            string bip = SaveAndLoad.LoadString(FlowManager.BIPKEYWORDS, System.Text.UTF8Encoding.UTF8.GetBytes(pincode));
            if (bip == null || !QRPayTools.VerifyBip(bip))
            {
                PopBox.Instance.ShowMsg("cant read bip from local");
                return null;
            }
            var mainNetPrivateKey = new NBitcoin.BitcoinSecret(QRPayTools.GetMnemonic(bip).PrivateKey, FlowManager.Instance.network);
            var changeScriptPubKey = mainNetPrivateKey.PubKey.GetAddress(FlowManager.Instance.network);
            return QRPayTools.CreateSignedPayWithoutBuilder(data, mainNetPrivateKey, FlowManager.Instance.network);
        }

        public string CreateAddressString(string pincode)
        {
            string bip = SaveAndLoad.LoadString(FlowManager.BIPKEYWORDS, System.Text.UTF8Encoding.UTF8.GetBytes(pincode));
            if (bip == null || !QRPayTools.VerifyBip(bip)) 
            {
                PopBox.Instance.ShowMsg("cant read bip from local");
                return null;
            }
            var mainNetPrivateKey = new NBitcoin.BitcoinSecret(QRPayTools.GetMnemonic(bip).PrivateKey, FlowManager.Instance.network);
            var changeScriptPubKey = mainNetPrivateKey.PubKey.GetAddress(FlowManager.Instance.network);
            string add = changeScriptPubKey.ToWif();
            return changeScriptPubKey.ToWif();
        }

        public bool GetPayInfo(string data, string pubkey, out List<string> sendToAddress,
            out string SendFromAddress,
            out double outAmount,
            out double outMinnerFee)
        {
            //string bip = SaveAndLoad.LoadString(FlowManager.BIPKEYWORDS, System.Text.UTF8Encoding.UTF8.GetBytes(pincode));
            sendToAddress = new List<string>();
            SendFromAddress = "";
            outAmount = 0;
            outMinnerFee = 0;
            //if (bip == null || !QRPayTools.VerifyBip(bip))
            //{
            //    PopBox.Instance.ShowMsg("cant read bip from local");
            //    return false;
           // }
            //var mainNetPrivateKey = new NBitcoin.BitcoinSecret(QRPayTools.GetMnemonic(bip), FlowManager.Instance.network);
            //var changeScriptPubKey = mainNetPrivateKey.PubKey.GetAddress(FlowManager.Instance.network);
            return QRPayTools.GetPayInfo(data, 
                out sendToAddress, 
                out SendFromAddress, 
                out outAmount, 
                out outMinnerFee,
                new BitcoinPubKeyAddress(pubkey, FlowManager.Instance.network),
                FlowManager.Instance.network);
        }
    }

    public class ETHWalletData : IWalletDataBase
    {
        public string data
        {
            get; set;
        }
        public string keyPath
        {
            get;set;
        }

        public string signMethod
        {
            get;set;
        }

        public string GetBipWords(string pincode)
        {
            string bip = SaveAndLoad.LoadString(FlowManager.BIPKEYWORDS, System.Text.UTF8Encoding.UTF8.GetBytes(pincode));
            if (bip == null || !QRPayTools.VerifyBip(bip))
            {
                PopBox.Instance.ShowMsg("cant read bip from local");
                return null;
            }
            return bip;
        }

        public string CreateSignedPayString(string data, string pincode)
        {
            string bip = SaveAndLoad.LoadString(FlowManager.BIPKEYWORDS, System.Text.UTF8Encoding.UTF8.GetBytes(pincode));
            if (bip == null || !QRPayTools.VerifyBip(bip))
            {
                PopBox.Instance.ShowMsg("cant read bip from local");
                return null;
            }
            var ethEck = new EthECKey(QRPayTools.GetMnemonic(bip, WalletType.ETH).PrivateKey.ToBytes(), true);
            string publicKey = ethEck.GetPublicAddress();
            string privateKey = ethEck.GetPrivateKey();
            //string spend = QRPayTools.CreateSignedPayWithoutBuilder_ETH(data, privateKey);
            //FlowManager.Instance.gameObject.GetComponent<MonoBehaviour>().StartCoroutine(Spend(spend));
            return QRPayTools.CreateSignedPayWithoutBuilder_ETH(data, privateKey);
        }

        IEnumerator Spend(string spendx)
        {
            var _ethSendTransactionRequest = new EthSendRawTransactionUnityRequest("https://mainnet.infura.io");
            yield return _ethSendTransactionRequest.SendRequest(spendx);
            if (_ethSendTransactionRequest.Exception == null)
            {
                Debug.Log(_ethSendTransactionRequest.Result);
            }
            else
            {
                Debug.Log(_ethSendTransactionRequest.Exception.Message);
                yield break;
            }
        }

        public string CreateAddressString(string pincode)
        {
            string bip = SaveAndLoad.LoadString(FlowManager.BIPKEYWORDS, System.Text.UTF8Encoding.UTF8.GetBytes(pincode));
            if (bip == null || !QRPayTools.VerifyBip(bip))
            {
                PopBox.Instance.ShowMsg("cant read bip from local");
                return null;
            }
            var ethEck = new EthECKey(QRPayTools.GetMnemonic(bip, WalletType.ETH).PrivateKey.ToBytes(), true);
            string publicKey = ethEck.GetPublicAddress();
            string privateKey = ethEck.GetPrivateKey();
            return publicKey;
        }
        public bool GetPayInfo(string data, string pubKey, out List<string> sendToAddress,
                    out string SendFromAddress,
                    out double outAmount,
                    out double outMinnerFee)
        {
            sendToAddress = new List<string>();
            SendFromAddress = "";
            outAmount = 0;
            outMinnerFee = 0;
            string sendToAddressItem = "";
            bool r = QRPayTools.GetPayInfo_ETH(data,
               out sendToAddressItem,
               out SendFromAddress,
               out outAmount,
               out outMinnerFee, pubKey);
            if (r)
            {
                sendToAddress.Add(sendToAddressItem);
                return true;
            }
            return false;
        }
    }

    public class EOSWalletData : IWalletDataBase
    {
        public string data
        {
            get; set;
        }
        public string keyPath
        {
            get;set;
        }

        public string signMethod
        {
            get;set;
        }

        public string GetBipWords(string pincode)
        {
            string bip = SaveAndLoad.LoadString(FlowManager.BIPKEYWORDS, System.Text.UTF8Encoding.UTF8.GetBytes(pincode));
            if (bip == null || !QRPayTools.VerifyBip(bip))
            {
                PopBox.Instance.ShowMsg("cant read bip from local");
                return null;
            }
            return bip;
        }

        public string CreateSignedPayString(string data, string pincode)
        {
            try
            {
                string bip = GetBipWords(pincode);
                byte[] privateKey = QRPayTools.GetPrivateKeyByPath(bip, keyPath);
                return QRPayTools.SignHashEOS(data, privateKey);
            }
            catch
            {
                return null;
            }
            /*
            ECKey ecKey = new ECKey(QRPayTools.GetMnemonic(bip, WalletType.EOS).PrivateKey.ToBytes(), true);
            var privateKey = QRPayTools.GetMnemonic(bip, WalletType.EOS).PrivateKey.ToBytes();
            var private_key = new byte[privateKey.Length + 1];
            Array.Copy(privateKey, 0, private_key, 1, privateKey.Length);
            private_key[0] = 0x80;
            var checksum = Hashes.SHA256(private_key);
            checksum = Hashes.SHA256(checksum);
            var private_wif = new byte[private_key.Length + 4];
            Array.Copy(private_key, 0, private_wif, 0, private_key.Length);
            Array.Copy(checksum, 0, private_wif, private_key.Length, 4);
            string wif = Encoders.Base58.EncodeData(private_wif);

            byte[] key = Hashes.SHA256(System.Text.UTF8Encoding.UTF8.GetBytes(pincode));
            byte[] result = Encry.EncryData(System.Text.UTF8Encoding.UTF8.GetBytes(wif), key);
            return QRPayTools.ToHexString(result);
            */
        }

        public string CreateAddressString(string pincode)
        {
            string bip = SaveAndLoad.LoadString(FlowManager.BIPKEYWORDS, System.Text.UTF8Encoding.UTF8.GetBytes(pincode));
            if (bip == null || !QRPayTools.VerifyBip(bip))
            {
                PopBox.Instance.ShowMsg("cant read bip from local");
                return null;
            }
            ECKey ecKey = new ECKey(QRPayTools.GetMnemonic(bip, WalletType.EOS).PrivateKey.ToBytes(), true);
            var pub_buf = ecKey.GetPubKey(true);
            var checksum = Hashes.RIPEMD160(pub_buf, pub_buf.Length);
            var addy = new byte[pub_buf.Length + 4];
            Array.Copy(pub_buf, 0, addy, 0, pub_buf.Length);
            Array.Copy(checksum, 0, addy, pub_buf.Length, 4);
            string address = "EOS" + Encoders.Base58.EncodeData(addy);
            return address;
        }

        public string CreateAddressStringWithPrivateKey(string pvtHex)
        {
            byte[] pvt = pvtHex.HexToByteArray();
            ECKey ecKey = new ECKey(pvt, true);
            var pub_buf = ecKey.GetPubKey(true);
            var checksum = Hashes.RIPEMD160(pub_buf, pub_buf.Length);
            var addy = new byte[pub_buf.Length + 4];
            Array.Copy(pub_buf, 0, addy, 0, pub_buf.Length);
            Array.Copy(checksum, 0, addy, pub_buf.Length, 4);
            string address = "EOS" + Encoders.Base58.EncodeData(addy);
            return address;
        }

        public bool GetPayInfo(string data, string pubKey, out List<string> sendToAddress,
                    out string SendFromAddress,
                    out double outAmount,
                    out double outMinnerFee)
        {
            sendToAddress = new List<string>();
            SendFromAddress = "";
            outAmount = 0;
            outMinnerFee = 0;
            string sendToAddressItem = "";
            bool r = QRPayTools.GetPayInfo_EOS(data,
               out sendToAddressItem,
               out SendFromAddress,
               out outAmount,
               out outMinnerFee, pubKey);
            if (r)
            {
                sendToAddress.Add(sendToAddressItem);
                return true;
            }
            return false;
        }
    }

    public class OtherWalletData : IWalletDataBase
    {
        public string data
        {
            get; set;
        }
        private string path;
        private string method;
        public string keyPath
        {
            get
            {
                return path;
            }

            set
            {
                path = value;
            }
        }

        public string signMethod
        {
            get
            {
                return method;
            }

            set
            {
                method = value;
            }
        }

        public string CreateAddressString(string pincode)
        {
            throw new NotImplementedException();
        }

        public string CreateSignedPayString(string data, string pincode)
        {
            try
            {
                string bip = GetBipWords(pincode);
                byte[] privateKey = QRPayTools.GetPrivateKeyByPath(bip, keyPath);
                if (signMethod == "OT1")
                {
                    return QRPayTools.SignAllWithNoneCoin(data, privateKey);
                }
                else
                {
                    return QRPayTools.SignAllWithNoneCoinA(data, privateKey);
                }
            }
            catch
            {
                return null;
            }
        }

        public string GetBipWords(string pincode)
        {
            string bip = SaveAndLoad.LoadString(FlowManager.BIPKEYWORDS, System.Text.UTF8Encoding.UTF8.GetBytes(pincode));
            if (bip == null || !QRPayTools.VerifyBip(bip))
            {
                PopBox.Instance.ShowMsg("cant read bip from local");
                return null;
            }
            return bip;
        }

        public bool GetPayInfo(string data, string pubKey, out List<string> sendToAddress, out string SendFromAddress, out double outAmount, out double outMinnerFee)
        {
            throw new NotImplementedException();
        }
    }

    public class WalletDataFactory
    {
        public static IWalletDataBase CreateWalletData(WalletType type, string data = "", string keyPath = "", string method = "")
        {
            IWalletDataBase result = null;
            switch (type)
            {
                case WalletType.BTC:
                    result= new BTCWalletData();
                    break;
                case WalletType.ETH:
                    result= new ETHWalletData();
                    break;
                case WalletType.EOS:
                    result =  new EOSWalletData();
                    break;
                default:
                    result = new OtherWalletData();
                    break;
            }
            result.data = data;
            result.signMethod = method;
            result.keyPath = keyPath;
            return result;
        }

        
        //根据签名信息生成钱包信息
        public static IWalletDataBase CreateWalletData(string jsonData)
        {
            try
            {
                Hashtable data = Json.jsonDecode(jsonData) as Hashtable;
                string path = data["path"].ToString();
                KeyPath keyPath = new KeyPath(path);
                if (!path.StartsWith("m/44") || keyPath == null)
                {
                    throw new System.Exception("json error");
                }
                string sign = data["sign"].ToString();
                List<string> signType = new List<string>();
                signType.Add("BTC");
                signType.Add("ETH");
                signType.Add("EOS");
                signType.Add("OT1");
                signType.Add("OT2");
                if (!signType.Contains(sign))
                {
                    throw new System.Exception("json error");
                }
                string dataIn = data["data"].ToString();
                if (string.IsNullOrEmpty(dataIn))
                {
                    throw new System.Exception("json error");
                }
                dataIn = Encry.UnBase64String(dataIn);
                if (string.IsNullOrEmpty(dataIn))
                {
                    throw new System.Exception("json error");
                }
                switch (sign)
                {
                    case "BTC":
                        return CreateWalletData(WalletType.BTC, dataIn);
                    case "ETH":
                        return CreateWalletData(WalletType.ETH, dataIn);
                    case "EOS":
                        return CreateWalletData(WalletType.EOS, dataIn, path, sign);
                    default:
                        return CreateWalletData(WalletType.Other, dataIn, path, sign);
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
