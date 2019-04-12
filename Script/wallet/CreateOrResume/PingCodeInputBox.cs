using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Nethereum.Signer;
using NBitcoin;
using HardwareWallet;
using Nethereum.Hex.HexConvertors.Extensions;

public class PingCodeInputBox : MonoBehaviour
{
    public enum SingType
    {
        None,
        BTC,
        ETH,
        Kyber,
        EOS,
    }
    public SingType currentType = SingType.None;
    private EosGetSingInfoPanel.EosSingType cur_eosSingType;

    public InputField inputPassWord;

    public GameObject openEye;
    public GameObject closeEye;

    public Text noteText;

    public Action callBack;

    private string singStr;
    private string keyPath;
    private string singAddress;

    private Action onColseCallBack;

    public void OpenMe(string needSingStr, SingType _singType, string address, Action colseCallBack, Action _callBack = null)
    {
        gameObject.SetActive(true);
        inputPassWord.text = "";
        noteText.text = "";
        openEye.SetActive(false);
        closeEye.SetActive(true);

        inputPassWord.contentType = InputField.ContentType.Password;

        singStr = needSingStr;
        currentType = _singType;
        singAddress = address;

        callBack = _callBack;
        onColseCallBack = colseCallBack;
    }

    public void OnClickClosedBtn()
    {
        if (onColseCallBack != null)
        {
            onColseCallBack();
            onColseCallBack = null;
        }
        gameObject.SetActive(false);
    }


    public void SetEosSingInfo(string _keyPath, EosGetSingInfoPanel.EosSingType singType)
    {
        keyPath = _keyPath;
        cur_eosSingType = singType;
    }

    public void OnClickHidePassWord()
    {
        if (inputPassWord.contentType == InputField.ContentType.Password)
        {
            inputPassWord.contentType = InputField.ContentType.Standard;
            openEye.SetActive(true);
            closeEye.SetActive(false);
        }
        else
        {
            openEye.SetActive(false);
            closeEye.SetActive(true);

            inputPassWord.contentType = InputField.ContentType.Password;
        }

        inputPassWord.gameObject.SetActive(false);
        inputPassWord.gameObject.SetActive(true);
    }

    public void OnClickOkBtn()
    {
        string mypingCode = SeedKeyManager.Instance.pingCode;
        if (mypingCode.Equals(inputPassWord.text))
        {
            if (callBack != null)
            {
                callBack();
                callBack = null;
            }
            gameObject.SetActive(false);
            string bip = SeedKeyManager.Instance.GetSeedBip(singAddress);
            bool isPrivatKey = false;
            if (string.IsNullOrEmpty(bip))
            {
                isPrivatKey = true;
                bip = SeedKeyManager.Instance.GetOutSidePrivateKey(singAddress);
            }

            if (string.IsNullOrEmpty(bip))
            {
                PopupLine.Instance.Show("签名失败，请重试！");
                return;
            }

            SingAndSend(singStr, bip, isPrivatKey);
        }
        else
        {
            ShowNoticeText("您输入的密码不正确，请重新输入");
        }
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

    public void SingAndSend(string str, string bip, bool isPrivateKey)
    {
        IWalletDataBase walletBase = WalletDataFactory.CreateWalletData(str);

        if (currentType == SingType.BTC)
        {
            NBitcoin.BitcoinSecret mainNetPrivateKey = null; // = new NBitcoin.BitcoinSecret(QRPayTools.GetMnemonic(bip).PrivateKey, NBitcoin.Network.Main); ;

            if (isPrivateKey)
            {
                string keyLast = bip.Remove(0, bip.Length - 3);

                string key = bip.Substring(0, bip.Length - 4);

                if (keyLast.Equals("Hex"))
                {
                    byte[] tytes = key.HexToByteArray();
                    Key k = new Key(tytes, tytes.Length, true);
                    mainNetPrivateKey = new NBitcoin.BitcoinSecret(k, NBitcoin.Network.Main);
                }
                else if (keyLast.Equals("B58"))
                {
                    mainNetPrivateKey = new NBitcoin.BitcoinSecret(key);
                }

            }
            else
            {
                mainNetPrivateKey = new NBitcoin.BitcoinSecret(QRPayTools.GetMnemonic(bip).PrivateKey, NBitcoin.Network.Main);
            }

            string singData = QRPayTools.CreateSignedPayWithoutBuilder(walletBase.data, mainNetPrivateKey, NBitcoin.Network.Main);

            if (string.IsNullOrEmpty(singData))
            {
                PopupLine.Instance.Show("签名失败，请重试！");
            }
            else
            {
                PanelManager._Instance._WalletInfoPanel.sendCoinPanel.sendCansing.OnScanEnd(singData);
            }
        }
        else if (currentType == SingType.ETH || currentType == SingType.Kyber)
        {
            try
            {
                EthECKey ethEck = null;

                if (isPrivateKey)
                {
                    ethEck = new Nethereum.Signer.EthECKey(bip.HexToByteArray(), true);
                }
                else
                {
                    ethEck = new EthECKey(QRPayTools.GetMnemonic(bip, HardwareWallet.WalletType.ETH).PrivateKey.ToBytes(), true);
                }

                string privateKey = ethEck.GetPrivateKey();
                string singData = QRPayTools.CreateSignedPayWithoutBuilder_ETH(walletBase.data, privateKey);

                if (string.IsNullOrEmpty(singData))
                {
                    PopupLine.Instance.Show("签名失败，请重试！");
                }
                else
                {
                    if (currentType == SingType.ETH)
                    {
                        PanelManager._Instance._WalletInfoPanel.sendCoinPanel.sendCansing.OnScanEnd(singData);
                    }
                    else if (currentType == SingType.Kyber)
                    {
                        KyberTools.instance.OnEndScan(singData);
                    }

                }
            } catch (Exception ex)
            {
                PopupLine.Instance.Show("签名失败，请重试！");
            }
           
        } else if (currentType == SingType.EOS)
        {
            byte[] privateKey = null;

            if (isPrivateKey)
            {
                string keyLast = bip.Remove(0, bip.Length - 3);

                string key = bip.Substring(0, bip.Length - 4);

                if (keyLast.Equals("Hex"))
                {
                    privateKey = key.HexToByteArray();
                }
                else if (keyLast.Equals("B58"))
                {
                    BitcoinSecret mainNetPrivateKey = new BitcoinSecret(key);
                    privateKey = mainNetPrivateKey.PrivateKey.ToBytes();
                }
            }
            else
            {
                privateKey = QRPayTools.GetPrivateKeyByPath(bip, keyPath);
            }

            string singData = QRPayTools.SignHashEOS(walletBase.data, privateKey);

            if (string.IsNullOrEmpty(singData))
            {
                PopupLine.Instance.Show("签名失败，请重试！");
            }
            else
            {
                PanelManager._Instance._eosScanSing.OnScanEnd(singData);
            }
        }
    }

}
