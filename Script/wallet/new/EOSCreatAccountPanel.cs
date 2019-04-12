using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EOSCreatAccountPanel : MonoBehaviour
{
    private string accountRule = "12345abcdefghijklmnopqrstuvwxyz";
    public InputField eosAccountInput;

    public Text noticText;

    public Text ownerAddressText;
    public Text adminAddressText;
    public Text registerRAM_text;
    public Text registerCUP_text;
    public Text registerNETWORK_text;

    private EOSWalletInfo m_eosWalletInfo;
    public string hash;
    public string goodid;

    public GameObject successObject;
    public Text accountText1;
    public Text ownerAddressText1;
    public Text adminAddressText1;
    public Text registerRAM_text1;
    public Text registerCUP_text1;
    public Text registerNETWORK_text1;
    private Timer m_timer;
    public EosItem eosItem;

    public void Show(EosItem _eosItem, string _hash, string _goodsid)
    {
        eosItem = _eosItem;
        ShowNoticText(false);
        hash = _hash;
        goodid = _goodsid;
        gameObject.SetActive(true);
        successObject.SetActive(false);
        m_eosWalletInfo = _eosItem.eosWalletInfo;
        ownerAddressText.text = m_eosWalletInfo.ownerAddress;
        adminAddressText.text = m_eosWalletInfo.adminAddress;
        registerRAM_text.text = EosGetSingInfoPanel.Instance.eos_register_ram.ToString();
        registerCUP_text.text = EosGetSingInfoPanel.Instance.eos_register_cpu.ToString();
        registerNETWORK_text.text = EosGetSingInfoPanel.Instance.eos_register_network.ToString();
    }

    private void ShowNoticText(bool isActive, string info = "")
    {
        if (isActive)
        {
            if (m_timer != null)
            {
                TimerManager.Instance.RemoveTimer(m_timer);
                m_timer = null;
            }
            noticText.text = info;
            noticText.gameObject.SetActive(true);
            m_timer = TimerManager.Instance.AddTimer(3,  () => ShowNoticText(false));
        }
        else
        {
            noticText.gameObject.SetActive(false);
            m_timer = null;
        }
    }

    public void OnClickCreatBtn()
    {
        string account = eosAccountInput.text;
        if (string.IsNullOrEmpty(account))
        {
            ShowNoticText(true, "账户不能为空");
            return;
        }
        else
        {
            if (account.Length != 12)
            {
                ShowNoticText(true, "账号名称必须为12位的英文小写字母与数字1-5的组合");
                return;
            }

            for (int i = 0; i < account.Length; i++)
            {
                string temp = account[i].ToString();
                if (!accountRule.Contains(temp))
                {
                    ShowNoticText(true, "账号名称必须为12位的英文小写字母与数字1-5的组合");
                    return;
                }
            }
        }
        m_eosWalletInfo.account = account;


        PanelManager._Instance.loadingPanel.SetActive(true);
        Debug.Log(m_eosWalletInfo.account);
        EosGetSingInfoPanel.Instance.GetAccount(m_eosWalletInfo.account, delegate (Hashtable t)
        {
            if (t.ContainsKey("error"))
            {
                string er = t["error"].ToString();
                if (er.Equals("unspecified"))
                {
                    UsePayOrder();
                    return;
                }
            }
            ShowNoticText(true, "该账户已存在，请换一个重试");
            PanelManager._Instance.loadingPanel.SetActive(false);

        }, delegate ()
        {
            PanelManager._Instance.loadingPanel.SetActive(false);
        }, false);
    }

    private void UsePayOrder()
    {
        List<KeyValuePair<string, string>> ws = new List<KeyValuePair<string, string>>();
        ws.Add(new KeyValuePair<string, string>("userid", NewWalletManager._Intance.userId));
        ws.Add(new KeyValuePair<string, string>("hash", hash));
        ws.Add(new KeyValuePair<string, string>("goodid", goodid));
        StartCoroutine(HttpManager._Intance.GetNodeJsRequest("useorder", ws, (Hashtable data) =>
        {

            if (data != null)
            {
                string result = data["result"].ToString();
                if (result.StartsWith("Su"))
                {
                    CreatEosAccount();
                }
                else
                {
                    PopupLine.Instance.Show("订单异常，请联系客服");
                }
            }else
            {
                PopupLine.Instance.Show("订单异常，请联系客服");
            }

            PanelManager._Instance._eosResourcesPanel.loadingPanel.SetActive(false);
        }));
    }


    private void CreatEosAccount()
    {
        WWWForm ws = new WWWForm();
        ws.AddField("name", m_eosWalletInfo.account);
        ws.AddField("ownerKey", m_eosWalletInfo.ownerAddress);
        ws.AddField("activeKey", m_eosWalletInfo.adminAddress);
        string key = "5JdjQE497TJRhsgb4ztpG6yX3Z37F6rzoKR7juz9yTdQrY59wgz";
        string sign = m_eosWalletInfo.account + m_eosWalletInfo.ownerAddress + m_eosWalletInfo.adminAddress;
        string signstr = HttpManager.HmacSHA256(sign, key);
        ws.AddField("sign", signstr);
        StartCoroutine(HttpManager._Intance.EOSSendRequest("newaccount", ws, true, delegate (Hashtable t)
        {
            PanelManager._Instance._eosResourcesPanel.loadingPanel.SetActive(false);
            if (t != null && t.ContainsKey("error"))
            {
                PopUpBox.Instance.Show(null, null, "确定", "", "错误提示", t["error"].ToString());
                Debug.Log("Eos 网络错误：" + t["error"]);
                return;
            }

            PlayerPrefs.SetString(m_eosWalletInfo.adminAddress + "registered", m_eosWalletInfo.account);
           
            gameObject.SetActive(false);

            PanelManager._Instance._eosRegisterPanel.gameObject.SetActive(false);


            if (eosItem != null)
            {
                eosItem.OnCreatAccountLater();
                TimerManager.Instance.AddTimer(10, () => eosItem.InitEOS(m_eosWalletInfo));
            }

            CreatSuccess();

           

        }, delegate () 
        {
            PanelManager._Instance._eosResourcesPanel.loadingPanel.SetActive(false);
            PopupLine.Instance.Show("当前网络不可用，请检查网络配置");
        }, false));
    }

    private void CreatSuccess()
    {
        successObject.SetActive(true);

        accountText1.text = m_eosWalletInfo.account;
        ownerAddressText1.text = m_eosWalletInfo.ownerAddress;
        adminAddressText1.text = m_eosWalletInfo.adminAddress;
        registerRAM_text1.text = EosGetSingInfoPanel.Instance.eos_register_ram.ToString();
        registerCUP_text1.text = EosGetSingInfoPanel.Instance.eos_register_cpu.ToString();
        registerNETWORK_text1.text = EosGetSingInfoPanel.Instance.eos_register_network.ToString();
    }
}
