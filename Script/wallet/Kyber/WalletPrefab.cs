using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WalletPrefab : MonoBehaviour {
    public Text walletText;
    public Image isSelected;
    public string address;
    public string walletName;
	// Use this for initialization
	 public void SelectWallet()
    {
        KyberTools.instance.SelectWallet(this);
    }

    public void ShutDownSelectPage()
    {
        KyberTools.instance.ShutDownWalletListPage();
    }
}
