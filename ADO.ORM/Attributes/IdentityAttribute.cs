using System;
using System.Collections.Generic;
using System.Text;

namespace ADO.ORM.Attributes {
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public class IdentityAttribute : Attribute {
        public String Step { get; set; }
        public IdentityAttribute() {
            this.Step = "(1, 1)";
        }

        public IdentityAttribute(int value1, int value2) {
            this.Step = $"({value1}, {value2})";
        }
    }
}
