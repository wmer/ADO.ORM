using Reflection.Optimization;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ADO.ORM.Helpers {
    public class PropertyHelper {
        private MethodHelper _methodHelper;

        private object lock1 = new object();
        private object lock2 = new object();

        public PropertyHelper(MethodHelper methodHelper) {
            _methodHelper = methodHelper;
        }

        public object GetPropertyValue(object instanceClass, PropertyInfo property) {
            lock (lock1) {
                var getMethod = property.GetGetMethod();
                return _methodHelper.CreatePropertyGetterMethod(property, instanceClass).DynamicInvoke();
            }
        }

        public void SetPropertyValue(object instanceClass, PropertyInfo property, object parameter) {
            lock (lock2) {
                if (parameter is DBNull) {
                    parameter = null;
                }
                if ((parameter as string).ToBytes(out var val)) {
                    try {
                        var converterResult = Convert.ChangeType(val, property.PropertyType);
                        parameter = converterResult;
                    } catch { }
                }

                if (parameter != null) {
                    _methodHelper.CretatePropertySetterMethod(property, instanceClass).DynamicInvoke(Convert.ChangeType(parameter, property.PropertyType));
                }
            }
        }
    }
}
