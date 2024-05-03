using System;
using System.Collections.Generic;
using System.Text;

namespace ADO.ORM.Attributes {
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public class CollumnTypeAttributeAttribute : Attribute {
        public String Type { get; set; }
        public String Size { get; set; }
        public string Default { get; set; }

        public CollumnTypeAttributeAttribute(String type) : this(type, null) {
        }

        public CollumnTypeAttributeAttribute(String type, String size) : this(type, size, null) {
        }

        public CollumnTypeAttributeAttribute(String type, String size, String defaultValue) {
            this.Type = type;
            this.Size = size;
            this.Default = defaultValue;
        }
    }
}
