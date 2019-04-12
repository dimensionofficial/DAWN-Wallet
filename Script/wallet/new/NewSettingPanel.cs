using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class NewSettingPanel : MonoBehaviour
{
    public List<GameObject> hotWalletBtn = new List<GameObject>();

    public Text userNameText;
    public ShowWebView helpPanel;

    public Text versionText;
    public ShowWebView aboutWeb;
    private bool isShowAboutWeb;

	public GameObject VerPanel;
    public Text verNumberText;
    public Image logoImage;

    public GameObject changePassObject;

    public void Open()
    {
        versionText.text = NewWalletManager._Intance.versionNumber;
        verNumberText.text = "版本号 " + NewWalletManager._Intance.versionNumber;
        if (NewWalletManager._Intance.IsNeedColdWallet)
        {
            logoImage.sprite = TextureUIAsset._Instance.cLogo;
            for (int i = 0; i < hotWalletBtn.Count; i++)
            {
                hotWalletBtn[i].SetActive(false);
            }

            changePassObject.SetActive(false);
        }
        else
        {
            logoImage.sprite = TextureUIAsset._Instance.hLogo;
            for (int i = 0; i < hotWalletBtn.Count; i++)
            {
                hotWalletBtn[i].SetActive(true);
            }

            changePassObject.SetActive(true);
            if (SeedKeyManager.Instance.IsBackUp())
            {
                hotWalletBtn[0].SetActive(false);
            }
        }

        string myName = NewWalletManager._Intance.userName;
        userNameText.text = "+" + myName.Substring(2, myName.Length - 2);
        gameObject.SetActive(true);
        ShareManager.init.shareStr = "Dimension钱包 · 资产的保险箱 http://dimensionchain.io/";

    }


    public void OnClickExitAndClear()
    {

        PopUpBox.Instance.Show(delegate () 
        {
            if (!SeedKeyManager.Instance.IsBackUp())
            {
                PopUpBox.Instance.Show(delegate ()
                {
                    PanelManager._Instance._backUpPrivateKeyPanel.OpenMe();
                }, delegate ()
                {
                    OnResetLogin();
                }, "马上备份", "不需要", "重要提示", "您尚未备份钱包,如无妥善备份,退出登录后将无法找回钱包");
            }
            else
            {
                OnResetLogin();
            }
        }, delegate () 
        {

        }, "知道了", "再想想", "重要提示", "退出登录将清空当前钱包所有数据，请慎重处理改操作！！");
        
    }

    private void OnResetLogin()
    {
        PanelManager._Instance._mainPanel.CleanDic();
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("wallet");
        PlayerPrefs.SetInt("ResetLoginAgain",1);
    }

	public void OnClickVerPanelBtn()
	{
		VerPanel.SetActive (true);
	}

    public void OnClickHelpBtn()
    {
        //Application.OpenURL("http://dimension.io");
        // helpPanel.gameObject.SetActive(true);
        // NewWalletManager._Intance.DOTweenCome(helpPanel.transform, -1000, 0);
        helpPanel.ShowClicked();
    }

    public void OnClickBackHelpPanel()
    {
        helpPanel.Back();
    //    NewWalletManager._Intance.DoTweenBack(helpPanel.transform, -1000);
    }

    public void OnClickAboutUsPanel()
    {
        //Application.OpenURL("http://dimensionchain.io/");
        aboutWeb.ShowClicked();
        //    NewWalletManager._Intance.DOTweenCome(aboutUsPanel.transform, -1000, 0);
    }

    public void OnClickBackAboutUsPanel()
    {
        aboutWeb.Back();
        helpPanel.Back();
        //       NewWalletManager._Intance.DoTweenBack(aboutUsPanel.transform, -1000);
    }

    public void OnClickMine()
    {
        PanelManager._Instance._minExitPanel.Open();
    }
}
