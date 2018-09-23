using ADO.ORM.Core.Attributes;
using ADO.ORM.Core.Builders;
using ADO.ORM.Core.Enumerators;
using ADO.ORM.Core.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace ADO.ORM.SQLite.Helpers {
    public class SqlStringCreatorHelper : ISqlCreator {
        protected ITableHelper _tableHelper;
        protected EntityHelper _entityHelper;
        protected PropertyHelper _propertyHelper;

        protected readonly object lock1 = new object();
        protected readonly object lock2 = new object();
        protected readonly object lock3 = new object();
        protected readonly object lock4 = new object();
        protected readonly object lock5 = new object();
        protected readonly object lock6 = new object();
        protected readonly object lock7 = new object();
        protected readonly object lock8 = new object();
        protected readonly object lock9 = new object();
        protected readonly object lock10 = new object();
        protected readonly object lock11 = new object();
        protected readonly object lock12 = new object();
        protected readonly object lock13 = new object();
        protected readonly object lock14 = new object();
        protected readonly object lock15 = new object();
        protected readonly object lock16 = new object();
        protected readonly object lock17 = new object();
        protected readonly object lock18 = new object();

        public SqlStringCreatorHelper(ITableHelper tableHelper, EntityHelper entityHelper, PropertyHelper propertyHelper) {
            _tableHelper = tableHelper;
            _entityHelper = entityHelper;
            _propertyHelper = propertyHelper;
        }

        public virtual string Select<T>() {
            lock (lock1) {
                String tableName = _tableHelper.GetTableName(typeof(T));
                return $"SELECT * FROM '{tableName}'";
            }
        }

        public virtual string Select<T>(string whereClausure) {
            lock (lock2) {
                return $"{Select<T>()} WHERE {whereClausure}";
            }
        }

        public virtual string Select<T>(Expression<Func<T, object>> parameters, bool distincted = false) {
            lock (lock3) {
                String tableName = _tableHelper.GetTableName(typeof(T));
                var expression = parameters.Body;
                PropertyInfo[] properties;
                if (IsAnonymousType(expression.Type)) {
                    properties = expression.Type.GetProperties();
                    AvalieFields(properties);
                } else {
                    var experessionStr = expression.ToString();
                    var expressionSplited = experessionStr.Split('.');
                    var property = typeof(T).GetProperty(expressionSplited[1].Replace(")", ""));
                    properties = new PropertyInfo[] { property };
                    AvalieFields(properties);
                }
                return $"SELECT{(distincted ? " DISTINCT" : "")} {FieldsForFind<T>(properties)} FROM '{tableName}'";
            }
        }

        public virtual string Insert<T>(T entity) {
            lock (lock4) {
                String tableName = _tableHelper.GetTableName(entity.GetType());
                var sql = $"INSERT INTO '{tableName}' {GetToInsert<T>(entity)}";
                return sql;
            }
        }

        public virtual string Insert<T>(IEnumerable<T> entities) {
            lock (lock5) {
                var sql = "";
                foreach (var entity in entities) {
                    sql += $"{Insert<T>(entity)};{Environment.NewLine}";
                }
                return sql;
            }
        }

        public string Insert<T>(T entity, Conflict conflict) {
            lock (lock6) {
                String tableName = _tableHelper.GetTableName(entity.GetType());
                var sql = $"INSERT OR {conflict.ToString()} INTO '{tableName}' {GetToInsert<T>(entity, true)}";
                return sql;
            }
        }

        public virtual string Update<T>(T entity) {
            lock (lock8) {
                String tableName = _tableHelper.GetTableName(entity.GetType());
                var clausule = "";
                if (_entityHelper.GetPrimaryKey(typeof(T)) is PropertyInfo pkProperty) {
                    object primaryKey = null;
                    if (_entityHelper.GetPrimaryKey(pkProperty.PropertyType) is PropertyInfo pkProp) {
                        primaryKey = _propertyHelper.GetPropertyValue(_propertyHelper.GetPropertyValue(entity, pkProperty), pkProp);
                    } else {
                        primaryKey = _propertyHelper.GetPropertyValue(entity, pkProperty);
                    }
                    clausule = $"{_tableHelper.GetCollumName(pkProperty)}='{primaryKey}'";
                } else {
                    throw new Exception("O modelo não possui chave primária");
                }
                var sql = $"UPDATE '{tableName}' SET {FieldsForUpdate<T>(entity, entity.GetType().GetProperties())} WHERE {clausule}";
                return sql;
            }
        }

        public virtual string Update<T>(IEnumerable<T> entities) {
            lock (lock9) {
                var sql = "";
                foreach (var entity in entities) {
                    sql += $"{Update<T>(entity)};{Environment.NewLine}";
                }
                return sql;
            }
        }

        public virtual string Update<T>(Expression<Func<T, object>> parameters, params object[] values) {
            lock (lock10) {
                String tableName = _tableHelper.GetTableName(typeof(T));
                var expression = parameters.Body;
                PropertyInfo[] properties;
                if (IsAnonymousType(expression.Type)) {
                    properties = expression.Type.GetProperties();
                    AvalieFields(properties);
                } else {
                    properties = new PropertyInfo[] { typeof(T).GetProperty(expression.ToString().Split('.')[1].Replace(")", "")) };
                    AvalieFields(properties);
                }

                var sql = $"UPDATE '{tableName}' SET {FieldsForUpdate<T>(properties, values)}";
                return sql;
            }
        }

        public virtual string Delete<T>(T entity) {
            lock (lock11) {
                String tableName = _tableHelper.GetTableName(typeof(T));
                var clausule = "";
                if (_entityHelper.GetPrimaryKey(typeof(T)) is PropertyInfo pkProperty) {
                    object primaryKey = null;
                    if (_entityHelper.GetPrimaryKey(pkProperty.PropertyType) is PropertyInfo pkProp) {
                        primaryKey = _propertyHelper.GetPropertyValue(_propertyHelper.GetPropertyValue(entity, pkProperty), pkProp);
                    } else {
                        primaryKey = _propertyHelper.GetPropertyValue(entity, pkProperty);
                    }
                    clausule = $"{_tableHelper.GetCollumName(pkProperty)}='{primaryKey}'";
                } else {
                    throw new Exception("O modelo não possui chave primária");
                }
                return $"DELETE FROM '{tableName}' Where {clausule}";
            }
        }

        public virtual string Delete<T>() {
            lock (lock12) {
                String tableName = _tableHelper.GetTableName(typeof(T));
                return $"DELETE FROM '{tableName}'";
            }
        }

        public virtual string OrderBy<T>(Expression<Func<T, object>> parameters, params SortedBy[] sortedBy) {
            lock (lock13) {
                var fields = "";
                var countFields = 0;
                var countSorted = 0;
                PropertyInfo[] properties;
                var expression = parameters.Body;
                if (IsAnonymousType(expression.Type)) {
                    properties = expression.Type.GetProperties();
                    AvalieFields(properties);
                } else {
                    var propName = expression.ToString().Split('.')[1].Replace(")", "");
                    properties = new PropertyInfo[] { typeof(T).GetProperty(propName) };
                    AvalieFields(properties);
                }
                if (!sortedBy.Any()) {
                    fields = FieldsForFind<T>(properties);
                } else {
                    foreach (var field in FieldsForFind<T>(properties).Split(',')) {
                        foreach (var sorted in sortedBy) {
                            if (countFields == countSorted) {
                                fields += $"'{field}' {(((int)sorted == 0) ? "ASC" : "DESC")}, ";
                            }
                            countSorted++;
                        }
                        if (countSorted == countFields) {
                            fields += $"'{field}' ASC, ";
                        }
                        countSorted = 0;
                        countFields++;
                    }
                    fields = fields.Substring(0, (fields.Length - 2));
                }

                var sqlString = $" ORDER BY {fields.Replace("  ", " ")}";

                return sqlString;
            }
        }

        public virtual string Where<T>(Expression<Func<T, bool>> predicate, string like = "") {
            lock (lock14) {
                var sq = new WhereBuilder(_tableHelper, _propertyHelper).ToSql<T>(predicate);
                var fields = sq.Sql;
                var i = 0;
                if (sq.Parameters.Count > 0) {
                    foreach (var s in fields) {
                        if (int.TryParse(s.ToString(), out int n)) {
                            var part = sq.Sql.Substring(0, i);
                            var prop = part.Substring(part.LastIndexOf('['));
                            prop = prop.Replace(" = @", "");
                            prop = prop.Replace(" =", "");
                            prop = prop.Replace("[", "");
                            prop = prop.Replace("]", "");
                            var val = GetObjectValue<T>(prop, sq.Parameters[s.ToString()]);
                            fields = fields.Replace($"@{s}", $"'{val}'");
                        }
                        i++;
                    }
                }

                var sqlString = $" WHERE {fields}{((like != "") ? " LIKE '" + like + "'" : "")}";
                return sqlString;
            }
        }

        public string CreateTables(List<Type> models) {
            lock (lock18) {
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

        #region filds helper

        protected virtual String FieldsForFind<T>(PropertyInfo[] properties) {
            lock (lock2) {
                var fields = "";
                foreach (var prop in properties) {
                    var property = typeof(T).GetProperty(prop.Name);
                    var collumnName = _tableHelper.GetCollumName(property);
                    var notEnter = false;
                    var attributes = property.GetCustomAttributes(true);

                    if (property.GetAccessors()[0].IsVirtual && !property.GetAccessors()[0].IsFinal && !property.PropertyType.IsGenericType) {
                        if (_entityHelper.GetPrimaryKey(property.PropertyType) is PropertyInfo pkProperty) {
                            collumnName = _tableHelper.GetCollumName(pkProperty);
                        } else if (_entityHelper.GetForeignKey(property) is PropertyInfo fkProperty) {
                            collumnName = _tableHelper.GetCollumName(fkProperty);
                        }
                    }

                    if (attributes.Length > 0) {
                        foreach (var attr in attributes) {
                            if (attr is IgnoreAttribute) {
                                notEnter = true;
                            }
                        }
                    }

                    if (!notEnter) {
                        fields += $"'{collumnName}', ";
                    }
                }

                return fields.Substring(0, (fields.Length - 2));
            }
        }

        protected virtual String GetToInsert<T>(T entity, bool conflict = false, bool isMysql = false) {
            lock (lock1) {
                var properties = entity.GetType().GetProperties();
                var fields = "";
                var values = "";

                foreach (var property in properties) {
                    var notEnter = false;
                    var attributes = property.GetCustomAttributes(true);
                    var collumnName = _tableHelper.GetCollumName(property);
                    var collumnValue = _propertyHelper.GetPropertyValue(entity, property);
                    var collumnType = _tableHelper.GetCollumnType(property);

                    if (collumnValue != null) {
                        if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType) && !typeof(String).IsAssignableFrom(property.PropertyType) && !(property.PropertyType.IsArray && property.PropertyType.IsAssignableFrom(typeof(byte[])))) {
                            notEnter = true;
                        }

                        if (property.GetAccessors()[0].IsVirtual && !property.GetAccessors()[0].IsFinal && !property.PropertyType.IsGenericType) {
                            if (_entityHelper.GetPrimaryKey(property.PropertyType) is PropertyInfo pkProperty) {
                                collumnValue = _propertyHelper.GetPropertyValue(collumnValue, pkProperty);
                            } else if (_entityHelper.GetForeignKey(property) is PropertyInfo fkProperty) {
                                collumnValue = _propertyHelper.GetPropertyValue(collumnValue, fkProperty);
                            }
                        }

                        if (attributes.Length > 0) {
                            foreach (var attr in attributes) {
                                if (attr is PrimaryKeyAttribute && typeof(int).IsAssignableFrom(property.PropertyType) && !conflict) {
                                    notEnter = true;
                                    break;
                                }
                                if (attr is IgnoreAttribute) {
                                    notEnter = true;
                                }
                            }
                        }

                        if (!notEnter) {
                            fields += $"'{collumnName}', ";
                            if (collumnType == "BLOB") {
                                values += $"'{Convert.ToBase64String(collumnValue as byte[])}', ";
                            } else if (collumnType == "BOOLEAN" && isMysql) {
                                values += $"'{Convert.ToInt32(collumnValue)}', ";
                            } else if (collumnType == "DATETIME(0)" && isMysql) {
                                values += $"'{((DateTime)collumnValue).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss")}', ";
                            } else {
                                values += $"'{collumnValue}', ";
                            }
                        }
                    }
                }

                fields = fields.Substring(0, (fields.Length - 2));
                values = values.Substring(0, (values.Length - 2));
                return $"({fields}) VALUES({values})";
            }
        }

        protected virtual String FieldsForUpdate<T>(T entity, PropertyInfo[] properties, bool isMysql = false) {
            lock (lock3) {
                var fields = "";
                var pkProp = _entityHelper.GetPrimaryKey(typeof(T));

                foreach (var prop in properties) {
                    var property = typeof(T).GetProperty(prop.Name);
                    var collumnName = _tableHelper.GetCollumName(property);
                    var collumnValue = _propertyHelper.GetPropertyValue(entity, property);
                    var collumnType = _tableHelper.GetCollumnType(property);
                    var attributes = property.GetCustomAttributes(true);
                    var notEnter = false;

                    if (pkProp != property && collumnValue != null) {
                        if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType) && !typeof(String).IsAssignableFrom(property.PropertyType) && !(property.PropertyType.IsArray && property.PropertyType.IsAssignableFrom(typeof(byte[])))) {
                            notEnter = true;
                        }

                        if (attributes.Length > 0) {
                            foreach (var attr in attributes) {
                                if (attr is IgnoreAttribute) {
                                    notEnter = true;
                                }
                            }
                        }

                        if (property.GetAccessors()[0].IsVirtual && !property.GetAccessors()[0].IsFinal && !property.PropertyType.IsGenericType) {
                            if (_entityHelper.GetPrimaryKey(property.PropertyType) is PropertyInfo pkProperty) {
                                collumnName = _tableHelper.GetCollumName(pkProperty);
                            } else if (_entityHelper.GetForeignKey(property) is PropertyInfo fkProperty) {
                                collumnName = _tableHelper.GetCollumName(fkProperty);
                            }
                        }

                        if (!notEnter) {
                            if (collumnType == "BLOB") {
                                fields += $"'{collumnName}'='{Convert.ToBase64String(collumnValue as byte[])}', ";
                            } else if (collumnType == "BOOLEAN" && isMysql) {
                                fields += $"'{Convert.ToInt32(collumnValue)}', ";
                            } else if (collumnType == "DATETIME(0)" && isMysql) {
                                fields += $"'{((DateTime)collumnValue).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss")}', ";
                            } else {
                                fields += $"'{collumnName}'='{collumnValue}', ";
                            }
                        }
                    }
                }

                return fields.Substring(0, (fields.Length - 2));
            }
        }

        protected virtual String FieldsForUpdate<T>(PropertyInfo[] properties, object[] values, bool isMysql = false) {
            lock (lock4) {
                var fields = "";
                var i = 0;
                var pkProp = _entityHelper.GetPrimaryKey(typeof(T));
                foreach (var prop in properties) {
                    var property = typeof(T).GetProperty(prop.Name);
                    var collumnName = _tableHelper.GetCollumName(property);
                    var collumnType = _tableHelper.GetCollumnType(property);
                    var collumnValue = values[i];
                    var notEnter = false;

                    if (pkProp != property && collumnValue != null) {
                        if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType) && !typeof(String).IsAssignableFrom(property.PropertyType) && !(property.PropertyType.IsArray && property.PropertyType.IsAssignableFrom(typeof(byte[])))) {
                            notEnter = true;
                        }

                        if (property.GetAccessors()[0].IsVirtual && !property.GetAccessors()[0].IsFinal && !property.PropertyType.IsGenericType) {
                            if (_entityHelper.GetPrimaryKey(property.PropertyType) is PropertyInfo pkProperty) {
                                collumnName = _tableHelper.GetCollumName(pkProperty);
                            } else if (_entityHelper.GetForeignKey(property) is PropertyInfo fkProperty) {
                                collumnName = _tableHelper.GetCollumName(fkProperty);
                            }
                        }

                        if (!notEnter) {
                            if (collumnType == "BLOB") {
                                fields += $"'{collumnName}'='{Convert.ToBase64String(collumnValue as byte[])}', ";
                            } else if (collumnType == "BOOLEAN" && isMysql) {
                                fields += $"'{Convert.ToInt32(collumnValue)}', ";
                            } else if (collumnType == "DATETIME(0)" && isMysql) {
                                fields += $"'{((DateTime)collumnValue).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss")}', ";
                            } else {
                                fields += $"'{collumnName}'='{collumnValue}', ";
                            }
                        }
                    }
                    i++;
                }

                return fields.Substring(0, (fields.Length - 2));
            }
        }

        #endregion
        #region lambda helper

        protected object GetObjectValue<T>(String propName, object optValue) {
            lock (lock15) {
                var properties = typeof(T).GetProperties();
                var collumnValue = optValue;
                foreach (var property in properties) {
                    var collumnName = _tableHelper.GetCollumName(property);
                    if (propName == collumnName) {
                        if (property.GetAccessors()[0].IsVirtual && !property.GetAccessors()[0].IsFinal && !property.PropertyType.IsGenericType) {
                            if (_entityHelper.GetPrimaryKey(property.PropertyType) is PropertyInfo pkProperty) {
                                collumnValue = _propertyHelper.GetPropertyValue(collumnValue, pkProperty);
                            } else if (_entityHelper.GetForeignKey(property) is PropertyInfo fkProperty) {
                                collumnValue = _propertyHelper.GetPropertyValue(collumnValue, fkProperty);
                            }
                        }
                    }
                }
                return collumnValue;
            }
        }

        protected bool IsAnonymousType(Type type) {
            lock (lock16) {
                return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                    && type.IsGenericType && type.Name.Contains("AnonymousType")
                    && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
                    && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
            }
        }

        protected void AvalieFields(PropertyInfo[] properties) {
            lock (lock17) {
                foreach (var property in properties) {
                    if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType) && !typeof(String).IsAssignableFrom(property.PropertyType) && !(property.PropertyType.IsArray && property.PropertyType.IsAssignableFrom(typeof(byte[])))) {
                        throw new ArgumentException("Esta propriedade não é permitida", property.Name);
                    }
                }
            }
        }

        #endregion
    }
}
