using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SendNewNoticPanel : BasePanel {

    public Text newCodeText;
    public void ShowNewCode()
    {
        newCodeText.text = GameManager.Instance.currentBitcoin.bitcoinAddress;
    }

    public void OnClickOK()
    {
        GameManager.Instance.OpenPanel(this, GameManager.Instance.recievSendPanel);
        GameManager.Instance.ShowCode(GameManager.Instance.recievSendPanel.codeText);
    }

    public void OnClickCanel()
    {
        GameManager.Instance.ReLastCode();
        GameManager.Instance.OpenPanel(this, GameManager.Instance.recievSendPanel);
    }
}
