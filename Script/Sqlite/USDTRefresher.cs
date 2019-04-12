using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class USDTRefresher : MonoBehaviour
{
    public int onepageNum = 20;
    private static USDTRefresher instance;
    public static USDTRefresher Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject o = new GameObject();
                o.name = "USDTRefresher";
                instance = o.AddComponent<USDTRefresher>();
            }
            return instance;
        }
    }

    bool isRunning = false;
    public void Refresh(string address, int page)
    {
        StartCoroutine(RefreshIE(address, page));

    }

    IEnumerator RefreshIE(string address, int page)
    {
        BTCGetHistory.Instance.GetUSDTHistory(address, (o) =>
        {
            
        }, (err) =>
        {

        }, page, onepageNum);

        yield return new WaitForSeconds(0.5F);
    }
}
