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
                ExpressionType.Add => throw new NotSupportedException("Resolving the parent of an expression of type 'Add' is not supported by ReactiveUI."),
                ExpressionType.AddChecked => throw new NotSupportedException("Resolving the parent of an expression of type 'AddChecked' is not supported by ReactiveUI."),
                ExpressionType.And => throw new NotSupportedException("Resolving the parent of an expression of type 'And' is not supported by ReactiveUI."),
                ExpressionType.AndAlso => throw new NotSupportedException("Resolving the parent of an expression of type 'AndAlso' is not supported by ReactiveUI."),
                ExpressionType.ArrayLength => throw new NotSupportedException("Resolving the parent of an expression of type 'ArrayLength' is not supported by ReactiveUI."),
                ExpressionType.ArrayIndex => throw new NotSupportedException("Resolving the parent of an expression of type 'ArrayIndex' is not supported by ReactiveUI."),
                ExpressionType.Call => throw new NotSupportedException("Resolving the parent of an expression of type 'Call' is not supported by ReactiveUI."),
                ExpressionType.Coalesce => throw new NotSupportedException("Resolving the parent of an expression of type 'Coalesce' is not supported by ReactiveUI."),
                ExpressionType.Conditional => throw new NotSupportedException("Resolving the parent of an expression of type 'Conditional' is not supported by ReactiveUI."),
                ExpressionType.Constant => throw new NotSupportedException("Resolving the parent of an expression of type 'Constant' is not supported by ReactiveUI."),
                ExpressionType.Convert => throw new NotSupportedException("Resolving the parent of an expression of type 'Convert' is not supported by ReactiveUI."),
                ExpressionType.ConvertChecked => throw new NotSupportedException("Resolving the parent of an expression of type 'ConvertChecked' is not supported by ReactiveUI."),
                ExpressionType.Divide => throw new NotSupportedException("Resolving the parent of an expression of type 'Divide' is not supported by ReactiveUI."),
                ExpressionType.Equal => throw new NotSupportedException("Resolving the parent of an expression of type 'Equal' is not supported by ReactiveUI."),
                ExpressionType.ExclusiveOr => throw new NotSupportedException("Resolving the parent of an expression of type 'ExclusiveOr' is not supported by ReactiveUI."),
                ExpressionType.GreaterThan => throw new NotSupportedException("Resolving the parent of an expression of type 'GreaterThan' is not supported by ReactiveUI."),
                ExpressionType.GreaterThanOrEqual => throw new NotSupportedException("Resolving the parent of an expression of type 'GreaterThanOrEqual' is not supported by ReactiveUI."),
                ExpressionType.Invoke => throw new NotSupportedException("Resolving the parent of an expression of type 'Invoke' is not supported by ReactiveUI."),
                ExpressionType.Lambda => throw new NotSupportedException("Resolving the parent of an expression of type 'Lambda' is not supported by ReactiveUI."),
                ExpressionType.LeftShift => throw new NotSupportedException("Resolving the parent of an expression of type 'LeftShift' is not supported by ReactiveUI."),
                ExpressionType.LessThan => throw new NotSupportedException("Resolving the parent of an expression of type 'LessThan' is not supported by ReactiveUI."),
                ExpressionType.LessThanOrEqual => throw new NotSupportedException("Resolving the parent of an expression of type 'LessThanOrEqual' is not supported by ReactiveUI."),
                ExpressionType.ListInit => throw new NotSupportedException("Resolving the parent of an expression of type 'ListInit' is not supported by ReactiveUI."),
                ExpressionType.MemberInit => throw new NotSupportedException("Resolving the parent of an expression of type 'MemberInit' is not supported by ReactiveUI."),
                ExpressionType.Modulo => throw new NotSupportedException("Resolving the parent of an expression of type 'Modulo' is not supported by ReactiveUI."),
                ExpressionType.Multiply => throw new NotSupportedException("Resolving the parent of an expression of type 'Multiply' is not supported by ReactiveUI."),
                ExpressionType.MultiplyChecked => throw new NotSupportedException("Resolving the parent of an expression of type 'MultiplyChecked' is not supported by ReactiveUI."),
                ExpressionType.Negate => throw new NotSupportedException("Resolving the parent of an expression of type 'Negate' is not supported by ReactiveUI."),
                ExpressionType.UnaryPlus => throw new NotSupportedException("Resolving the parent of an expression of type 'UnaryPlus' is not supported by ReactiveUI."),
                ExpressionType.NegateChecked => throw new NotSupportedException("Resolving the parent of an expression of type 'NegateChecked' is not supported by ReactiveUI."),
                ExpressionType.New => throw new NotSupportedException("Resolving the parent of an expression of type 'New' is not supported by ReactiveUI."),
                ExpressionType.NewArrayInit => throw new NotSupportedException("Resolving the parent of an expression of type 'NewArrayInit' is not supported by ReactiveUI."),
                ExpressionType.NewArrayBounds => throw new NotSupportedException("Resolving the parent of an expression of type 'NewArrayBounds' is not supported by ReactiveUI."),
                ExpressionType.Not => throw new NotSupportedException("Resolving the parent of an expression of type 'Not' is not supported by ReactiveUI."),
                ExpressionType.NotEqual => throw new NotSupportedException("Resolving the parent of an expression of type 'NotEqual' is not supported by ReactiveUI."),
                ExpressionType.Or => throw new NotSupportedException("Resolving the parent of an expression of type 'Or' is not supported by ReactiveUI."),
                ExpressionType.OrElse => throw new NotSupportedException("Resolving the parent of an expression of type 'OrElse' is not supported by ReactiveUI."),
                ExpressionType.Parameter => throw new NotSupportedException("Resolving the parent of an expression of type 'Parameter' is not supported by ReactiveUI."),
                ExpressionType.Power => throw new NotSupportedException("Resolving the parent of an expression of type 'Power' is not supported by ReactiveUI."),
                ExpressionType.Quote => throw new NotSupportedException("Resolving the parent of an expression of type 'Quote' is not supported by ReactiveUI."),
                ExpressionType.RightShift => throw new NotSupportedException("Resolving the parent of an expression of type 'RightShift' is not supported by ReactiveUI."),
                ExpressionType.Subtract => throw new NotSupportedException("Resolving the parent of an expression of type 'Subtract' is not supported by ReactiveUI."),
                ExpressionType.SubtractChecked => throw new NotSupportedException("Resolving the parent of an expression of type 'SubtractChecked' is not supported by ReactiveUI."),
                ExpressionType.TypeAs => throw new NotSupportedException("Resolving the parent of an expression of type 'TypeAs' is not supported by ReactiveUI."),
                ExpressionType.TypeIs => throw new NotSupportedException("Resolving the parent of an expression of type 'TypeIs' is not supported by ReactiveUI."),
                ExpressionType.Assign => throw new NotSupportedException("Resolving the parent of an expression of type 'Assign' is not supported by ReactiveUI."),
                ExpressionType.Block => throw new NotSupportedException("Resolving the parent of an expression of type 'Block' is not supported by ReactiveUI."),
                ExpressionType.DebugInfo => throw new NotSupportedException("Resolving the parent of an expression of type 'DebugInfo' is not supported by ReactiveUI."),
                ExpressionType.Decrement => throw new NotSupportedException("Resolving the parent of an expression of type 'Decrement' is not supported by ReactiveUI."),
                ExpressionType.Dynamic => throw new NotSupportedException("Resolving the parent of an expression of type 'Dynamic' is not supported by ReactiveUI."),
                ExpressionType.Default => throw new NotSupportedException("Resolving the parent of an expression of type 'Default' is not supported by ReactiveUI."),
                ExpressionType.Extension => throw new NotSupportedException("Resolving the parent of an expression of type 'Extension' is not supported by ReactiveUI."),
                ExpressionType.Goto => throw new NotSupportedException("Resolving the parent of an expression of type 'Goto' is not supported by ReactiveUI."),
                ExpressionType.Increment => throw new NotSupportedException("Resolving the parent of an expression of type 'Increment' is not supported by ReactiveUI."),
                ExpressionType.Label => throw new NotSupportedException("Resolving the parent of an expression of type 'Label' is not supported by ReactiveUI."),
                ExpressionType.RuntimeVariables => throw new NotSupportedException("Resolving the parent of an expression of type 'RuntimeVariables' is not supported by ReactiveUI."),
                ExpressionType.Loop => throw new NotSupportedException("Resolving the parent of an expression of type 'Loop' is not supported by ReactiveUI."),
                ExpressionType.Switch => throw new NotSupportedException("Resolving the parent of an expression of type 'Switch' is not supported by ReactiveUI."),
                ExpressionType.Throw => throw new NotSupportedException("Resolving the parent of an expression of type 'Throw' is not supported by ReactiveUI."),
                ExpressionType.Try => throw new NotSupportedException("Resolving the parent of an expression of type 'Try' is not supported by ReactiveUI."),
                ExpressionType.Unbox => throw new NotSupportedException("Resolving the parent of an expression of type 'Unbox' is not supported by ReactiveUI."),
                ExpressionType.AddAssign => throw new NotSupportedException("Resolving the parent of an expression of type 'AddAssign' is not supported by ReactiveUI."),
                ExpressionType.AndAssign => throw new NotSupportedException("Resolving the parent of an expression of type 'AndAssign' is not supported by ReactiveUI."),
                ExpressionType.DivideAssign => throw new NotSupportedException("Resolving the parent of an expression of type 'DivideAssign' is not supported by ReactiveUI."),
                ExpressionType.ExclusiveOrAssign => throw new NotSupportedException("Resolving the parent of an expression of type 'ExclusiveOrAssign' is not supported by ReactiveUI."),
                ExpressionType.LeftShiftAssign => throw new NotSupportedException("Resolving the parent of an expression of type 'LeftShiftAssign' is not supported by ReactiveUI."),
                ExpressionType.ModuloAssign => throw new NotSupportedException("Resolving the parent of an expression of type 'ModuloAssign' is not supported by ReactiveUI."),
                ExpressionType.MultiplyAssign => throw new NotSupportedException("Resolving the parent of an expression of type 'MultiplyAssign' is not supported by ReactiveUI."),
                ExpressionType.OrAssign => throw new NotSupportedException("Resolving the parent of an expression of type 'OrAssign' is not supported by ReactiveUI."),
                ExpressionType.PowerAssign => throw new NotSupportedException("Resolving the parent of an expression of type 'PowerAssign' is not supported by ReactiveUI."),
                ExpressionType.RightShiftAssign => throw new NotSupportedException("Resolving the parent of an expression of type 'RightShiftAssign' is not supported by ReactiveUI."),
                ExpressionType.SubtractAssign => throw new NotSupportedException("Resolving the parent of an expression of type 'SubtractAssign' is not supported by ReactiveUI."),
                ExpressionType.AddAssignChecked => throw new NotSupportedException("Resolving the parent of an expression of type 'AddAssignChecked' is not supported by ReactiveUI."),
                ExpressionType.MultiplyAssignChecked => throw new NotSupportedException("Resolving the parent of an expression of type 'MultiplyAssignChecked' is not supported by ReactiveUI."),
                ExpressionType.SubtractAssignChecked => throw new NotSupportedException("Resolving the parent of an expression of type 'SubtractAssignChecked' is not supported by ReactiveUI."),
                ExpressionType.PreIncrementAssign => throw new NotSupportedException("Resolving the parent of an expression of type 'PreIncrementAssign' is not supported by ReactiveUI."),
                ExpressionType.PreDecrementAssign => throw new NotSupportedException("Resolving the parent of an expression of type 'PreDecrementAssign' is not supported by ReactiveUI."),
                ExpressionType.PostIncrementAssign => throw new NotSupportedException("Resolving the parent of an expression of type 'PostIncrementAssign' is not supported by ReactiveUI."),
                ExpressionType.PostDecrementAssign => throw new NotSupportedException("Resolving the parent of an expression of type 'PostDecrementAssign' is not supported by ReactiveUI."),
                ExpressionType.TypeEqual => throw new NotSupportedException("Resolving the parent of an expression of type 'TypeEqual' is not supported by ReactiveUI."),
                ExpressionType.OnesComplement => throw new NotSupportedException("Resolving the parent of an expression of type 'OnesComplement' is not supported by ReactiveUI."),
                ExpressionType.IsTrue => throw new NotSupportedException("Resolving the parent of an expression of type 'IsTrue' is not supported by ReactiveUI."),
                ExpressionType.IsFalse => throw new NotSupportedException("Resolving the parent of an expression of type 'IsFalse' is not supported by ReactiveUI."),
                _ => throw new NotSupportedException($"Unsupported expression type: '{expression.NodeType}'")
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
