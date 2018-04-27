using ADO.ORM.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ADO.ORM.Helpers {
    public class EntityHelper {
        private object lock1 = new object();
        private object lock2 = new object();

        public PropertyInfo GetPrimaryKey(Type model) {
            lock (lock1) {
                var associTableProps = model.GetProperties();
                return associTableProps.Where(prop => Attribute.IsDefined(prop, typeof(PrimaryKeyAttribute))).FirstOrDefault();
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
