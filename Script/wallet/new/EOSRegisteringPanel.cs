using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EOSRegisteringPanel : MonoBehaviour
{
    public Text accountText;
    public Text ownerAddressText;
    public Text adminAddressText;
    public Text registerRAM_text;
    public Text registerCUP_text;
    public Text registerNETWORK_text;

    private EOSWalletInfo eosInfo;
    public void Show(EOSWalletInfo eosWalletInfo)
    {
//      PlayerPrefs.SetString(eosWalletInfo.adminAddress + "registered", account);

        gameObject.SetActive(true);
        eosInfo = eosWalletInfo;
        eosInfo.account = eosWalletInfo.account;
        accountText.text = eosWalletInfo.account;
        ownerAddressText.text = eosInfo.ownerAddress;
        adminAddressText.text = eosInfo.adminAddress;
        registerRAM_text.text = EosGetSingInfoPanel.Instance.eos_register_ram.ToString();
        registerCUP_text.text = EosGetSingInfoPanel.Instance.eos_register_cpu.ToString();
        registerNETWORK_text.text = EosGetSingInfoPanel.Instance.eos_register_network.ToString();
    }

    public void OnClickClosed()
    {
        gameObject.SetActive(false);
    }
}
