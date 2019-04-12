using Nethereum.Hex.HexConvertors.Extensions;
using NBitcoin;
using Nethereum.Signer;
using NBitcoin.Crypto;
using NBitcoin.DataEncoders;
using System;
using HardwareWallet;
using UnityEngine;
using System.Collections;

[System.Serializable]
public class GoodsInfo
{
    public enum GoodsType
    {
        Noen,
        /// <summary>
        /// eos注册账户
        /// </summary>
        EOS_CVT,
    }

    /// <summary>
    /// 支付类型
    /// </summary>
    public enum PayType
    {
        None,
        BTC,
        EHT,
        ETHToken,
    }
    public int id;
    public GoodsType goodsType = GoodsType.Noen;
    public decimal price;
    public PayType payType = PayType.None;

    public string toAddress;

    public int tokenDecimal;
    public string contactAddress;
    public string symbol;
    public string fullName;

   
}

public class WalletTools
{
    public NBitcoin.Network network = NBitcoin.Network.Main;
    public const string btcPath = "m/44/0/0/0/0";
    public const string ethPath = "m/44/60/0/0/0";

    /// <summary>
    /// EOS拥有者路径
    /// </summary>
    public const string eosPath_owner = "m/44/194/0/0/0";
    /// <summary>
    /// EOS管理者路径
    /// </summary>
    public const string eospath_admin = "m/44/194/0/0/1";
    //EOS81GU1cAvBTVmToNrkRr8i4pnKXEaU7tRFmKJkTh7iqoCcMVoPy
    //EOS8399mt9XJEUaSesMfFcaB3rvrHw41TV3boEumpp7X8xpzS1FKw
    public void Test()
    {
        string t = "ODAwJFsiMC4wMDAwMDImMUFqdXRmTlRNWEx0cmIxS3dtQnY3VFFqbXg0R1ZHWHV5aCJdJFsiOGYxZmNlZTE3OGEwOGRmNjFlNTM5N2Y4NDNmMjdiYTc0MDJlN2RhYjJlMjkxNWNiYTQxZTIyMDZhYjdlZmFlOC0xJjc2YTkxNDZhZDZjMGRlOGI1NTA0MzhjNmJjYzhjYmVkY2EyOGI2YTA1ODQxZjg4OGFjJjEwMDAiLCAiZWQ3YzIzMGE4NGI1YmNiNWI5MzkxMmQzZDViMjQyZjNhMmMxODMxNjYxMzk1ZDA0ZjIxOGVmYjJmZThkMThiMC0wJjc2YTkxNDZhZDZjMGRlOGI1NTA0MzhjNmJjYzhjYmVkY2EyOGI2YTA1ODQxZjg4OGFjJjYyNyJd";

        string tt = Encry.UnBase64String(t);

        Debug.Log(QRPayTools.GetHashtableStr(tt));

        //string str = "7F6F32B1BD83E59707EF61B0848D769979E26837D9D162CDF787265411572FDFEC68B4C93BAEB189B38DE4E37792F24ACFEEB2965E56383DE4CCBD8791B728F611F0F0BC4D711CFD0B57C16F8DB255F2624D05B2BB55F294254673EC0E46D800559BE290F3B8451CA40F38E1790FAF54446D80F80E1A11C44D13A1143DB2F497CE3C20583F854087076D78C4441C76B82E77A13A5F58388725C9B9B53A5C94F0C0EFC5ED1F62E2B2EE73857E1DD5AACD55AE9A03B0FA0C1944195C6DA1BE055360496C07C7803C7F1066F8BBF991669F";

        //byte[] key = Hashes.SHA256(System.Text.UTF8Encoding.UTF8.GetBytes("fdsamcldi123sawqa"));
        //byte[] tempAddress = QRPayTools.HexToByte(str);
        //byte[] addressDecode = Encry.DecryData(tempAddress, key);
        //if (addressDecode != null && addressDecode.Length > 0)
        //{
        //    string address = System.Text.UTF8Encoding.UTF8.GetString(addressDecode);
        //    string[] scanInfos = address.Split('&');

        //   string btcAddress = GetBTCPubAddress(scanInfos[1]);
        //   string ethAddress =  GetETHPubAddress(scanInfos[1]);
        //   string eosAddress_owner = GetEOSPubAddress_owner(scanInfos[1]);
        //   string eosAddress_admin = GetEOSPubAddress_admin(scanInfos[1]);

        //    Hashtable h = new Hashtable();
        //    h["BTC"] = btcAddress;
        //    h["ETH"] = ethAddress;
        //    h["EOSowner"] = eosAddress_owner;
        //    h["EOSadmin"] = eosAddress_admin;
        //    h["walletName"] = "SN 1234";
        //    h["machineid"] = "fsdokfjlkasdjID";
        //    string jsonstring = Json.jsonEncode(h);
        //}
    }

    /// <summary>
    /// 按照路径获取公钥
    /// </summary>
    /// <param name="masterKeyHex"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    public string GetPubKeyByPath(string masterKeyHex, string path)
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

    /// <summary>
    /// 获取以太坊地址
    /// </summary>
    /// <param name="masterKeyHex"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    public string GetETHPubAddress(string masterKeyHex)
    {
        string key = GetPubKeyByPath(masterKeyHex, ethPath);
        if (key == null)
        {
            return null;
        }

        EthECKey ethEckey = new EthECKey(key.HexToByteArray(), false);
        string address = ethEckey.GetPublicAddress();
        return address;
    }

    /// <summary>
    /// 获取比特币地址
    /// </summary>
    /// <param name="masterKeyHex"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    public string GetBTCPubAddress(string masterKeyHex)
    {
        string key = GetPubKeyByPath(masterKeyHex, btcPath);
        if (key == null)
        {
            return null;
        }
        
        PubKey pubkey = new PubKey(key);
        string address = pubkey.GetAddress(network).ToWif();
        return address;
    }
    /// <summary>
    /// 获取比特币公钥
    /// </summary>
    /// <param name="masterKeyHex"></param>
    /// <returns></returns>
    public PubKey GetBTCPUK(string masterKeyHex)
    {
        string key = GetPubKeyByPath(masterKeyHex, btcPath);
        if (key == null)
        {
            return null;
        }

        PubKey pubkey = new PubKey(key);
        return pubkey;
    }
    /// <summary>
    /// 获取比特币地址
    /// </summary>
    /// <param name="puk"></param>
    /// <returns></returns>
    public string GetBTCAddressByPuk(string puk)
    {
        if (puk == null)
        {
            return null;
        }
        PubKey pubkey = new PubKey(puk);
        string address = pubkey.GetAddress(network).ToWif();
        return address;
    }
    /// <summary>
    /// 获取EOS拥有者地址
    /// </summary>
    /// <param name="masterKeyHex"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    public string GetEOSPubAddress_owner(string masterKeyHex)
    {
        return GetEOSPubAddress(masterKeyHex, eosPath_owner);
    }

    public string GetEOSPubAddress_admin(string masterKeyHex)
    {
        return GetEOSPubAddress(masterKeyHex, eospath_admin);
    }

    private string GetEOSPubAddress(string masterKeyHex, string path)
    {
        string key = GetPubKeyByPath(masterKeyHex, path);
        if (key == null)
        {
            return null;
        }
        byte[] pub_buf = key.HexToByteArray();
        var checksum = Hashes.RIPEMD160(pub_buf, pub_buf.Length);
        var addy = new byte[pub_buf.Length + 4];
        Array.Copy(pub_buf, 0, addy, 0, pub_buf.Length);
        Array.Copy(checksum, 0, addy, pub_buf.Length, 4);
        string address = "EOS" + Encoders.Base58.EncodeData(addy);
        return address;
    }



}
