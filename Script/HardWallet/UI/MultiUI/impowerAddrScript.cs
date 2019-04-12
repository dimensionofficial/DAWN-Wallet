using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class impowerAddrScript : MonoBehaviour {
    public string title;
    public string address;
    public string sn;
    public  Text text_title;
    public  Text text_address;
    public  Text text_sn;
    // Use this for initialization
    void Start () {
        //text_title = transform.Find("title").GetComponent<Text>();
        //text_address = transform.Find("address").GetComponent<Text>();
        //text_sn = transform.Find("name").GetComponent<Text>();
    }
	
	// Update is called once per frame
	void Update () {
        ShowInfo();

    }
    public void ShowInfo()
    {
        text_title.text = title;
        string qianAdr = address.Substring(0, 10);
        string houAdr = address.Substring(address.Length - 10, 10);
        text_address.text = qianAdr + "..." + houAdr;
        text_sn.text = sn;
    }
}
