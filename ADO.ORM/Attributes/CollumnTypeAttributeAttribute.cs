using System;
using System.Collections.Generic;
using System.Text;

namespace ADO.ORM.Attributes {
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public class CollumnTypeAttributeAttribute : Attribute {
        public String Type { get; set; }
        public String Size { get; set; }

        public CollumnTypeAttributeAttribute(String type) {
            this.Type = type;
        }

        public CollumnTypeAttributeAttribute(String type, String size) {
            this.Type = type;
            this.Size = size;
        }
    }
}
