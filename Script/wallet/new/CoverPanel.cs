using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverPanel : HotBasePanel
{

    public MainPanel mainPanel;

    public GameObject object1;
    public GameObject object2;

    //void Start()
    //{
    //    if (PlayerPrefs.HasKey(WalletManager.EmailAddress))
    //    {
    //        string email = PlayerPrefs.GetString(WalletManager.EmailAddress);
    //        if (email != "")
    //        {
    //            SkipMianPanel();
    //            return;
    //        }
            
    //    }
    //        object1.SetActive(true);
    //        object2.gameObject.SetActive(false);
    //}

    public void OnClickNext1Btn()
    {
        object1.SetActive(false);
        object2.SetActive(true);
    }

    public void OnClickLoginBtn()
    {
        PanelManager._Instance._loginPanel.gameObject.SetActive(true);
    }

    public void OnClickRegisterBtn()
    {
        PanelManager._Instance._loginPanel.OnRegisterPanel();
    }

    //public void OnClickNext2Btn()
    //{
    //    object2.gameObject.SetActive(false);
    //}

    //public void SkipMianPanel()
    //{
    //    gameObject.SetActive(false);
    //    mainPanel.Open();
    //}

}
