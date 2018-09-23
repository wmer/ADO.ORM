using ADO.ORM.Core.Attributes;
using ADO.ORM.Core.Helpers;
using DependencyInjectionResolver;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace ADO.ORM.Core {
    public class DBContext<T> : IDisposable where T : DbConnection {
        protected IDBConnection _dbConnection;
        protected DependencyInjection _dependencyInjection;
        protected ISqlCreator _sqlCreator;
        protected List<Type> _models;

        private readonly object lock1 = new object();
        private readonly object lock5 = new object();

        protected void InitializeProperties() {
            lock (lock1) {
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
                                        .Resolve(prop.PropertyType, InstanceOptions.DiferentInstances);
                        prop.SetValue(this, value, null);
                        _models.Add(genericArguments.Single());
                    }
                }

                var sql = _sqlCreator.CreateTables(_models);
                _dbConnection.ExecuteQueryNonData(sql);
            }
        }

        public void Dispose() {
            lock (lock5) {
                _dbConnection.Dispose();
            }
        }
    }
}
