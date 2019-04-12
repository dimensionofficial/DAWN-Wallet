using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class EosRecordPrefab : MonoBehaviour
{
    public Color normalColor;
    public Color receiveColor;
    public Color myselfReceiveColor;
    public Color sendColor;
    public GameObject failure;
    public GameObject sendingObjct;
	public Image icon;
    public Text outOrIN;
    public Text date;
    public Text coin;
    public Text fromText;
    public Button Open;
    public bool isFailure;
    public HistroyRecord.TabType tabtype;

    public EOSHistoryRcord eosHistoryRcord;

    private int SetSourceType(string myAccount, string from, string to)
    {
        if (myAccount.Equals(to))
        {
            if (myAccount.Equals(from))
            {
                outOrIN.color = myselfReceiveColor;
                return 1;
            }
            else
            {
                outOrIN.color = receiveColor;
                return 2;
            }

        }
        else
        {
            outOrIN.color = sendColor;
            return 3;
        }
    }

    public void Init(EOSHistoryRcord eosHistory, string myAccount)
    {
        gameObject.SetActive(true);
        
        eosHistoryRcord = eosHistory;
  //      DateTime d = new DateTime()
        date.text = eosHistory.block_time;
        int type;
        switch (eosHistory.actType)
        {
            case EOSHistoryRcord.ActType.refund:
            case EOSHistoryRcord.ActType.transfer:

                type = SetSourceType(myAccount, eosHistory.transferdata.from, eosHistory.transferdata.to);
                if (type == 1)
                {
                    outOrIN.text = "自收";
                    icon.overrideSprite = TextureUIAsset._Instance.myselMark;
                }
                else if (type == 2)
                {
                    outOrIN.text = "转入";
                    icon.overrideSprite = TextureUIAsset._Instance.getMark;
                }
                else
                {
                    outOrIN.text = "转出";
                    icon.overrideSprite = TextureUIAsset._Instance.sendMark;
                }

                fromText.text = "发到:" + eosHistory.transferdata.to;
                coin.text = eosHistory.transferdata.quantity;
                break;

            case EOSHistoryRcord.ActType.delegatebw:

                icon.overrideSprite = TextureUIAsset._Instance.eos_delegatebw_in;

                type = SetSourceType(myAccount, eosHistory.delegatebwdata.from, eosHistory.delegatebwdata.receiver);

                outOrIN.text = "抵押";
                fromText.text = "抵押给:" + eosHistory.delegatebwdata.receiver;
                
                string a1 = eosHistory.delegatebwdata.stake_cpu_quantity;
                decimal c1 = decimal.Parse(a1.Substring(0, a1.Length - 4));
                string b1 = eosHistory.delegatebwdata.stake_net_quantity;
                decimal n1 = decimal.Parse(b1.Substring(0, b1.Length - 4));
                decimal t1 = c1 + n1;
                coin.text = t1 + " EOS"; ;
                break;

            case EOSHistoryRcord.ActType.undelegatebw:
                outOrIN.color = receiveColor;
                outOrIN.text = "取消抵押";
                icon.overrideSprite = TextureUIAsset._Instance.eos_delegatebw_out;
                string a = eosHistory.undelegatebwdata.unstake_cpu_quantity;
                decimal c = decimal.Parse(a.Substring(0, a.Length - 4));
                string b = eosHistory.undelegatebwdata.unstake_net_quantity;
                decimal n = decimal.Parse(b.Substring(0, b.Length - 4));
                decimal t = c + n;
                coin.text = t + " EOS";
                fromText.text = "接收方:" + eosHistory.undelegatebwdata.receiver;
                break;

            case EOSHistoryRcord.ActType.buyram:
                // type = SetSourceType(myAccount, , eosHistory.buramData.receiver);
                outOrIN.color = sendColor;
                icon.overrideSprite = TextureUIAsset._Instance.eos_ram_in;
                outOrIN.text = "买内存";

                fromText.text = "接收方:" + eosHistory.buramData.receiver;

                coin.text = eosHistory.buramData.quant;

               
                break;

            case EOSHistoryRcord.ActType.sellram:
                outOrIN.color = receiveColor;
                outOrIN.text = "卖内存";
                icon.overrideSprite = TextureUIAsset._Instance.eos_ram_out;
                fromText.text = "出售方:" + eosHistory.transferdata.from;
                coin.text = eosHistory.transferdata.quantity;
                break;

            case EOSHistoryRcord.ActType.buyrambytes:

                outOrIN.color = sendColor;
                icon.overrideSprite = TextureUIAsset._Instance.eos_ram_in;
                outOrIN.text = "买内存";
                fromText.text = "接收方:" + eosHistory.buyrambytesdata.receiver;

                coin.text = eosHistory.transferdata.quantity;

                break;
        }
    }

    public void OnClickMe()
    {
        PanelManager._Instance._eosTransactionPanel.Open(this);
    }
}
