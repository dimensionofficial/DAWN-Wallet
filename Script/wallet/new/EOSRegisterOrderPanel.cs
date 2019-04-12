using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EOSRegisterOrderPanel : MonoBehaviour
{
   
    public Transform parent;
    public GameObject cloneObject;
    public List<EosPaymentwalletItem> itemList = new List<EosPaymentwalletItem>();
    public EosPaymentwalletItem currentPayWallet;

//    public Text accountText;
    public Text ownerAddressText;
    public Text adminAddressText;
    public Text registerRAM_text;
    public Text registerCUP_text;
    public Text registerNETWORK_text;
    public Text costText;

    private EOSWalletInfo m_eosWalletInfo;

    

    public GameObject payMentObject;

    public GoodsInfo orderInfo;

    private decimal haveValue;

    public void Show(EOSWalletInfo eosWalletInfo, GoodsInfo payorder)
    {
        payMentObject.SetActive(false);
        gameObject.SetActive(true);
        m_eosWalletInfo = eosWalletInfo;
        orderInfo = payorder;
//        accountText.text = eosWalletInfo.account;
        ownerAddressText.text = eosWalletInfo.ownerAddress;
        adminAddressText.text = eosWalletInfo.adminAddress;
        costText.text = payorder.price + " " + payorder.symbol;
        registerRAM_text.text = EosGetSingInfoPanel.Instance.eos_register_ram.ToString();
        registerCUP_text.text = EosGetSingInfoPanel.Instance.eos_register_cpu.ToString();
        registerNETWORK_text.text = EosGetSingInfoPanel.Instance.eos_register_network.ToString();
    }

    public void OnClickPaymentBtn()
    {
        currentPayWallet = null;

        for (int i = 0; i < itemList.Count; i++)
        {
           DestroyImmediate(itemList[i].gameObject);
        }
        itemList.Clear();

        payMentObject.SetActive(true);

        foreach (var kvp in PanelManager._Instance._mainPanel.ethItemList)
        {
            GameObject g = GameObject.Instantiate(cloneObject) as GameObject;
            g.transform.SetParent(parent,false);
            EosPaymentwalletItem itemp = g.GetComponent<EosPaymentwalletItem>();
            itemp.Show(kvp.Value);
            itemList.Add(itemp);
        }
    }



    public void OnClickSelectPayItem(EosPaymentwalletItem item, decimal _haveValue)
    {
        if (currentPayWallet != null)
            currentPayWallet.selectMark.SetActive(false);

        haveValue = _haveValue;
        currentPayWallet = item;
        currentPayWallet.selectMark.SetActive(true);
    }

    public void OnPayment()
    {
        if (currentPayWallet == null)
        {
            PopupLine.Instance.Show("请选择支付钱包");
            return;
        }
        PanelManager._Instance._eosPaymentPanel.Open(currentPayWallet.walletItem, m_eosWalletInfo, haveValue);
  //    CreatEosAccount();
    }

   
}
