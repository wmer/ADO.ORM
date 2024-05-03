using HelpersLibs.Reflection.Helpers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ADO.ORM.Helpers; 
public class PropertyHelper(MethodHelper methodHelper) {
    private MethodHelper _methodHelper = methodHelper;

    public object GetPropertyValue(object instanceClass, PropertyInfo property) {
        var getMethod = property.GetGetMethod();
        return _methodHelper.CreatePropertyGetterMethod(property, instanceClass).DynamicInvoke();
    }

    public void SetPropertyValue(object instanceClass, PropertyInfo property, object parameter) {
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
