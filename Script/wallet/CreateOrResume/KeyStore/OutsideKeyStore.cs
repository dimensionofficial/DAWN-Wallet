using System.Collections;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.KeyStore;
using Nethereum.KeyStore.Model;
using UnityEngine;
using System;
using Nethereum.Signer.Crypto;
using NBitcoin.Crypto;
using NBitcoin.DataEncoders;
using NBitcoin;

public class OutsideKeyStore
{
    #region ETH 

    /// <summary>
    /// KeyStore
    /// </summary>
    /// <param name="password"></param>
    /// <param name="storeJson"></param>
    /// <returns></returns>
    public string GetPrivateKeyByKeyStoreJson(string password, string storeJson)
    {
        KeyStoreKdfChecker _keyStoreKdfChecker = new KeyStoreKdfChecker();
        string _priavteKey = "";
        try
        {
            var type = _keyStoreKdfChecker.GetKeyStoreKdfType(storeJson);

            if (type == KeyStoreKdfChecker.KdfType.pbkdf2)
            {
                var keyStorePbkdf2Service = new KeyStorePbkdf2Service();
                var keyStore = GetKeyStore_pbkdf2Params(storeJson);
                var _key = keyStorePbkdf2Service.DecryptKeyStore(password, keyStore);
                _priavteKey = _key.ToHex();
            }
            if (type == KeyStoreKdfChecker.KdfType.scrypt)
            {
                var _keyStoreScryptService = new KeyStoreScryptService();
                var keyStore = GetKeyStore_scryptParams(storeJson);
                var _key = _keyStoreScryptService.DecryptKeyStore(password, keyStore);
                _priavteKey = _key.ToHex();
            }
        } catch (Exception ex)
        {
            return "";
        }

        return _priavteKey;
    }

    private KeyStore<Pbkdf2Params> GetKeyStore_pbkdf2Params(string keyStoreJson)
    {
        KeyStore<Pbkdf2Params> keyStore = new KeyStore<Pbkdf2Params>();
        Hashtable hastable = Json.jsonDecode(keyStoreJson) as Hashtable;
        keyStore.Address = hastable["address"].ToString();
        keyStore.Id = hastable["id"].ToString();
        keyStore.Version = int.Parse(hastable["version"].ToString());

        Hashtable targetHash = hastable["crypto"] as Hashtable;

        CryptoInfo<Pbkdf2Params> _crypto = new CryptoInfo<Pbkdf2Params>();
        _crypto.Cipher = targetHash["cipher"].ToString();

        CipherParams _cipherParams = new CipherParams();
        Hashtable cipherParHas = targetHash["cipherparams"] as Hashtable;
        _cipherParams.Iv = cipherParHas["iv"].ToString();
        _crypto.CipherParams = _cipherParams;

        _crypto.CipherText = targetHash["ciphertext"].ToString();
        _crypto.Kdf = targetHash["kdf"].ToString();

        Hashtable _scryptHash = targetHash["kdfparams"] as Hashtable;
        Pbkdf2Params _pbkdf2Params = new Pbkdf2Params();
        _pbkdf2Params.Dklen = int.Parse(_scryptHash["dklen"].ToString());
        _pbkdf2Params.Salt = _scryptHash["salt"].ToString();
        _pbkdf2Params.Count = int.Parse(_scryptHash["c"].ToString());
        _pbkdf2Params.Prf = _scryptHash["prf"].ToString();
        _crypto.Kdfparams = _pbkdf2Params;

        _crypto.Mac = targetHash["mac"].ToString();
        keyStore.Crypto = _crypto;

        return keyStore;
    }

    private KeyStore<ScryptParams> GetKeyStore_scryptParams(string keyStoreJson)
    {
        KeyStore<ScryptParams> keyStore = new KeyStore<ScryptParams>();

        Hashtable hastable = Json.jsonDecode(keyStoreJson) as Hashtable;
        keyStore.Address = hastable["address"].ToString();
        keyStore.Id = hastable["id"].ToString();
        keyStore.Version = int.Parse(hastable["version"].ToString());

        Hashtable targetHash = hastable["crypto"] as Hashtable;

        CryptoInfo<ScryptParams> _crypto = new CryptoInfo<ScryptParams>();
        _crypto.Cipher = targetHash["cipher"].ToString();

        CipherParams _cipherParams = new CipherParams();
        Hashtable cipherParHas = targetHash["cipherparams"] as Hashtable;
        _cipherParams.Iv = cipherParHas["iv"].ToString();
        _crypto.CipherParams = _cipherParams;

        _crypto.CipherText = targetHash["ciphertext"].ToString();
        _crypto.Kdf = targetHash["kdf"].ToString();

        Hashtable _scryptHash = targetHash["kdfparams"] as Hashtable;
        ScryptParams _scryptParams = new ScryptParams();
        _scryptParams.Dklen = int.Parse(_scryptHash["dklen"].ToString());
        _scryptParams.Salt = _scryptHash["salt"].ToString();
        _scryptParams.N = int.Parse(_scryptHash["n"].ToString());
        _scryptParams.P = int.Parse(_scryptHash["p"].ToString());
        _scryptParams.R = int.Parse(_scryptHash["r"].ToString());
        _crypto.Kdfparams = _scryptParams;

        _crypto.Mac = targetHash["mac"].ToString();
        keyStore.Crypto = _crypto;

        return keyStore;
    }


    public string GetEthAddressByPrivateKey(string privateKeyToHex)
    {
        try
        {
            var ethEck = new Nethereum.Signer.EthECKey(privateKeyToHex);
            string address = ethEck.GetPublicAddress();

            if (!string.IsNullOrEmpty(address))
            {
                SeedKeyManager.Instance.SetOutSidePrivateKey(address, privateKeyToHex);
            }

            return address;
        }
        catch (Exception ex)
        {
            return "";
        }
    }
    #endregion



    #region BTC
    public string GetBtcAddressByPrivateKey(string privateKey)
    {
        string address = GetBtcAddressByBase58(privateKey);
        if (string.IsNullOrEmpty(address))
        {
            address = GetBtcAddressByHex(privateKey);
        }

        return address;
    }

    private string GetBtcAddressByHex(string privateKeyToHex)
    {
        try
        {
            byte[] tytes = privateKeyToHex.HexToByteArray();
            Key key = new Key(tytes, tytes.Length, true);
            var mainNetPrivateKey = new NBitcoin.BitcoinSecret(key, NBitcoin.Network.Main);
            string address = mainNetPrivateKey.PubKey.GetAddress(NBitcoin.Network.Main).ToString();

            if (!string.IsNullOrEmpty(address))
            {
                SeedKeyManager.Instance.SetOutSidePrivateKey(address, privateKeyToHex + "_Hex");
            }

            return address;
        }
        catch (Exception ex)
        {
            return "";
        }
    }

    private string GetBtcAddressByBase58(string privateKeyBase56)
    {
        try
        {
            var mainNetPrivateKey = new NBitcoin.BitcoinSecret(privateKeyBase56);
            string address = mainNetPrivateKey.PubKey.GetAddress(NBitcoin.Network.Main).ToString();

            if (!string.IsNullOrEmpty(address))
            {
                SeedKeyManager.Instance.SetOutSidePrivateKey(address, privateKeyBase56 + "_B58");
            }

            return address;
        }
        catch (Exception ex)
        {
            return "";
        }

    }

    #endregion

    #region EOS
    public string GetEosAddess(string privateKey)
    {
        string address = GetEosOwerAddessByBase58(privateKey);
        if (string.IsNullOrEmpty(address))
        {
            address = GetEosOwerAddessByHex(privateKey);
        }
        return address;
    }

    public string GetEosBas58Private(string privateHex)
    {
        Base58Encoder ben = new Base58Encoder();
        byte[] b = privateHex.HexToByteArray();
        byte[] bb = new byte[b.Length + 1];
        bb[0] = 0x80;
        for (int i = 1; i < bb.Length; i++)
        {
            bb[i] = b[i - 1];
        }

        byte[] temp = Hashes.SHA256(Hashes.SHA256(bb));

        byte[] endstr = new byte[4];
        for (int i = 0; i < 4; i++)
        {
            endstr[i] = temp[i];
        }

        byte[] target = new byte[bb.Length + 4];

        for (int i = 0; i < bb.Length; i++)
        {
            target[i] = bb[i];
        }
        for (int i = 0; i < 4; i++)
        {
            target[bb.Length + i] = endstr[i];
        }

        return ben.EncodeData(target);
    }

    private string GetEosOwerAddessByHex(string privateKeyHex)
    {
        try
        {
            byte[] bytes = privateKeyHex.HexToByteArray();
            string address = GetEosAddress(bytes);

            if (!string.IsNullOrEmpty(address))
            {
                SeedKeyManager.Instance.SetOutSidePrivateKey(address, privateKeyHex + "_Hex");
            }

            return address;
        }
        catch (Exception ex)
        {
            return "";
        }
    }
    private string GetEosOwerAddessByBase58(string privateKeyBase58)
    {
        try
        {
            BitcoinSecret mainNetPrivateKey = new BitcoinSecret(privateKeyBase58);
            byte[] bytes = mainNetPrivateKey.PrivateKey.ToBytes();
            string address = GetEosAddress(bytes);

            if (!string.IsNullOrEmpty(address))
            {
                SeedKeyManager.Instance.SetOutSidePrivateKey(address, privateKeyBase58 + "_B58");
            }

            return address;
        }
        catch (Exception ex)
        {
            return "";
        }
    }

    private string GetEosAddress(byte[] privateKey)
    {
        ECKey ecKey = new ECKey(privateKey, true);
        var pub_buf = ecKey.GetPubKey(true);
        var checksum = Hashes.RIPEMD160(pub_buf, pub_buf.Length);
        var addy = new byte[pub_buf.Length + 4];
        Array.Copy(pub_buf, 0, addy, 0, pub_buf.Length);
        Array.Copy(checksum, 0, addy, pub_buf.Length, 4);
        string address = "EOS" + Encoders.Base58.EncodeData(addy);
        return address;
    }
    #endregion

}
