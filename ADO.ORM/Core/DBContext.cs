using DependencyInjectionResolver;
using ADO.ORM.Contracts;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using ADO.ORM.Attributes;
using ADO.ORM.Cache;
using ADO.ORM.Converters;
using ADO.ORM.Helpers;

namespace ADO.ORM.Core; 
public class DBContext<T> : IDisposable where T : DbConnection {
    internal DBConnection<T> _dbConnection;
    internal DependencyInjection _dependencyInjection;
    internal ISqlCreator _sqlCreator;
    protected ITableHelper _tableHelper;
    protected EntityHelper _entityHelper;
    internal DictionaryToListConverter _dictionaryToList;
    internal EntityCache _entityCache;
    internal List<Type> _models;

    internal void InitializeProperties() {
        var properties = GetType().GetProperties();
        foreach (var prop in properties) {
            var attributes = prop.GetCustomAttributes(true);
            var isIgnorable = attributes.Where(x => x is IgnoreAttribute).FirstOrDefault() is IgnoreAttribute;
            if (!isIgnorable) {
                var genericArguments = prop.PropertyType.GenericTypeArguments;
                var value = _dependencyInjection
                                .DefineDependency(prop.PropertyType, 0, _dependencyInjection)
                                .DefineDependency(prop.PropertyType, 1, _dbConnection)
                                .DefineDependency(prop.PropertyType, 2, _sqlCreator)
                                .DefineDependency(prop.PropertyType, 3, _tableHelper)
                                .DefineDependency(prop.PropertyType, 4, _dictionaryToList)
                                .DefineDependency(prop.PropertyType, 5, _entityHelper)
                                .DefineDependency(prop.PropertyType, 6, _entityCache)
                                .Resolve(prop.PropertyType, InstanceOptions.DiferentInstances);
                prop.SetValue(this, value, null);
                _models.Add(genericArguments.Single());
            }
        }

        var sql = _sqlCreator.CreateTables(_models);
        _dbConnection.ExecuteQueryNonData(sql);
    }

    public void Dispose() => _dbConnection.Dispose();
}
 