using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;

public class HistoryManagerNew : MonoBehaviour {

    private static HistoryManagerNew instance;
    public static HistoryManagerNew Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject o = new GameObject();
                o.name = "HistoryManagerBase";
                instance = o.AddComponent<HistoryManagerNew>();
            }
            return instance;
        }
    }
    
    private Dictionary<string, SqliteDicItem> SqlHelperDic = new Dictionary<string, SqliteDicItem>();

    /// <summary>
    /// 获取历史记录,如果上一轮还没有查询完毕，则直接返回上一次相同查询条件的缓存数据
    /// </summary>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <param name="address"></param>
    /// <param name="OnFinished"></param>
    /// <param name="contactAddress"></param>
    /// <param name="isKyber"></param>
    /// <param name="isToken"></param>
    public void GetHistory(int page, int pageSize, string address, Action<List<ETHHistoryRcord>> OnFinished, string contactAddress = "", bool isKyber = false, bool isToken = false, SqliteDicItem.HistoryType historyType = SqliteDicItem.HistoryType.All)
    {
        SqliteDicItem item = QueueWorkItem(isToken, address);
        item.GetHistory(page, pageSize, OnFinished, contactAddress, isKyber, historyType);
    }

    /// <summary>
    /// 运行自定义sql
    /// </summary>
    /// <param name="address"></param>
    /// <param name="sql"></param>
    /// <param name="OnFinished"></param>
    /// <param name="isToken"></param>
    public void RunCustomQuery(string address, string sql, Action<SqliteDataReader> OnFinished, bool isToken = false)
    {
        SqliteDicItem item = QueueWorkItem(isToken, address);
        item.RunCustomQuery(sql, OnFinished);
    }

    /// <summary>
    /// 查询token种类，返回List<ETHHistoryRcord>，但只有contactaddress，fullname，和tokensymbol有值
    /// </summary>
    /// <param name="address"></param>
    /// <param name="OnFinished"></param>
    public void GetAllTokenInfo(string address, System.Action<List<ETHHistoryRcord>> OnFinished)
    {
        SqliteDicItem item = QueueWorkItem(true, address);
        item.GetAllTokenInfo(OnFinished);
    }

    /// <summary>
    /// 启动刷新,在需要的地方调用，如果上一次还没有刷新完，则直接OnFinished(false)
    /// </summary>
    /// <param name="address"></param>
    /// <param name="OnFinished"></param>
    /// <param name="type">btc/eth/token</param>
    public void Refresh(string address, Action<bool> OnFinished, RefreshType type)
    {
        if (type!= RefreshType.USDT)
        {
            if (type != RefreshType.BTC)
            {
                address = address.ToLower();
            }
        }
        SqliteDicItem item = null;
        string key = type == RefreshType.Token ? (address + "Token") : address;
        if (!SqlHelperDic.ContainsKey(key))
        {
            item = new SqliteDicItem(this, address, type == RefreshType.Token);
            SqlHelperDic.Add(key, item);
        }
        else
        {
            item = SqlHelperDic[key];
        }
        if (!item.CheckAddress(address))
        {
            if (OnFinished != null)
            {
                OnFinished(false);
            }
            return;
        }
        switch (type)
        {
            case RefreshType.BTC:
                BTCRefresher.Instance.Refresh(address, OnFinished, item);
                break;
            case RefreshType.USDT:
            //    USDTRefresher.Instance.Refresh(address, OnFinished, item);
                break;
            case RefreshType.ETH:
                ETHRefresher.Instance.Refresh(address, OnFinished, item);
                break;
            case RefreshType.Token:
                TokenRefresher.Instance.Refresh(address, OnFinished, item);
                break;
        }
    }

    SqliteDicItem QueueWorkItem(bool isToken, string address)
    {
        SqliteDicItem item = null;
        string key = isToken ? (address + "Token") : address;
        if (!SqlHelperDic.ContainsKey(key))
        {
            item = new SqliteDicItem(this, address, isToken);
            SqlHelperDic.Add(key, item);
        }
        else
        {
            item = SqlHelperDic[key];
        }
        return item;
    }
}

/// <summary>
/// history
/// </summary>
public class SqliteDicItem
{
    public enum HistoryType
    {
        All,
        Self,
        Send,
        Received
    }
    public SqliteDicItem(MonoBehaviour o, string address, bool isToken = false)
    {
        sqliteHelper = o.gameObject.AddComponent<SqliteHelperA>();
        sqliteHelper.SQLiteInit(isToken ? (address + "Token") : address);
        this.address = address;
        owner = o;
        this.isToken = isToken;
    }
    private SqliteHelperA sqliteHelper = null;
    private Dictionary<string, List<ETHHistoryRcord>> catche = new Dictionary<string, List<ETHHistoryRcord>>();
    private bool IsRunning = false;
    private bool isRunning
    {
        get {
            return IsRunning;
        }
        set {
            //Debug.Log("set running " + value);
            IsRunning = value;
        }
    }
    private string address;
    private MonoBehaviour owner;
    private bool isToken = false;

    public void GetAllTokenInfo(System.Action<List<ETHHistoryRcord>> OnFinished)
    {
        string sql = "SELECT " + SqliteHelperA.SQL_COL_CONTACT_ADDRESS + "," + SqliteHelperA.SQL_COL_FULLNAME + "," +
                SqliteHelperA.SQL_COL_TOKENSYMBO + " FROM " + SqliteHelperA.SQL_TABLE_NAME +
                " where " + SqliteHelperA.SQL_COL_ADDRESS + "==" + GetValue(address) + " and " + SqliteHelperA.SQL_COL_CONTACT_ADDRESS + " <>'NULL' " +
                " GROUP BY " + SqliteHelperA.SQL_COL_CONTACT_ADDRESS;
        if (isRunning)
        {
            if (OnFinished != null)
            {
                Debug.Log("return catche");
                OnFinished(FindCatche(sql));
            }
            return;
        }
        isRunning = true;
        owner.StartCoroutine(QueueThread(() =>
        {
            while (!sqliteHelper.Query(sql, (reader) =>
            {
                List<ETHHistoryRcord> recordList = new List<ETHHistoryRcord>();
                while (reader.Read())
                {
                    ETHHistoryRcord record = new ETHHistoryRcord();
                    record.tokenSymbol = reader.GetValue(reader.GetOrdinal(SqliteHelperA.SQL_COL_TOKENSYMBO)).ToString();
                    record.fullName = reader.GetValue(reader.GetOrdinal(SqliteHelperA.SQL_COL_FULLNAME)).ToString();
                    record.contractAddress = reader.GetValue(reader.GetOrdinal(SqliteHelperA.SQL_COL_CONTACT_ADDRESS)).ToString();
                    recordList.Add(record);
                }
                UpdateCatche(sql, Copy(recordList));
                LoomRefresher.QueueOnMainThread(() =>
                {
                    if (OnFinished != null)
                    {
                        OnFinished(recordList);
                    }
                    isRunning = false;
                });
            }))
            {
                System.Threading.Thread.Sleep(50);
            }
        }));
    }

    public void InsertData(List<ETHHistoryRcord> rcordList, System.Action<bool> OnFinished)
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
        owner.StartCoroutine(QueueThread(() => {
            if (rcordList.Count <= 0)
            {
                LoomRefresher.QueueOnMainThread(() => {
                    if (OnFinished != null)
                    {
                        OnFinished(true);
                    } 
                    isRunning = false;
                });
                return;
            }
            string[] fieldNames = new string[15] { SqliteHelperA.SQL_COL_DECIMALS, SqliteHelperA.SQL_COL_BLOCKNUMBER, SqliteHelperA.SQL_COL_TIMESTAMP,
                SqliteHelperA.SQL_COL_HASH, SqliteHelperA.SQL_COL_FROM, SqliteHelperA.SQL_COL_TO, SqliteHelperA.SQL_COL_VALUE, SqliteHelperA.SQL_COL_GAS,
                SqliteHelperA.SQL_COL_TOKENSYMBO, SqliteHelperA.SQL_COL_FULLNAME, SqliteHelperA.SQL_COL_ADDRESS, SqliteHelperA.SQL_COL_CONTACT_ADDRESS,
                SqliteHelperA.SQL_COL_CONFIXMATIONS, SqliteHelperA.SQL_COL_TXRECEIPT_STATUS, SqliteHelperA.SQL_COL_INPUT };

            string sql = "insert OR IGNORE into " + SqliteHelperA.SQL_TABLE_NAME + "(";

            for (int i = 0; i < fieldNames.Length; i++)
            {
                sql += fieldNames[i];
                if (i < fieldNames.Length - 1)
                {
                    sql += ",";
                }
            }
            sql += ")" + "values ";
            List<string> sqllist = new List<string>();
            for (int i = 0; i < rcordList.Count; i++)
            {
                if (string.IsNullOrEmpty(rcordList[i].hash) || string.IsNullOrEmpty(address))
                    continue;
                string sqlItem = sql;
                sqlItem += "(";
                string value = GetValue(rcordList[i].tokendecimals.ToString()) + "," + 
                GetValue(rcordList[i].blockNumber) + "," + GetValue(rcordList[i].timeStamp) + "," + 
                GetValue(rcordList[i].hash) + "," + GetValue(rcordList[i].from) + "," + 
                GetValue(rcordList[i].to) + "," + GetValue(rcordList[i].value) + "," + 
                GetValue(rcordList[i].gas) + "," + GetValue(rcordList[i].tokenSymbol) + "," + 
                GetValue(rcordList[i].fullName) + "," + GetValue(address) + "," + 
                GetValue(rcordList[i].contractAddress) + "," + GetValue(rcordList[i].confixmations) + "," + 
                GetValue(rcordList[i].txReceiptStatus) + "," + GetValue(rcordList[i].input);
                sqlItem += value;
                sqlItem += ")";
                sqllist.Add(sqlItem);
            }
            while (!sqliteHelper.QueryMulti(sqllist, () =>
            {
                LoomRefresher.QueueOnMainThread(() =>
                {
                    if (OnFinished != null)
                    {
                        OnFinished(true);
                    }
                    isRunning = false;
                });
            }))
            {
                System.Threading.Thread.Sleep(50);
            }
        }));
    }
    
    public bool GetMaxBlockNumber(System.Action<int> OnFinished)
    {
        if (isRunning)
        {
            return false;
        }
        owner.StartCoroutine(QueueThread(() => {
            string sql = "";
            if (isToken)
            {
                sql = "select " + SqliteHelperA.SQL_COL_BLOCKNUMBER + 
                " from " + SqliteHelperA.SQL_TABLE_NAME + " where " + 
                SqliteHelperA.SQL_COL_ADDRESS + " == " + GetValue(address) + " and " +
                SqliteHelperA.SQL_COL_CONTACT_ADDRESS + " <> 'NULL' " + " order by " + SqliteHelperA.SQL_COL_BLOCKNUMBER + " DESC LIMIT 1";
            }
            else
            {
                sql = "select " + SqliteHelperA.SQL_COL_BLOCKNUMBER +
                " from " + SqliteHelperA.SQL_TABLE_NAME + " where " +
                SqliteHelperA.SQL_COL_ADDRESS + " == " + GetValue(address) + " and " +
                SqliteHelperA.SQL_COL_CONTACT_ADDRESS + " == 'NULL' " + " order by " + SqliteHelperA.SQL_COL_BLOCKNUMBER + " DESC LIMIT 1";
            }
            while (!sqliteHelper.Query(sql, (reader) =>
            {
                if (reader.Read())
                {
                    int result = int.Parse(reader.GetString(0));
                    LoomRefresher.QueueOnMainThread(() =>
                    {
                        if (OnFinished != null)
                        {
                            OnFinished(result);
                        }
                        isRunning = false;
                    });
                }
                else
                {
                    LoomRefresher.QueueOnMainThread(() =>
                    {
                        if (OnFinished != null)
                        {
                            OnFinished(0);
                        }
                        isRunning = false;
                    });
                }
            }))
            {
                System.Threading.Thread.Sleep(50);
            }
        }));
        return true;
    }

    public void GetHistory(int page, int pageSize, System.Action<List<ETHHistoryRcord>> OnFinished, string contactAddress = "", bool isKyber = false, HistoryType historyType = HistoryType.All)
    {
        string sql = "";
        string addFilter = "";
        switch (historyType)
        {
            case HistoryType.All:
                break;
            case HistoryType.Received:
                addFilter = " p_to == " + GetValue(address) + " and p_from <> p_to and ";
                break;
            case HistoryType.Self:
                addFilter = " p_from == p_to and ";
                break;
            case HistoryType.Send:
                addFilter = " p_from == " + GetValue(address) + " and p_from <> p_to and ";
                break;
        }
        if (isToken)
        {
            sql = "select * from " + SqliteHelperA.SQL_TABLE_NAME + " where " + addFilter +
            SqliteHelperA.SQL_COL_ADDRESS + " == " + GetValue(address) + " and " + SqliteHelperA.SQL_COL_CONTACT_ADDRESS + " == " + GetValue(contactAddress);
        }
        else if (isKyber)
        {
            sql = "select * from " + SqliteHelperA.SQL_TABLE_NAME + " where " + addFilter +
            SqliteHelperA.SQL_COL_ADDRESS + " == " + GetValue(address) + " and " + SqliteHelperA.SQL_COL_CONTACT_ADDRESS + " == 'NULL'" + 
            " and (" + SqliteHelperA.SQL_COL_TO + "=='0x818e6fecd516ecc3849daf6845e3ec868087b755'" + " or " + SqliteHelperA.SQL_COL_INPUT + 
            " like '0x095ea7b3%' )";
        }
        else
        {
            sql = "select * from " + SqliteHelperA.SQL_TABLE_NAME + " where " + addFilter +
            SqliteHelperA.SQL_COL_ADDRESS + " == " + GetValue(address) + " and " + SqliteHelperA.SQL_COL_CONTACT_ADDRESS + " == 'NULL'" +
            " and (" + SqliteHelperA.SQL_COL_INPUT + " == '0x' or " + SqliteHelperA.SQL_COL_INPUT + " == 'NULL' )";
        }
        page = page < 0 ? 0 : page;
        sql += " order by " + SqliteHelperA.SQL_COL_BLOCKNUMBER + " DESC LIMIT " + pageSize + " OFFSET " + (page - 1) * pageSize;
        if (isRunning)
        {
            if (OnFinished != null)
            {
                Debug.Log("return catche");
                OnFinished(FindCatche(sql));
            }
            return;
        }
        isRunning = true;
        owner.StartCoroutine(QueueThread(() =>
        {
            while (!sqliteHelper.Query(sql, (reader) =>
            {
                List<ETHHistoryRcord> recordList = new List<ETHHistoryRcord>();
                while (reader.Read())// 循环遍历数据
                {
                    ETHHistoryRcord record = new ETHHistoryRcord();
                    string t_decimals = reader.GetValue(reader.GetOrdinal(SqliteHelperA.SQL_COL_DECIMALS)).ToString();
                    record.tokendecimals = string.IsNullOrEmpty(t_decimals) ? 0 : int.Parse(t_decimals);
                    record.blockNumber = reader.GetValue(reader.GetOrdinal(SqliteHelperA.SQL_COL_BLOCKNUMBER)).ToString();
                    record.timeStamp = reader.GetValue(reader.GetOrdinal(SqliteHelperA.SQL_COL_TIMESTAMP)).ToString();
                    record.hash = reader.GetValue(reader.GetOrdinal(SqliteHelperA.SQL_COL_HASH)).ToString();
                    record.from = reader.GetValue(reader.GetOrdinal(SqliteHelperA.SQL_COL_FROM)).ToString();
                    record.to = reader.GetValue(reader.GetOrdinal(SqliteHelperA.SQL_COL_TO)).ToString();
                    record.value = reader.GetValue(reader.GetOrdinal(SqliteHelperA.SQL_COL_VALUE)).ToString();
                    record.gas = reader.GetValue(reader.GetOrdinal(SqliteHelperA.SQL_COL_GAS)).ToString();
                    record.tokenSymbol = reader.GetValue(reader.GetOrdinal(SqliteHelperA.SQL_COL_TOKENSYMBO)).ToString();
                    record.fullName = reader.GetValue(reader.GetOrdinal(SqliteHelperA.SQL_COL_FULLNAME)).ToString();
                    record.contractAddress = reader.GetValue(reader.GetOrdinal(SqliteHelperA.SQL_COL_CONTACT_ADDRESS)).ToString();
                    record.confixmations = reader.GetValue(reader.GetOrdinal(SqliteHelperA.SQL_COL_CONFIXMATIONS)).ToString();
                    record.txReceiptStatus = reader.GetValue(reader.GetOrdinal(SqliteHelperA.SQL_COL_TXRECEIPT_STATUS)).ToString();
                    record.input = reader.GetValue(reader.GetOrdinal(SqliteHelperA.SQL_COL_INPUT)).ToString();
                    recordList.Add(record);
                }
                UpdateCatche(sql, Copy(recordList));
                LoomRefresher.QueueOnMainThread(() =>
                {
                    if (OnFinished != null)
                    {
                        OnFinished(recordList);
                    }
                    isRunning = false;
                });
            }))
            {
                System.Threading.Thread.Sleep(50);
            }
        }));
    }

    /// <summary>
    /// 自定义sql
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="OnFinished">查询完毕返回reader</param>
    /// <returns></returns>
    public bool RunCustomQuery(string sql, System.Action<SqliteDataReader> OnFinished)
    {
        if (isRunning)
        {
            return false;
        }
        isRunning = true;
        owner.StartCoroutine(QueueThread(() =>
        {
            while (!sqliteHelper.Query(sql, (reader) =>
            {
                bool finishedTag = false;
                LoomRefresher.QueueOnMainThread(() =>
                {
                    if (OnFinished != null)
                    {
                        OnFinished(reader);
                    }
                    finishedTag = true; 
                    isRunning = false;
                });
                while (!finishedTag)
                {
                    System.Threading.Thread.Sleep(50);
                }
            }))
            {
                System.Threading.Thread.Sleep(50);
            }
        }));
        return true;
    }

    IEnumerator QueueThread(System.Action callback)
    {
        while (!LoomRefresher.RunAsync(callback))
        {
            yield return new WaitForFixedUpdate();
        }
    }

    private string GetValue(string v)
    {
        return string.IsNullOrEmpty(v) ? "\"NULL\"" : "\"" + v + "\"";
    }

    private List<ETHHistoryRcord> FindCatche(string sql)
    {
        if (catche.ContainsKey(sql))
        {
            return Copy(catche[sql]);
        }
        return new List<ETHHistoryRcord>();
    }

    private void UpdateCatche(string sql, List<ETHHistoryRcord> c)
    {
        catche[sql] = c;
    }

    List<ETHHistoryRcord> Copy(List<ETHHistoryRcord> data)
    {
        List<ETHHistoryRcord> result = new List<ETHHistoryRcord>();
        foreach (var v in data)
        {
            result.Add((ETHHistoryRcord)v.Clone());
        }
        return result;
    }

    public bool CheckAddress(string address)
    {
        return this.address == address;
    }
}
