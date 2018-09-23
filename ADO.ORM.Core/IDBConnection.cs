using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace ADO.ORM.Core {
    public interface IDBConnection : IDisposable {
        int ExecuteQueryNonData(string query);
        ConcurrentDictionary<int, ConcurrentDictionary<String, object>> QueryWithData(string query);
        object ExecuteScalar(string query);
    }
}