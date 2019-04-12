using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ApproveAuthPrefab : MonoBehaviour {

    public string symbol;
    public string _name;
    public int decimals;
    public string contractAddress;
    public Text tokenName;
    public Text tokenSymbol;
    public GameObject state0;
    public GameObject state1;
    public GameObject state2;
    public Image tokenImage;
    public void ShowApproveInfoPage()
    {
        KyberTools.instance.ShowApproveInfo(contractAddress);
    }

    public void ShowState(ApproveState state)
    {
        if (state0 == null || state1 == null || state2 == null)
            return;
        state0.SetActive(false);
        state1.SetActive(false);
        state2.SetActive(false);
        switch (state)
        {
            case ApproveState.No:
                state0.SetActive(true);
                break;
            case ApproveState.Pending:
                state1.SetActive(true);
                break;
            case ApproveState.Success:
                state2.SetActive(true);
                break;
            default:
                break;
        }
    }
}
