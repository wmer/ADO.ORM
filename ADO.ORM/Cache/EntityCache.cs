using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace ADO.ORM.Cache {
    internal class EntityCache {
        private static ConcurrentDictionary<Type, ConcurrentDictionary<object, object>> _entiChache = new ConcurrentDictionary<Type, ConcurrentDictionary<object, object>>();

        private object lock1 = new object();
        private object lock2 = new object();
        private object lock3 = new object();
        private object lock4 = new object();

        public bool ExistInCache<T>(object key) {
            lock (lock1) {
                var contains = false;
                if (_entiChache.ContainsKey(typeof(T))) {
                    contains = _entiChache[typeof(T)].ContainsKey(key);
                }
                return contains;
            }
        }

        public void SaveInCache<T>(object key, T entity) {
            lock (lock2) {
                if (_entiChache.ContainsKey(typeof(T))) {
                    if (!ExistInCache<T>(key)) {
                        _entiChache[typeof(T)][key] = entity;
                    }
                } else {
                    var dic = new ConcurrentDictionary<object, object>();
                    dic[key] = entity;
                    _entiChache[typeof(T)] = dic;
                }
            }
        }

        public T GetInCache<T>(object key) {
            lock (lock3) {
                T entity = default(T);
                if (ExistInCache<T>(key)) {
                    entity = (T)_entiChache[typeof(T)][key];
                }
                return entity;
            }
        }

        public void CleanCache() {
            lock (lock4) {
                _entiChache.Clear();
            }
        }
    }
}
