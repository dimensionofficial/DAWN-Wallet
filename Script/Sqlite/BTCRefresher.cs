using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTCRefresher : MonoBehaviour, IRefresher
{

    private static BTCRefresher instance;
    public static BTCRefresher Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject o = new GameObject();
                o.name = "BTCRefresher";
                instance = o.AddComponent<BTCRefresher>();
            }
            return instance;
        }
    }

    bool isRunning = false;
    public void Refresh(string address, System.Action<bool> OnFinished, SqliteDicItem sqlite)
    {
        if (isRunning)
        {
            if (OnFinished != null)
            {
                OnFinished(false);
            }
            return;
        }
        isRunning = true;
        StartCoroutine(RefreshIE(address, OnFinished, sqlite));
    }

    IEnumerator RefreshIE(string address, System.Action<bool> OnFinished, SqliteDicItem sqlite)
    {
        int page = PlayerPrefs.GetInt(address + "his_new_btc1", 1);
        int iscomplete = PlayerPrefs.GetInt(address + "his_new_btc_com1", 0);
        int maxBlockNumber = -1;
        if (page == 1)
        {
            while (!sqlite.GetMaxBlockNumber((i) =>
            {
                maxBlockNumber = i;
                PlayerPrefs.SetInt(address + "his_new_btc_max1", maxBlockNumber);
            }))
            {
                yield return new WaitForFixedUpdate();
            }
        }
        else
        {
            maxBlockNumber = PlayerPrefs.GetInt(address + "his_new_btc_max1", 0);
        }
        while (maxBlockNumber == -1)
        {
            yield return new WaitForFixedUpdate();
        }
        int pagesize = 20;
        bool needRefreshNextPage = false;
        bool tags = false;
        BTCGetHistory.Instance.GetBTCHistory(address, (o) =>
        {
            if (o.Count == 0)
            {
                needRefreshNextPage = false;
                tags = true;
                PlayerPrefs.SetInt(address + "his_new_btc_com1", 1);
            }
            else
            {
                sqlite.InsertData(o, (r)=> {
                    bool needEnd = false;
                    foreach (var v in o)
                    {
                        if (int.Parse(v.blockNumber) < maxBlockNumber)
                        {
                            needEnd = true;
                            break;
                        }
                    }
                    if (!r)
                    {
                        needRefreshNextPage = true;
                        tags = true;
                    }
                    else if (needEnd && iscomplete == 1)
                    {
                        needRefreshNextPage = false;
                        tags = true;
                    }
                    else
                    {
                        needRefreshNextPage = true;
                        PlayerPrefs.SetInt(address + "his_new_btc1", page + 1);
                        tags = true;
                    }
                });
            }
        },(error)=> {
            needRefreshNextPage = true;
            tags = true;
        }, page, pagesize);
        while (!tags)
        {
            yield return new WaitForFixedUpdate();
        }
        if (needRefreshNextPage)
        {
            yield return new WaitForSeconds(1f);
            StartCoroutine(RefreshIE(address, OnFinished, sqlite));
        }
        else
        {
            if (OnFinished != null)
            {
                OnFinished(true);
            }
            iscomplete = PlayerPrefs.GetInt(address + "his_new_btc_com1", 0);
            if (iscomplete == 1)
            {
                PlayerPrefs.SetInt(address + "his_new_btc1", 1);
            }
            isRunning = false;
        }
    }
}
