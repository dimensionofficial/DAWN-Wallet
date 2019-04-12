using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MineExitPanel : HotBasePanel
{
    public Text userName;

    public override void Open()
    {
        string myName = NewWalletManager._Intance.userName;
        userName.text = "+" + myName.Substring(2, myName.Length - 2);
        base.Open();
    }



    public void OnClickExit()
    {
        NewWalletManager._Intance.CleanData();
        PanelManager._Instance._mainPanel.OnExitLogin();
        gameObject.SetActive(false);

        string userName = HttpManager._Intance.loginInfo[0];
        string[] info = new string[1] { userName };
        HttpManager._Intance.loginInfo = info;
        PlayerPrefsX.SetStringArray(HttpManager.LoginMark, info);

        PanelManager._Instance.OpenPanel(PanelManager._Instance._loginPanel, this);
    }
}
