using System;
using System.Collections;
using System.Collections.Generic;
using HardwareWallet;
using NBitcoin;
using NBitcoin.Crypto;
using NBitcoin.DataEncoders;
using Nethereum.Signer;
using Nethereum.Signer.Crypto;
using UnityEngine;
using UnityEngine.UI;

public class CreateWalletPanel : MonoBehaviour {

    public InputField inputWalletName;

    public InputField inputPassword;
    public GameObject eyeOpenMark;
    public GameObject eyeClosedMark;

    public InputField inputDbPassword;
    public GameObject eyeOpenMark1;
    public GameObject eyeClosedMark1;

    public Text noteText;


    public void OpenMe()
    {
        gameObject.SetActive(true);
        inputWalletName.text = "";
        inputDbPassword.text = "";
        inputPassword.text = "";
        inputDbPassword.contentType = InputField.ContentType.Password;
        inputPassword.contentType = InputField.ContentType.Password;
        eyeOpenMark.SetActive(false);
        eyeClosedMark.SetActive(true);
        eyeOpenMark1.SetActive(false);
        eyeClosedMark1.SetActive(true);
        noteText.gameObject.SetActive(false);
    }


    public void ShowOrHidePassword(InputField inputText)
    {
        if (inputText.contentType == InputField.ContentType.Password)
        {
            inputText.contentType = InputField.ContentType.Standard;
            if (inputText == inputPassword)
            {
                eyeOpenMark.SetActive(true);
                eyeClosedMark.SetActive(false);
            }
            else
            {
                eyeOpenMark1.SetActive(true);
                eyeClosedMark1.SetActive(false);
            }
        }
        else
        {
            inputText.contentType = InputField.ContentType.Password;
            if (inputText == inputPassword)
            {
                eyeOpenMark.SetActive(false);
                eyeClosedMark.SetActive(true);
            }
            else
            {
                eyeOpenMark1.SetActive(false);
                eyeClosedMark1.SetActive(true);
            }
        }

        inputText.gameObject.SetActive(false);
        inputText.gameObject.SetActive(true);
    }

    public void OnClickCreateBtn()
    {
        if (string.IsNullOrEmpty(inputWalletName.text))
        {
            ShowNoticeText("钱包名不能为空");
            return;
        }

        if (string.IsNullOrEmpty(inputPassword.text))
        {
            ShowNoticeText("密码不能为空");
            return;
        }
        else
        {
            if (inputPassword.text.Length > 12 || inputPassword.text.Length < 8)
            {
                ShowNoticeText("密码长度必须在8-12位之间");
                return;
            }

            if (!SeedKeyManager.Instance.IsContainLetter(inputPassword.text) || !SeedKeyManager.Instance.IsContainNumber(inputPassword.text))
            {
                ShowNoticeText("密码必须同时包含数字与字母");
                return;
            }
            else if (!inputPassword.text.Equals(inputDbPassword.text))
            {
                ShowNoticeText("两次输入的密码不相同");
                return;
            }
        }

        CreatWalletSeed();
    }


    private void CreatWalletSeed()
    {
        string bip = QRPayTools.CreateBipString();
        PanelManager._Instance.loadingPanel.SetActive(true);
        if (!string.IsNullOrEmpty(bip))
        {
            SeedKeyManager.Instance.firstSeedBip = bip;

            SeedKeyManager.Instance.SetSeedBipArr(bip);

   //       SeedKeyManager.Instance.masterPuKey = QRPayTools.GetMastPubKey(bip);
            SeedKeyManager.Instance.pingCode = inputPassword.text;

            string walletName = "";
            if (inputWalletName.text.Length < 4)
            {
                walletName = inputWalletName.text;
            }
            else
            {
                walletName = inputWalletName.text.Substring(0, 4);
            }
            
            SeedKeyManager.Instance.walletSN = walletName;

            PanelManager._Instance._loginPanel.InitApp();

            PanelManager._Instance.loadingPanel.SetActive(false);

            PanelManager._Instance._createOrResume.gameObject.SetActive(false);
        }
        else
        {
            PanelManager._Instance.loadingPanel.SetActive(false);
            PopupLine.Instance.Show("创建钱包失败！请重试");
            //StartCoroutine(ShakeErrorWhenCreateBip());
        }
        //Debug.Log(QRPayTools.GetMnemonic(bip, HardwareWallet.WalletType.BTC).PrivateKey.ToString());
        //var mainNetPrivateKey = new NBitcoin.BitcoinSecret(QRPayTools.GetMnemonic(bip).PrivateKey, NBitcoin.Network.Main);
        //var changeScriptPubKey = mainNetPrivateKey.PubKey.GetAddress(NBitcoin.Network.Main);
        //Debug.Log(changeScriptPubKey);

        //Debug.Log(QRPayTools.GetMnemonic(bip, HardwareWallet.WalletType.ETH).PrivateKey.ToString());
        //var ethEck = new EthECKey(QRPayTools.GetMnemonic(bip, HardwareWallet.WalletType.ETH).PrivateKey.ToBytes(), true);
        //string publicKey = ethEck.GetPublicAddress();
        //Debug.Log(publicKey);

        //Debug.Log(QRPayTools.GetMnemonic(bip, HardwareWallet.WalletType.EOS).PrivateKey.ToString());
        //ECKey ecKey = new ECKey(QRPayTools.GetMnemonic(bip, WalletType.EOS).PrivateKey.ToBytes(), true);
        //var pub_buf = ecKey.GetPubKey(true);
        //var checksum = Hashes.RIPEMD160(pub_buf, pub_buf.Length);
        //var addy = new byte[pub_buf.Length + 4];
        //Array.Copy(pub_buf, 0, addy, 0, pub_buf.Length);
        //Array.Copy(checksum, 0, addy, pub_buf.Length, 4);
        //string address = "EOS" + Encoders.Base58.EncodeData(addy);
        //Debug.Log(address);

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
