using DependencyInjectionResolver;
using Microsoft.Data.Sqlite;
using ADO.ORM.Contracts;
using ADO.ORM.Helpers.SqLite;
using ADO.ORM.SqlCreator.SqLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ADO.ORM.Core.SqLite {
    public abstract class SqLiteContext : DBContext<SqliteConnection> {
        protected SqLiteContext(string path, String dbName) {
            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }
            var strConn = $"{path}\\{dbName}";
            var conn = new SqliteConnection($"Data Source={strConn}");
            _dependencyInjection = new DependencyInjection()
                                    .BindingTypes<ISqlCreator, SqlStringCreatoForSQLite>()
                                    .BindingTypes<IDBConnection, DBConnection<SqliteConnection>>()
                                    .BindingTypes<ITableHelper, TableHelper>();
            _sqlCreator = _dependencyInjection
                                    .Resolve<SqlStringCreatoForSQLite>();
            _dbConnection = _dependencyInjection
                                    .DefineDependency<DBConnection<SqliteConnection>>(0, conn)
                                    .Resolve<DBConnection<SqliteConnection>>();
            _models = new List<Type>();

            InitializeProperties();
        }
    }
}
