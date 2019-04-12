using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System;

public interface IHistoryRecord {

    void Refresh(string address, System.Action<bool> OnFinished, RefreshType type);
    void RunCustomQuery(string address, string sql, System.Action<SqliteDataReader> OnFinished, bool isToken = false);
    void GetHistory(int page, int pageSize, string address, System.Action<List<ETHHistoryRcord>> OnFinished, string contactAddress = "", bool isKyber = false, bool isToken = false);
}

public interface IRefresher
{
    void Refresh(string address, System.Action<bool> OnFinished, SqliteDicItem sqlite);
}

public enum RefreshType
{
    BTC,
    ETH,
    Token,
    USDT
}

