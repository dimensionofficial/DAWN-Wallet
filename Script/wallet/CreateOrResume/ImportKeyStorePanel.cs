using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImportKeyStorePanel : MonoBehaviour {


    public NativeEditBox nativaeEditoBox;
    public InputField privateInputText;

    public InputField walletNameText;

    public InputField keyStroePass;

    public Text noteText;

    public void OpenMe()
    {
        noteText.text = "";
        gameObject.SetActive(true);
        nativaeEditoBox.text = "";
        privateInputText.text = "";
        keyStroePass.text = "";
        walletNameText.text = "";
        nativaeEditoBox.gameObject.SetActive(true);
    }

    public void OnClickScanPanl()
    {
        nativaeEditoBox.gameObject.SetActive(false);
        PanelManager._Instance._openCameraScanPanel.OpenMe(OnScanEnd, delegate () { nativaeEditoBox.gameObject.SetActive(true); });
    }

    private void OnScanEnd(string str)
    {
        nativaeEditoBox.gameObject.SetActive(true);
        nativaeEditoBox.text = str;
        privateInputText.text = str;
    }

    public void OnImportBtn()
    {
        string keystoreJson = nativaeEditoBox.text;
        if (string.IsNullOrEmpty(keystoreJson))
        {
            keystoreJson = privateInputText.text;
        }
        keystoreJson = keystoreJson.Replace(" ", "");

        if (string.IsNullOrEmpty(keystoreJson))
        {
            ShowNoticeText("请输入keystore");
            return;
        }

        if (string.IsNullOrEmpty(walletNameText.text))
        {
            ShowNoticeText("钱包名称不能为空");
            return;
        }

        OutsideKeyStore ousideKey = new OutsideKeyStore();

        string keyStr = ousideKey.GetPrivateKeyByKeyStoreJson(keyStroePass.text, keystoreJson);

        if (string.IsNullOrEmpty(keyStr))
        {
            ShowNoticeText("keystory 或 keystory密码输入不正确");
            return;
        }

        string address = ousideKey.GetEthAddressByPrivateKey(keyStr);

        if (string.IsNullOrEmpty(address))
        {
            ShowNoticeText("keystory 或 keystory密码输入不正确");
            return;
        }

        if (NewWalletManager._Intance.IsSameAsAddress(address, NewWalletManager.CoinType.ETH))
        {
            ShowNoticeText("该keystoryd对应的钱包已导入过");
            return;
        }

        PanelManager._Instance.loadingPanel.SetActive(true);
        SeedKeyManager.Instance.SavaAddressToServer(walletNameText.text, address, NewWalletManager.CoinType.ETH, delegate ()
        {
            PanelManager._Instance.loadingPanel.SetActive(false);
            gameObject.SetActive(false);
            PopupLine.Instance.Show("导入成功");
        }, delegate ()
        {
            PanelManager._Instance.loadingPanel.SetActive(false);
        });

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
