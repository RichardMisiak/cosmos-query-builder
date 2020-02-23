using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CosmosQueryBuilder
{
    public static class QueryBuilder<T> where T : class
    {
        public static string Where(string alias, Expression<Predicate<T>> predicate)
        {
            return HandleExpression(alias, predicate.Body);
        }

        private static string HandleExpression(string alias, Expression e)
        {
            var handlers = new Dictionary<ExpressionType, Func<Expression, string>>
            {
                [ExpressionType.Constant] = exp =>
                {
                    var cve = exp as ConstantExpression;
                    switch (cve.Value)
                    {
                        case string s:
                            return $"'{s}'";
                        case int i:
                            return i.ToString();
                        case long l:
                            return l.ToString();
                        case float f:
                            return f.ToString();
                        case double d:
                            return d.ToString();
                        case decimal d:
                            return d.ToString();
                        case bool b:
                            return b.ToString().ToLowerInvariant();
                        default:
                            throw new NotImplementedException();
                    }
                },
                [ExpressionType.MemberAccess] = exp =>
                {
                    return $"{alias}.{(exp as MemberExpression).Member.Name}";
                },
                [ExpressionType.Equal] = exp =>
                {
                    var be = exp as BinaryExpression;
                    return $"{HandleExpression(alias, be.Left)} = {HandleExpression(alias, be.Right)}";
                },
                [ExpressionType.NotEqual] = exp =>
                {
                    var be = exp as BinaryExpression;
                    return $"{HandleExpression(alias, be.Left)} != {HandleExpression(alias, be.Right)}";
                },
                [ExpressionType.LessThan] = exp =>
                {
                    var be = exp as BinaryExpression;
                    return $"{HandleExpression(alias, be.Left)} < {HandleExpression(alias, be.Right)}";
                },
                [ExpressionType.LessThanOrEqual] = exp =>
                {
                    var be = exp as BinaryExpression;
                    return $"{HandleExpression(alias, be.Left)} <= {HandleExpression(alias, be.Right)}";
                },
                [ExpressionType.GreaterThan] = exp =>
                {
                    var be = exp as BinaryExpression;
                    return $"{HandleExpression(alias, be.Left)} > {HandleExpression(alias, be.Right)}";
                },
                [ExpressionType.GreaterThanOrEqual] = exp =>
                {
                    var be = exp as BinaryExpression;
                    return $"{HandleExpression(alias, be.Left)} >= {HandleExpression(alias, be.Right)}";
                },
                [ExpressionType.AndAlso] = exp =>
                {
                    var be = exp as BinaryExpression;
                    return $"{HandleExpression(alias, be.Left)} AND {HandleExpression(alias, be.Right)}";
                },
                [ExpressionType.OrElse] = exp =>
                {
                    var be = exp as BinaryExpression;
                    return $"({HandleExpression(alias, be.Left)}) OR ({HandleExpression(alias, be.Right)})";
                },
                [ExpressionType.Not] = exp =>
                {
                    return $"!({HandleExpression(alias, (exp as UnaryExpression).Operand)})";
                }
            };

            if (!handlers.ContainsKey(e.NodeType))
            {
                throw new NotImplementedException($"The expression contained an unsupported node type {e.NodeType}");
            }
            
            return handlers[e.NodeType](e);
        }
    }
}
