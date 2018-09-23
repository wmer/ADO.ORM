using System;
using System.Collections.Generic;
using System.Text;

namespace ADO.ORM.Core.Events {
    public class DatabaseOperationFailedEventArgs : EventArgs {
        public string Sql { get; set; }
        public Exception Exception { get; set; }
        public DateTime DateTime { get; set; }

        public DatabaseOperationFailedEventArgs(string sql, Exception exception, DateTime dateTime) {
            Sql = sql;
            Exception = exception;
            DateTime = dateTime;
        }
    }

    public delegate void DatabaseOperationFailedEventHandler(object sender, DatabaseOperationFailedEventArgs ev);
}
