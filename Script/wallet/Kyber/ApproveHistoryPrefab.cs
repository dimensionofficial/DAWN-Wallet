using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;

public class ApproveHistoryPrefab : MonoBehaviour {

    public Text stateText;
    public Text tipsText;
    public Text date;
    public string from;
    public string des;
    public string gas;
    public string value;
    public string hash;
    public string block;
    public ExchangeState exchangeState;

    private void OnEnable()
    {
        if (exchangeState == ExchangeState.Pending)
        {
            //刷新交易信息
            string query = "select * FROM " + SqliteHelperA.SQL_TABLE_NAME + " where " + SqliteHelperA.SQL_COL_HASH + " = " + hash;
            HistoryManagerNew.Instance.RunCustomQuery(KyberTools.instance.walletAddress, query, (r) =>
            {
                while (r.Read())
                {
                    string txReceiptStatus = r.GetValue(r.GetOrdinal(SqliteHelperA.SQL_COL_TXRECEIPT_STATUS)).ToString();
                    TokenContract destToken = KyberTools.instance.GetTokenInfoByAddress(des);
                    if (destToken != null)
                    {
                        if (txReceiptStatus == "1")
                        {
                            stateText.text = "授权成功";
                            stateText.color = KyberTools.instance.successHistoryColor;
                            exchangeState = ExchangeState.Success;
                        }
                        else
                        {
                            stateText.text = "授权失败";
                            stateText.color = KyberTools.instance.faildHistoryColor;
                            exchangeState = ExchangeState.Faild;
                        }
                        KyberTools.instance.DeleteLocalData(KyberTools.instance.walletAddress, hash);
                    }
                }
            }, false);
        }
    }
}
