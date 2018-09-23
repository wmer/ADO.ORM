using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using ADO.ORM.Attributes;
using ADO.ORM.Contracts;
using ADO.ORM.Helpers;

namespace ADO.ORM.SqlCreator.SqlServer {
    public class SqlStringCreatorForSqlServer : SqlAnsiCreator {
        public SqlStringCreatorForSqlServer(ITableHelper tableHelper, EntityHelper entityHelper, PropertyHelper propertyHelper) : base(tableHelper, entityHelper, propertyHelper) {
        }

        public override string CreateTables(List<Type> models) {
            lock (lock18) {
                var tableSql = "";
                foreach (var table in models) {
                    var tableName = _tableHelper.GetTableName(table);
                    var inicio = $"IF NOT EXISTS (" +
                                $"SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'dbo.{tableName}') " +
                                $"BEGIN CREATE TABLE [dbo].[{tableName}] (";

                    var fim = $"); END";
                    var collumns = "";
                    var primaryKey = "";
                    var required = "NULL";
                    String collumnType = null;
                    String collumnName = null;
                    String collumSize = null;
                    String identity = null;
                    String defaultSize = null;

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
                                if (attr is CollumnTypeAttributeAttribute collumnTypeAttr) {
                                    if (!string.IsNullOrWhiteSpace(collumnTypeAttr.Size)) {
                                        collumSize = $"({collumnTypeAttr.Size})";
                                    }
                                    if (!string.IsNullOrWhiteSpace(collumnTypeAttr.Default)) {
                                        defaultSize = $"DEFAULT ('{collumnTypeAttr.Default}')";
                                    }
                                }
                                if (attr is IdentityAttribute identityAttr) {
                                    identity = $"IDENTITY {identityAttr.Step}";
                                }
                                if (attr is RequiredAttribute requiredAttr) {
                                    required = requiredAttr.IsRequired ? "NOT NULL" : "NULL";
                                }
                                if (attr is IgnoreAttribute) {
                                    notEnter = true;
                                }
                                if (attr is PrimaryKeyAttribute) {
                                    required = "NOT NULL";
                                    primaryKey = tableName;
                                    fim = $"PRIMARY KEY CLUSTERED ([{collumnName}])); END";
                                }
                            }
                        }

                        if (!notEnter && !String.IsNullOrEmpty(collumnType)) {
                            collumns += $"[{collumnName}] {collumnType} {collumSize} {identity} {defaultSize} {required}, ";
                        }
                    }

                    //if (String.IsNullOrEmpty(primaryKey)) {
                    //    collumns = collumns.Remove((collumns.Length - 2), 2).TrimEnd();
                    //}
                    tableSql += $"{inicio}{collumns}{fim}{Environment.NewLine}";
                    RegexOptions options = RegexOptions.None;
                    Regex regex = new Regex("[ ]{2,}", options);
                    tableSql = regex.Replace(tableSql, " ");
                    var ddss = "";
                }        

                return tableSql;
            }
        }
    }
}
