using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : BasePanel
{

    public SetPinCode setPinCode;
    public GameObject loginObject;

    public InputField m_inputField;

    public Text noticeText;

    private string m_code;

    public override void Open()
    {
        m_inputField.text = "";
        base.Open();
    }

    public void SkipMe(string str)
    {
        setPinCode.gameObject.SetActive(false);
        loginObject.SetActive(true);
        m_inputField.text = str;
        SkipSelectPanel();
    }

    void Start()
    {
        if (GameManager.Instance.loginAccount == "")
        {
            setPinCode.gameObject.SetActive(true);
            loginObject.SetActive(false);
        }
        else
        {
            setPinCode.gameObject.SetActive(false);
            loginObject.SetActive(true);
        }
    }

    public void ClickOKBtn()
    {
        m_code = m_inputField.text;
        if (m_code == "")
        {
            noticeText.text = "Enter your Pin Code:";
        }
        else
        {
            if (m_code != GameManager.Instance.loginAccount)
            {
                noticeText.text = "The Pin Code is error";
            }
            else
            {
                SkipSelectPanel();
            }
        }
       
    }

    private void SkipSelectPanel()
    {
        GameManager.Instance.OpenPanel(this, GameManager.Instance.selectPanel);
    }

	
}
