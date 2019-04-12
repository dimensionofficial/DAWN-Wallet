using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EOSResourcesARMPanel : EOSResourcesBasePanel
{
    public override void InitObjct()
    {
        base.InitObjct();
    }

    public override void RefreshInfo()
    {
        string temp = (PanelManager._Instance._eosResourcesPanel.eosInfo.balance / PanelManager._Instance._eosResourcesPanel.eosGlobalInfo.ramPriceEos).ToString("f2");

        if (receiver == ResouresReceiver.Myslef)
        {
            //<color=#DCDCDC>≈7.10KB</color>

            
            reMyslef.eosCountText.text = "(" + PanelManager._Instance._eosResourcesPanel.eosInfo.balance.ToString() + " EOS)<color=#C9C9C9>≈" + temp + "KB</color>";
            string tempEos = (decimal.Parse(PanelManager._Instance._eosResourcesPanel.eosInfo.total_resources.ram_bytes) / (decimal)1024 * PanelManager._Instance._eosResourcesPanel.eosGlobalInfo.ramPriceEos).ToString("f2");
            reMyslef.cancelNumberText.text = "(" + PanelManager._Instance._eosResourcesPanel.eosInfo.total_resources.ram_bytes + "bytes)<color=#C9C9C9>≈" + tempEos + "EOS</color>";
        }
        else if (receiver == ResouresReceiver.Other)
        {
            reOther.eosCountText.text = "(" + PanelManager._Instance._eosResourcesPanel.eosInfo.balance.ToString() + " EOS)<color=#C9C9C9>≈" + temp + "KB</color>";
        }
    }

    public void BuyARM()
    {
        string senderName = PanelManager._Instance._eosResourcesPanel.eosInfo.account;
        string reciverName ;
        string number;
        if (receiver == ResouresReceiver.Myslef)
        {          
            reciverName = senderName;
            number = reMyslef.affirmText.text;

        } else if (receiver == ResouresReceiver.Other)
        {
            senderName = PanelManager._Instance._eosResourcesPanel.eosInfo.account;
            reciverName = reOther.accountText.text;
            number = reOther.affirmText.text;

        } else
        {
            return;
        }

        if (string.IsNullOrEmpty(reciverName))
        {
            PopupLine.Instance.Show("请填写接收人的账户");
            return;
        }

        if (string.IsNullOrEmpty(number))
        {
            PopupLine.Instance.Show("请输入购买数量");
            return;
        }

        decimal t = decimal.Parse(number);
        if (t > PanelManager._Instance._eosResourcesPanel.eosInfo.balance)
        {
            PopupLine.Instance.Show("余额不足");
            return;
        }

        if (PanelManager._Instance._eosResourcesPanel.JudgeResourceLimit())
        {
            EosGetSingInfoPanel.Instance.BuyramEos(PanelManager._Instance._eosResourcesPanel.eosInfo.adminAddress, EOSSendScanSing.LastPanel.buyram, PanelManager._Instance._eosResourcesPanel.eosInfo.account, reciverName, number);
        }
        else
        {
            PopUpBox.Instance.Show(null, null, "确定", "", "", "资源不足");
        }

    }


    public void SellARM()
    {
        string seller;
        string number;
        if (receiver == ResouresReceiver.Myslef)
        {
            seller = PanelManager._Instance._eosResourcesPanel.eosInfo.account;
            number = reMyslef.cancelText.text;
        }
        else
        {
            return;
        }

        if (string.IsNullOrEmpty(number))
        {
            PopupLine.Instance.Show("请输入出售数量");
            return;
        }

        decimal t = decimal.Parse(number);
        decimal tp = decimal.Parse(PanelManager._Instance._eosResourcesPanel.eosInfo.total_resources.ram_bytes);
        if (t > tp)
        {
            PopupLine.Instance.Show("余额不足");
            return;
        }
        if (PanelManager._Instance._eosResourcesPanel.JudgeResourceLimit())
        {
            EosGetSingInfoPanel.Instance.Sellram(PanelManager._Instance._eosResourcesPanel.eosInfo.adminAddress, EOSSendScanSing.LastPanel.sellram, seller, number);
        }
        else
        {
            PopUpBox.Instance.Show(null, null, "确定", "", "", "资源不足");
        } 
    }
}
