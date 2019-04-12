using System.Collections;
using System;
using System.Collections.Generic;
using NBitcoin;
using NBitcoin.Crypto;
using Nethereum.Hex.HexConvertors.Extensions;
using UnityEngine;

public class SeedKeyManager : MonoBehaviour
{

    public static SeedKeyManager Instance;

    /// <summary>
    /// 每个钱包地址对应的bip（key 为地址， value 为bip）
    /// </summary>
    public Dictionary<string, string> seedBipDic = new Dictionary<string, string>();

    public string pingCode
    {
        get
        {
            string pc = PlayerPrefs.GetString("MyWalletPingCode", "");
            return pc;
        }
        set
        {
            PlayerPrefs.SetString("MyWalletPingCode", value);
        }
    }

    //public ExtPubKey masterPuKey
    //{
    //    get
    //    {
    //        string mp = PlayerPrefs.GetString("masterKey", "");
    //        return new ExtPubKey(mp.HexToByteArray());
    //    }
    //    set
    //    {
    //        PlayerPrefs.SetString("masterKey", value.ToBytes().ToHex());
    //    }
    //}

    public string firstSeedBip
    {
        get
        {
            string bip = PlayerPrefs.GetString("SeedPrivateKey", "");
            return bip;
        }
        set
        {
            PlayerPrefs.SetString("SeedPrivateKey", value);
        }
    }

    public string walletSN;

    void Awake()
    {
        Instance = this;
    }

    public void SetSeedBipArr(string bip)
    {
        string[] bipArr = PlayerPrefsX.GetStringArray("SeedPrivateKeyArr");
        for (int i = 0; i < bipArr.Length; i++)
        {
            if (bipArr[i].Equals(bip))
                return;
        }
        List<string> bipList = new List<string>(bipArr);
        bipList.Add(bip);

        //AddBipToDic(bip);

        PlayerPrefsX.SetStringArray("SeedPrivateKeyArr", bipList.ToArray());
    }

    public string[] GetSeedBipList()
    {
        string[] bipArr = PlayerPrefsX.GetStringArray("SeedPrivateKeyArr");
        return bipArr;
    }

    public string GetSeedBip(string address)
    {
        if (seedBipDic.ContainsKey(address.ToLower()))
        {
            return seedBipDic[address.ToLower()];
        }

        return "";
    }

    public string GetOutSidePrivateKey(string address)
    {
        return PlayerPrefs.GetString("myOutSidePrivateKey" + address);
    }

    public void SetOutSidePrivateKey(string address, string privatkey)
    {
        PlayerPrefs.SetString("myOutSidePrivateKey" + address, privatkey);
    }

    //public string GetMasterPublicKey()
    //{
    //    ExtPubKey pubkey = new ExtPubKey(masterPuKey.ToBytes());
    //    return pubkey.ToBytes().ToHex();
    //}

    /// <summary>
    /// 是否备份
    /// </summary>
    /// <returns></returns>
    public bool IsBackUp()
    {
        if (PlayerPrefs.HasKey("SeedIsBackUp") && PlayerPrefs.GetString("SeedIsBackUp") == "true")
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 设置已备份
    /// </summary>
    public void SetBackup()
    {
        PlayerPrefs.SetString("SeedIsBackUp", "true");
    }

    public bool isFirstSeedBip(string addrss)
    {
        
        string bip = GetSeedBip(addrss);

        return firstSeedBip.Equals(bip);
    }

    public void AddBipToDic(string bip)
    {
        ExtPubKey _extPubkey = QRPayTools.GetMastPubKey(bip);
        string _extPubkeyStr = _extPubkey.ToBytes().ToHex();

        string addressStr = "walletSN" + "&" + _extPubkeyStr + "&" + SystemInfo.deviceUniqueIdentifier + "&" + "V123";
        byte[] addr = System.Text.UTF8Encoding.UTF8.GetBytes(addressStr);
        byte[] key = Hashes.SHA256(System.Text.UTF8Encoding.UTF8.GetBytes("fdsamcldi123sawqa"));
        byte[] result = Encry.EncryData(addr, key);
        string data = QRPayTools.ToHexString(result);

        
        byte[] tempAddress = QRPayTools.HexToByte(data);
        byte[] addressDecode = Encry.DecryData(tempAddress, key);
        if (addressDecode != null && addressDecode.Length > 0)
        {
            string address = System.Text.UTF8Encoding.UTF8.GetString(addressDecode);

            string[] addressArr = SetBipToDic(address);
            if (addressArr.Length == 4)
            {
                for (int i = 0; i < addressArr.Length; i++)
                {
                    if (!seedBipDic.ContainsKey(addressArr[i].ToLower()))
                    {
                        seedBipDic.Add(addressArr[i].ToLower(), bip);
                    }
                }
            }
        }
    }



    /// <summary>
    /// 生成配对地信息
    /// </summary>
    /// <returns></returns>
    public string GetAddresses(string bip)
    {
        ExtPubKey masterKey= QRPayTools.GetMastPubKey(bip);
        string addressStr = walletSN + "&" + masterKey.ToBytes().ToHex() + "&" + SystemInfo.deviceUniqueIdentifier + "&" + "V123";
        byte[] addr = System.Text.UTF8Encoding.UTF8.GetBytes(addressStr);
        byte[] key = Hashes.SHA256(System.Text.UTF8Encoding.UTF8.GetBytes("fdsamcldi123sawqa"));
        byte[] result = Encry.EncryData(addr, key);
        string data = QRPayTools.ToHexString(result);
        return data;
    }

    public void SavaAddressToServer(string str, Action theSameCallback, Action successCallBack, Action failerCallBack, bool isAllrecover)
    {
        byte[] key = Hashes.SHA256(System.Text.UTF8Encoding.UTF8.GetBytes("fdsamcldi123sawqa"));
        byte[] tempAddress = QRPayTools.HexToByte(str);
        byte[] addressDecode = Encry.DecryData(tempAddress, key);
        if (addressDecode != null && addressDecode.Length > 0)
        {
            string address = System.Text.UTF8Encoding.UTF8.GetString(addressDecode);

            string[] addressArr = SetBipToDic(address);

            ShowScanAddress(address, addressArr, theSameCallback, successCallBack, failerCallBack, isAllrecover);
        }
        else
        {
            if (failerCallBack != null)
                failerCallBack();
        }
    }

    private string[] SetBipToDic(string str)
    {
        string[] scanInfos = str.Split('&');
        if (scanInfos.Length != 4)
        {
            return null;
        }

        string k = scanInfos[1];
        WalletTools tools = new WalletTools();

        string btcAddress = tools.GetBTCPubAddress(k);
        string ethAddress = tools.GetETHPubAddress(k);
        string eosAddress_owner = tools.GetEOSPubAddress_owner(k);
        string eosAddress_admin = tools.GetEOSPubAddress_admin(k);

        string[] addressArr = new string[4] { btcAddress, ethAddress, eosAddress_owner, eosAddress_admin };
        return addressArr;
    }

    public void SavaAddressToServer(string walletName, string address, NewWalletManager.CoinType coinType, Action successCallback, Action failCallback,bool isMulit=false)
    {
        Hashtable h = new Hashtable();
        h["BTC"] = "";
        h["ETH"] = "";
        h["EOSowner"] = "";
        h["EOSadmin"] = "";
        h["walletName"] = walletName;
        h["machineid"] = SystemInfo.deviceUniqueIdentifier;
        switch (coinType)
        {
            case NewWalletManager.CoinType.BTC:
                h["BTC"] = address;
                break;
            case NewWalletManager.CoinType.ETH:
                h["ETH"] = address;
                break;
            case NewWalletManager.CoinType.EOS:
                h["EOSadmin"] = address;
                break;
        }

        string jsonstring = Json.jsonEncode(h);
        List<KeyValuePair<string, string>> ws = new List<KeyValuePair<string, string>>();
        ws.Add(new KeyValuePair<string, string>("op", "addAddress"));
        ws.Add(new KeyValuePair<string, string>("userName", NewWalletManager._Intance.userName));
        ws.Add(new KeyValuePair<string, string>("addressInfo", jsonstring));

        StartCoroutine(HttpManager._Intance.SendRequest(ws, delegate (Hashtable table)
        {
            NewWalletManager._Intance.SaveAddress(jsonstring,true);

            switch (coinType)
            {
                case NewWalletManager.CoinType.BTC:
                    Debug.Log("isMulit = " + isMulit);
                    PanelManager._Instance._mainPanel.AddBitItem(new string[2] { address, walletName });
                    if (!isMulit)
                    {
                        PanelManager._Instance._mainPanel.AddUsdtItem(new string[2] { address, walletName });
                    }
                    else
                    {
                        List<KeyValuePair<string, string>> wsw = new List<KeyValuePair<string, string>>();
                        wsw.Add(new KeyValuePair<string, string>("op", "deletAddress"));
                        wsw.Add(new KeyValuePair<string, string>("userName", NewWalletManager._Intance.userName));
                        wsw.Add(new KeyValuePair<string, string>("address", address));
                        wsw.Add(new KeyValuePair<string, string>("type", ((int)(NewWalletManager.CoinType.USDT)).ToString()));
                        StartCoroutine(HttpManager._Intance.SendRequest(wsw, null));
                    }
                    break;
                case NewWalletManager.CoinType.ETH:
                    PanelManager._Instance._mainPanel.AddEthItem(new string[2] { address, walletName });
                    break;
                case NewWalletManager.CoinType.EOS:
                    PanelManager._Instance._mainPanel.AddEosItem(new string[2] { address, walletName }); ;
                    break;
            }
            if (successCallback != null)
            {
                successCallback();
            }
        }, failCallback,isMulit));
    }



    private void ShowScanAddress(string str, string[] addressArr, Action theSameCallback, Action successCallBack, Action failerCallBack, bool isAllRecover,bool isMulit=false)
    {
        string[] scanInfos = str.Split('&');
        if (scanInfos.Length != 4 && addressArr.Length != 4)
        {
            return;
        }

        //string k = scanInfos[1];
        //WalletTools tools = new WalletTools();

        string btcAddress = addressArr[0];
        string ethAddress = addressArr[1];
        string eosAddress_owner = addressArr[2];
        string eosAddress_admin = addressArr[3];

        string walletName = "SN " + scanInfos[0]; //钱包名称
        string machineid = scanInfos[2]; //机器id

        string k = scanInfos[1];
        NewWalletManager._Intance.SavaBtcPubkey(btcAddress, k);

        if (isAllRecover)
        {
            if (NewWalletManager._Intance.IsSameAsAddress(btcAddress, NewWalletManager.CoinType.BTC) &&
        NewWalletManager._Intance.IsSameAsAddress(ethAddress, NewWalletManager.CoinType.ETH) &&
        NewWalletManager._Intance.IsSameAsAddress(eosAddress_admin, NewWalletManager.CoinType.EOS) &&
        NewWalletManager._Intance.IsSameAsAddress(btcAddress, NewWalletManager.CoinType.USDT))
            {
                if (theSameCallback != null)
                {
                    theSameCallback();
                }
                return;
            }
        }
        else
        {
            if (NewWalletManager._Intance.IsSameAsAddress(btcAddress, NewWalletManager.CoinType.BTC) ||
      NewWalletManager._Intance.IsSameAsAddress(ethAddress, NewWalletManager.CoinType.ETH) ||
      NewWalletManager._Intance.IsSameAsAddress(eosAddress_admin, NewWalletManager.CoinType.EOS) ||
       NewWalletManager._Intance.IsSameAsAddress(btcAddress, NewWalletManager.CoinType.USDT))
            {
                if (theSameCallback != null)
                {
                    theSameCallback();
                }
                return;
            }
        }

        if (NewWalletManager._Intance.JudgeBTCAddress(btcAddress) != null &&
           NewWalletManager._Intance.JudgeETHAddress(ethAddress) &&
           NewWalletManager._Intance.JudgeEosAddress(eosAddress_owner))
        {
            Hashtable h = new Hashtable();
            h["BTC"] = btcAddress;
            h["ETH"] = ethAddress;
            h["EOSowner"] = eosAddress_owner;
            h["EOSadmin"] = eosAddress_admin;
            h["walletName"] = walletName;
            h["machineid"] = machineid;
            string jsonstring = Json.jsonEncode(h);
            List<KeyValuePair<string, string>> ws = new List<KeyValuePair<string, string>>();
            ws.Add(new KeyValuePair<string, string>("op", "addAddress"));
            ws.Add(new KeyValuePair<string, string>("userName", NewWalletManager._Intance.userName));
            ws.Add(new KeyValuePair<string, string>("addressInfo", jsonstring));

            StartCoroutine(HttpManager._Intance.SendRequest(ws, delegate (Hashtable table)
            {
                NewWalletManager._Intance.SaveAddress(jsonstring);

                PanelManager._Instance._mainPanel.AddBitItem(new string[2] { btcAddress, walletName });

                PanelManager._Instance._mainPanel.AddEthItem(new string[2] { ethAddress, walletName });

                PanelManager._Instance._mainPanel.AddEosItem(new string[2] { eosAddress_admin, walletName });
                if (!isMulit)
                {
                    PanelManager._Instance._mainPanel.AddUsdtItem(new string[2] { btcAddress, walletName });
                }
                if (successCallBack != null)
                {
                    successCallBack();
                }
            }));
        }
        else
        {
            if (failerCallBack != null)
                failerCallBack();
        }
    }


    public bool IsContainLetter(string str)
    {
        for (int i = 0; i < str.Length; i++)
        {
            if ((str[i] >= 65 && str[i] <= 90) || (str[i] >= 97 && str[i] <= 122))
            {
                return true;
            }
        }
        return false;
    }
    public bool IsContainNumber(string str)
    {
        for (int i = 0; i < str.Length; i++)
        {
            if (str[i] >= 48 && str[i] <= 57)
            {
                return true;
            }
        }
        return false;
    }


}
