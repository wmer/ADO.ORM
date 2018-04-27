using DependencyInjectionResolver;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADO.ORM.Builders {
    internal class RepositoryBuilder {
        private DependencyInjection _dependencyInjection;

        private object lock1 = new object();

        public RepositoryBuilder(DependencyInjection dependencyInjection) {
            _dependencyInjection = dependencyInjection;
        }

        public object Create(Type genericType) {
            lock (lock1) {
                var rpositoryType = typeof(Repository<>);
                var repository = rpositoryType.MakeGenericType(genericType);
                return _dependencyInjection.Resolve(repository, InstanceOptions.DiferentInstances);
            }
        }
    }
}
