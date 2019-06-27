﻿// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveUI
{
    /// <summary>
    /// Extension methods associated with the Expression class.
    /// </summary>
    public static class ExpressionMixins
    {
        /// <summary>
        /// Gets all the chain of child expressions within a Expression.
        /// Handles property member accesses, objects and indexes.
        /// </summary>
        /// <param name="this">The expression.</param>
        /// <returns>An enumerable of expressions.</returns>
        public static IEnumerable<Expression> GetExpressionChain(this Expression @this)
        {
            var expressions = new List<Expression>();
            var node = @this;

            while (node.NodeType != ExpressionType.Parameter)
            {
                switch (node.NodeType)
                {
                case ExpressionType.Index:
                    var indexExpr = (IndexExpression)node;
                    if (indexExpr.Object.NodeType != ExpressionType.Parameter)
                    {
                        expressions.Add(indexExpr.Update(Expression.Parameter(indexExpr.GetParent().Type), indexExpr.Arguments));
                    }
                    else
                    {
                        expressions.Add(indexExpr);
                    }

                    node = indexExpr.Object;
                    break;
                case ExpressionType.MemberAccess:
                    var memberExpr = (MemberExpression)node;
                    if (memberExpr.Expression.NodeType != ExpressionType.Parameter)
                    {
                        expressions.Add(memberExpr.Update(Expression.Parameter(memberExpr.GetParent().Type)));
                    }
                    else
                    {
                        expressions.Add(memberExpr);
                    }

                    node = memberExpr.Expression;
                    break;
                default:
                    throw new NotSupportedException($"Unsupported expression type: '{node.NodeType}'");
                }
            }

            expressions.Reverse();
            return expressions;
        }

        /// <summary>
        /// Gets the MemberInfo where a Expression is pointing towards.
        /// Can handle MemberAccess and Index types and will handle
        /// going through the Conversion Expressions.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>The member info from the expression.</returns>
        public static MemberInfo GetMemberInfo(this Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            MemberInfo info;
            switch (expression.NodeType)
            {
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
                throw new NotSupportedException($"Unsupported expression type: '{expression.NodeType}'");
            }

            return info;
        }

        /// <summary>
        /// Gets the parent Expression of the current Expression object.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>The parent expression.</returns>
        public static Expression GetParent(this Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            switch (expression.NodeType)
            {
            case ExpressionType.Index:
                return ((IndexExpression)expression).Object;
            case ExpressionType.MemberAccess:
                return ((MemberExpression)expression).Expression;
            default:
                throw new NotSupportedException($"Unsupported expression type: '{expression.NodeType}'");
            }
        }

        /// <summary>
        /// For a Expression which is a Index type, will get all the arguments passed to the indexer.
        /// Useful for when you are attempting to find the indexer when a constant value is passed in.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>An array of arguments.</returns>
        public static object[] GetArgumentsArray(this Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            if (expression.NodeType == ExpressionType.Index)
            {
                return ((IndexExpression)expression).Arguments.Cast<ConstantExpression>().Select(c => c.Value).ToArray();
            }

            return null;
        }
    }
}
