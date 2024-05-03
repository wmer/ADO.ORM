using DependencyInjectionResolver;
using Microsoft.Data.Sqlite;
using ADO.ORM.Contracts;
using ADO.ORM.Helpers.SqLite;
using ADO.ORM.SqlCreator.SqLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ADO.ORM.Cache;
using ADO.ORM.Converters;
using ADO.ORM.Helpers;

namespace ADO.ORM.Core.SqLite; 
public abstract class SqLiteContext : DBContext<SqliteConnection> {

    protected SqLiteContext(string path, string dbName) {
        if (!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
        }
        var strConn = $"{path}\\{dbName}";
        Init($"Data Source={strConn}");
    }

    private void Init(string conectionString) {
        var conn = new SqliteConnection(conectionString);
        _dependencyInjection = new DependencyInjection()
                                .BindingTypes<ISqlCreator, SqlStringCreatoForSQLite>()
                                .BindingTypes<IDBConnection, DBConnection<SqliteConnection>>()
                                .BindingTypes<ITableHelper, TableHelper>();

        var propertyHelper = _dependencyInjection.Resolve<PropertyHelper>();
        _entityCache = _dependencyInjection.Resolve<EntityCache>();
        _entityHelper = _dependencyInjection.Resolve<EntityHelper>();
        _tableHelper = _dependencyInjection.Resolve<TableHelper>();
        _sqlCreator = _dependencyInjection.Resolve<SqlStringCreatoForSQLite>();
        _dictionaryToList = _dependencyInjection.Resolve<DictionaryToListConverter>();

        _dbConnection = _dependencyInjection
                                .DefineDependency<DBConnection<SqliteConnection>>(0, conn)
                                .Resolve<DBConnection<SqliteConnection>>();
        _models = new List<Type>();

        InitializeProperties(); 
    }
}
