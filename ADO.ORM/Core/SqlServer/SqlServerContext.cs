using ADO.ORM.Contracts;
using ADO.ORM.Helpers.SqlServer;
using ADO.ORM.SqlCreator;
using ADO.ORM.SqlCreator.SqlServer;
using DependencyInjectionResolver;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;

namespace ADO.ORM.Core.SqlServer {
    public class SqlServerContext : DBContext<SqlConnection> {
        private readonly object _lock1 = new object();
        private readonly object _lock2 = new object();

        public SqlServerContext(string path, String dbName) {
            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }
            var file = $"{path}\\{dbName}.mdf";
            if (!File.Exists(file)) {
                CreateDatabase(file);
            }
            var strConn = $"Server=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"{file}\";Database={dbName};Integrated Security=true;Connect Timeout=30;";
            Init(strConn);
        }


        public SqlServerContext(string conectionString) => Init(conectionString);

        private void Init(string conectionString) {
            lock (_lock1) {
                var conn = new SqlConnection(conectionString);
                _dependencyInjection = new DependencyInjection()
                                        .BindingTypes<ISqlCreator, SqlStringCreatorForSqlServer>()
                                        .BindingTypes<IDBConnection, DBConnection<SqlConnection>>()
                                        .BindingTypes<ITableHelper, TableHelper>();
                _sqlCreator = _dependencyInjection
                                        .Resolve<SqlStringCreatorForSqlServer>();
                _dbConnection = _dependencyInjection
                                        .DefineDependency<DBConnection<SqlConnection>>(0, conn)
                                        .Resolve<DBConnection<SqlConnection>>();
                _models = new List<Type>();

                InitializeProperties();
            }
        }

        private void CreateDatabase(string filename) {
            lock (_lock2) {
                string databaseName = Path.GetFileNameWithoutExtension(filename);
                using (var connection = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=master; Integrated Security=true;Connect Timeout=30;")) {
                    connection.Open();
                    using (var command = connection.CreateCommand()) {
                        command.CommandText =
                            String.Format($"CREATE DATABASE {databaseName} ON PRIMARY (NAME={databaseName}, FILENAME='{filename}')");
                        command.ExecuteNonQuery();

                        command.CommandText =
                            String.Format($"EXEC sp_detach_db '{databaseName}', 'true'");
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
