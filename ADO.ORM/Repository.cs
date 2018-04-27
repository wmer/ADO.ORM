using DependencyInjectionResolver;
using ADO.ORM.Cache;
using ADO.ORM.Contracts;
using ADO.ORM.Converters;
using ADO.ORM.Core;
using ADO.ORM.Enumerators;
using ADO.ORM.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ADO.ORM {
    public class Repository<T> {
        protected DependencyInjection _dependencyInjection;
        protected IDBConnection _dBConnection;
        protected ISqlCreator _sqlCreatorHelper;
        protected EntityHelper _entityHelper;
        internal DictionaryToListConverter _dictionaryToList;
        protected ITableHelper _tableHelper;
        internal EntityCache _entityCache;

        protected String _sqlString;

        private object lock1 = new object();
        private object lock2 = new object();
        private object lock3 = new object();
        private object lock4 = new object();
        private object lock5 = new object();
        private object lock6 = new object();
        private object lock7 = new object();
        private object lock10 = new object();
        private object lock11 = new object();
        private object lock12 = new object();
        private object lock13 = new object();
        private object lock14 = new object();
        private object lock15 = new object();
        private object lock16 = new object();
        private object lock17 = new object();
        private object lock18 = new object();

        public Repository(
                DependencyInjection dependencyInjection,
                IDBConnection dBConnection,
                ISqlCreator sqlCreator,
                ITableHelper tableHelper) {
            _dBConnection = dBConnection;
            _dependencyInjection = dependencyInjection;
            _sqlCreatorHelper = sqlCreator;
            _tableHelper = tableHelper;
            var ts = typeof(T);
            _dictionaryToList = _dependencyInjection.Resolve<DictionaryToListConverter>();
            _entityHelper = _dependencyInjection.Resolve<EntityHelper>();
            _entityCache = _dependencyInjection.Resolve<EntityCache>();
        }

        public List<T> FindAll() {
            lock (lock1) {
                var dic = _dBConnection.QueryWithData(_sqlCreatorHelper.Select<T>());
                _entityCache.CleanCache();
                return _dictionaryToList.Converte<T>(dic);
            }
        }

        internal T Find(String wherClausule) {
            lock (lock2) {
                var dic = _dBConnection.QueryWithData(_sqlCreatorHelper.Select<T>(wherClausule));
                _entityCache.CleanCache();
                return _dictionaryToList.Converte<T>(dic).FirstOrDefault();
            }
        }

        public T FindOne(string primaryKey) {
            lock (lock3) {
                if (_entityHelper.GetPrimaryKey(typeof(T)) is PropertyInfo pkProperty) {
                    return Find($"{_tableHelper.GetCollumName(pkProperty)}='{primaryKey}'");
                }
                throw new Exception("O Modelo especificado não possui chave primária");
            }
        }

        public Repository<T> Find() {
            lock (lock4) {
                _sqlString = _sqlCreatorHelper.Select<T>();
                return this;
            }
        }

        public Repository<T> Find(Expression<Func<T, object>> parameters) {
            lock (lock5) {
                _sqlString = _sqlCreatorHelper.Select(parameters);
                return this;
            }
        }

        public object Save(T entity) {
            lock (lock6) {
                var sql = _sqlCreatorHelper.Insert<T>(entity);
                _dBConnection.ExecuteQueryNonData(sql);
                return _dBConnection.ExecuteScalar("SELECT last_insert_rowid()");
            }
        }

        public void Save(IEnumerable<T> entities) {
            lock (lock7) {
                var sql = _sqlCreatorHelper.Insert<T>(entities);
                _dBConnection.ExecuteQueryNonData(sql);
            }
        }

        public void Update(T entity) {
            lock (lock10) {
                _dBConnection.ExecuteQueryNonData(_sqlCreatorHelper.Update<T>(entity));
            }
        }

        public void Update(IEnumerable<T> entities) {
            lock (lock11) {
                var sql = _sqlCreatorHelper.Update<T>(entities);
                _dBConnection.ExecuteQueryNonData(sql);
            }
        }

        public Repository<T> Update(Expression<Func<T, object>> parameters, params object[] values) {
            lock (lock12) {
                _sqlString = _sqlCreatorHelper.Update(parameters, values);
                return this;
            }
        }

        public void Delete(T entity) {
            lock (lock13) {
                var sql = _sqlCreatorHelper.Delete<T>(entity);
                _dBConnection.ExecuteQueryNonData(sql);
            }
        }

        public Repository<T> Delete() {
            lock (lock14) {
                _sqlString = _sqlCreatorHelper.Delete<T>();
                return this;
            }
        }

        public Repository<T> Where(Expression<Func<T, bool>> predicate, string like = "") {
            lock (lock15) {
                if (!String.IsNullOrEmpty(_sqlString)) {
                    _sqlString += _sqlCreatorHelper.Where(predicate, like);
                }
                return this;
            }
        }

        public Repository<T> OrderBy(Expression<Func<T, object>> parameters, params SortedBy[] sortedBy) {
            lock (lock16) {
                if (!String.IsNullOrEmpty(_sqlString)) {
                    _sqlString += _sqlCreatorHelper.OrderBy(parameters, sortedBy);
                }
                return this;
            }
        }

        public List<T> Execute() {
            lock (lock17) {
                if (!String.IsNullOrEmpty(_sqlString)) {
                    var dic = _dBConnection.QueryWithData(_sqlString);
                    _entityCache.CleanCache();
                    return _dictionaryToList.Converte<T>(dic);
                }
                return new List<T>();
            }
        }

        public T GetOne() {
            lock (lock18) {
                var list = Execute();
                if (list != null && list.Count > 0) {
                    return list[0];
                }
                return default(T);
            }
        }
    }
}
