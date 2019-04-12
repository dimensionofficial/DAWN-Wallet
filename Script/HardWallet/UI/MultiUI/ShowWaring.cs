using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowWaring : MonoBehaviour {

    public Text waringN;
    public Text waringM;
    public Text textN;
    public Text textM;
    public NewMultiSigWallet newMultiSigWallet;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (textN.text!="")
        {
            if (newMultiSigWallet.StrToInt(textN.text) >= 2 && newMultiSigWallet.StrToInt(textN.text) <= 4)
            {
                waringN.gameObject.SetActive(false);
            }
            else
            {
                waringN.gameObject.SetActive(true);
            }
            if (textM.text != "")
            {
                if (newMultiSigWallet.StrToInt(textM.text) >= 2 && newMultiSigWallet.StrToInt(textM.text) <= newMultiSigWallet.StrToInt(textN.text))
                {
                    waringM.gameObject.SetActive(false);
                }
                else
                {
                    waringM.gameObject.SetActive(true);
                }
            }
        }
	}
}
