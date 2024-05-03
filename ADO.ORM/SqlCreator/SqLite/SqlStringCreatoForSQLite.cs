using ADO.ORM.Attributes;
using ADO.ORM.Enumerators;
using ADO.ORM.Helpers;
using ADO.ORM.Helpers.SqLite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ADO.ORM.SqlCreator.SqLite; 
public class SqlStringCreatoForSQLite : SqlAnsiCreator {
    public SqlStringCreatoForSQLite(TableHelper tableHelper, EntityHelper entityHelper, PropertyHelper propertyHelper) : base(tableHelper, entityHelper, propertyHelper) {
    }

    public string Insert<T>(T entity, Conflict conflict) {
        String tableName = _tableHelper.GetTableName(entity.GetType());
        var sql = $"INSERT OR {conflict.ToString()} INTO '{tableName}' {GetToInsert<T>(entity, true)}";
        return sql;
    }

    public string Insert<T>(IEnumerable<T> entities, Conflict conflict) {
        var sql = "";
        foreach (var entity in entities) {
            sql += $"{Insert<T>(entity, conflict)};{Environment.NewLine}";
        }
        return sql;
    }

    public override string CreateTables(List<Type> models) {
        var tableSql = "";
        foreach (var table in models) {
            var tableName = _tableHelper.GetTableName(table);
            var inicio = $"CREATE TABLE IF NOT EXISTS [{tableName}] (";
            var collumns = "";
            var primaryKey = "";

            var required = "NULL";
            String collumnType = null;
            String collumnName = null;
            foreach (var property in table.GetProperties(BindingFlags.Public | BindingFlags.Instance)) {
                required = "NULL";
                var notEnter = false;
                collumnType = _tableHelper.GetCollumnType(property);
                collumnName = _tableHelper.GetCollumName(property);
                var attributes = property.GetCustomAttributes(true);

                if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType) && !typeof(String).IsAssignableFrom(property.PropertyType) && !(property.PropertyType.IsArray && property.PropertyType.IsAssignableFrom(typeof(byte[])))) {
                    notEnter = true;
                }

                if (property.GetAccessors()[0].IsVirtual && !property.GetAccessors()[0].IsFinal && !property.PropertyType.IsGenericType) {
                    collumnName = _tableHelper.GetCollumName(property);
                    required = "NOT NULL";
                }

                if (attributes.Length > 0) {
                    foreach (var attr in attributes) {
                        if (attr is RequiredAttribute requiredAttr) {
                            required = requiredAttr.IsRequired ? "NOT NULL" : "NULL";
                        }
                        if (attr is IgnoreAttribute) {
                            notEnter = true;
                        }
                        if (attr is PrimaryKeyAttribute) {
                            required = "NOT NULL";
                            primaryKey = $"CONSTRAINT[PK_{tableName}] PRIMARY KEY([{collumnName}])";
                        }
                    }
                }

                if (!notEnter && !String.IsNullOrEmpty(collumnType)) {
                    collumns += $"[{collumnName}] {collumnType} {required}, ";
                }
            }

            if (String.IsNullOrEmpty(primaryKey)) {
                collumns = collumns.Remove((collumns.Length - 2), 2).TrimEnd();
            }
            tableSql += $"{inicio}{collumns}{primaryKey});{Environment.NewLine}";
        }

        return tableSql;
    }
}
