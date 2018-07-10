using DependencyInjectionResolver.Extensions;
using ADO.ORM.Builders;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ADO.ORM.Converters {
    internal class DictionaryToListConverter {
        private EntityBuilder _entityBuilder;

        private readonly object lock1 = new object();

        public DictionaryToListConverter(EntityBuilder entityBuilder) {
            _entityBuilder = entityBuilder;
        }

        public List<T> Converte<T>(ConcurrentDictionary<int, ConcurrentDictionary<String, object>> dictionary) {
            lock (lock1) {
                var list = new BlockingCollection<T>();
                foreach (var dic in dictionary) {
                    T entity = _entityBuilder.Create<T>(dic.Value);
                    list.Add(entity);
                }
                return list.ToList<T>();
            }
        }
    }
}
