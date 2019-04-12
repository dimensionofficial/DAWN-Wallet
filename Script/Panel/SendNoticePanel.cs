using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendNoticePanel : BasePanel
{
    public void OnClickOK()
    {
        GameManager.Instance.NewCoinInfo();
        GameManager.Instance.sendCodePanel.ShowQRCode();
        GameManager.Instance.OpenPanel(this, GameManager.Instance.sendCodePanel);
    }

    public void OnClickCancel()
    {
        GameManager.Instance.ReLastCode();
        GameManager.Instance.OpenPanel(this, GameManager.Instance.recievSendPanel);
    }
}
