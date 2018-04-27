using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ADO.ORM.Contracts {
    public interface ITableHelper {
        string GetTableName(Type type);
        string GetCollumName(PropertyInfo property);
        string GetCollumnType(PropertyInfo property);
    }
}
