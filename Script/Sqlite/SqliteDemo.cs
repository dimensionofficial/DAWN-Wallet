using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SqliteDemo : MonoBehaviour {

    public bool startInser = false;
    public string address;
    public List<ETHHistoryRcord> recordList = new List<ETHHistoryRcord>();

    // Update is called once per frame
    void Update ()
    {
        if (startInser)
        {
            startInser = false;
            SqliteHelper.Instance.Insert(address, recordList);
        }


	}
}
