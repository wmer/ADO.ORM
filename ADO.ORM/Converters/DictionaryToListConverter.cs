using DependencyInjectionResolver.Extensions;
using ADO.ORM.Builders;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ADO.ORM.Converters; 
public class DictionaryToListConverter {
    private EntityBuilder _entityBuilder;

    public DictionaryToListConverter(EntityBuilder entityBuilder) {
        _entityBuilder = entityBuilder;
    }

    public List<T> Converte<T>(Dictionary<int, Dictionary<String, object>> dictionary) {
        var list = new BlockingCollection<T>();
        foreach (var dic in dictionary) {
            T entity = _entityBuilder.Create<T>(dic.Value);
            list.Add(entity);
        }
        return list.ToList<T>();
    }
}
