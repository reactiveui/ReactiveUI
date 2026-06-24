// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ReactiveUI;

/// <summary>Extension methods associated with the Expression class.</summary>
public static class ExpressionMixins
{
    /// <summary>Provides expression-chain and member-resolution extension members for <see cref="Expression"/>.</summary>
    /// <param name="expression">The expression.</param>
    extension(Expression expression)
    {
        /// <summary>Gets all the chain of child expressions within a Expression. Handles property member accesses, objects and indexes.</summary>
        /// <returns>An enumerable of expressions.</returns>
        public IEnumerable<Expression> GetExpressionChain()
        {
            List<Expression> expressions = [];
            var node = expression;

            while (node is not null && node.NodeType != ExpressionType.Parameter)
            {
                switch (node.NodeType)
                {
                    case ExpressionType.Index when node is IndexExpression indexExpression:
                        {
                            expressions.Add(NormalizeIndexExpression(indexExpression));
                            node = indexExpression.Object;
                            break;
                        }

                    case ExpressionType.MemberAccess when node is MemberExpression memberExpression:
                        {
                            expressions.Add(NormalizeMemberExpression(memberExpression));
                            node = memberExpression.Expression;
                            break;
                        }

                    default:
                        {
                            StringBuilder errorMessageBuilder = new($"Unsupported expression of type '{node.NodeType}'.");

                            if (node is ConstantExpression)
                            {
                                _ = errorMessageBuilder.Append(" Did you miss the member access prefix in the expression?");
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
        /// <returns>The member info from the expression.</returns>
        public MemberInfo? GetMemberInfo()
        {
            ArgumentExceptionHelper.ThrowIfNull(expression);

            MemberInfo? info;
            switch (expression.NodeType)
            {
                case ExpressionType.Index when expression is IndexExpression indexExpression:
                    {
                        info = indexExpression.Indexer;
                        break;
                    }

                case ExpressionType.MemberAccess when expression is MemberExpression memberExpression:
                    {
                        info = memberExpression.Member;
                        break;
                    }

                case ExpressionType.Convert or ExpressionType.ConvertChecked
                    when expression is UnaryExpression unaryExpression:
                    return GetMemberInfo(unaryExpression.Operand);
                default:
                    throw new NotSupportedException($"Unsupported {nameof(expression)} type: '{expression.NodeType}'");
            }

            return info;
        }

        /// <summary>Gets the parent Expression of the current Expression object.</summary>
        /// <returns>The parent expression.</returns>
        [SuppressMessage("Critical Code Smell", "S1541:Methods and properties should not be too complex", Justification = "Look up table for performance.")]
        [SuppressMessage("Major Code Smell", "S138:Functions should not have too many lines of code", Justification = "Look up table for performance.")]
        public Expression? GetParent()
        {
            ArgumentExceptionHelper.ThrowIfNull(expression);

            return expression.NodeType switch
            {
                ExpressionType.Index when expression is IndexExpression indexExpression => indexExpression.Object,
                ExpressionType.MemberAccess when expression is MemberExpression memberExpression => memberExpression
                    .Expression,
                _ => throw new NotSupportedException($"Resolving the parent of an expression of type '{expression.NodeType}' is not supported by ReactiveUI.")
            };
        }

        /// <summary>
        /// For a Expression which is a Index type, will get all the arguments passed to the indexer.
        /// Useful for when you are attempting to find the indexer when a constant value is passed in.
        /// </summary>
        /// <returns>An array of arguments.</returns>
        public object?[]? GetArgumentsArray()
        {
            ArgumentExceptionHelper.ThrowIfNull(expression);

            if (expression.NodeType != ExpressionType.Index)
            {
                return null;
            }

            var arguments = ((IndexExpression)expression).Arguments;
            var result = new object?[arguments.Count];
            for (var i = 0; i < arguments.Count; i++)
            {
                result[i] = ((ConstantExpression)arguments[i]).Value;
            }

            return result;
        }
    }

    /// <summary>Rewrites an index expression's receiver to a parameter placeholder when it is a nested member access.</summary>
    /// <param name="indexExpression">The index expression to normalize.</param>
    /// <returns>The normalized expression to add to the chain.</returns>
    private static IndexExpression NormalizeIndexExpression(IndexExpression indexExpression)
    {
        var parent = indexExpression.GetParent();
        return indexExpression.Object is not null && parent is not null && indexExpression.Object.NodeType != ExpressionType.Parameter
            ? indexExpression.Update(Expression.Parameter(parent.Type), indexExpression.Arguments)
            : indexExpression;
    }

    /// <summary>Rewrites a member expression's receiver to a parameter placeholder when it is a nested member access.</summary>
    /// <param name="memberExpression">The member expression to normalize.</param>
    /// <returns>The normalized expression to add to the chain.</returns>
    private static MemberExpression NormalizeMemberExpression(MemberExpression memberExpression)
    {
        var parent = memberExpression.GetParent();
        return parent is not null && memberExpression.Expression is not null && memberExpression.Expression.NodeType != ExpressionType.Parameter
            ? memberExpression.Update(Expression.Parameter(parent.Type))
            : memberExpression;
    }
}
