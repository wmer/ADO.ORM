using ADO.ORM.Attributes;
using ADO.ORM.Contracts;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ADO.ORM.Helpers {
    public abstract class TableHelperBase : ITableHelper {
        protected EntityHelper _entityHelper;

        protected readonly object lock1 = new object();
        protected readonly object lock2 = new object();
        protected readonly object lock3 = new object();

        public TableHelperBase(EntityHelper entityHelper) {
            _entityHelper = entityHelper;
        }

        public string GetTableName(Type type) {
            lock (lock1) {
                String tableName = "";
                if (type.GetCustomAttribute(typeof(TableAttribute)) is TableAttribute tableAttribute) {
                    tableName = tableAttribute.Name;
                } else {
                    tableName = type.Name;
                }
                return tableName;
            }
        }

        public string GetCollumName(PropertyInfo property) {
            lock (lock2) {
                String collumnName = null;
                if (property.GetCustomAttribute(typeof(ColumnAttribute)) is ColumnAttribute columnAttribute) {
                    collumnName = columnAttribute.Name;
                } else {
                    collumnName = property.Name;
                }

                return collumnName;
            }
        }

        public virtual string GetCollumnType(PropertyInfo property) {
            throw new NotImplementedException();
        }
    }
}
