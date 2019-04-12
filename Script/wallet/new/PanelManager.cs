using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    public enum SubPage
    {
        /// <summary>
        /// 资产界面
        /// </summary>
        Property,//
        /// <summary>
        ///钱包配对界面 
        /// </summary>
        WalletMatching, //
        /// <summary>
        /// 我的界面
        /// </summary>
        Mine,//
        /// <summary>
        /// 钱包主界面
        /// </summary>
        WalletInfo,//
        /// <summary>
        /// 行情界面
        /// </summary>
        HangQing,
    }

    public SubPage currentSubPage = SubPage.Property;

    public static PanelManager _Instance;

    public GameObject loadingPanel;
    public HotWalletLoginPanel _loginPanel;
    public MainPanel _mainPanel;
    public MineExitPanel _minExitPanel;
    public WalletInfoPanel _WalletInfoPanel;
    public GameObject kyberPanel;

    public EOSRegisterPanel _eosRegisterPanel;
    public EOSRegisterOrderPanel _eosOrderPanel;
    public EOSRegisteringPanel _eosRegisteringPanel;
    public EOSWalletInfoPanel _eosWalletInfoPanel;
    public EOSSendCoinPanel _eosSendCoinPanel;
    public EOSTransactionPanel _eosTransactionPanel;
    public EOSSendScanSing _eosScanSing;
    public EOSPaymentPanel _eosPaymentPanel;
    public EOSCreatAccountPanel _eosCreatAccountPanel;

    public EosResourcesPanel _eosResourcesPanel;

    public CreateOrResume _createOrResume;

    public PingCodeInputBox _pingCodeInputBox;
    public BackUpPrivateKeyPanel _backUpPrivateKeyPanel;
    public ImportSeedBipPanel _importSeedBipPanel;
    public ImportPrivateKeyPanel _importPrivateKeyPanel;
    public OpenCameraScan _openCameraScanPanel;
    public ImportKeyStorePanel _importKeyStorePanel;

    void Awake()
    {
        _Instance = this;
    }

    public void OpenPanel(HotBasePanel openPanl)
    {
        openPanl.Open();
    }

    public void OpenPanel(HotBasePanel openPanl, params HotBasePanel[] closedPanel)
    {
        openPanl.Open();
        for (int i = 0; i < closedPanel.Length; i++)
        {
            closedPanel[i].Closed();
        }
    }

    public void OpenKyberPanel()
    {
        kyberPanel.SetActive(true);
    }
}
