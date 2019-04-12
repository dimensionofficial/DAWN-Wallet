using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EOSResourcesCPuPanel : EOSResourcesBasePanel
{
    public override void InitObjct()
    {
        base.InitObjct();
        
    }

    public override void RefreshInfo()
    {
        base.RefreshInfo();

        //string temp = (PanelManager._Instance._eosResourcesPanel.eosInfo.balance / PanelManager._Instance._eosResourcesPanel.eosGlobalInfo.cpuPriceEos).ToString("f2");
        string cup_weight = PanelManager._Instance._eosResourcesPanel.eosInfo.self_delegated_bandwidth.cpu_weight;
        //decimal cpuDelegated = decimal.Parse(cup_weight.Substring(0, cup_weight.Length - 4));
        //string temp1 = (cpuDelegated / PanelManager._Instance._eosResourcesPanel.eosGlobalInfo.cpuPriceEos).ToString("f2");
        if (receiver == ResouresReceiver.Myslef)
        {
            reMyslef.eosCountText.text = "(" + PanelManager._Instance._eosResourcesPanel.eosInfo.balance.ToString() + " EOS)";
            reMyslef.cancelNumberText.text = "(" + cup_weight + ")";
        }
        else if (receiver == ResouresReceiver.Other)
        {
            reOther.eosCountText.text = "(" + PanelManager._Instance._eosResourcesPanel.eosInfo.balance.ToString() + " EOS)";
            reOther.eosCancelText.text = "(" + cup_weight + ")";
        }
    }

    public void DelegatebwCPU()
    {
        string sender = PanelManager._Instance._eosResourcesPanel.eosInfo.account;
        string reciverName;
        string number;
        if (receiver == ResouresReceiver.Myslef)
        {
            reciverName = sender;
            number = reMyslef.affirmText.text;

        }
        else if (receiver == ResouresReceiver.Other)
        {
            reciverName = reOther.accountText.text;
            number = reOther.affirmText.text;

        }
        else
        {
            return;
        }

        if (string.IsNullOrEmpty(reciverName))
        {
            PopupLine.Instance.Show("请填写抵押的账户");
            return;
        }

        if (string.IsNullOrEmpty(number))
        {
            PopupLine.Instance.Show("请输入抵押数量");
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
            EosGetSingInfoPanel.Instance.Delegatebw(PanelManager._Instance._eosResourcesPanel.eosInfo.adminAddress, EOSSendScanSing.LastPanel.delegatebw, sender, reciverName, "0", number);
        }
        else
        {
            PopUpBox.Instance.Show(null, null, "确定", "", "", "资源不足");
        }
       
    }

    public void UndelegatebwCPU()
    {
        string sender = PanelManager._Instance._eosResourcesPanel.eosInfo.account;
        string reciverName;
        string number;
        if (receiver == ResouresReceiver.Myslef)
        {
            reciverName = sender;
            number = reMyslef.cancelText.text;
        }
        else if (receiver == ResouresReceiver.Other)
        {
            reciverName = reOther.accountText.text;
            number = reOther.cancelText.text;

        }
        else
        {
            return;
        }

        if (string.IsNullOrEmpty(number))
        {
            PopupLine.Instance.Show("请输入抵押数量");
            return;
        }

        decimal t = decimal.Parse(number);
        string cpuW = PanelManager._Instance._eosResourcesPanel.eosInfo.self_delegated_bandwidth.cpu_weight;
        decimal tp = decimal.Parse(cpuW.Substring(0, cpuW.Length - 4));
        if (t > tp)
        {
            PopupLine.Instance.Show("余额不足");
            return;
        }

        
        if (PanelManager._Instance._eosResourcesPanel.JudgeResourceLimit())
        {
            EosGetSingInfoPanel.Instance.Undelegatebw(PanelManager._Instance._eosResourcesPanel.eosInfo.adminAddress, EOSSendScanSing.LastPanel.undelegatebw, sender, reciverName, "0", number);
        }
        else
        {
            PopUpBox.Instance.Show(null, null, "确定", "", "", "资源不足");
        }
    }
}
