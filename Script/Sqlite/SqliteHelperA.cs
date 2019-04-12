using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;

public class SqliteHelperA : MonoBehaviour
{
    public const string SQL_TABLE_NAME = "Records";

    public const string SQL_COL_ADDRESS = "coinAddress";
    public const string SQL_COL_DECIMALS = "m_tokendecimals";
    public const string SQL_COL_BLOCKNUMBER = "m_blockNumber";
    public const string SQL_COL_TIMESTAMP = "m_timeStamp";
    public const string SQL_COL_HASH = "p_hash";
    public const string SQL_COL_FROM = "p_from";
    public const string SQL_COL_TO = "p_to";
    public const string SQL_COL_VALUE = "p_value";
    public const string SQL_COL_GAS = "p_gas";
    public const string SQL_COL_TOKENSYMBO = "p_tokenSymbol";
    public const string SQL_COL_FULLNAME = "p_fullName";
    public const string SQL_COL_CONTACT_ADDRESS = "p_contactaddress";
    public const string SQL_COL_CONFIXMATIONS = "p_confixmations";
    public const string SQL_COL_TXRECEIPT_STATUS = "m_txReceiptStatus";
    public const string SQL_COL_INPUT = "m_input";

    private string _sqlDBLocation = "URI=file:";

    private class SqliteConnectionPoolItem
    {
        public SqliteConnection _connection;
        /// <summary>
        /// 声明一个操作数据库命令
        /// </summary>
        public SqliteCommand _command;
        /// <summary>
        /// 声明一个读取结果集的一个或多个结果流
        /// </summary>
        public SqliteDataReader _reader;

        public SqliteConnectionPoolItem(string path)
        {
            _connection = new SqliteConnection(path);
            _command = _connection.CreateCommand();

            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            // journal mode = look it up on google, I don't remember
            _command.CommandText = "PRAGMA journal_mode = OFF";
            _reader = _command.ExecuteReader();
            Close();
            if (_connection.State != ConnectionState.Open)
                _connection.Open();
            _command.CommandText = "PRAGMA busy_timeout = 4000";
            _reader = _command.ExecuteReader();
            Close();
        }

        public void Open()
        {
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }
        }

        public void Close()
        {
            if (_connection != null && _connection.State != ConnectionState.Closed)
            {
                _connection.Close();
            }
            if (_reader != null && !_reader.IsClosed)
            {
                _reader.Close();
                _reader = null;
            }
        }

        public void CloseDataBase()
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
    }

    private Queue<SqliteConnectionPoolItem> pool = new Queue<SqliteConnectionPoolItem>();

    private int poolSize = 1;

    private bool inied = false;

    private SqliteConnectionPoolItem currentSqliteConnectionPoolItem = null;

    private SqliteConnectionPoolItem QueueItem()
    {
        if (pool.Count > 0)
        {
            var r =  pool.Dequeue(); 
            r.Open();
            currentSqliteConnectionPoolItem = r;
            return r;
        }
        else
        {
            return null;
        }
    }

    private void DeQueueItem(SqliteConnectionPoolItem item)
    {
        item.Close();
        pool.Enqueue(item);
        currentSqliteConnectionPoolItem = null;
    }

    private void CreatTable(string tableName)
    {
        Loom.RunAsync(() =>
        {
            var sqlHelper = QueueItem();
            if (sqlHelper == null)
            {
                return;
            }
            bool needCreateTable = false;
            sqlHelper._command.CommandText = "SELECT name FROM sqlite_master WHERE name='" + tableName + "'";
            sqlHelper._reader = sqlHelper._command.ExecuteReader();
            if (!sqlHelper._reader.Read())
            {
                needCreateTable = true;
            }
            DeQueueItem(sqlHelper);
            if (needCreateTable)
            {
                sqlHelper = QueueItem();
                //Debug.Log("SQLiter - Creating new SQLite table " + tableName);
                //sqlHelper._command.CommandText = "DROP TABLE IF EXISTS " + tableName;
                //sqlHelper._command.ExecuteNonQuery();
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
                    "" + SQL_COL_CONTACT_ADDRESS + " VARCHAR(255) NOT NULL," +
                    "" + SQL_COL_CONFIXMATIONS + " VARCHAR(255) NULL," +
                    "" + SQL_COL_TXRECEIPT_STATUS + " VARCHAR(255) NULL," +
                    "" + SQL_COL_INPUT + " VARCHAR(510) NULL," +
                    "PRIMARY KEY (" + SQL_COL_HASH + "," + SQL_COL_ADDRESS + "," + SQL_COL_CONTACT_ADDRESS + ")" +
                    ");";
                sqlHelper._command.CommandText = sql;
                sqlHelper._command.ExecuteNonQuery();
                DeQueueItem(sqlHelper);
            }
            inied = true;
        });
    }


    public void SQLiteInit(string address)
    {
        _sqlDBLocation += Application.persistentDataPath + "/"+ address + "db.db";
        for (int i = 0; i < poolSize; i++)
        {
            pool.Enqueue(new SqliteConnectionPoolItem(_sqlDBLocation));
        }
        //inied = true;
        CreatTable(SQL_TABLE_NAME);
    }

    void OnDestroy()
    {
        foreach (var v in pool)
        {
            v.CloseDataBase();
        }
        if(currentSqliteConnectionPoolItem != null)
            currentSqliteConnectionPoolItem.CloseDataBase();
    }

    public bool Query(string sql, System.Action<SqliteDataReader> OnFinished)
    {
        if (!inied)
        {
            return false;
        }
        var sqlHelper = QueueItem();
        if (sqlHelper == null)
        {
            return false;
        }
        sqlHelper._command.CommandText = sql;
        //Debug.Log(sql);
        try
        {
            sqlHelper._reader = sqlHelper._command.ExecuteReader();
            //Debug.Log("insert ok");
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }
        if (OnFinished != null)
        {
            OnFinished(sqlHelper._reader);
        }
        DeQueueItem(sqlHelper);
        return true;
    }

    public bool QueryMulti(List<string> sqlList, System.Action OnFinished)
    {
        if (!inied)
        {
            return false;
        }
        var sqlHelper = QueueItem();
        if (sqlHelper == null)
        {
            return false;
        }
        SqliteCommand cmd = new SqliteCommand();
        cmd.Connection = sqlHelper._connection;
        SqliteTransaction tx = sqlHelper._connection.BeginTransaction();
        cmd.Transaction = tx;
        try
        {
            foreach (var v in sqlList)
            {
                cmd.CommandText = v;
                cmd.ExecuteNonQuery();
            }
            tx.Commit();
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }
        if (OnFinished != null)
        {
            OnFinished();
        }
        DeQueueItem(sqlHelper);
        return true;
    }

    private string GetValue(string v)
    {
        return string.IsNullOrEmpty(v) ? "NULL" : "\"" + v + "\"";
    }
}
