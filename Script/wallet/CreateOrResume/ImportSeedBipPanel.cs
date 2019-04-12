using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NBitcoin;
using NBitcoin.Crypto;
using NBitcoin.DataEncoders;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Signer.Crypto;
using Org.BouncyCastle.Math.EC;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ImportSeedBipPanel : MonoBehaviour
{
    public InputField inputWalletName;

    public Text noteText;


    public List<InputField> seedWordInputList = new List<InputField>();

    public void OpenMe()
    {
        gameObject.SetActive(true);
        inputWalletName.text = "";
        for (int i = 0; i < seedWordInputList.Count; i++)
        {
            seedWordInputList[i].text = "";
            seedWordInputList[i].onValueChanged.RemoveListener(CHeckInput);
            seedWordInputList[i].onValueChanged.AddListener(CHeckInput);
        }

        noteText.text = "";
    }

    private string GetEosBas58Private(string privateHex)
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

    void Start()
    {
        //
        //var mainNetPrivateKey = new NBitcoin.BitcoinSecret(QRPayTools.GetMnemonic(SeedKeyManager.Instance.firstSeedBip).PrivateKey, NBitcoin.Network.Main);
        //string bas58 = mainNetPrivateKey.ToWif();
        //Debug.Log("BTC 私钥_B58 = " + bas58);
        //Debug.Log("BTC 私钥_Hex = " + mainNetPrivateKey.PrivateKey.ToBytes().ToHex());
        //mainNetPrivateKey = new NBitcoin.BitcoinSecret(bas58);

        Mnemonic mnemo = new Mnemonic(SeedKeyManager.Instance.firstSeedBip, Wordlist.ChineseSimplified);
        var Seed = mnemo.DeriveSeed().ToHex();
        var masterKey = new ExtKey(Seed);
        KeyPath keyPath = new KeyPath(WalletTools.eospath_admin);
        ExtKey key = masterKey.Derive(keyPath);

       

        Base58Encoder ben = new Base58Encoder();
        byte[] b = key.PrivateKey.ToBytes();
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

        Debug.Log("EOS 私钥_Hex = " + key.PrivateKey.ToBytes().ToHex());
        Debug.Log("EOS 私钥_B58 = " + ben.EncodeData(target));



        //Debug.Log("EOS8HuvjfQeUS7tMdHPPrkTFMnEP7nr6oivvuJyNcvW9Sx5MxJSkZ");

        ////byte[] requestBytes = Encoding.UTF8.GetBytes("0fccf24e46102781a9848eb6f38c7ddd458fecf4843335005c5ea7416a5fed58");
        ////Base58Encoder encoder = new Base58Encoder();
        ////string output = encoder.EncodeData(requestBytes);
        ////var mainNetPrivateKey = new NBitcoin.BitcoinSecret(output, NBitcoin.Network.Main);
        //string key = mainNetPrivateKey.PrivateKey.ToBytes().ToHex();
        //Debug.Log("比特币私钥："  + mainNetPrivateKey.ToWif()  + " || " + key + " 地址：" + mainNetPrivateKey.PubKey.GetAddress(NBitcoin.Network.Main));
        ////
        //var ethEck = new Nethereum.Signer.EthECKey(QRPayTools.GetMnemonic(SeedKeyManager.Instance.firstSeedBip, HardwareWallet.WalletType.ETH).PrivateKey.ToBytes(), true);
        ////var ethEck = new Nethereum.Signer.EthECKey("0x57d3abdf89768323b78dc90a0a3410b94f14ae5e1a3106053796f6e38378c61a");
        //Debug.Log("以太坊私钥： " + ethEck.GetPrivateKey() + "  地址： " + ethEck.GetPublicAddress());


        OutsideKeyStore outStore = new OutsideKeyStore();
        Debug.Log(outStore.GetEosAddess("5KCfLXrpB913R2WJLLb8TF24AW2d2xRcJWG4y15QaUWqM6s9JqB"));//
        Debug.Log(outStore.GetEosAddess("L3LUDDCsDkKBf3eKWMTdtpsexw8A3ddcpG9WWBLT4kSgPizigkxZ"));
        Debug.Log(outStore.GetEosAddess("b68062cc24a340b5ca7a73a25079698360e3afb7ad658aef7c4458f86e86bb0e"));

        //Debug.Log(outStore.GetEosAddess("5KHmBV47Cm6Vj8Ab8CSVgZXBiNFR9za2DxFeXC55gJ1AZcbmcgy")); //147ebbff10acc44937851fec3b5289f49a23ee8b1af0ed111469a9364e624ef6
        //Debug.Log(outStore.GetEosAddess("c21559f23e825cbf65735071e7b6c57e064aed6ebd14aa20584917d9b056ac83"));  //5JS9bTWMc52HWmMC8v58hdfePTxPV5dd5fcxq92xUzbfmafeeRo
        //EOS8eF1w6sDm4PxyMf9g5vM89L1VvF9uzU99PNJrpPWKELSnPT17g   147ebbff10acc44937851fec3b5289f49a23ee8b1af0ed111469a9364e624ef6
        //Debug.Log("base56: " + outStore.GetBtcAddressByPrivateKey("KxVe9JMNevrxSpTWUKU41mAyNiB8x51rou7VMPaE3pjLjsD68uTY"));
        //Debug.Log("" + outStore.GetBtcAddressByPrivateKey("260856143ace3559b84617360d6dea20fdccf7552f69d01a5cb99756c698524b"));
        //OutsideKeyStore osk = new OutsideKeyStore();
        //byte[] privateKey = QRPayTools.GetPrivateKeyByPath(SeedKeyManager.Instance.firstSeedBip, WalletTools.eospath_admin);
        //Debug.Log("管理者地址：" + osk.GetEosAddess(privateKey.ToHex()));

        //////GetEosAddress(privateKey);
        //Debug.Log("管理者私钥：" + privateKey.ToHex());

        //privateKey = QRPayTools.GetPrivateKeyByPath(SeedKeyManager.Instance.firstSeedBip, WalletTools.eosPath_owner);


        //Debug.Log("拥有者私钥："+ privateKey.ToHex());
        //Debug.Log("拥有者地址：" + osk.GetEosAddess(privateKey.ToHex()));
        //Debug.Log("_____________________");
        //Debug.Log("EOS8HuvjfQeUS7tMdHPPrkTFMnEP7nr6oivvuJyNcvW9Sx5MxJSkZ");
        //Debug.Log(outStore.GetEosAddess("5JS9bTWMc52HWmMC8v58hdfePTxPV5dd5fcxq92xUzbfmafeeRo"));
        //Debug.Log("_____________________");
        //Debug.Log("EOS7pscBeDbJTNn5SNxxowmWwoM7hGj3jDmgxp5KTv7gR89Ny5ii3");
        //Debug.Log(osk.GetEosAddess("5KgKxmnm8oh5WbHC4jmLARNFdkkgVdZ389rdxwGEiBdAJHkubBH"));
        //Debug.Log("_____________________");
        //Debug.Log("EOS833HgCT3egUJRDnW5k3BQGqXAEDmoYo6q1s7wWnovn6B9Mb1pd");
        //Debug.Log(osk.GetEosAddess("5JFLPVygcZZdEno2WWWkf3fPriuxnvjtVpkThifYM5HwcKg6ndu"));
        //Debug.Log("_____________________");
        //Debug.Log("0xD969fbF8E28d61202AD370EE8C6017d7e9660230");
        //Debug.Log(osk.GetEthAddressByPrivateKey("0x7b61a533f2ab08eec89a6f0cf2fa5139d1feafed2311aee6eda4c4470717e00e"));
        //Debug.Log("_____________________");
        //Debug.Log("0x61C605C2bD284Cb4E093b27463bC9FF2dDFA11AA");
        //Debug.Log(osk.GetEthAddressByPrivateKey("0x4497e5c00f4bfe1f11d8eb4fcc7f3043e906ee83e5785bd952c69a820bb8ab30"));

    }
    public GameObject lastSelectedGameObject;

    private void CHeckInput(string v)
    {
        GameObject g = EventSystem.current.currentSelectedGameObject;
        bool isSelect = false;
        int id = 0;
        for (int i = 0; i < seedWordInputList.Count; i++)
        {
            if (g == seedWordInputList[i].gameObject)
            {
                isSelect = true;
                id = i;
                break;
            }
        }

        if (isSelect && id != seedWordInputList.Count - 1)
        {
            if (seedWordInputList[id].text.Length == 4)
            {
                lastSelectedGameObject = seedWordInputList[id + 1].gameObject;
                g.GetComponent<InputField>().DeactivateInputField();
                TimerManager.Instance.AddTimer(1F, () => ActivateNext());
            }
        }
    }

    private void ActivateNext()
    {
        EventSystem.current.SetSelectedGameObject(lastSelectedGameObject);
        lastSelectedGameObject.GetComponent<InputField>().ActivateInputField();
    }


    public void OnClickResumeBtn()
    {
        if (!isWriteOver())
        {
            ShowNoticeText("请输入完整的助记词");
            return;
        }

        if (string.IsNullOrEmpty(inputWalletName.text))
        {
            ShowNoticeText("钱包名不能为空");
            return;
        }

        string numberBip = seedWordInputList[0].text;
        for (int i = 1; i < seedWordInputList.Count; i++)
        {
            numberBip += " " + seedWordInputList[i].text;
        }

        PanelManager._Instance.loadingPanel.SetActive(true);

        string bip = QRPayTools.ChangeBipNumberToSpChinese(numberBip);
        if (string.IsNullOrEmpty(bip))
        {
            ShowNoticeText("输入的助记词不正确");
            PanelManager._Instance.loadingPanel.SetActive(false);
            return;
        }

        // string str = SeedKeyManager.Instance.GetAddresses();
        SeedKeyManager.Instance.SetSeedBipArr(bip);

        PanelManager._Instance.loadingPanel.SetActive(true);
        ExtPubKey masterKey = QRPayTools.GetMastPubKey(bip);
        string addressStr = inputWalletName.text + "&" + masterKey.ToBytes().ToHex() + "&" + SystemInfo.deviceUniqueIdentifier + "&" + "V123";
        byte[] addr = System.Text.UTF8Encoding.UTF8.GetBytes(addressStr);
        byte[] key = Hashes.SHA256(System.Text.UTF8Encoding.UTF8.GetBytes("fdsamcldi123sawqa"));
        byte[] result = Encry.EncryData(addr, key);
        string data = QRPayTools.ToHexString(result);

        SeedKeyManager.Instance.AddBipToDic(bip);

        SeedKeyManager.Instance.SavaAddressToServer(data, delegate() 
        {
            PopupLine.Instance.Show("该钱包已导入过");
            PanelManager._Instance.loadingPanel.SetActive(false);
        }, delegate ()
        {
            PopupLine.Instance.Show("导入成功");
            gameObject.SetActive(false);
            PanelManager._Instance.loadingPanel.SetActive(false);
        }, delegate() 
        {
            PopupLine.Instance.Show("导入失败，请重试！");
            PanelManager._Instance.loadingPanel.SetActive(false);
        }, true);
    }

    public bool isWriteOver()
    {
        for (int i = 0; i < seedWordInputList.Count; i++)
        {
            if (seedWordInputList[i].text.Length != 4)
            {
                return false;
            }
        }

        return true;
    }

    private void ShowNoticeText(string notex)
    {
        CancelInvoke();
        noteText.gameObject.SetActive(true);
        noteText.text = notex;
        Invoke("HideNoticeText", 2);
    }

    private void HideNoticeText()
    {
        noteText.gameObject.SetActive(false);
    }
}
