using DependencyInjectionResolver;
using ADO.ORM.Contracts;
using ADO.ORM.Core;
using ADO.ORM.Enumerators;
using ADO.ORM.SqlCreator.SqLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADO.ORM.SqLite {
    public class SqLiteRepository<T> : Repository<T> {
        private readonly object lock8 = new object();
        private readonly object lock9 = new object();

        public SqLiteRepository(DependencyInjection dependencyInjection, IDBConnection dBConnection, ISqlCreator sqlCreator, ITableHelper tableHelper) : base(dependencyInjection, dBConnection, sqlCreator, tableHelper) {
        }

        public object Save(T entity, Conflict conflict) {
            lock (lock8) {
                var sql = (_sqlCreatorHelper as SqlStringCreatoForSQLite).Insert<T>(entity, conflict);
                _dBConnection.ExecuteQueryNonData(sql);
                return _dBConnection.ExecuteScalar("SELECT last_insert_rowid()");
            }
        }

        public void Save(IEnumerable<T> entities, Conflict conflict) {
            lock (lock9) {
                var sql = (_sqlCreatorHelper as SqlStringCreatoForSQLite).Insert<T>(entities, conflict);
                _dBConnection.ExecuteQueryNonData(sql);
            }
        }
    }
}
