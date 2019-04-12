using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRefresherCode : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //PlayerPrefs.DeleteAll();
        HistoryManagerNew.Instance.Refresh("0xAd332e08675e5f18a0617Af804A4EFd30e2b540b", (b) => {
            Debug.Log("refresh ETH ok " + b);
        }, RefreshType.ETH);
        HistoryManagerNew.Instance.Refresh("0xAd332e08675e5f18a0617Af804A4EFd30e2b540b", (b) => {
            Debug.Log("refresh token ok " + b);
        }, RefreshType.Token); 
        HistoryManagerNew.Instance.Refresh("1QJrJUgAtFJYKLPAj8b8TnCUMy8XPn5DKr", (b) => {
            Debug.Log("refresh BTC ok " + b);
        }, RefreshType.BTC);
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.W))
        {
            HistoryManagerNew.Instance.GetHistory(1, 10, "0xad332e08675e5f18a0617af804a4efd30e2b540b", (o) =>
            {
                Debug.Log(o.Count);
            });
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            HistoryManagerNew.Instance.GetHistory(1, 10, "0xad332e08675e5f18a0617af804a4efd30e2b540b", (o) =>
            {
                Debug.Log(o.Count);
            }, "", false, false, SqliteDicItem.HistoryType.All);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            HistoryManagerNew.Instance.GetHistory(1, 10, "0xad332e08675e5f18a0617af804a4efd30e2b540b", (o) =>
            {
                Debug.Log(o.Count);
            }, "", false, false, SqliteDicItem.HistoryType.Received);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            HistoryManagerNew.Instance.GetHistory(1, 10, "0xad332e08675e5f18a0617af804a4efd30e2b540b", (o) =>
            {
                Debug.Log(o.Count);
            }, "", false, false, SqliteDicItem.HistoryType.Self);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            HistoryManagerNew.Instance.GetHistory(1, 10, "0xad332e08675e5f18a0617af804a4efd30e2b540b", (o) =>
            {
                Debug.Log(o.Count);
            }, "", false, false, SqliteDicItem.HistoryType.Send);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            HistoryManagerNew.Instance.GetHistory(1, 100, "0xad332e08675e5f18a0617af804a4efd30e2b540b", (o) =>
            {
                Debug.Log(o.Count);
            }, "0x4d415ebf33ddb206d858fea170ad55375e8848fd", false, true, SqliteDicItem.HistoryType.Send);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            HistoryManagerNew.Instance.GetHistory(1, 10, "1QJrJUgAtFJYKLPAj8b8TnCUMy8XPn5DKr", (o) =>
            {
                Debug.Log(o.Count);
            });
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            HistoryManagerNew.Instance.GetHistory(1, 10, "0xAd332e08675e5f18a0617Af804A4EFd30e2b540b", (o) =>
            {
                Debug.Log(o.Count);
            },"", true);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            HistoryManagerNew.Instance.GetAllTokenInfo("0xAd332e08675e5f18a0617Af804A4EFd30e2b540b", (o) =>
            {
                Debug.Log(o.Count);
            });
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            string address = "0xad332e08675e5f18a0617af804a4efd30e2b540b";
            int page = 1;
            int pagesize = 10;
            string sql = "SELECT * FROM Records WHERE p_from == '"+
                address + "' AND p_to == '" +
                address + "' AND (m_input == '0x' OR m_input == 'NULL') ORDER BY m_blockNumber LIMIT "+
                pagesize + " OFFSET " + (page - 1) * pagesize;
            HistoryManagerNew.Instance.RunCustomQuery(address, sql, (o) => {
                while (o.Read())
                {
                    Debug.Log(o[1].ToString());
                }
            });
        }
    }
}
