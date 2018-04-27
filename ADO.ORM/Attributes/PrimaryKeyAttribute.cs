using System;
using System.Collections.Generic;
using System.Text;

namespace ADO.ORM.Attributes {
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class PrimaryKeyAttribute : Attribute {
        public bool IsAutoIncrement { get; set; }
        public PrimaryKeyAttribute() { }
        public PrimaryKeyAttribute(bool AutoIncrement) {
            IsAutoIncrement = AutoIncrement;
        }
    }
}
