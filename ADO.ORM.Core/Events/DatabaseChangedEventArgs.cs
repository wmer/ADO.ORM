using System;
using System.Collections.Generic;
using System.Text;

namespace ADO.ORM.Core.Events {
    public class DatabaseChangedEventArgs : EventArgs {
        public DateTime DateTime { get; set; }
        public string Sql { get; set; }

        public DatabaseChangedEventArgs(DateTime dateTime, string sql) {
            DateTime = dateTime;
            Sql = sql;
        }
    }

    public delegate void DatabaseChangedEventHandler(object sender, DatabaseChangedEventArgs ev);
}
