using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReturnSN : MonoBehaviour {

	public void RetureChooseSN()
    {
        try
        {
            MultiSignatureManager.instance.chooseWallet = transform.parent.Find("Text").GetComponent<Text>().text;
            impowerManager.instance.chooseWallet = transform.parent.Find("Text").GetComponent<Text>().text;
        }
        catch (System.Exception)
        {
            
        }
        
    }
}
