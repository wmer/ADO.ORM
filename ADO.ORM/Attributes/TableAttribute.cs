using System;
using System.Collections.Generic;
using System.Text;

namespace ADO.ORM.Attributes {
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class TableAttribute : Attribute {
        public String Name { get; set; }

        public TableAttribute(String Name) {
            this.Name = Name;
        }
    }
}
