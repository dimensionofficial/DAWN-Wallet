using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SendInputPanel : BasePanel
{
    public InputField inputField;
    public Text noticeText;
    public override void Open()
    {
        inputField.text = "";
        noticeText.text = "";
        base.Open();
    }

    public void OnClickCancelBtn()
    {
        GameManager.Instance.OpenPanel(this, GameManager.Instance.recievSendPanel);
    }

    public void OnClickOkBtn()
    {
        string str = inputField.text;
        if (str == GameManager.Instance.loginAccount)
        {
            GameManager.Instance.OpenPanel(this, GameManager.Instance.sendNoticePanel);
        }
        else
        {
            noticeText.text = "The Pin Code is error, Please re-enter";
        }
        
    }
}
