// Copyright (c) 2022 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ReactiveUI;

/// <summary>
/// Extension methods associated with the Expression class.
/// </summary>
public static class ExpressionMixins
{
    /// <summary>
    /// Gets all the chain of child expressions within a Expression.
    /// Handles property member accesses, objects and indexes.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <returns>An enumerable of expressions.</returns>
    public static IEnumerable<Expression> GetExpressionChain(this Expression expression)
    {
        var expressions = new List<Expression>();
        var node = expression;

        while (node is not null && node.NodeType != ExpressionType.Parameter)
        {
            switch (node.NodeType)
            {
                case ExpressionType.Index when node is IndexExpression indexExpression:
                    {
                        var parent = indexExpression.GetParent();
                        if (indexExpression.Object is not null && parent is not null && indexExpression.Object.NodeType != ExpressionType.Parameter)
                        {
                            expressions.Add(
                                            indexExpression.Update(Expression.Parameter(parent.Type), indexExpression.Arguments));
                        }
                        else
                        {
                            expressions.Add(indexExpression);
                        }

                        node = indexExpression.Object;
                        break;
                    }

                case ExpressionType.MemberAccess when node is MemberExpression memberExpression:
                    {
                        var parent = memberExpression.GetParent();
                        if (parent is not null && memberExpression.Expression is not null && memberExpression.Expression.NodeType != ExpressionType.Parameter)
                        {
                            expressions.Add(memberExpression.Update(Expression.Parameter(parent.Type)));
                        }
                        else
                        {
                            expressions.Add(memberExpression);
                        }

                        node = memberExpression.Expression;
                        break;
                    }

                default:
                    {
                        var errorMessageBuilder = new StringBuilder($"Unsupported expression of type '{node.NodeType}'.");

                        if (node is ConstantExpression)
                        {
                            errorMessageBuilder.Append(" Did you miss the member access prefix in the expression?");
                        }

                        throw new NotSupportedException(errorMessageBuilder.ToString());
                    }
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
    public static MemberInfo? GetMemberInfo(this Expression expression)
    {
        if (expression is null)
        {
            throw new ArgumentNullException(nameof(expression));
        }

        MemberInfo? info;
        switch (expression.NodeType)
        {
            case ExpressionType.Index when expression is IndexExpression indexExpression:
                info = indexExpression.Indexer;
                break;
            case ExpressionType.MemberAccess when expression is MemberExpression memberExpression:
                info = memberExpression.Member;
                break;
            case ExpressionType.Convert or ExpressionType.ConvertChecked when expression is UnaryExpression unaryExpression:
                return GetMemberInfo(unaryExpression.Operand);
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
    public static Expression? GetParent(this Expression expression) // TODO: Create Test
    {
        if (expression is null)
        {
            throw new ArgumentNullException(nameof(expression));
        }

        return expression.NodeType switch
        {
            ExpressionType.Index when expression is IndexExpression indexExpression => indexExpression.Object,
            ExpressionType.MemberAccess when expression is MemberExpression memberExpression => memberExpression
                .Expression,
            _ => throw new NotSupportedException($"Unsupported expression type: '{expression.NodeType}'")
        };
    }

    /// <summary>
    /// For a Expression which is a Index type, will get all the arguments passed to the indexer.
    /// Useful for when you are attempting to find the indexer when a constant value is passed in.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <returns>An array of arguments.</returns>
    public static object?[]? GetArgumentsArray(this Expression expression) // TODO: Create Test
    {
        if (expression is null)
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