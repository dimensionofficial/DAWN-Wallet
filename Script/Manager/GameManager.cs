using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public bool isClean = false;
    private string m_loginAccount;
    public string loginAccount
    {
        get { return m_loginAccount; }
        set { m_loginAccount = value; }
    }
    public const string accountKey = "LoginAccount";
    public const string keyName = "BitcoinAddress";



    #region 比特币信息
    private Bitcoin m_currentBitcoin;
    public Bitcoin currentBitcoin
    {
        get { return m_currentBitcoin; }
    }
    private Bitcoin m_lastBitcoin;
    #endregion

    #region 各种面板
    public LoginPanel loginPanel;
    public SelectPanel selectPanel;
    public NewObjectPanel newObjectPanel;
    public RecievSendPanel recievSendPanel;
    public RecievCodePanel recievCodePanel;
    public SendInputPanel sendInputPanel;
    public SendNoticePanel sendNoticePanel;
    public SendCodePanel sendCodePanel;
    public SendNewNoticPanel sendNewNoticPanel;
    #endregion

    public enum CoinType
    {
        None,
        BTC,
        ETH,
    }
    private CoinType m_currentCoinType = CoinType.None;
    public CoinType currentCoinType
    {
        get { return m_currentCoinType; }
    }

    public static GameManager m_Instance;

    public static GameManager Instance
    {
        get
        {
            return m_Instance;
        }
    }

    private BasePanel m_lastPanel;
    private BasePanel m_currentPanel;

    private QRCodeManager m_qRCodeManager;

    void Awake()
    {
        m_Instance = this;
        m_qRCodeManager = new QRCodeManager();
        if (isClean)
        {
            PlayerPrefs.DeleteKey(accountKey);
        }
        loginAccount = PlayerPrefs.GetString(accountKey);

        string[] temp = PlayerPrefsX.GetStringArray(loginAccount+keyName);

        if (temp.Length > 0)
        {
            m_currentBitcoin = new Bitcoin(temp);
        }
    }



    public void ReLastCode()
    {
        m_currentBitcoin = m_lastBitcoin;
    }

    public void EncodeQRCode(Image qrCodeImage, string code)
    {
        m_qRCodeManager.EncodeQRCode(qrCodeImage, code);
    }
    public void EncodeQRCode(Image qrCodeImage)
    {
        m_qRCodeManager.EncodeQRCode(qrCodeImage, m_currentBitcoin.bitcoinAddress);
    }

    public void OnSelectItem(Item item)
    {
        m_currentCoinType = item.m_type;
        switch (m_currentCoinType)
        {
            case CoinType.BTC:

                if (m_currentBitcoin == null)
                {
                    OpenPanel(selectPanel, newObjectPanel);
                }
                else
                {
                    OpenPanel(selectPanel, recievSendPanel);
                }

                break;
            case CoinType.ETH:

                break;
        }
       
    }



    public void NewCoinInfo()
    {
        switch (m_currentCoinType)
        {
            case CoinType.BTC:
                m_lastBitcoin = m_currentBitcoin;
                m_currentBitcoin = new Bitcoin();
                string str1 = m_currentBitcoin.privateKey;
                string str2 = m_currentBitcoin.pubKeyAddress;
                string str3 = m_currentBitcoin.bitcoinAddress;
                string[] strs = new string[3];
                strs[0] = str1;
                strs[1] = str2;
                strs[2] = str3;
                PlayerPrefsX.SetStringArray(loginAccount+keyName, strs);
                break;
            case CoinType.ETH:

                break;
        }
       
    }

    public void ShowCode(Text codeText)
    {
        string str1 = m_currentBitcoin.bitcoinAddress.Substring(0, 6);
        string str2 = m_currentBitcoin.bitcoinAddress.Substring(m_currentBitcoin.bitcoinAddress.Length - 6, 6);
        codeText.text = str1 + "******" + str2;
    }


    public void OnReciev()
    {
        OpenPanel(newObjectPanel, recievSendPanel);
    }


    public void OpenPanel(BasePanel closedPanel, BasePanel openPanel)
    {
        m_lastPanel = closedPanel;
        m_lastPanel.Hide();
        m_currentPanel = openPanel;
        m_currentPanel.Open();
    }

}
