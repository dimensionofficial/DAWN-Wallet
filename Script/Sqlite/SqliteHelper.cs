using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;

public class SqliteHelper : MonoBehaviour
{
    private static SqliteHelper instance = null;

    public static SqliteHelper Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject o = new GameObject();
                o.name = "SqliteHelper";
                instance = o.AddComponent<SqliteHelper>();
                instance.SQLiteInit();
            }
            return instance;
        }
    }

    private const string SQL_DB_NAME = "PlayersSQLite";

    private const string SQL_TABLE_NAME = "Records";

    public const string SQL_COL_ADDRESS = "coinAddress";
    private const string SQL_COL_DECIMALS = "m_tokendecimals";
    private const string SQL_COL_BLOCKNUMBER = "m_blockNumber";
    private const string SQL_COL_TIMESTAMP = "m_timeStamp";
    private const string SQL_COL_HASH = "p_hash";
    private const string SQL_COL_FROM = "p_from";
    private const string SQL_COL_TO = "p_to";
    private const string SQL_COL_VALUE = "p_value";
    private const string SQL_COL_GAS = "p_gas";
    private const string SQL_COL_TOKENSYMBO = "p_tokenSymbol";
    private const string SQL_COL_FULLNAME = "p_fullName";
    private const string SQL_COL_CONTACT_ADDRESS = "p_contactaddress";
    private const string SQL_COL_CONFIXMATIONS = "p_confixmations";
    private const string SQL_COL_TXRECEIPT_STATUS = "m_txReceiptStatus";
    private const string SQL_COL_INPUT = "m_input";

    private string _sqlDBLocation = "URI=file:";

    /// <summary>
    /// 声明一个连接对象
    /// </summary>
    private SqliteConnection _connection;
    /// <summary>
    /// 声明一个操作数据库命令
    /// </summary>
    private SqliteCommand _command;
    /// <summary>
    /// 声明一个读取结果集的一个或多个结果流
    /// </summary>
    private SqliteDataReader _reader;

    //private IDbConnection _connection = null;
    //private IDbCommand _command = null;
    //private IDataReader _reader = null;

    private bool inied = false;

    private void CreatTable(string tableName)
    {
        try
        {
            bool needCreateTable = false;
            _command.CommandText = "SELECT name FROM sqlite_master WHERE name='" + tableName + "'";
            _reader = _command.ExecuteReader();
            if (!_reader.Read())
            {
                needCreateTable = true;
            }
            CloseReader();
            if (needCreateTable)
            {
                Debug.Log("SQLiter - Creating new SQLite table " + tableName);
                _command.CommandText = "DROP TABLE IF EXISTS " + tableName;
                _command.ExecuteNonQuery();
                string sql = "CREATE TABLE " + tableName + " (" +
                  "" + SQL_COL_DECIMALS + " VARCHAR(255) NULL," +
                  "" + SQL_COL_BLOCKNUMBER + " VARCHAR(255) NULL," +
                  "" + SQL_COL_TIMESTAMP + " VARCHAR(255) NULL," +
                  "" + SQL_COL_HASH + " VARCHAR(255) NOT NULL," +
                  "" + SQL_COL_FROM + " VARCHAR(255) NULL," +
                  "" + SQL_COL_TO + " VARCHAR(255) NULL," +
                  "" + SQL_COL_VALUE + " VARCHAR(255) NULL," +
                  "" + SQL_COL_GAS + " VARCHAR(255) NULL," +
                  "" + SQL_COL_TOKENSYMBO + " VARCHAR(255) NULL," +
                  "" + SQL_COL_FULLNAME + " VARCHAR(255) NULL," +
                  "" + SQL_COL_ADDRESS + " VARCHAR(255) NOT NULL," +
                  "" + SQL_COL_CONTACT_ADDRESS + " VARCHAR(255) NULL," +
                  "" + SQL_COL_CONFIXMATIONS + " VARCHAR(255) NULL," +
                  "" + SQL_COL_TXRECEIPT_STATUS + " VARCHAR(255) NULL," +
                  "" + SQL_COL_INPUT + " VARCHAR(255) NULL," +
                  "PRIMARY KEY (" + SQL_COL_HASH + "," + SQL_COL_ADDRESS + "," + SQL_COL_CONTACT_ADDRESS + ")" +
                  ");";
                _command.CommandText = sql;
                _command.ExecuteNonQuery();
            }
        }
        catch (System.Exception e)
        {
            Debug.Log("SQLiter - Creating new SQLite table " + tableName + ":" + e.Message);
            ClosedSQL();
            CloseReader();
            inied = false;
        }
    }

    private void ClosedSQL()
    {
        if (_connection != null)
        {
            _connection.Close();
        }
    }

    private void SQLiteClose()
    {
        if (_reader != null && !_reader.IsClosed)
            _reader.Close();
        _reader = null;

        if (_command != null)
            _command.Dispose();
        _command = null;

        if (_connection != null && _connection.State != ConnectionState.Closed)
            _connection.Close();
        _connection = null;
    }


    private void SQLiteInit()
    {
        _sqlDBLocation += Application.temporaryCachePath + "/PlayersSQLiteCd.db";
        Debug.Log(Application.temporaryCachePath);
        _connection = new SqliteConnection(_sqlDBLocation);
        _command = _connection.CreateCommand();

        if (_connection.State == ConnectionState.Closed)
            _connection.Open();

        // journal mode = look it up on google, I don't remember
        _command.CommandText = "PRAGMA journal_mode = OFF";
        _reader = _command.ExecuteReader();
        _reader.Close();

        CreatTable(SQL_TABLE_NAME);

        ClosedSQL();
        inied = true;
    }

    void OnDestroy()
    {
        SQLiteClose();
    }

   

    bool CheckEnable()
    {
        return inied;
    }

    public void Insert(string address, List<ETHHistoryRcord> rcordList)
    {
        if (!CheckEnable())
        {
            Debug.Log("not inied");
            return;
        }
        InsertData(SQL_TABLE_NAME, address, rcordList);
    }

    public int GetMaxBlockNumber(string address, bool isToken = false)
    {
        int reselt = 0;
        try
        {
            if (_connection.State == ConnectionState.Closed)
                _connection.Open();

            if (isToken)
            {
                _command.CommandText = "select " + SQL_COL_BLOCKNUMBER + " from " + SQL_TABLE_NAME + " where " + SQL_COL_ADDRESS + " == " + GetValue(address) + " and " + SQL_COL_CONTACT_ADDRESS + " is not null " + " order by " + SQL_COL_BLOCKNUMBER + " DESC";
            }
            else
            {
                _command.CommandText = "select " + SQL_COL_BLOCKNUMBER + " from " + SQL_TABLE_NAME + " where " + SQL_COL_ADDRESS + " == " + GetValue(address) + " and " + SQL_COL_CONTACT_ADDRESS + " is null " + " order by " + SQL_COL_BLOCKNUMBER + " DESC";
            }

            _reader = _command.ExecuteReader();
            if (_reader.Read())
            {
                reselt = int.Parse(_reader.GetString(0));
            }

            CloseReader();

            ClosedSQL();
        } catch (System.Exception e)
        {
            CloseReader();

            ClosedSQL();
        }
        
        return reselt;
    }


    private void InsertData(string table_name, string address, List<ETHHistoryRcord> rcordList)
    {
        if (rcordList.Count <= 0)
            return;
        try
        {
            if (_connection.State == ConnectionState.Closed)
                _connection.Open();

            string[] fieldNames = new string[15] { SQL_COL_DECIMALS, SQL_COL_BLOCKNUMBER, SQL_COL_TIMESTAMP, SQL_COL_HASH, SQL_COL_FROM, SQL_COL_TO, SQL_COL_VALUE, SQL_COL_GAS, SQL_COL_TOKENSYMBO, SQL_COL_FULLNAME, SQL_COL_ADDRESS, SQL_COL_CONTACT_ADDRESS, SQL_COL_CONFIXMATIONS, SQL_COL_TXRECEIPT_STATUS, SQL_COL_INPUT };

            _command.CommandText = "insert into " + table_name + "(";

            for (int i = 0; i < fieldNames.Length; i++)
            {
                _command.CommandText += fieldNames[i];
                if (i < fieldNames.Length - 1)
                {
                    _command.CommandText += ",";
                }
            }

            _command.CommandText += ")" + "values ";
            for (int i = 0; i < rcordList.Count; i++)
            {
                if (string.IsNullOrEmpty(rcordList[i].hash) || string.IsNullOrEmpty(address))
                    continue;



                _command.CommandText += "(";

                string value = GetValue(rcordList[i].tokendecimals.ToString()) + "," + GetValue(rcordList[i].blockNumber) + "," + GetValue(rcordList[i].timeStamp) + "," + GetValue(rcordList[i].hash) + "," + GetValue(rcordList[i].from) + "," + GetValue(rcordList[i].to) + "," + GetValue(rcordList[i].value) + "," + GetValue(rcordList[i].gas) + "," + GetValue(rcordList[i].tokenSymbol) + "," + GetValue(rcordList[i].fullName) + "," + GetValue(address) + "," + GetValue(rcordList[i].contractAddress) + "," + GetValue(rcordList[i].confixmations) + "," + GetValue(rcordList[i].txReceiptStatus) + "," + GetValue(rcordList[i].input);

                _command.CommandText += value;

                _command.CommandText += ")";

                if (i < rcordList.Count - 1)
                {
                    _command.CommandText += ",";
                }
            }

            _command.ExecuteReader();
            ClosedSQL();
            CloseReader();
        }
        catch (System.Exception e)
        {
            ClosedSQL();
            CloseReader();
        }

    }

    private string GetValue(string v)
    {
        return string.IsNullOrEmpty(v) ? "NULL" : "\"" + v + "\"";
    }

    public List<ETHHistoryRcord> GetHistoryByAddress(string address, bool isToken = false)
    {
        List<ETHHistoryRcord> recordList = new List<ETHHistoryRcord>();
        try
        {
       
            if(_connection.State == ConnectionState.Closed)
                _connection.Open();

            if (isToken)
            {
                _command.CommandText = "select * from " + SQL_TABLE_NAME + " where " + SQL_COL_ADDRESS + " == " + GetValue(address) + " and " + SQL_COL_CONTACT_ADDRESS + " is not null";
            }
            else
            {
                _command.CommandText = "select * from " + SQL_TABLE_NAME + " where " + SQL_COL_ADDRESS + " == " + GetValue(address) + " and " + SQL_COL_CONTACT_ADDRESS + " is null";
            }

            _reader = _command.ExecuteReader();
       
            while (_reader.Read())// 循环遍历数据
            {
                ETHHistoryRcord record = new ETHHistoryRcord();

                string t_decimals = _reader.GetValue(_reader.GetOrdinal(SQL_COL_DECIMALS)).ToString();

                record.tokendecimals = string.IsNullOrEmpty(t_decimals) ? 0 : int.Parse(t_decimals);

                record.blockNumber = _reader.GetValue(_reader.GetOrdinal(SQL_COL_BLOCKNUMBER)).ToString();
                record.timeStamp = _reader.GetValue(_reader.GetOrdinal(SQL_COL_TIMESTAMP)).ToString();
                record.hash = _reader.GetValue(_reader.GetOrdinal(SQL_COL_HASH)).ToString();
                record.from = _reader.GetValue(_reader.GetOrdinal(SQL_COL_FROM)).ToString();
                record.to = _reader.GetValue(_reader.GetOrdinal(SQL_COL_TO)).ToString();
                record.value = _reader.GetValue(_reader.GetOrdinal(SQL_COL_VALUE)).ToString();
                record.gas = _reader.GetValue(_reader.GetOrdinal(SQL_COL_GAS)).ToString();
                record.tokenSymbol = _reader.GetValue(_reader.GetOrdinal(SQL_COL_TOKENSYMBO)).ToString();
                record.fullName = _reader.GetValue(_reader.GetOrdinal(SQL_COL_FULLNAME)).ToString();
                record.contractAddress = _reader.GetValue(_reader.GetOrdinal(SQL_COL_CONTACT_ADDRESS)).ToString();
                record.confixmations = _reader.GetValue(_reader.GetOrdinal(SQL_COL_CONFIXMATIONS)).ToString();
                record.txReceiptStatus = _reader.GetValue(_reader.GetOrdinal(SQL_COL_TXRECEIPT_STATUS)).ToString();
                record.input = _reader.GetValue(_reader.GetOrdinal(SQL_COL_INPUT)).ToString();

                recordList.Add(record);

            }
            CloseReader();

            ClosedSQL();
        }
        catch (System.Exception e)
        {
            CloseReader();

            ClosedSQL();
        }
        return recordList;
    }

    private void CloseReader()
    {
        if (_reader != null)
        {
            _reader.Close();
            _reader = null;
        }
    }
}
