using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateMnlitSuccess : MonoBehaviour {
    public  NewMultiSigWallet newMultiSigWallet;
    public Text BtcAddress;
    public Button btn_copy;
    public GameObject addressPrefab;
    public GameObject nameWaring;
    public RectTransform parent_Prefab;
    List<GameObject> addrsPreList=new List<GameObject>();
	// Use this for initialization
	void Start () {
        //addrsPreList = new List<GameObject>();

    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void Btn_ShowSuccess()
    {
        newMultiSigWallet.AddNotes();
        BtcAddress.text = newMultiSigWallet.Multi_btcAddress;
        for (int i = 0; i < newMultiSigWallet.pubstr.Count; i++)
        {
            GameObject gameObject = Instantiate(addressPrefab, parent_Prefab);
            gameObject.GetComponent<impowerAddrScript>().title = "地址" + (i + 1).ToString() + ":";
            gameObject.GetComponent<impowerAddrScript>().address = newMultiSigWallet.btcAddress[i];
            gameObject.GetComponent<impowerAddrScript>().sn = "( " + newMultiSigWallet.walletName[i] + " )";
            addrsPreList.Add(gameObject);
        }
    }
    
    public void Btn_SuccessReturn()
    {
        newMultiSigWallet.walletName.Clear();
        for (int i = 0; i < addrsPreList.Count; i++)
        {
            Destroy(addrsPreList[i]);
        }
        addrsPreList.Clear();
        newMultiSigWallet.ResetSuccessPlanle();
    }
    void PrintWalletInfo()
    {
        for (int i = 0; i < newMultiSigWallet.pubstr.Count; i++)
        {
            Debug.Log("btcAddress" + newMultiSigWallet.btcAddress[i]);
            Debug.Log("walletName" + newMultiSigWallet.walletName[i]);
        }
    }
    public void Btn_Success()
    {
        for (int i = 0; i < addrsPreList.Count; i++)
        {
            Destroy(addrsPreList[i]);
        }
        addrsPreList.Clear();
    }
    
    public void OnClickAddressBtn()
    {
        CMGE_Clipboard.CopyToClipboard(newMultiSigWallet.Multi_btcAddress);
        PopupLine.Instance.Show("复制成功");
    }
}
