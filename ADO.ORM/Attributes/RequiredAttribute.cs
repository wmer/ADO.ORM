using System;
using System.Collections.Generic;
using System.Text;

namespace ADO.ORM.Attributes {
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public class RequiredAttribute : Attribute {
        public bool IsRequired { get; set; }

        public RequiredAttribute() {
            this.IsRequired = true;
        }

        public RequiredAttribute(bool IsRequired) {
            this.IsRequired = IsRequired;
        }
    }
}
