using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ADO.ORM.Helpers.SqLite; 
public class TableHelper(EntityHelper entityHelper) : TableHelperBase(entityHelper) {
    public override string GetCollumnType(PropertyInfo property) {
        String collumnType = null;
        var propertyType = property.PropertyType.Name;
        if (property.PropertyType.IsAssignableFrom(typeof(int))) {
            collumnType = "INTEGER";
        }
        if (property.PropertyType.IsAssignableFrom(typeof(float)) || property.PropertyType.IsAssignableFrom(typeof(double))) {
            collumnType = "REAL";
        }
        if (property.PropertyType.IsAssignableFrom(typeof(decimal)) || property.PropertyType.IsAssignableFrom(typeof(bool)) || property.PropertyType.IsAssignableFrom(typeof(DateTime))) {
            collumnType = "NUMERIC";
        }
        if (property.PropertyType.IsAssignableFrom(typeof(string))) {
            collumnType = "TEXT";
        }
        if (property.PropertyType.IsArray && property.PropertyType.IsAssignableFrom(typeof(byte[]))) {
            collumnType = "BLOB";
        }

        if (property.GetAccessors()[0].IsVirtual && !property.GetAccessors()[0].IsFinal && !property.PropertyType.IsGenericType) {
            var associTable = GetTableName(property.PropertyType);

            if (_entityHelper.GetPrimaryKey(property.PropertyType) is PropertyInfo pkProperty) {
                collumnType = GetCollumnType(pkProperty);
            } else if (_entityHelper.GetForeignKey(property) is PropertyInfo fkProperty) {
                collumnType = GetCollumnType(fkProperty);
            } else {
                throw new Exception("Ao usar virtual, é nescessário definir chave estrangeira ou chave primaria na classe associada!");
            }
        }

        return collumnType;
    }
}
