using System.Collections;
using System.Collections.Generic;
using Nethereum.Util;
using Org.BouncyCastle.Math;
using UnityEngine;
using UnityEngine.UI;

public class EOSRegisterPanel : MonoBehaviour
{
  
    public Text ownerAddressText;
    public Text adminAddressText;
//  public InputField eosAccountInput;


    public Text registerRAM_text;
    public Text registerCUP_text;
    public Text registerNETWORK_text;


    public EOSWalletInfo eosWalletInfo;

    public EosItem _item;

    public GoodsInfo paymentOrderInfo;
    public Text costText;

    public void Show(EosItem item)
    {
        eosWalletInfo = item.eosWalletInfo;
        _item = item;
        gameObject.SetActive(true);

        ownerAddressText.text = eosWalletInfo.ownerAddress;
        adminAddressText.text = eosWalletInfo.adminAddress;
        registerRAM_text.text = EosGetSingInfoPanel.Instance.eos_register_ram.ToString();
        registerCUP_text.text = EosGetSingInfoPanel.Instance.eos_register_cpu.ToString();
        registerNETWORK_text.text = EosGetSingInfoPanel.Instance.eos_register_network.ToString();

        eosWalletInfo.eosRAM.max =decimal.Parse(EosGetSingInfoPanel.Instance.eos_register_ram.ToString());
        eosWalletInfo.eosCPU.max = decimal.Parse(EosGetSingInfoPanel.Instance.eos_register_cpu.ToString());
        eosWalletInfo.eosNetwork.max = decimal.Parse(EosGetSingInfoPanel.Instance.eos_register_network.ToString());

        paymentOrderInfo = NewWalletManager._Intance.GetPayOrder(GoodsInfo.GoodsType.EOS_CVT);

        if (paymentOrderInfo != null)
        {

            costText.text = "需要花费 " + paymentOrderInfo.price + " " + paymentOrderInfo.symbol;

        }
        
    }

    string accountRule = "12345abcdefghijklmnopqrstuvwxyz";

    public void OnClickCreatOrderBtn()
    {
        PanelManager._Instance._eosOrderPanel.Show(eosWalletInfo, paymentOrderInfo);

        //string account = eosAccountInput.text;

        //if (string.IsNullOrEmpty(account))
        //{
        //    PopupLine.Instance.Show("账户不能为空");
        //    return;
        //}
        //else
        //{
        //    if (account.Length != 12)
        //    {
        //        PopupLine.Instance.Show("账户必须是12位(小写字母和1-5的数字组合)");
        //        return;
        //    }

        //    for (int i = 0; i < account.Length; i++)
        //    {
        //        string temp = account[i].ToString();
        //        if (!accountRule.Contains(temp))
        //        {
        //            PopupLine.Instance.Show("账户必须是12位(小写字母和1-5的数字组合)");
        //            return;
        //        }
        //    }
        //}

        //eosWalletInfo.account = account;


        //PanelManager._Instance._eosResourcesPanel.loadingPanel.SetActive(true);
        //EosGetSingInfoPanel.Instance.GetAccount(eosWalletInfo.account, delegate (Hashtable t)
        //{
        //    PanelManager._Instance._eosResourcesPanel.loadingPanel.SetActive(false);
        //    if (t.ContainsKey("error"))
        //    {
        //        string er = t["error"].ToString();
        //        if (er.Equals("unspecified"))
        //        {
        //            PanelManager._Instance._eosOrderPanel.Show(eosWalletInfo, paymentOrderInfo);
        //            return;
        //        } 
        //    }

        //    PopupLine.Instance.Show("该账户已存在，请换一个重试");

        //}, delegate()
        //{
        //    PanelManager._Instance._eosResourcesPanel.loadingPanel.SetActive(false);
        //}, false);
    }

  
}
