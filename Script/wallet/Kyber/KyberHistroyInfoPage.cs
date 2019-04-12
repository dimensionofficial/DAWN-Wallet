using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KyberHistroyInfoPage : MonoBehaviour {

    public GameObject pendingObj;
    public GameObject successObj;
    public GameObject faildObj;
    public Text date;
    public Text paid;
    public Text get;
    public Text gas;
    public Text desAddress;
    public Text srcAddress;
    public Text comment;
    public Text hash;
    public Text block;
    public Image qrImage;
    public string hashStr;

    public void CopyToAddress()
    {
        CMGE_Clipboard.CopyToClipboard(desAddress.text);
        PopupLine.Instance.Show("复制成功");
    }

    public void CopyFromAddress()
    {
        CMGE_Clipboard.CopyToClipboard(srcAddress.text);
        PopupLine.Instance.Show("复制成功");
    }

    public void CopyHashNumber()
    {
        CMGE_Clipboard.CopyToClipboard(hashStr);
        PopupLine.Instance.Show("复制成功");
    }

    public void Show(KyberHistoryPrefab kyberHistory)
    {
        gameObject.SetActive(true);
        faildObj.SetActive(false);
        pendingObj.SetActive(false);
        successObj.SetActive(false);
        switch (kyberHistory.exchangeState)
        {
            case ExchangeState.Faild:
                faildObj.SetActive(true);
                break;
            case ExchangeState.Pending:
                pendingObj.SetActive(true);
                break;
            case ExchangeState.Success:
                successObj.SetActive(true);
                break;
            default:
                break;
        }
        if(!kyberHistory.isApprove)
        {
            paid.text = kyberHistory.paid.text + kyberHistory.from.text;
            get.text = kyberHistory.get.text + kyberHistory.to.text;
        }
        else
        {
            paid.text = kyberHistory.value;
        }
        date.text = kyberHistory.data.text;
        gas.text = kyberHistory.gas;
        srcAddress.text = kyberHistory.paidAddress;
        desAddress.text = kyberHistory.getAddress;
        hash.text = kyberHistory.hax.Substring(0,6) + "..." + kyberHistory.hax.Substring(kyberHistory.hax.Length - 6, 6);
        hashStr = kyberHistory.hax;
        block.text = kyberHistory.block;
        string url = "https://etherscan.io/tx/" + kyberHistory.hax;
        NewWalletManager._Intance.QRCodeManager.EncodeQRCode(qrImage, url);
    }


    public void Show(ApproveHistoryPrefab kyberHistory)
    {
        gameObject.SetActive(true);
        faildObj.SetActive(false);
        pendingObj.SetActive(false);
        successObj.SetActive(false);
        switch (kyberHistory.exchangeState)
        {
            case ExchangeState.Faild:
                faildObj.SetActive(true);
                break;
            case ExchangeState.Pending:
                pendingObj.SetActive(true);
                break;
            case ExchangeState.Success:
                successObj.SetActive(true);
                break;
            default:
                break;
        }
        paid.text = kyberHistory.value;
        date.text = kyberHistory.date.text;
        gas.text = kyberHistory.gas;
        srcAddress.text = kyberHistory.from;
        desAddress.text = kyberHistory.des;
        hash.text = kyberHistory.hash.Substring(0, 6) + "..." + kyberHistory.hash.Substring(kyberHistory.hash.Length - 6, 6);
        block.text = kyberHistory.block;
        hashStr = kyberHistory.hash;
        string url = "https://etherscan.io/tx/" + kyberHistory.hash;
        NewWalletManager._Intance.QRCodeManager.EncodeQRCode(qrImage, url);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
