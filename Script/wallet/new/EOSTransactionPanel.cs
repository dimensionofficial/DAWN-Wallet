using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EOSTransactionPanel : MonoBehaviour {

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

    public void Open(EosRecordPrefab prefab)
    {
        gameObject.SetActive(true);
        switch (prefab.eosHistoryRcord.actType)
        {
            case EOSHistoryRcord.ActType.refund:
            case EOSHistoryRcord.ActType.buyrambytes:
            case EOSHistoryRcord.ActType.transfer:
            case EOSHistoryRcord.ActType.sellram:
            case EOSHistoryRcord.ActType.buyram:
                from.text = prefab.eosHistoryRcord.transferdata.from;
                to.text = prefab.eosHistoryRcord.transferdata.to;
                value1.text = prefab.eosHistoryRcord.transferdata.quantity;
                value2.text = prefab.eosHistoryRcord.transferdata.quantity;
                break;
            case EOSHistoryRcord.ActType.delegatebw:
                from.text = prefab.eosHistoryRcord.delegatebwdata.from;
                to.text = prefab.eosHistoryRcord.delegatebwdata.receiver;
                string a1 = prefab.eosHistoryRcord.delegatebwdata.stake_cpu_quantity;
                decimal c1 = decimal.Parse(a1.Substring(0, a1.Length - 4));
                string b1 = prefab.eosHistoryRcord.delegatebwdata.stake_net_quantity;
                decimal n1 = decimal.Parse(b1.Substring(0, b1.Length - 4));
                decimal t1 = c1 + n1;
                value1.text = t1 + " EOS";
                value2.text = t1 + " EOS";
                break;
            case EOSHistoryRcord.ActType.undelegatebw:
                from.text = prefab.eosHistoryRcord.undelegatebwdata.from;
                to.text = prefab.eosHistoryRcord.undelegatebwdata.receiver;
                string a = prefab.eosHistoryRcord.undelegatebwdata.unstake_cpu_quantity;
                decimal c =decimal.Parse(a.Substring(0, a.Length - 4));
                string b = prefab.eosHistoryRcord.undelegatebwdata.unstake_net_quantity;
                decimal n = decimal.Parse(b.Substring(0, b.Length - 4));
                decimal t = c + n;
                value1.text = t + " EOS";
                value2.text = t + " EOS";
                break;
        }
   //    value1.text = prefab.value + prefab.type.text;
        //value2.text = prefab.value + " " + prefab.type.text;
        hashNumber = prefab.eosHistoryRcord.trx_id;
        string one = hashNumber.Substring(0, 6);
        string two = hashNumber.Substring(hashNumber.Length - 6, 6);
        hash.text = one + "..." + two;
        //from.text = prefab.from;
        //to.text = prefab.to;
        timeText.text = prefab.eosHistoryRcord.block_time;
        string url = "https://eospark.com/MainNet/tx/" + hashNumber;
        NewWalletManager._Intance.QRCodeManager.EncodeQRCode(QRImage, url);
        string st = prefab.eosHistoryRcord.transferdata.memo;
        if (string.IsNullOrEmpty(st))
        {
            info.text = "无";
        }
        else
        {
            info.text = st;
        }

        blockNumber.text = prefab.eosHistoryRcord.block_num;

        sendingObject.SetActive(false);
        failureImage.SetActive(false);
        successObject.SetActive(true);
        //if (prefab.isFailure)
        //{
        //    sendingObject.SetActive(false);
        //    successObject.SetActive(false);
        //    failureImage.SetActive(true);
        //}
        //else
        //{
        //    sendingObject.SetActive(true);
        //    successObject.SetActive(false);
        //    failureImage.SetActive(false);

        //    if (!string.IsNullOrEmpty(prefab.confixmations))
        //    {
        //        if (int.Parse(prefab.confixmations) > 0)
        //        {
        //            sendingObject.SetActive(false);
        //            successObject.SetActive(true);
        //        }
        //    }
        //}
    }

    public void ShowDetail(MessageCenter.MessageType messageType,EthHistory history)
    {

    }
}
