using ADO.ORM.Enumerators;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ADO.ORM.Contracts {
    public interface ISqlCreator {
        string CreateTables(List<Type> models);
        string Select<T>();
        string Select<T>(string whereClausure);
        string Select<T>(Expression<Func<T, object>> parameters, bool distincted = false);
        string Insert<T>(T entity);
        string Insert<T>(IEnumerable<T> entities);
        string Update<T>(T entity);
        string Update<T>(IEnumerable<T> entities);
        string Update<T>(Expression<Func<T, object>> parameters, params object[] values);
        string Delete<T>(T entity);
        string Delete<T>();
        string OrderBy<T>(Expression<Func<T, object>> parameters, params SortedBy[] sortedBy);
        string Where<T>(Expression<Func<T, bool>> predicate, string like = "");
    }
}
