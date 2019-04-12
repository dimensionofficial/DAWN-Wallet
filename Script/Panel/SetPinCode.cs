using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetPinCode : MonoBehaviour
{
    public LoginPanel loginPanel;
    public InputField m_inputField1;
    public InputField m_inputField2;

    public GameObject noticeObject;

    public void OnClickOK()
    {
        string str1 = m_inputField1.text; 
        string str2 = m_inputField2.text;

        if (str1 != "" && str2 != "")
        {
            if (str1 == str2)
            {
                PlayerPrefs.SetString(GameManager.accountKey, str1);
                GameManager.Instance.loginAccount = str1;
                loginPanel.SkipMe(str1);
            }
            else
            {
                noticeObject.gameObject.SetActive(true);
            }
        }
    }

}
