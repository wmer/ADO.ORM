using System;
using System.Collections.Generic;
using System.Text;

namespace ADO.ORM.Core.Attributes {
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public class IgnoreAttribute : Attribute {
        public IgnoreAttribute() { }
    }
}
