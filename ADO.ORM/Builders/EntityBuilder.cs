using DependencyInjectionResolver;
using ADO.ORM.Cache;
using ADO.ORM.Contracts;
using ADO.ORM.Helpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using HelpersLibs.Reflection.Helpers;

namespace ADO.ORM.Builders; 
public class EntityBuilder(DependencyInjection _classBuilder, PropertyHelper _propertyHelper, ITableHelper _tableHelper, 
                                                MethodHelper _reflectionOptimizations, RepositoryBuilder _repositoryBuilder, 
                                                EntityHelper _entityHelper, EntityCache _entityCache) {

    public T Create<T>(Dictionary<string, object> dictionary) {
        var entity = _classBuilder.Resolve<T>(InstanceOptions.DiferentInstances);
        var properties = entity.GetType().GetProperties();

        if (_entityHelper.GetPrimaryKey(typeof(T)) is PropertyInfo pkProp) {
            if (_entityCache.ExistInCache<T>(dictionary[_tableHelper.GetCollumName(pkProp)])) {
                entity = _entityCache.GetInCache<T>(dictionary[_tableHelper.GetCollumName(pkProp)]);
            } else {
                ChangePropertiesValue<T>(ref entity, ref properties, dictionary);
                _entityCache.SaveInCache<T>(dictionary[_tableHelper.GetCollumName(pkProp)], entity);
            }
        } else {
            ChangePropertiesValue<T>(ref entity, ref properties, dictionary);
        }

        return entity;
    }

    private void ChangePropertiesValue<T>(ref T entity, ref PropertyInfo[] properties, Dictionary<String, object> dictionary) {
        for (int i = 0; i < properties.Length; i++) {
            PropertyInfo? property = properties[i];
            var collumnName = _tableHelper.GetCollumName(property);
            if (dictionary.TryGetValue(collumnName, out object? value)) {
                if (property.GetAccessors()[0].IsVirtual && !property.GetAccessors()[0].IsFinal && !property.PropertyType.IsGenericType) {
                    var repository = _repositoryBuilder.Create(property.PropertyType);
                    var propertyName = "";
                    if (_entityHelper.GetPrimaryKey(property.PropertyType) is PropertyInfo pkProperty) {
                        propertyName = _tableHelper.GetCollumName(pkProperty);
                    } else if (_entityHelper.GetForeignKey(property) is PropertyInfo fkProperty) {
                        propertyName = _tableHelper.GetCollumName(fkProperty);
                    }
                    var result = _reflectionOptimizations.CreateMethod(typeof(Func<,>), [typeof(string), property.PropertyType], repository, "Find", BindingFlags.Instance | BindingFlags.NonPublic).DynamicInvoke($"{propertyName}='{value}'");
                    _propertyHelper.SetPropertyValue(entity, property, result);
                } else {
                    _propertyHelper.SetPropertyValue(entity, property, value);
                }
            }
        }
    }
}
