using System;
using System.Collections.Generic;
using System.Text;

namespace ADO.ORM.Attributes {
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public class ColumnAttribute : Attribute {
        public String Name { get; set; }

        public ColumnAttribute(String Name) {
            this.Name = Name;
        }
    }
}
