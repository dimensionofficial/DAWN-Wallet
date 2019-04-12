using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SendCodePanel : BasePanel
{
    public Text codeText;
    public Image qrCodeImage;
    public void ShowQRCode()
    {
        GameManager.Instance.ShowCode(codeText);
        GameManager.Instance.EncodeQRCode(qrCodeImage);
    }

    public void OnClickBack()
    {
        GameManager.Instance.ReLastCode();
        GameManager.Instance.OpenPanel(this, GameManager.Instance.recievSendPanel);
    }

    public void OnClickOK()
    {
        GameManager.Instance.OpenPanel(this, GameManager.Instance.sendNewNoticPanel);
        GameManager.Instance.sendNewNoticPanel.ShowNewCode();
    }
}
