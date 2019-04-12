using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WalletRenamePanel : HotBasePanel
{

    public InputField renameText;
    public Text oldNameText;
    public Text walletName;
    public Text noteText;

    public override void Open()
    {
        oldNameText.text = PanelManager._Instance._WalletInfoPanel.currentItem.coinInfo.walletname;
        base.Open();
    }

    public void OnClickOkRenameBtn()
    {
        //if (renameText.text == "" || renameText.text.Equals(oldNameText.text))
        //{
        //    Closed();
        //    return;
        //}

        //List<KeyValuePair<string, string>> ws = new List<KeyValuePair<string, string>>();
        //ws.Add(new KeyValuePair<string, string>("op", "renameWallet"));
        //ws.Add(new KeyValuePair<string, string>("userName", NewWalletManager._Intance.userName));
        //ws.Add(new KeyValuePair<string, string>("address", PanelManager._Instance._WalletInfoPanel.currentItem.coinInfo.address));
        //ws.Add(new KeyValuePair<string, string>("walletName", renameText.text));
        //StartCoroutine(HttpManager._Intance.SendRequest(ws, delegate (Hashtable table)
        //{
        //    int re = System.Convert.ToInt32(table["error"]);
        //    if (re == -1)
        //    {
        //        noteText.text = "网络异常,修改失败";
        //    }
        //    else if (re == 0)
        //    {
        //        Closed();
        //        noteText.text = "";
        //        walletName.text = renameText.text;
        //        PanelManager._Instance._WalletInfoPanel.walletName.text = renameText.text;
        //        PanelManager._Instance._WalletInfoPanel.currentItem.coinInfo.walletname = renameText.text;
        //        PanelManager._Instance._WalletInfoPanel.currentItem.nameText.text = renameText.text;
        //        PanelManager._Instance._WalletInfoPanel.currentItem.iconName.text = renameText.text.Substring(0,1);
        //        switch (PanelManager._Instance._WalletInfoPanel.currentItem.type)
        //        {
        //            case NewWalletManager.CoinType.BTC:
        //                NewWalletManager._Intance.bitcoinAddresListInfo[PanelManager._Instance._WalletInfoPanel.currentItem.coinInfo.address] = renameText.text;
        //                break;
        //            case NewWalletManager.CoinType.ETH:
        //                NewWalletManager._Intance.ethcoinAddresListInfo[PanelManager._Instance._WalletInfoPanel.currentItem.coinInfo.address] = renameText.text;
        //                break;
        //            case NewWalletManager.CoinType.EOS:
        //                NewWalletManager._Intance.eoscoinAddressListInfo[PanelManager._Instance._WalletInfoPanel.currentItem.coinInfo.address] = renameText.text;
        //                break;
        //        }
        //    }
        //    else
        //    {
        //        noteText.text = "网络异常，修改失败";
        //    }

        //}));
    }
}
