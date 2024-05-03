using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using ADO.ORM.Converters;

namespace ADO.ORM.Core; 
public class DBConnection<T>(T connection, DataReaderToDictionaryConverter dataReaderConverter) : IDBConnection where T : DbConnection {
    private T _connection = connection;
    private DataReaderToDictionaryConverter _dataReaderConverter = dataReaderConverter;

    public virtual int ExecuteQueryNonData(string query) {
        var command = CreateQuery(query);
        var ststus = command.ExecuteNonQuery();
        _connection.Close();
        return ststus;
    }

    public virtual Dictionary<int, Dictionary<string, object>> QueryWithData(string query) {
        var command = CreateQuery(query);
        var result = command.ExecuteReader();
        var Dictionary = _dataReaderConverter.Converte(result);
        _connection.Close();
        return Dictionary;
    }

    public virtual object ExecuteScalar(string query) {
        var command = CreateQuery(query);
        var result = command.ExecuteScalar();
        _connection.Close();
        return result;
    }

    public virtual DbCommand CreateQuery(string query) {
        _connection.Open();
        var command = _connection.CreateCommand();
        command.CommandText = query;
        return command;
    }

    public void Dispose() => _connection.Dispose();
}
