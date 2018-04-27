using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using ADO.ORM.Converters;

namespace ADO.ORM.Core {
    public class DBConnection<T> : IDBConnection where T : DbConnection {
        private T _connection;
        private DataReaderToDictionaryConverter _dataReaderConverter;

        private object lock1 = new object();
        private object lock2 = new object();
        private object lock3 = new object();
        private object lock4 = new object();
        private object lock5 = new object();

        public DBConnection(T connection, DataReaderToDictionaryConverter dataReaderConverter) {
            _connection = connection;
            _dataReaderConverter = dataReaderConverter;
        }

        public virtual int ExecuteQueryNonData(string query) {
            lock (lock1) {
                var command = CreateQuery(query);
                var ststus = command.ExecuteNonQuery();
                _connection.Close();
                return ststus;
            }
        }

        public virtual ConcurrentDictionary<int, ConcurrentDictionary<String, object>> QueryWithData(string query) {
            lock (lock2) {
                var command = CreateQuery(query);
                var result = command.ExecuteReader();
                _connection.Close();
                return _dataReaderConverter.Converte(result);
            }
        }

        public virtual object ExecuteScalar(string query) {
            lock (lock3) {
                var command = CreateQuery(query);
                var result = command.ExecuteScalar();
                _connection.Close();
                return result;
            }
        }

        public virtual DbCommand CreateQuery(string query) {
            lock (lock4) {
                _connection.Open();
                var command = _connection.CreateCommand();
                command.CommandText = query;
                return command;
            }
        }

        public void Dispose() {
            lock (lock5) {
                _connection.Dispose();
            }
        }
    }
}
