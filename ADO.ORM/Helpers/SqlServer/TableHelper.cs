using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ADO.ORM.Helpers.SqlServer {
    public class TableHelper : TableHelperBase {
        public TableHelper(EntityHelper entityHelper) : base(entityHelper) {
        }

        public override string GetCollumnType(PropertyInfo property) {
            lock (lock3) {
                String collumnType = null;
                var propertyType = property.PropertyType.Name;
                collumnType = propertyType.ToUpper();
                if (property.PropertyType.IsAssignableFrom(typeof(int))) {
                    collumnType = "INT";
                }
                if (property.PropertyType.IsAssignableFrom(typeof(double))) {
                    collumnType = "FLOAT";
                }
                if (property.PropertyType.IsAssignableFrom(typeof(bool))) {
                    collumnType = "BIT";
                }
                if (property.PropertyType.IsAssignableFrom(typeof(string))) {
                    collumnType = "VARCHAR";
                }
                if (property.PropertyType.IsArray && property.PropertyType.IsAssignableFrom(typeof(byte[]))) {
                    collumnType = "VARBINARY";
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
    }
}
