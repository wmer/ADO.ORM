using DependencyInjectionResolver;
using Reflection.Optimization;
using ADO.ORM.Cache;
using ADO.ORM.Contracts;
using ADO.ORM.Helpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ADO.ORM.Builders {
    internal class EntityBuilder {
        private DependencyInjection _classBuilder;
        private PropertyHelper _propertyHelper;
        private ITableHelper _tableHelper;
        private MethodHelper _reflectionOptimizations;
        private RepositoryBuilder _repositoryBuilder;
        private EntityHelper _entityHelper;
        private EntityCache _entityCache;

        private object lock1 = new object();
        private object lock2 = new object();

        public EntityBuilder(DependencyInjection classBuilder, PropertyHelper propertyHelper, ITableHelper tableHelper, MethodHelper reflectionOptimizations, RepositoryBuilder repositoryBuilder, EntityHelper entityHelper, EntityCache entityCache) {
            _classBuilder = classBuilder;
            _propertyHelper = propertyHelper;
            _tableHelper = tableHelper;
            _reflectionOptimizations = reflectionOptimizations;
            _repositoryBuilder = repositoryBuilder;
            _entityHelper = entityHelper;
            _entityCache = entityCache;
        }

        public T Create<T>(ConcurrentDictionary<String, object> dictionary) {
            lock (lock1) {
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
        }

        private void ChangePropertiesValue<T>(ref T entity, ref PropertyInfo[] properties, ConcurrentDictionary<String, object> dictionary) {
            lock (lock2) {
                foreach (var property in properties) {
                    var collumnName = _tableHelper.GetCollumName(property);
                    if (dictionary.ContainsKey(collumnName)) {
                        if (property.GetAccessors()[0].IsVirtual && !property.GetAccessors()[0].IsFinal && !property.PropertyType.IsGenericType) {
                            var value = dictionary[collumnName];
                            var repository = _repositoryBuilder.Create(property.PropertyType);
                            var propertyName = "";
                            if (_entityHelper.GetPrimaryKey(property.PropertyType) is PropertyInfo pkProperty) {
                                propertyName = _tableHelper.GetCollumName(pkProperty);
                            } else if (_entityHelper.GetForeignKey(property) is PropertyInfo fkProperty) {
                                propertyName = _tableHelper.GetCollumName(fkProperty);
                            }
                            var result = _reflectionOptimizations.CreateMethod(typeof(Func<,>), new Type[] { typeof(String), property.PropertyType }, repository, "Find", BindingFlags.Instance | BindingFlags.NonPublic).DynamicInvoke($"{propertyName}='{value}'");
                            _propertyHelper.SetPropertyValue(entity, property, result);
                        } else {
                            _propertyHelper.SetPropertyValue(entity, property, dictionary[collumnName]);
                        }
                    }
                }
            }
        }
    }
}
