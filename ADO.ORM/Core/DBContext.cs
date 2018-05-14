using DependencyInjectionResolver;
using ADO.ORM.Contracts;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using ADO.ORM.Attributes;

namespace ADO.ORM.Core {
    public class DBContext<T> : IDisposable where T : DbConnection {
        internal DBConnection<T> _dbConnection;
        internal DependencyInjection _dependencyInjection;
        internal ISqlCreator _sqlCreator;
        internal List<Type> _models;

        private object lock1 = new object();
        private object lock5 = new object();

        internal void InitializeProperties() {
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
