using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ApplicationCore.Interfaces;
using Infrastructure.Exceptions;
using Infrastructure.Helpers;
using Infrastructure.Mapping;

namespace Infrastructure.Extensions
{
    internal static class LinqMappingExtension
    {
        public static IEnumerable<string> IncludeMemberNames<T>(this ISpecification<T> spec)
        {
            return spec.Includes
                .Where(expression => expression.Body is MemberExpression)
                .Select(expression => ((MemberExpression) expression.Body).Member.Name);
        }

        public static ISqlCommandMapping LinqToSqlWhere<T>(this IEntityMapping mapping,
            Expression<Func<T, bool>> expression)
        {
            return mapping.LinqToSqlWhere(new List<Expression<Func<T, bool>>> {expression});
        }

        public static ISqlCommandMapping LinqToSqlWhere<T>(this IEntityMapping mapping,
            IEnumerable<Expression<Func<T, bool>>> expressionList)
        {
            var sqlMapping = new SqlCommandMapping
            {
                CommandText = string.Empty,
                Parameters = new Dictionary<string, object>()
            };

            foreach (var expression in expressionList)
            {
                var expressionMap = expression.GetWhereMapping();
                var property = mapping.GetPropertyByName(expressionMap.PropertyName);

                sqlMapping.Parameters.Add($"@{expressionMap.PropertyName}", expressionMap.Value);

                if (sqlMapping.CommandText.Contains(SqlTerm.Where))
                {
                    sqlMapping.CommandText +=
                        $"{SqlTerm.And} {property.ColumnName} {expressionMap.Operator} :{expressionMap.PropertyName} ";
                }
                else
                {
                    sqlMapping.CommandText +=
                        $"{SqlTerm.Where} {property.ColumnName} {expressionMap.Operator} :{expressionMap.PropertyName} ";
                }
            }

            return sqlMapping;
        }

        private static LinqWhereMapping GetWhereMapping<T>(this Expression<Func<T, bool>> expression)
        {
            var mapping = new LinqWhereMapping();
            switch (expression.Body)
            {
                case MethodCallExpression callExpression:
                    if (callExpression.Method.Name.Equals("Equals"))
                    {
                        mapping.PropertyName = ((MemberExpression) callExpression.Object)?.Member.Name;
                        mapping.Value = ((ConstantExpression) callExpression.Arguments.First()).Value;
                        mapping.Operator = "=";
                    }
                    else
                    {
                        throw new MehtodCallMappingException();
                    }

                    break;
                case BinaryExpression binaryExpression:
                    mapping.PropertyName = ((MemberExpression) binaryExpression.Left).Member.Name;
                    mapping.Value = ((ConstantExpression) binaryExpression.Right).Value;
                    mapping.Operator = binaryExpression.NodeType.GetExpressionOperator();
                    break;
            }

            return mapping;
        }

        private static void GetIncludeMapping<T>(this Expression<Func<T, object>> expression)
        {
            switch (expression.Body)
            {
                case MemberExpression property:
                    var propertyName = property.Member.Name;
                    break;
                default:
                    throw new MehtodCallMappingException();
            }
        }

        private static string GetExpressionOperator(this ExpressionType expressionType)
        {
            string operatorChar;
            switch (expressionType)
            {
                case ExpressionType.Equal:
                    operatorChar = "=";
                    break;
                case ExpressionType.GreaterThan:
                    operatorChar = ">";
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    operatorChar = ">=";
                    break;
                case ExpressionType.LessThan:
                    operatorChar = "<";
                    break;
                case ExpressionType.LessThanOrEqual:
                    operatorChar = "<=";
                    break;
                case ExpressionType.NotEqual:
                    operatorChar = "<>";
                    break;
                default:
                    throw new OperatorMappingException();
            }

            return operatorChar;
        }
    }

    internal class SqlCommandMapping : ISqlCommandMapping
    {
        public string CommandText { get; set; }
        public IDictionary<string, object> Parameters { get; set; }
    }

    internal class LinqWhereMapping
    {
        public string PropertyName { get; set; }
        public object Value { get; set; }
        public string Operator { get; set; }
    }
}