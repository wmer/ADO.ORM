using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace ADO.ORM.Core {
    public interface IDBConnection : IDisposable {
        int ExecuteQueryNonData(string query);
        Dictionary<int, Dictionary<string, object>> QueryWithData(string query);
        object ExecuteScalar(string query);
    }
}
