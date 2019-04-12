using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Nethereum.Util;

public class ETHRefresher : MonoBehaviour, IRefresher
{

    private static ETHRefresher instance;
    public static ETHRefresher Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject o = new GameObject();
                o.name = "ETHRefresher";
                instance = o.AddComponent<ETHRefresher>();
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
        //PlayerPrefs.SetInt(address + "his_new", 1);
        StartCoroutine(RefreshIE(address, OnFinished, sqlite));
    }

    IEnumerator RefreshIE(string address, System.Action<bool> OnFinished, SqliteDicItem sqlite)
    {
        int page = PlayerPrefs.GetInt(address + "his_new1", 1);
        int pagesize = 20;
        bool needRefreshNextPage = false;
        bool tags = false;
        EthHistory.Instance.GetEthHistory(address, 0, (j) =>
        {
            bool rr = LoomRefresher.RunAsync(() =>
            {
                var o = ParseETHRecord(j, false);
                LoomRefresher.QueueOnMainThread(() =>
                {
                    if (o.Count == 0)
                    {
                        needRefreshNextPage = false;
                        tags = true;
                    }
                    else if (o.Count >= pagesize)
                    {
                        sqlite.InsertData(o, (r) =>
                        {
                            if (!r)
                            {
                                needRefreshNextPage = true;
                                tags = true;
                            }
                            else
                            {
                                needRefreshNextPage = true;
                                PlayerPrefs.SetInt(address + "his_new1", page + 1);
                                tags = true;
                            }
                        });
                    }
                    else
                    {
                        sqlite.InsertData(o, (r) =>
                        {
                            if (!r)
                            {
                                needRefreshNextPage = true;
                                tags = true;
                            }
                            else
                            {
                                needRefreshNextPage = false;
                                tags = true;
                            }
                        });
                    }
                });
            });
            if (rr == false)
            {
                needRefreshNextPage = true;
                tags = true;
            }
        }, (error) => {
            needRefreshNextPage = true;
            tags = true;
        }, page, pagesize);
        while (!tags)
        {
            yield return new WaitForFixedUpdate();
        }
        if (needRefreshNextPage)
        {
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(RefreshIE(address, OnFinished, sqlite));
        }
        else
        {
            if (OnFinished != null)
            {
                OnFinished(true);
            }
            isRunning = false;
        }
    }

    List<ETHHistoryRcord> ParseETHRecord(string str, bool isErc20)
    {
        List<ETHHistoryRcord> result = new List<ETHHistoryRcord>();
        Hashtable d = Json.jsonDecode(str) as Hashtable;
        if (d == null || !d.ContainsKey("result"))
        {
            return result;
        }
        ArrayList ar = d["result"] as ArrayList;
        if (ar == null)
        {
            //Debug.Log(str);
            return result;
        }
        var startTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        for (int i = 0; i < ar.Count; i++)
        {
            Hashtable h = ar[i] as Hashtable;
            ETHHistoryRcord ethRcord = new ETHHistoryRcord();
            long time = long.Parse(h["timeStamp"].ToString());
            DateTime dt = startTime.AddSeconds(time).ToLocalTime();
            ethRcord.timeStamp = dt.ToString("yyyy-MM-dd HH:mm:ss");
            ethRcord.hash = h["hash"].ToString();
            ethRcord.from = h["from"].ToString();
            ethRcord.blockNumber = h["blockNumber"].ToString();
            ethRcord.to = h["to"].ToString();
            System.Numerics.BigInteger gasUsed = System.Numerics.BigInteger.Parse(h["gasUsed"].ToString());
            System.Numerics.BigInteger gasPrice = System.Numerics.BigInteger.Parse(h["gasPrice"].ToString());
            ethRcord.gas = UnitConversion.Convert.FromWei(gasUsed * gasPrice, 18).ToString();
            ethRcord.value = h["value"].ToString();
            if (h.ContainsKey("txreceipt_status"))
            {
                ethRcord.txReceiptStatus = h["txreceipt_status"].ToString();
            }
            if (h.ContainsKey("input"))
            {
                string inp = h["input"].ToString();
                ethRcord.input = inp.Substring(0, inp.Length >= 510 ? 509 : inp.Length);
            }

            ethRcord.confixmations = h["confirmations"].ToString();
            if (isErc20)
            {
                ethRcord.fullName = h["tokenName"].ToString();
                ethRcord.tokenSymbol = h["tokenSymbol"].ToString();
                ethRcord.contractAddress = h["contractAddress"].ToString();
                result.Add(ethRcord);
            }
            else
            {
                result.Add(ethRcord);
            }
        }
        return result;
    }
}
