using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ReactiveUI
{
    /// <summary>
    /// Expression Mixins
    /// </summary>
    public static class ExpressionMixins
    {
        /// <summary>
        /// Gets the arguments array.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public static object[] GetArgumentsArray(this Expression expression)
        {
            if (expression.NodeType == ExpressionType.Index) {
                return ((IndexExpression)expression).Arguments.Cast<ConstantExpression>().Select(c => c.Value).ToArray();
            } else {
                return null;
            }
        }

        /// <summary>
        /// Gets the expression chain.
        /// </summary>
        /// <param name="This">The this.</param>
        /// <returns></returns>
        /// <exception cref="System.NotSupportedException"></exception>
        public static IEnumerable<Expression> GetExpressionChain(this Expression This)
        {
            var expressions = new List<Expression>();
            var node = This;

            while (node.NodeType != ExpressionType.Parameter) {
                switch (node.NodeType) {
                case ExpressionType.Index:
                    var indexExpr = (IndexExpression)node;
                    if (indexExpr.Object.NodeType != ExpressionType.Parameter) {
                        expressions.Add(indexExpr.Update(Expression.Parameter(indexExpr.GetParent().Type), indexExpr.Arguments));
                    } else {
                        expressions.Add(indexExpr);
                    }
                    node = indexExpr.Object;
                    break;

                case ExpressionType.MemberAccess:
                    var memberExpr = (MemberExpression)node;
                    if (memberExpr.Expression.NodeType != ExpressionType.Parameter) {
                        expressions.Add(memberExpr.Update(Expression.Parameter(memberExpr.GetParent().Type)));
                    } else {
                        expressions.Add(memberExpr);
                    }
                    node = memberExpr.Expression;
                    break;

                default:
                    throw new NotSupportedException(string.Format("Unsupported expression type: '{0}'", node.NodeType));
                }
            }

            expressions.Reverse();
            return expressions;
        }

        /// <summary>
        /// Gets the member information.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        /// <exception cref="System.NotSupportedException"></exception>
        public static MemberInfo GetMemberInfo(this Expression expression)
        {
            MemberInfo info = null;
            switch (expression.NodeType) {
            case ExpressionType.Index:
                info = ((IndexExpression)expression).Indexer;
                break;

            case ExpressionType.MemberAccess:
                info = ((MemberExpression)expression).Member;
                break;

            case ExpressionType.Convert:
            case ExpressionType.ConvertChecked:
                return GetMemberInfo(((UnaryExpression)expression).Operand);

            default:
                throw new NotSupportedException(string.Format("Unsupported expression type: '{0}'", expression.NodeType));
            }

            return info;
        }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        /// <exception cref="System.NotSupportedException"></exception>
        public static Expression GetParent(this Expression expression)
        {
            switch (expression.NodeType) {
            case ExpressionType.Index:
                return ((IndexExpression)expression).Object;

            case ExpressionType.MemberAccess:
                return ((MemberExpression)expression).Expression;

            default:
                throw new NotSupportedException(string.Format("Unsupported expression type: '{0}'", expression.NodeType));
            }
        }
    }
}