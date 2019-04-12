using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReturnWalletInfo : MonoBehaviour {
    public Text address;
    public Text btn_tex;
    public Image btn_img;
    public Button button;
    public SendCoinPanel SendCoinPanel;
    public void ReturnAddress()
    {
        string[] addr = address.text.Split(':');
        SendCoinPanel.curWalletAddress = addr[1];
        foreach (var item in SendCoinPanel.oldwalletsAddress)
        {
            if (addr[1]==item)
            {
                button.interactable = false;
                btn_tex.gameObject.SetActive(true);
                btn_img.gameObject.SetActive(false);
                return;
            }
        }
        SendCoinPanel.Send2();
    }
    public void Rest()
    {
        button.interactable = true;
        btn_tex.gameObject.SetActive(false);
        btn_img.gameObject.SetActive(true);
        address.text = null;
    }
}
