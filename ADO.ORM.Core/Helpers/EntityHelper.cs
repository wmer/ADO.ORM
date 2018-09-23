using ADO.ORM.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ADO.ORM.Core.Helpers {
    public class EntityHelper {
        private readonly object lock1 = new object();
        private readonly object lock2 = new object();

        public PropertyInfo GetPrimaryKey(Type model) {
            lock (lock1) {
                var associTableProps = model.GetProperties();
                return associTableProps.Where(prop => System.Attribute.IsDefined(prop, typeof(PrimaryKeyAttribute))).FirstOrDefault();
            }
        }

        public PropertyInfo GetForeignKey(PropertyInfo property) {
            lock (lock2) {
                PropertyInfo prop = null;
                var attrs = property.GetCustomAttributes(true);
                if (attrs.Where(x => x is ForeignKeyAttribute).FirstOrDefault() is ForeignKeyAttribute foreignKeyAttr) {
                    prop = property.PropertyType.GetProperties().Where(x => x.Name == foreignKeyAttr.Name).FirstOrDefault();
                }
                return prop;
            }
        }
    }
}
