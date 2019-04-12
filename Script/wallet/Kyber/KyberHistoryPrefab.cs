using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;

public class KyberHistoryPrefab : MonoBehaviour {
    public Text stateText;
    public Text from;
    public Text to;
    public Text paid;
    public Text get;
    public Text data;
    public bool isApprove;
    public string paidAddress;//通常为钱包地址
    public string getAddress;//通常为kyber合约地址
    public string hax;
    public string block;
    public decimal srcAmount;
    public decimal desAmount;
    public string gas;
    public string value;
    public ExchangeState exchangeState;
    public int decimals;

    private void OnEnable()
    {
        if(exchangeState == ExchangeState.Pending)
        {
            //刷新交易信息
            string query = "select * FROM " + SqliteHelperA.SQL_TABLE_NAME + " where " + SqliteHelperA.SQL_COL_HASH + " = " + hax;
            HistoryManagerNew.Instance.RunCustomQuery(KyberTools.instance.walletAddress, query, (r) =>
            {
                while (r.Read())
                {
                    string _value = r.GetValue(r.GetOrdinal(SqliteHelperA.SQL_COL_VALUE)).ToString();
                    string _to = r.GetValue(r.GetOrdinal(SqliteHelperA.SQL_COL_TO)).ToString();
                    string txReceiptStatus= r.GetValue(r.GetOrdinal(SqliteHelperA.SQL_COL_TXRECEIPT_STATUS)).ToString();
                    TokenContract destToken = KyberTools.instance.GetTokenInfoBySymbol(to.text);
                    if (destToken != null)
                    {
                        if (KyberTools.instance.walletAddress.ToLower() == _to)
                        {
                            desAmount = Nethereum.Util.UnitConversion.Convert.FromWei(BigInteger.Parse(_value), destToken.decimals);
                            get.text = desAmount.ToString("f8");
                            if(txReceiptStatus == "1")
                            {
                                stateText.text = "交易成功";
                                stateText.color = KyberTools.instance.successHistoryColor;
                                exchangeState = ExchangeState.Success;
                            }
                            else
                            {
                                stateText.text = "交易失败";
                                stateText.color = KyberTools.instance.faildHistoryColor;
                                exchangeState = ExchangeState.Faild;
                                get.text = "0";
                                desAmount = 0;
                            }
                            KyberTools.instance.DeleteLocalData(KyberTools.instance.walletAddress, hax);
                        }
                        else
                        {
                            Debug.Log(KyberTools.instance.walletAddress + "|" + _to);
                        }
                    }
                }
            }, true);
        }
    }
    // Use this for initialization
}
