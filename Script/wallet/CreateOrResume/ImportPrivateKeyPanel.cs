using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImportPrivateKeyPanel : MonoBehaviour
{
    public Sprite normarMark;
    public Sprite selectMark;

    public GameObject eosnotice;

    public enum PrivateKeyTypeEnum
    {
        BTC = 0,
        ETH,
        EOS,
    }
    public PrivateKeyTypeEnum currentKeyType = PrivateKeyTypeEnum.BTC;

    public Text keyNameText;

    public Text noteText;


    public NativeEditBox nativaeEditoBox;
    public InputField privateInputText;

    public InputField walletNameText;

    public GameObject selectCoinTypeObject;

    public List<Image> selectIamge = new List<Image>();

    private void ShowTypeText()
    {
        eosnotice.SetActive(false);
        switch (currentKeyType)
        {
            case PrivateKeyTypeEnum.BTC:
                keyNameText.text = "BTC";
                break;
            case PrivateKeyTypeEnum.ETH:
                keyNameText.text = "ETH";
                break;
            case PrivateKeyTypeEnum.EOS:
                eosnotice.SetActive(true);
                keyNameText.text = "EOS";
                break;
        }
    }

    public void OpenMe()
    {
        eosnotice.SetActive(false);
        noteText.text = "";
        gameObject.SetActive(true);
        selectCoinTypeObject.SetActive(false);
        nativaeEditoBox.text = "";
        privateInputText.text = "";
        walletNameText.text = "";
        currentKeyType = PrivateKeyTypeEnum.BTC;
        ShowTypeText();
        nativaeEditoBox.gameObject.SetActive(true);
    }

    public void OpenSelectPanel()
    {
        selectCoinTypeObject.SetActive(true);
        SHowCoinMark();
        nativaeEditoBox.gameObject.SetActive(false);
    }

    private void SHowCoinMark()
    {
        for (int i = 0; i < selectIamge.Count; i++)
        {
            selectIamge[i].sprite = normarMark;
        }

        selectIamge[(int)currentKeyType].sprite = selectMark;
    }

    public void OnSelectCoin(Image target)
    {
        for (int i = 0; i < selectIamge.Count; i++)
        {
            if (target == selectIamge[i])
            {
                currentKeyType = (PrivateKeyTypeEnum)(i);
                break;
            }
        }

        SHowCoinMark();

        CloseSelectPanel();

        ShowTypeText();
    }

    public void CloseSelectPanel()
    {
        nativaeEditoBox.gameObject.SetActive(true);
        selectCoinTypeObject.SetActive(false);

    }

    public void OnClickScanPanl()
    {
        nativaeEditoBox.gameObject.SetActive(false);
        PanelManager._Instance._openCameraScanPanel.OpenMe(OnScanEnd,delegate() { nativaeEditoBox.gameObject.SetActive(true); });
    }

    public void OnImportBtn()
    {
        string privat = nativaeEditoBox.text;
        if (string.IsNullOrEmpty(privat))
        {
            privat = privateInputText.text;
        }
        privat = privat.Replace(" ", "");

        if (string.IsNullOrEmpty(privat))
        {
            ShowNoticeText("请输入私钥");
            return;
        }

        if (string.IsNullOrEmpty(walletNameText.text))
        {
            ShowNoticeText("钱包名称不能为空");
            return;
        }

        OutsideKeyStore ousideKey = new OutsideKeyStore();
        string address = "";
        NewWalletManager.CoinType coinType = NewWalletManager.CoinType.BTC;
        switch (currentKeyType)
        {
            case PrivateKeyTypeEnum.BTC:
                address = ousideKey.GetBtcAddressByPrivateKey(privat);
                coinType = NewWalletManager.CoinType.BTC;
                break;
            case PrivateKeyTypeEnum.ETH:
                address = ousideKey.GetEthAddressByPrivateKey(privat);
                coinType = NewWalletManager.CoinType.ETH;
                break;
            case PrivateKeyTypeEnum.EOS:

                address = ousideKey.GetEosAddess(privat);

                //if (!string.IsNullOrEmpty(address))
                //{
                //    if (privat.Length == 51)
                //    {
                //        if (!privat.StartsWith("5"))
                //        {
                //                ShowNoticeText("导入的EOS私钥不是管理者私钥");
                //                return;
                //        }
                //    }
                //    else
                //    {
                //       string bas58 = ousideKey.GetEosBas58Private(privat);
                //        Debug.Log(bas58 + "  Count = " + bas58.Length);
                //        if (bas58.Length != 51 || !bas58.StartsWith("5"))
                //        {
                //            ShowNoticeText("导入的EOS私钥不是管理者私钥");
                //            return;
                //        }
                //    }
                //}

                coinType = NewWalletManager.CoinType.EOS;
                break;
        }

        if (string.IsNullOrEmpty(address))
        {
            ShowNoticeText("私钥不正确");
            return;
        }
        if (NewWalletManager._Intance.IsSameAsAddress(address, coinType))
        {
            ShowNoticeText("该私钥已导入过");
            return;
        }

        PanelManager._Instance.loadingPanel.SetActive(true);
        SeedKeyManager.Instance.SavaAddressToServer(walletNameText.text, address, coinType, delegate()
        {
            PanelManager._Instance.loadingPanel.SetActive(false);
            gameObject.SetActive(false);
            PopupLine.Instance.Show("导入成功");
        }, delegate() 
        {
            PanelManager._Instance.loadingPanel.SetActive(false);
        });
    }

    private void OnScanEnd(string str)
    {
        nativaeEditoBox.gameObject.SetActive(true);
        nativaeEditoBox.text = str;
        privateInputText.text = str;
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
