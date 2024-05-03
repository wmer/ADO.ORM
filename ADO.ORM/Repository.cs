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
using ADO.ORM.SqlCreator.SqLite;

namespace ADO.ORM; 
public class Repository<T> {
    protected DependencyInjection _dependencyInjection;
    protected IDBConnection _dBConnection;
    protected ISqlCreator _sqlCreatorHelper;
    protected EntityHelper _entityHelper;
    internal DictionaryToListConverter _dictionaryToList;
    protected ITableHelper _tableHelper;
    internal EntityCache _entityCache;
     
    protected string _sqlString = "";

    public Repository(
            DependencyInjection dependencyInjection,
            IDBConnection dBConnection,
            ISqlCreator sqlCreator,
            ITableHelper tableHelper,
            DictionaryToListConverter dictionaryToList,
            EntityHelper entityHelper,
            EntityCache entityCache) {
        _dBConnection = dBConnection;
        _dependencyInjection = dependencyInjection;
        _sqlCreatorHelper = sqlCreator;
        _tableHelper = tableHelper;
        _dictionaryToList = dictionaryToList;
        _entityHelper = entityHelper;
        _entityCache = entityCache;
    }

    public object Save(T entity, Conflict conflict) {
        var sql = (_sqlCreatorHelper as SqlStringCreatoForSQLite).Insert<T>(entity, conflict);
        _dBConnection.ExecuteQueryNonData(sql);
        return _dBConnection.ExecuteScalar("SELECT last_insert_rowid()");
    }

    public void Save(IEnumerable<T> entities, Conflict conflict) {
        var sql = (_sqlCreatorHelper as SqlStringCreatoForSQLite).Insert<T>(entities, conflict);
        _dBConnection.ExecuteQueryNonData(sql);
    }

    public List<T> FindAll() {
        var dic = _dBConnection.QueryWithData(_sqlCreatorHelper.Select<T>());
        _entityCache.CleanCache();
        return _dictionaryToList.Converte<T>(dic);
    }

    internal T Find(String wherClausule) {
        var dic = _dBConnection.QueryWithData(_sqlCreatorHelper.Select<T>(wherClausule));
        _entityCache.CleanCache();
        return _dictionaryToList.Converte<T>(dic).FirstOrDefault();
    }

    public T FindOne(string primaryKeyValue) {
        if (_entityHelper.GetPrimaryKey(typeof(T)) is PropertyInfo pkProperty) {
            return Find($"{_tableHelper.GetCollumName(pkProperty)}='{primaryKeyValue}'");
        }
        throw new Exception("O Modelo especificado não possui chave primária");
    }

    public Repository<T> Find() {
        _sqlString = _sqlCreatorHelper.Select<T>();
        return this;
    }

    public Repository<T> Find(Expression<Func<T, object>> parameters) {
        _sqlString = _sqlCreatorHelper.Select(parameters);
        return this;
    }

    public object Save(T entity) {
        var sql = _sqlCreatorHelper.Insert<T>(entity);
        _dBConnection.ExecuteQueryNonData(sql);
        return _dBConnection.ExecuteScalar("SELECT last_insert_rowid()");
    }

    public void Save(IEnumerable<T> entities) {
        var sql = _sqlCreatorHelper.Insert<T>(entities);
        _dBConnection.ExecuteQueryNonData(sql);
    }

    public void Update(T entity) {
        _dBConnection.ExecuteQueryNonData(_sqlCreatorHelper.Update<T>(entity));
    }

    public void Update(IEnumerable<T> entities) {
        var sql = _sqlCreatorHelper.Update<T>(entities);
        _dBConnection.ExecuteQueryNonData(sql);
    }

    public Repository<T> Update(Expression<Func<T, object>> parameters, params object[] values) {
        _sqlString = _sqlCreatorHelper.Update(parameters, values);
        return this;
    }

    public void Delete(T entity) {
        var sql = _sqlCreatorHelper.Delete<T>(entity);
        _dBConnection.ExecuteQueryNonData(sql);
    }

    public Repository<T> Delete() {
        _sqlString = _sqlCreatorHelper.Delete<T>();
        return this;
    }

    public Repository<T> Where(Expression<Func<T, bool>> predicate, string like = "") {
        if (!String.IsNullOrEmpty(_sqlString)) {
            _sqlString += _sqlCreatorHelper.Where(predicate, like);
        }
        return this;
    }

    public Repository<T> OrderBy(Expression<Func<T, object>> parameters, params SortedBy[] sortedBy) {
        if (!String.IsNullOrEmpty(_sqlString)) {
            _sqlString += _sqlCreatorHelper.OrderBy(parameters, sortedBy);
        }
        return this;
    }

    public List<T> Execute() {
        if (!String.IsNullOrEmpty(_sqlString)) {
            var dic = _dBConnection.QueryWithData(_sqlString);
            _entityCache.CleanCache();
            return _dictionaryToList.Converte<T>(dic);
        }
        return [];
    }

    public T? GetOne() {
        var list = Execute();
        if (list != null && list.Count > 0) {
            return list[0];
        }
        return default;
    }
}
