using DependencyInjectionResolver;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADO.ORM.Builders; 
public class RepositoryBuilder(DependencyInjection dependencyInjection) {
    private DependencyInjection _dependencyInjection = dependencyInjection;


    public object Create(Type genericType) {
        var rpositoryType = typeof(Repository<>);
        var repository = rpositoryType.MakeGenericType(genericType);
        return _dependencyInjection.Resolve(repository, InstanceOptions.DiferentInstances);
    }
}
