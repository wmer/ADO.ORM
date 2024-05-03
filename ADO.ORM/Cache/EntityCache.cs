using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace ADO.ORM.Cache; 
public class EntityCache {
    private static Dictionary<Type, Dictionary<object, object>> _entiChache = [];

    public bool ExistInCache<T>(object key) {
        var contains = false;
        if (_entiChache.ContainsKey(typeof(T))) {
            contains = _entiChache[typeof(T)].ContainsKey(key);
        }
        return contains;
    }

    public void SaveInCache<T>(object key, T entity) {
        if (_entiChache.ContainsKey(typeof(T))) {
            if (!ExistInCache<T>(key)) {
                _entiChache[typeof(T)][key] = entity;
            }
        } else {
            var dic = new Dictionary<object, object>();
            dic[key] = entity;
            _entiChache[typeof(T)] = dic;
        }
    }

    public T GetInCache<T>(object key) {
        T entity = default(T);
        if (ExistInCache<T>(key)) {
            entity = (T)_entiChache[typeof(T)][key];
        }
        return entity;
    }

    public void CleanCache() => _entiChache.Clear();
}
