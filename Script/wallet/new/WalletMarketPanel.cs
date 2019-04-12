using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalletMarketPanel : MonoBehaviour {

    public void Open()
    {
        PanelManager._Instance.currentSubPage = PanelManager.SubPage.HangQing;
        gameObject.SetActive(true);
    }
}
