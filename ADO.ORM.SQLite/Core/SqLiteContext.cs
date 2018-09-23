using ADO.ORM.Core;
using ADO.ORM.Core.Helpers;
using ADO.ORM.SQLite.Helpers;
using DependencyInjectionResolver;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ADO.ORM.SQLite.Core {
    public class SqLiteContext : DBContext<SqliteConnection> {
        private readonly object _lock1 = new object();

        protected SqLiteContext(string path, String dbName) {
            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }
            var strConn = $"{path}\\{dbName}";
            Init($"Data Source={strConn}");
        }

        public SqLiteContext(string conectionString) => Init(conectionString);

        private void Init(string conectionString) {
            lock (_lock1) {
                var conn = new SqliteConnection(conectionString);
                _dependencyInjection = new DependencyInjection()
                                        .BindingTypes<ISqlCreator, SqlStringCreatorHelper>()
                                        .BindingTypes<IDBConnection, DBConnection<SqliteConnection>>()
                                        .BindingTypes<ITableHelper, TableHelper>();
                _sqlCreator = _dependencyInjection
                                        .Resolve<SqlStringCreatorHelper>();
                _dbConnection = _dependencyInjection
                                        .DefineDependency<DBConnection<SqliteConnection>>(0, conn)
                                        .Resolve<DBConnection<SqliteConnection>>();
                _models = new List<Type>();

                InitializeProperties();
            }
        }
    }
}
