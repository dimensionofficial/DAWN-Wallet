using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransactionPanel : MonoBehaviour {

    public GameObject successObject;
    public GameObject sendingObject;
    public GameObject failureImage;

    public Text timeText;
    public Image QRImage;
    public Text value1;
    public Text hash;
    public string hashNumber;
    public Text from;
    public Text to;
    public Text value2;
    public Text gas;
    public Text date;
    public Text info;
    public Text blockNumber;

    public void CopyToAddress()
    {
        CMGE_Clipboard.CopyToClipboard(to.text);
        PopupLine.Instance.Show("复制成功");
    }

    public void CopyFromAddress()
    {
        CMGE_Clipboard.CopyToClipboard(from.text);
        PopupLine.Instance.Show("复制成功");
    }

    public void CopyHashNumber()
    {
        CMGE_Clipboard.CopyToClipboard(hashNumber);
        PopupLine.Instance.Show("复制成功");
    }

    public void Open(RecordPrefab prefab)
    {
        gameObject.SetActive(true);
        value1.text = prefab.value + prefab.type.text;
        value2.text = prefab.value + " " + prefab.type.text;
        hashNumber = prefab.hash;
        string one = prefab.hash.Substring(0, 6);
        string two = prefab.hash.Substring(prefab.hash.Length - 6, 6);
        hash.text = one + "..." + two;
        from.text = prefab.from;
        to.text = prefab.to;
        timeText.text = prefab.time;
        string unit = "";
        if (NewWalletManager._Intance.currentCoinType == NewWalletManager.CoinType.BTC|| NewWalletManager._Intance.currentCoinType == NewWalletManager.CoinType.USDT)
        {
            unit = "btc";
            string url = "https://btc.com/" + hashNumber;
            NewWalletManager._Intance.QRCodeManager.EncodeQRCode(QRImage, url);
        }
        else if (NewWalletManager._Intance.currentCoinType == NewWalletManager.CoinType.ETH)
        {
            unit = "eth";
            string url = "https://etherscan.io/tx/" + hashNumber;
            NewWalletManager._Intance.QRCodeManager.EncodeQRCode(QRImage, url);
        }
        else if (NewWalletManager._Intance.currentCoinType == NewWalletManager.CoinType.EOS)
        {
            unit = "eos";
        }
        gas.text = prefab.gas +" " + unit;
        date.text = prefab.time;
        string st = PlayerPrefs.GetString(prefab.hash);
        if (string.IsNullOrEmpty(st))
        {
            info.text = "无";
        }
        else
        {
            info.text = st;
        }

        blockNumber.text = prefab.blockNumber;

        if (prefab.isFailure)
        {
            sendingObject.SetActive(false);
            successObject.SetActive(false);
            failureImage.SetActive(true);
        }
        else
        {
            sendingObject.SetActive(true);
            successObject.SetActive(false);
            failureImage.SetActive(false);

            if (!string.IsNullOrEmpty(prefab.confixmations))
            {
                if (int.Parse(prefab.confixmations) > 0)
                {
                    sendingObject.SetActive(false);
                    successObject.SetActive(true);
                }
            }
        }
    }


    public void Open(string typeStr,string fromStr, string toStr, string valueStr, string coinName, string memoStr, string hashStr, string blockStr, string timeStr,string gasStr)
    {
        gas.transform.parent.gameObject.SetActive(true);
        gameObject.SetActive(true);
        sendingObject.SetActive(false);
        successObject.SetActive(true);
        failureImage.SetActive(false);
        timeText.text = timeStr;
        value1.text = valueStr + " " + coinName;
        value2.text = valueStr;
        gas.text = gasStr;
        from.text = fromStr;
        to.text = toStr;
        info.text = memoStr;
        hash.text = hashStr.Substring(0,6) + "..." + hashStr.Substring(hashStr.Length - 6,6);
        hashNumber = hashStr;
        blockNumber.text = blockStr;
        if (typeStr == "BTC")
        {
            string url = "https://btc.com/" + hashNumber;
            NewWalletManager._Intance.QRCodeManager.EncodeQRCode(QRImage, url);
        }
        else if (typeStr == "ETH" || typeStr == "ERC20")
        {
            string url = "https://etherscan.io/tx/" + hashNumber;
            NewWalletManager._Intance.QRCodeManager.EncodeQRCode(QRImage, url);
        }
        else if (typeStr == "EOS")
        {
            gas.transform.parent.gameObject.SetActive(false);
            string url = "https://eospark.com/MainNet/tx/" + hashNumber;
            NewWalletManager._Intance.QRCodeManager.EncodeQRCode(QRImage, url);
        }
    }
}
