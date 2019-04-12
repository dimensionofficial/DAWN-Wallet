using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewObjectPanel : BasePanel
{

    public void OnClickBack()
    {
        GameManager.Instance.OpenPanel(this, GameManager.Instance.selectPanel);
    }

    public void OnClickNew()
    {
        GameManager.Instance.NewCoinInfo();
        GameManager.Instance.ShowCode(GameManager.Instance.recievSendPanel.codeText);
        GameManager.Instance.OpenPanel(GameManager.Instance.newObjectPanel, GameManager.Instance.recievSendPanel);
    }

}
