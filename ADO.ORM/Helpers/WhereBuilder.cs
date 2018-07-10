using ADO.ORM.Contracts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace ADO.ORM.Helpers {
    internal class WhereBuilder {
        private ITableHelper _tableHelper;
        private PropertyHelper _propertyHelper;

        private readonly object lock1 = new object();
        private readonly object lock2 = new object();
        private readonly object lock3 = new object();
        private readonly object lock4 = new object();

        public WhereBuilder(ITableHelper tableHelper, PropertyHelper propertyHelper) {
            _tableHelper = tableHelper;
            _propertyHelper = propertyHelper;
        }

        public WherePart ToSql<T>(Expression<Func<T, bool>> expression) {
            lock (lock1) {
                var i = 1;
                return Recurse(ref i, expression.Body, isUnary: true);
            }
        }

        private WherePart Recurse(ref int i, Expression expression, bool isUnary = false, bool getValue = false, string prefix = null, string postfix = null) {
            lock (lock2) {
                if (expression is UnaryExpression unary) {
                    return WherePart.Concat(NodeTypeToString(unary.NodeType), Recurse(ref i, unary.Operand, true));
                }
                if (expression is BinaryExpression body) {
                    return WherePart.Concat(Recurse(ref i, body.Left), NodeTypeToString(body.NodeType), Recurse(ref i, body.Right, false, true));
                }
                if (expression is ConstantExpression constant) {
                    var value = constant.Value;
                    if (value is int) {
                        return WherePart.IsSql(value.ToString());
                    }
                    if (value is string) {
                        value = prefix + (string)value + postfix;
                    }
                    if (value is bool && isUnary) {
                        return WherePart.Concat(WherePart.IsParameter(i++, value), "=", WherePart.IsSql("1"));
                    }
                    return WherePart.IsParameter(i++, value);
                }
                if (expression is MemberExpression member) {
                    if (member.Member is PropertyInfo property) {
                        var colName = _tableHelper.GetCollumName(property);
                        if (isUnary && member.Type == typeof(bool)) {
                            return WherePart.Concat(Recurse(ref i, expression), "=", WherePart.IsParameter(i++, true));
                        }
                        if (getValue) {
                            var valMember = (MemberExpression)member.Expression;
                            //get value from ConstantExpression
                            var objectMember = Expression.Convert(valMember, typeof(object));
                            var getterLambda = Expression.Lambda<Func<object>>(objectMember);
                            var getter = getterLambda.Compile();
                            var val = getter();

                            var value = _propertyHelper.GetPropertyValue(val, property);
                            return WherePart.IsParameter(i++, value);
                        } else {
                            return WherePart.IsSql("[" + colName + "]");
                        }
                    }
                    if (member.Member is FieldInfo) {
                        var value = GetValue(member);
                        if (value is string) {
                            value = prefix + (string)value + postfix;
                        }
                        return WherePart.IsParameter(i++, value);
                    }
                    throw new Exception($"Expression does not refer to a property or field: {expression}");
                }
                if (expression is MethodCallExpression methodCall) {
                    // LIKE queries:
                    if (methodCall.Method == typeof(string).GetMethod("Contains", new[] { typeof(string) })) {
                        return WherePart.Concat(Recurse(ref i, methodCall.Object), "LIKE", Recurse(ref i, methodCall.Arguments[0], prefix: "%", postfix: "%"));
                    }
                    if (methodCall.Method == typeof(string).GetMethod("StartsWith", new[] { typeof(string) })) {
                        return WherePart.Concat(Recurse(ref i, methodCall.Object), "LIKE", Recurse(ref i, methodCall.Arguments[0], postfix: "%"));
                    }
                    if (methodCall.Method == typeof(string).GetMethod("EndsWith", new[] { typeof(string) })) {
                        return WherePart.Concat(Recurse(ref i, methodCall.Object), "LIKE", Recurse(ref i, methodCall.Arguments[0], prefix: "%"));
                    }
                    // IN queries:
                    if (methodCall.Method.Name == "Contains") {
                        Expression collection;
                        Expression property;
                        if (methodCall.Method.IsDefined(typeof(ExtensionAttribute)) && methodCall.Arguments.Count == 2) {
                            collection = methodCall.Arguments[0];
                            property = methodCall.Arguments[1];
                        } else if (!methodCall.Method.IsDefined(typeof(ExtensionAttribute)) && methodCall.Arguments.Count == 1) {
                            collection = methodCall.Object;
                            property = methodCall.Arguments[0];
                        } else {
                            throw new Exception("Unsupported method call: " + methodCall.Method.Name);
                        }
                        var values = (IEnumerable)GetValue(collection);
                        return WherePart.Concat(Recurse(ref i, property), "IN", WherePart.IsCollection(ref i, values));
                    }
                    throw new Exception("Unsupported method call: " + methodCall.Method.Name);
                }
                throw new Exception("Unsupported expression: " + expression.GetType().Name);
            }
        }

        private object GetValue(Expression member) {
            lock (lock3) {
                var objectMember = Expression.Convert(member, typeof(object));
                var getterLambda = Expression.Lambda<Func<object>>(objectMember);
                var getter = getterLambda.Compile();
                return getter();
            }
        }

        private string NodeTypeToString(ExpressionType nodeType) {
            lock (lock4) {
                switch (nodeType) {
                    case ExpressionType.Add:
                        return "+";
                    case ExpressionType.And:
                        return "&";
                    case ExpressionType.AndAlso:
                        return "AND";
                    case ExpressionType.Divide:
                        return "/";
                    case ExpressionType.Equal:
                        return "=";
                    case ExpressionType.ExclusiveOr:
                        return "^";
                    case ExpressionType.GreaterThan:
                        return ">";
                    case ExpressionType.GreaterThanOrEqual:
                        return ">=";
                    case ExpressionType.LessThan:
                        return "<";
                    case ExpressionType.LessThanOrEqual:
                        return "<=";
                    case ExpressionType.Modulo:
                        return "%";
                    case ExpressionType.Multiply:
                        return "*";
                    case ExpressionType.Negate:
                        return "-";
                    case ExpressionType.Not:
                        return "NOT";
                    case ExpressionType.NotEqual:
                        return "<>";
                    case ExpressionType.Or:
                        return "|";
                    case ExpressionType.OrElse:
                        return "OR";
                    case ExpressionType.Subtract:
                        return "-";
                }
                throw new Exception($"Unsupported node type: {nodeType}");
            }
        }
    }

    internal class WherePart {
        private readonly static object lock1 = new object();
        private readonly static object lock2 = new object();
        private readonly static object lock3 = new object();
        private readonly static object lock4 = new object();
        private readonly static object lock5 = new object();

        public string Sql { get; set; }
        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();

        public static WherePart IsSql(string sql) {
            lock (lock1) {
                return new WherePart() {
                    Parameters = new Dictionary<string, object>(),
                    Sql = sql
                };
            }
        }

        public static WherePart IsParameter(int count, object value) {
            lock (lock2) {
                return new WherePart() {
                    Parameters = { { count.ToString(), value } },
                    Sql = $"@{count}"
                };
            }
        }

        public static WherePart IsCollection(ref int countStart, IEnumerable values) {
            lock (lock3) {
                var parameters = new Dictionary<string, object>();
                var sql = new StringBuilder("(");
                foreach (var value in values) {
                    parameters.Add((countStart).ToString(), value);
                    sql.Append($"@{countStart},");
                    countStart++;
                }
                if (sql.Length == 1) {
                    sql.Append("null,");
                }
                sql[sql.Length - 1] = ')';
                return new WherePart() {
                    Parameters = parameters,
                    Sql = sql.ToString()
                };
            }
        }

        public static WherePart Concat(string @operator, WherePart operand) {
            lock (lock4) {
                return new WherePart() {
                    Parameters = operand.Parameters,
                    Sql = $"({@operator} {operand.Sql})"
                };
            }
        }

        public static WherePart Concat(WherePart left, string @operator, WherePart right) {
            lock (lock5) {
                return new WherePart() {
                    Parameters = left.Parameters.Union(right.Parameters).ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
                    Sql = $"({left.Sql} {@operator} {right.Sql})"
                };
            }
        }
    }
}
