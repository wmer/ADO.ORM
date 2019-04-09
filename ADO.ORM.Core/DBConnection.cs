using ADO.ORM.Core.Converters;
using ADO.ORM.Core.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace ADO.ORM.Core {
    public class DBConnection<T> : IDBConnection where T : DbConnection {
        private T _connection;
        private IDataReaderToDictionaryConverter _dataReaderConverter;


        private readonly object lock1 = new object();
        private readonly object lock2 = new object();
        private readonly object lock3 = new object();
        private readonly object lock4 = new object();
        private readonly object lock5 = new object();

        public DBConnection(T connection, IDataReaderToDictionaryConverter dataReaderConverter) {
            _connection = connection;
            _dataReaderConverter = dataReaderConverter;
        }

        public virtual int ExecuteQueryNonData(string query) {
            lock (lock1) {
                try {
                    var command = CreateQuery(query);
                    var ststus = command.ExecuteNonQuery();
                    _connection.Close();
                    DatabseEventHub.OnDatabaseChanged(this, new DatabaseChangedEventArgs(DateTime.Now, query));
                    return ststus;
                }catch(Exception e){
                    DatabseEventHub.OnDatabaseOperationFailed(this, new DatabaseOperationFailedEventArgs(query, e, DateTime.Now));
                    return 0;
                }
            }
        }

        public virtual ConcurrentDictionary<int, ConcurrentDictionary<String, object>> QueryWithData(string query) {
            lock (lock2) {
                try {
                    var command = CreateQuery(query);
                    var result = command.ExecuteReader();
                    var concurrentDictionary = _dataReaderConverter.Converte(result);
                    _connection.Close();
                    return concurrentDictionary;
                } catch(Exception e) {
                    DatabseEventHub.OnDatabaseOperationFailed(this, new DatabaseOperationFailedEventArgs(query, e, DateTime.Now));
                    return null;
                }
            }
        }

        public virtual object ExecuteScalar(string query) {
            lock (lock3) {
                try {
                    var command = CreateQuery(query);
                    var result = command.ExecuteScalar();
                    _connection.Close();
                    DatabseEventHub.OnDatabaseChanged(this, new DatabaseChangedEventArgs(DateTime.Now, query));
                    return result;
                } catch(Exception e) {
                    DatabseEventHub.OnDatabaseOperationFailed(this, new DatabaseOperationFailedEventArgs(query, e, DateTime.Now));
                    return null;
                }
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
