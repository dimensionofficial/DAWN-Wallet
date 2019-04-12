using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecievSendPanel : BasePanel
{
    public Text codeText;

    public override void Open()
    {
        GameManager.Instance.ShowCode(codeText);
        base.Open();
    }

    public void OnClickBack()
    {
        if (GameManager.Instance.currentBitcoin == null)
        {
            GameManager.Instance.OpenPanel(this, GameManager.Instance.newObjectPanel);
        }
        else
        {
            GameManager.Instance.OpenPanel(this, GameManager.Instance.selectPanel);
        }

       
    }

    public void OnClickRecievBtn()
    {
        GameManager.Instance.OpenPanel(this,GameManager.Instance.recievCodePanel);
    }

    public void OnClickSendBtn()
    {
        GameManager.Instance.OpenPanel(this,GameManager.Instance.sendInputPanel);
    }
}
